
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Tenants.FilterTenant;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Business.Handlers.Tenants.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.TenantDto;
using Core.Extensions;

namespace Business.Handlers.Tenants.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateTenantCommand : IRequest<IDataResult<TenantCreateResponseDto>>
    {

        public string Name { get; set; }
        public string Code { get; set; }
        public string LogoUrl { get; set; }
        public bool? IsActive { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Microsoft.AspNetCore.Http.IFormFile Logo { get; set; }


        public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, IDataResult<TenantCreateResponseDto>>
        {
            private readonly ITenantRepository _tenantRepository;
            private readonly Core.Services.IMinioService _minioService;
            private readonly IMediator _mediator;
            public CreateTenantCommandHandler(ITenantRepository tenantRepository, Core.Services.IMinioService minioService, IMediator mediator)
            {
                _tenantRepository = tenantRepository;
                _minioService = minioService;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateTenantValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<TenantCreateResponseDto>> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();

                var isThereTenantRecord = _tenantRepository.Query().Any(TenantFiltersHelper.CreateTenantCommandFilter(request));

                if (isThereTenantRecord)
                    return new ErrorDataResult<TenantCreateResponseDto>(Messages.NameAlreadyExist);

                string logoUrl = request.LogoUrl ?? "";
                if (request.Logo != null)
                {
                    try
                    {
                        using (var stream = request.Logo.OpenReadStream())
                        {
                            logoUrl = await _minioService.UploadFileAsync(stream, request.Logo.FileName);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        return new ErrorDataResult<TenantCreateResponseDto>($"Logo yükleme hatası: {ex.Message}");
                    }
                }

                var addedTenant = new Tenant
                {
                    Name = request.Name,
                    Code = request.Code,
                    LogoUrl = logoUrl,
                    IsActive = request.IsActive ?? true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = System.DateTime.Now
                };

                _tenantRepository.Add(addedTenant);
                await _tenantRepository.SaveChangesAsync();

                var dto = new TenantCreateResponseDto
                {
                    Id = addedTenant.Id,
                    Name = addedTenant.Name,
                    Code = addedTenant.Code,
                    LogoUrl = addedTenant.LogoUrl
                };

                return new SuccessDataResult<TenantCreateResponseDto>(dto, Messages.Added);
            }
        }
    }
}