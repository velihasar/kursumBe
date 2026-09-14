
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


namespace Business.Handlers.Tenants.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteTenantCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteTenantCommandHandler : IRequestHandler<DeleteTenantCommand, IResult>
        {
            private readonly ITenantRepository _tenantRepository;
            private readonly IMediator _mediator;

            public DeleteTenantCommandHandler(ITenantRepository tenantRepository, IMediator mediator)
            {
                _tenantRepository = tenantRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteTenantCommand request, CancellationToken cancellationToken)
            {
                var tenantToDelete = _tenantRepository.Get(p => p.Id == request.Id);

                _tenantRepository.Delete(tenantToDelete);
                await _tenantRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

