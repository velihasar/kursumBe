using Business.BusinessAspects;
using Business.Handlers.TeacherBranches.FilterTeacherBranch;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.TeacherBranchDto;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.TeacherBranches.Queries
{
    public class GetTeacherBranchQuery : IRequest<IDataResult<TeacherBranchGetByIdDto>>
    {
        public int TeacherId { get; set; }
        public int BranchId { get; set; }

        public class GetTeacherBranchQueryHandler : IRequestHandler<GetTeacherBranchQuery, IDataResult<TeacherBranchGetByIdDto>>
        {
            private readonly ITeacherBranchRepository _teacherBranchRepository;
            private readonly IMediator _mediator;

            public GetTeacherBranchQueryHandler(ITeacherBranchRepository teacherBranchRepository, IMediator mediator)
            {
                _teacherBranchRepository = teacherBranchRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<TeacherBranchGetByIdDto>> Handle(GetTeacherBranchQuery request, CancellationToken cancellationToken)
            {
                var record = await _teacherBranchRepository.GetAsync(TeacherBranchFiltersHelper.GetTeacherBranchQueryFilter(request));
                if (record == null)
                {
                    return new ErrorDataResult<TeacherBranchGetByIdDto>("Kayıt bulunamadı.");
                }

                var dto = new TeacherBranchGetByIdDto
                {
                    TeacherId = record.TeacherId,
                    BranchId = record.BranchId,
                    TenantId = record.TenantId,
                    IsActive = record.IsActive ?? true
                };

                return new SuccessDataResult<TeacherBranchGetByIdDto>(dto);
            }
        }
    }
}
