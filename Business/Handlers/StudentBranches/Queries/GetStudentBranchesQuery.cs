using Business.BusinessAspects;
using Business.Handlers.StudentBranches.FilterStudentBranch;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.StudentBranchDto;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.StudentBranches.Queries
{
    public class GetStudentBranchesQuery : IRequest<IDataResult<IEnumerable<StudentBranchGetAllDto>>>
    {
        public int? StudentId { get; set; }
        public int? BranchId { get; set; }

        public class GetStudentBranchesQueryHandler : IRequestHandler<GetStudentBranchesQuery, IDataResult<IEnumerable<StudentBranchGetAllDto>>>
        {
            private readonly IStudentBranchRepository _studentBranchRepository;
            private readonly IMediator _mediator;

            public GetStudentBranchesQueryHandler(IStudentBranchRepository studentBranchRepository, IMediator mediator)
            {
                _studentBranchRepository = studentBranchRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<StudentBranchGetAllDto>>> Handle(GetStudentBranchesQuery request, CancellationToken cancellationToken)
            {
                var list = await _studentBranchRepository.GetListAsync(StudentBranchFiltersHelper.GetStudentBranchesQueryFilter(request));
                var dtos = list.Select(x => new StudentBranchGetAllDto
                {
                    StudentId = x.StudentId,
                    BranchId = x.BranchId,
                    TenantId = x.TenantId,
                    IsActive = x.IsActive ?? true
                });

                return new SuccessDataResult<IEnumerable<StudentBranchGetAllDto>>(dtos);
            }
        }
    }
}