using Business.BusinessAspects;
using Business.Handlers.StudentParents.FilterStudentParent;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.StudentParentDto;

namespace Business.Handlers.StudentParents.Queries
{
    public class GetStudentParentQuery : IRequest<IDataResult<StudentParentGetByIdDto>>
    {
        public int Id { get; set; }

        public class GetStudentParentQueryHandler : IRequestHandler<GetStudentParentQuery, IDataResult<StudentParentGetByIdDto>>
        {
            private readonly IStudentParentRepository _studentParentRepository;
            private readonly IMediator _mediator;

            public GetStudentParentQueryHandler(IStudentParentRepository studentParentRepository, IMediator mediator)
            {
                _studentParentRepository = studentParentRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<StudentParentGetByIdDto>> Handle(GetStudentParentQuery request, CancellationToken cancellationToken)
            {
                var studentParent = await _studentParentRepository.GetAsync(StudentParentFiltersHelper.GetStudentParentQueryFilter(request));
                if (studentParent == null)
                    return new ErrorDataResult<StudentParentGetByIdDto>("Kayıt bulunamadı.");

                var dto = new StudentParentGetByIdDto
                {
                    Id = studentParent.Id,
                    StudentId = studentParent.StudentId,
                    ParentId = studentParent.ParentId,
                    Relationship = studentParent.Relationship,
                    IsPrimary = studentParent.IsPrimary
                };
                return new SuccessDataResult<StudentParentGetByIdDto>(dto);
            }
        }
    }
}
