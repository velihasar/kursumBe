using Business.BusinessAspects;
using Business.Handlers.TeacherBranches.FilterTeacherBranch;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.TeacherBranchDto;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.TeacherBranches.Queries
{
    public class GetTeacherBranchesQuery : IRequest<IDataResult<IEnumerable<TeacherBranchGetAllDto>>>
    {
        public int? TeacherId { get; set; }
        public int? BranchId { get; set; }

        public class GetTeacherBranchesQueryHandler : IRequestHandler<GetTeacherBranchesQuery, IDataResult<IEnumerable<TeacherBranchGetAllDto>>>
        {
            private readonly ITeacherBranchRepository _teacherBranchRepository;
            private readonly IMediator _mediator;

            public GetTeacherBranchesQueryHandler(ITeacherBranchRepository teacherBranchRepository, IMediator mediator)
            {
                _teacherBranchRepository = teacherBranchRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<TeacherBranchGetAllDto>>> Handle(GetTeacherBranchesQuery request, CancellationToken cancellationToken)
            {
                var list = await _teacherBranchRepository.GetListAsync(TeacherBranchFiltersHelper.GetTeacherBranchesQueryFilter(request));
                var dtos = list.Select(x => new TeacherBranchGetAllDto
                {
                    TeacherId = x.TeacherId,
                    BranchId = x.BranchId,
                    TenantId = x.TenantId,
                    IsActive = x.IsActive ?? true
                });

                return new SuccessDataResult<IEnumerable<TeacherBranchGetAllDto>>(dtos);
            }
        }
    }
}