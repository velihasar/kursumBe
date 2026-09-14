
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


namespace Business.Handlers.TenantUsers.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteTenantUserCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteTenantUserCommandHandler : IRequestHandler<DeleteTenantUserCommand, IResult>
        {
            private readonly ITenantUserRepository _tenantUserRepository;
            private readonly IMediator _mediator;

            public DeleteTenantUserCommandHandler(ITenantUserRepository tenantUserRepository, IMediator mediator)
            {
                _tenantUserRepository = tenantUserRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteTenantUserCommand request, CancellationToken cancellationToken)
            {
                var tenantUserToDelete = _tenantUserRepository.Get(p => p.Id == request.Id);

                _tenantUserRepository.Delete(tenantUserToDelete);
                await _tenantUserRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

