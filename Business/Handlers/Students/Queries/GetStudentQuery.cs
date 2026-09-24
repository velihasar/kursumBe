
using Business.BusinessAspects;
using Business.Handlers.Students.FilterStudent;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.StudentDto;

namespace Business.Handlers.Students.Queries
{
    public class GetStudentQuery : IRequest<IDataResult<StudentGetByIdDto>>
    {
        public int Id { get; set; }

        public class GetStudentQueryHandler : IRequestHandler<GetStudentQuery, IDataResult<StudentGetByIdDto>>
        {
            private readonly IStudentRepository _studentRepository;
            private readonly IMediator _mediator;

            public GetStudentQueryHandler(IStudentRepository studentRepository, IMediator mediator)
            {
                _studentRepository = studentRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<StudentGetByIdDto>> Handle(GetStudentQuery request, CancellationToken cancellationToken)
            {
                var student = await _studentRepository.GetAsync(StudentFiltersHelper.GetStudentQueryFilter(request));
                if (student == null)
                    return new ErrorDataResult<StudentGetByIdDto>("Kayıt bulunamadı.");

                var dto = new StudentGetByIdDto
                {
                    Id = student.Id,
                    TenantId = student.TenantId,
                    PersonId = student.PersonId,
                    StudentNumber = student.StudentNumber,
                    ParentAccessCode = student.ParentAccessCode ?? $"KRS-{student.Id:D5}",
                    EnrollmentDate = student.EnrollmentDate
                };
                return new SuccessDataResult<StudentGetByIdDto>(dto);
            }
        }
    }
}
