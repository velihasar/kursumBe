using Business.Constants;
using Business.BusinessAspects;
using Business.Handlers.StudentParents.FilterStudentParent;
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
using Business.Handlers.StudentParents.ValidationRules;
using Core.Entities.Dtos.StudentParentDto;
using Core.Extensions;

namespace Business.Handlers.StudentParents.Commands
{
    public class UpdateStudentParentCommand : IRequest<IDataResult<StudentParentUpdateResponseDto>>
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public int StudentId { get; set; }
        public int ParentId { get; set; }
        public string Relationship { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }

        public class UpdateStudentParentCommandHandler : IRequestHandler<UpdateStudentParentCommand, IDataResult<StudentParentUpdateResponseDto>>
        {
            private readonly IStudentParentRepository _studentParentRepository;
            private readonly IMediator _mediator;

            public UpdateStudentParentCommandHandler(IStudentParentRepository studentParentRepository, IMediator mediator)
            {
                _studentParentRepository = studentParentRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateStudentParentValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<StudentParentUpdateResponseDto>> Handle(UpdateStudentParentCommand request, CancellationToken cancellationToken)
            {
                var isThereStudentParentRecord = await _studentParentRepository.GetAsync(StudentParentFiltersHelper.UpdateStudentParentCommandFilter(request));
                if (isThereStudentParentRecord == null)
                    return new ErrorDataResult<StudentParentUpdateResponseDto>("Kayıt bulunamadı.");

                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();
                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);
                if (targetTenantId > 0)
                {
                    isThereStudentParentRecord.TenantId = targetTenantId;
                }
                if (userId > 0)
                {
                    isThereStudentParentRecord.UpdatedBy = userId;
                }
                isThereStudentParentRecord.UpdatedDate = System.DateTime.Now;

                if (request.Relationship != null)
                {
                    isThereStudentParentRecord.Relationship = request.Relationship;
                }
                isThereStudentParentRecord.IsPrimary = request.IsPrimary;
                isThereStudentParentRecord.IsActive = true;

                if (request.IsPrimary)
                {
                    var otherStudentParents = _studentParentRepository.Query()
                        .Where(sp => sp.StudentId == isThereStudentParentRecord.StudentId && sp.ParentId != isThereStudentParentRecord.ParentId && sp.IsPrimary && sp.IsDeleted == false)
                        .ToList();

                    foreach (var other in otherStudentParents)
                    {
                        other.IsPrimary = false;
                    }
                }

                await _studentParentRepository.SaveChangesAsync();

                var dto = new StudentParentUpdateResponseDto
                {
                    Id = isThereStudentParentRecord.Id,
                    StudentId = isThereStudentParentRecord.StudentId,
                    ParentId = isThereStudentParentRecord.ParentId,
                    Relationship = isThereStudentParentRecord.Relationship,
                    IsPrimary = isThereStudentParentRecord.IsPrimary
                };

                return new SuccessDataResult<StudentParentUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}
