
using Business.Constants;
using Business.BusinessAspects;
using Business.Handlers.Parents.FilterParent;
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
using Business.Handlers.Parents.ValidationRules;
using Core.Entities.Dtos.ParentDto;
using Core.Extensions;

namespace Business.Handlers.Parents.Commands
{
    public class UpdateParentCommand : IRequest<IDataResult<ParentUpdateResponseDto>>
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public bool IsActive { get; set; }

        public class UpdateParentCommandHandler : IRequestHandler<UpdateParentCommand, IDataResult<ParentUpdateResponseDto>>
        {
            private readonly IParentRepository _parentRepository;
            private readonly IMediator _mediator;

            public UpdateParentCommandHandler(IParentRepository parentRepository, IMediator mediator)
            {
                _parentRepository = parentRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateParentValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<ParentUpdateResponseDto>> Handle(UpdateParentCommand request, CancellationToken cancellationToken)
            {
                var isThereParentRecord = await _parentRepository.GetAsync(ParentFiltersHelper.UpdateParentCommandFilter(request));
                if (isThereParentRecord == null)
                    return new ErrorDataResult<ParentUpdateResponseDto>("Kayıt bulunamadı.");

                var tenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();
                if (tenantId > 0)
                {
                    isThereParentRecord.TenantId = tenantId;
                }
                if (userId > 0)
                {
                    isThereParentRecord.UpdatedBy = userId;
                }
                isThereParentRecord.UpdatedDate = System.DateTime.Now;

                isThereParentRecord.PersonId = request.PersonId;
                isThereParentRecord.IsActive = request.IsActive;

                _parentRepository.Update(isThereParentRecord);
                await _parentRepository.SaveChangesAsync();

                var dto = new ParentUpdateResponseDto
                {
                    Id = isThereParentRecord.Id,
                    PersonId = isThereParentRecord.PersonId
                };

                return new SuccessDataResult<ParentUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}

