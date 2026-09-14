
using Business.Constants;
using Business.BusinessAspects;
using Business.Handlers.Tenants.FilterTenant;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Core.Aspects.Autofac.Validation;
using Business.Handlers.Tenants.ValidationRules;
using Core.Entities.Dtos.TenantDto;
using Core.Extensions;

namespace Business.Handlers.Tenants.Commands
{
    public class UpdateTenantCommand : IRequest<IDataResult<TenantUpdateResponseDto>>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string LogoUrl { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        [Newtonsoft.Json.JsonIgnore]
        public Microsoft.AspNetCore.Http.IFormFile Logo { get; set; }
        public bool IsActive { get; set; }

        public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, IDataResult<TenantUpdateResponseDto>>
        {
            private readonly ITenantRepository _tenantRepository;
            private readonly Core.Services.IMinioService _minioService;
            private readonly IMediator _mediator;

            public UpdateTenantCommandHandler(ITenantRepository tenantRepository, Core.Services.IMinioService minioService, IMediator mediator)
            {
                _tenantRepository = tenantRepository;
                _minioService = minioService;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateTenantValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<TenantUpdateResponseDto>> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
            {
                var isThereTenantRecord = await _tenantRepository.GetAsync(TenantFiltersHelper.UpdateTenantCommandFilter(request));
                if (isThereTenantRecord == null)
                    return new ErrorDataResult<TenantUpdateResponseDto>("Kayıt bulunamadı.");

                var userId = UserInfoExtensions.GetUserIdOrZero();
                if (userId > 0)
                {
                    isThereTenantRecord.UpdatedBy = userId;
                }
                isThereTenantRecord.UpdatedDate = System.DateTime.Now;

                isThereTenantRecord.Name = request.Name;
                isThereTenantRecord.Code = request.Code;

                if (request.Logo != null)
                {
                    try
                    {
                        using (var stream = request.Logo.OpenReadStream())
                        {
                            isThereTenantRecord.LogoUrl = await _minioService.UploadFileAsync(stream, request.Logo.FileName);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        return new ErrorDataResult<TenantUpdateResponseDto>($"Logo yükleme hatası: {ex.Message}");
                    }
                }
                else if (request.LogoUrl != null)
                {
                    isThereTenantRecord.LogoUrl = request.LogoUrl;
                }

                isThereTenantRecord.IsActive = request.IsActive;

                _tenantRepository.Update(isThereTenantRecord);
                await _tenantRepository.SaveChangesAsync();

                var dto = new TenantUpdateResponseDto
                {
                    Id = isThereTenantRecord.Id,
                    Name = isThereTenantRecord.Name,
                    Code = isThereTenantRecord.Code,
                    LogoUrl = isThereTenantRecord.LogoUrl
                };

                return new SuccessDataResult<TenantUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}

