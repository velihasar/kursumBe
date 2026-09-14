
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


namespace Business.Handlers.Parents.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteParentCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteParentCommandHandler : IRequestHandler<DeleteParentCommand, IResult>
        {
            private readonly IParentRepository _parentRepository;
            private readonly IMediator _mediator;

            public DeleteParentCommandHandler(IParentRepository parentRepository, IMediator mediator)
            {
                _parentRepository = parentRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteParentCommand request, CancellationToken cancellationToken)
            {
                var parentToDelete = _parentRepository.Get(p => p.Id == request.Id);

                _parentRepository.Delete(parentToDelete);
                await _parentRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

