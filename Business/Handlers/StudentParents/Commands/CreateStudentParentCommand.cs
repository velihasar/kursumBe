
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.StudentParents.FilterStudentParent;
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
using Business.Handlers.StudentParents.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.StudentParentDto;
using Core.Extensions;

namespace Business.Handlers.StudentParents.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateStudentParentCommand : IRequest<IDataResult<StudentParentCreateResponseDto>>
    {

        public int StudentId { get; set; }
        public int ParentId { get; set; }
        public string Relationship { get; set; }
        public bool IsPrimary { get; set; }


        public class CreateStudentParentCommandHandler : IRequestHandler<CreateStudentParentCommand, IDataResult<StudentParentCreateResponseDto>>
        {
            private readonly IStudentParentRepository _studentParentRepository;
            private readonly IMediator _mediator;
            public CreateStudentParentCommandHandler(IStudentParentRepository studentParentRepository, IMediator mediator)
            {
                _studentParentRepository = studentParentRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateStudentParentValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<StudentParentCreateResponseDto>> Handle(CreateStudentParentCommand request, CancellationToken cancellationToken)
            {
                var tenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();

                var isThereStudentParentRecord = _studentParentRepository.Query().Any(StudentParentFiltersHelper.CreateStudentParentCommandFilter(request));

                if (isThereStudentParentRecord)
                    return new ErrorDataResult<StudentParentCreateResponseDto>(Messages.NameAlreadyExist);

                if (request.IsPrimary)
                {
                    var existingPrimaryParents = _studentParentRepository.Query()
                        .Where(sp => sp.StudentId == request.StudentId && sp.IsPrimary && sp.IsDeleted == false)
                        .ToList();

                    foreach (var other in existingPrimaryParents)
                    {
                        other.IsPrimary = false;
                        _studentParentRepository.Update(other);
                    }
                }

                var addedStudentParent = new StudentParent
                {
                    TenantId = tenantId,
                    StudentId = request.StudentId,
                    ParentId = request.ParentId,
                    Relationship = request.Relationship,
                    IsPrimary = request.IsPrimary,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = System.DateTime.Now
                };

                _studentParentRepository.Add(addedStudentParent);
                await _studentParentRepository.SaveChangesAsync();

                var dto = new StudentParentCreateResponseDto
                {
                    Id = addedStudentParent.Id,
                    StudentId = addedStudentParent.StudentId,
                    ParentId = addedStudentParent.ParentId,
                    Relationship = addedStudentParent.Relationship,
                    IsPrimary = addedStudentParent.IsPrimary
                };

                return new SuccessDataResult<StudentParentCreateResponseDto>(dto, Messages.Added);
            }
        }
    }
}