
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


namespace Business.Handlers.People.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeletePersonCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, IResult>
        {
            private readonly IPersonRepository _personRepository;
            private readonly IMediator _mediator;

            public DeletePersonCommandHandler(IPersonRepository personRepository, IMediator mediator)
            {
                _personRepository = personRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
            {
                var personToDelete = _personRepository.Get(p => p.Id == request.Id);

                _personRepository.Delete(personToDelete);
                await _personRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

