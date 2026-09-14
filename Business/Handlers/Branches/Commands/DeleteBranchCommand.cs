
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;


namespace Business.Handlers.Branches.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteBranchCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand, IResult>
        {
            private readonly IBranchRepository _branchRepository;
            private readonly IMediator _mediator;

            public DeleteBranchCommandHandler(IBranchRepository branchRepository, IMediator mediator)
            {
                _branchRepository = branchRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteBranchCommand request, CancellationToken cancellationToken)
            {
                var branchToDelete = _branchRepository.Get(p => p.Id == request.Id);

                _branchRepository.Delete(branchToDelete);
                await _branchRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

