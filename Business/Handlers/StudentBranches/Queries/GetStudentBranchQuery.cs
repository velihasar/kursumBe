using Business.BusinessAspects;
using Business.Handlers.StudentBranches.FilterStudentBranch;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.StudentBranchDto;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.StudentBranches.Queries
{
    public class GetStudentBranchQuery : IRequest<IDataResult<StudentBranchGetByIdDto>>
    {
        public int StudentId { get; set; }
        public int BranchId { get; set; }

        public class GetStudentBranchQueryHandler : IRequestHandler<GetStudentBranchQuery, IDataResult<StudentBranchGetByIdDto>>
        {
            private readonly IStudentBranchRepository _studentBranchRepository;
            private readonly IMediator _mediator;

            public GetStudentBranchQueryHandler(IStudentBranchRepository studentBranchRepository, IMediator mediator)
            {
                _studentBranchRepository = studentBranchRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<StudentBranchGetByIdDto>> Handle(GetStudentBranchQuery request, CancellationToken cancellationToken)
            {
                var record = await _studentBranchRepository.GetAsync(StudentBranchFiltersHelper.GetStudentBranchQueryFilter(request));
                if (record == null)
                {
                    return new ErrorDataResult<StudentBranchGetByIdDto>("Kayıt bulunamadı.");
                }

                var dto = new StudentBranchGetByIdDto
                {
                    StudentId = record.StudentId,
                    BranchId = record.BranchId,
                    TenantId = record.TenantId,
                    IsActive = record.IsActive ?? true
                };

                return new SuccessDataResult<StudentBranchGetByIdDto>(dto);
            }
        }
    }
}
