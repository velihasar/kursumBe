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
using Core.Extensions;

namespace Business.Handlers.FeeDues.Commands
{
    public class DeleteFeeDueCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteFeeDueCommandHandler : IRequestHandler<DeleteFeeDueCommand, IResult>
        {
            private readonly IFeeDueRepository _feeDueRepository;
            private readonly IMediator _mediator;

            public DeleteFeeDueCommandHandler(IFeeDueRepository feeDueRepository, IMediator mediator)
            {
                _feeDueRepository = feeDueRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteFeeDueCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();
                var feeDueToDelete = await _feeDueRepository.GetAsync(p => p.Id == request.Id && p.IsDeleted == false);

                if (feeDueToDelete == null)
                    return new ErrorResult("Kayıt bulunamadı.");

                feeDueToDelete.IsDeleted = true;
                feeDueToDelete.DeletedBy = userId > 0 ? userId : null;
                feeDueToDelete.DeletedDate = System.DateTime.Now;

                _feeDueRepository.Update(feeDueToDelete);
                await _feeDueRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}
