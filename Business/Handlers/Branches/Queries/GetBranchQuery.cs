
using Business.BusinessAspects;
using Business.Handlers.Branches.FilterBranch;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.BranchDto;

namespace Business.Handlers.Branches.Queries
{
    public class GetBranchQuery : IRequest<IDataResult<BranchGetByIdDto>>
    {
        public int Id { get; set; }

        public class GetBranchQueryHandler : IRequestHandler<GetBranchQuery, IDataResult<BranchGetByIdDto>>
        {
            private readonly IBranchRepository _branchRepository;
            private readonly IMediator _mediator;

            public GetBranchQueryHandler(IBranchRepository branchRepository, IMediator mediator)
            {
                _branchRepository = branchRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<BranchGetByIdDto>> Handle(GetBranchQuery request, CancellationToken cancellationToken)
            {
                var branch = await _branchRepository.GetAsync(BranchFiltersHelper.GetBranchQueryFilter(request));
                if (branch == null)
                    return new ErrorDataResult<BranchGetByIdDto>("Kayıt bulunamadı.");

                var dto = new BranchGetByIdDto
                {
                    Id = branch.Id,
                    Name = branch.Name,
                    Address = branch.Address,
                    Phone = branch.Phone
                };
                return new SuccessDataResult<BranchGetByIdDto>(dto);
            }
        }
    }
}
