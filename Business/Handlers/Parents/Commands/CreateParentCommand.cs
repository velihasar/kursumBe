
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Parents.FilterParent;
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
using Business.Handlers.Parents.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.ParentDto;
using Core.Extensions;

namespace Business.Handlers.Parents.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateParentCommand : IRequest<IDataResult<ParentCreateResponseDto>>
    {
        public int? TenantId { get; set; }
        public int PersonId { get; set; }


        public class CreateParentCommandHandler : IRequestHandler<CreateParentCommand, IDataResult<ParentCreateResponseDto>>
        {
            private readonly IParentRepository _parentRepository;
            private readonly IMediator _mediator;
            public CreateParentCommandHandler(IParentRepository parentRepository, IMediator mediator)
            {
                _parentRepository = parentRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateParentValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<ParentCreateResponseDto>> Handle(CreateParentCommand request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();

                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                if (targetTenantId <= 0)
                {
                    return new ErrorDataResult<ParentCreateResponseDto>("SuperAdmin olarak işlem yapmaktasınız. Lütfen geçerli bir kurum (kurs/okul) seçiniz.");
                }

                var isThereParentRecord = _parentRepository.Query().Any(ParentFiltersHelper.CreateParentCommandFilter(request));

                if (isThereParentRecord)
                    return new ErrorDataResult<ParentCreateResponseDto>(Messages.NameAlreadyExist);

                var addedParent = new Parent
                {
                    TenantId = targetTenantId,
                    PersonId = request.PersonId,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = System.DateTime.Now
                };

                _parentRepository.Add(addedParent);
                await _parentRepository.SaveChangesAsync();

                var dto = new ParentCreateResponseDto
                {
                    Id = addedParent.Id,
                    PersonId = addedParent.PersonId
                };

                return new SuccessDataResult<ParentCreateResponseDto>(dto, Messages.Added);
            }
        }
    }
}