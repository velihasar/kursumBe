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

namespace Business.Handlers.Payments.Commands
{
    public class DeletePaymentCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeletePaymentCommandHandler : IRequestHandler<DeletePaymentCommand, IResult>
        {
            private readonly IPaymentRepository _paymentRepository;
            private readonly IFeeDueRepository _feeDueRepository;
            private readonly IMediator _mediator;

            public DeletePaymentCommandHandler(IPaymentRepository paymentRepository, IFeeDueRepository feeDueRepository, IMediator mediator)
            {
                _paymentRepository = paymentRepository;
                _feeDueRepository = feeDueRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();
                var paymentToDelete = await _paymentRepository.GetAsync(p => p.Id == request.Id && p.IsDeleted == false);

                if (paymentToDelete == null)
                    return new ErrorResult("Kayıt bulunamadı.");

                paymentToDelete.IsDeleted = true;
                paymentToDelete.DeletedBy = userId > 0 ? userId : null;
                paymentToDelete.DeletedDate = System.DateTime.Now;

                _paymentRepository.Update(paymentToDelete);

                // Restore FeeDue balance if linked
                if (paymentToDelete.FeeDueId.HasValue && paymentToDelete.FeeDueId.Value > 0)
                {
                    var feeDue = await _feeDueRepository.GetAsync(f => f.Id == paymentToDelete.FeeDueId.Value && f.IsDeleted == false);
                    if (feeDue != null)
                    {
                        feeDue.PaidAmount = System.Math.Max(0, feeDue.PaidAmount - paymentToDelete.Amount);
                        feeDue.RemainingAmount = System.Math.Max(0, feeDue.Amount - feeDue.PaidAmount);
                        feeDue.Status = feeDue.RemainingAmount <= 0 ? 2 : (feeDue.PaidAmount > 0 ? 1 : 0);
                        feeDue.UpdatedBy = userId > 0 ? userId : null;
                        feeDue.UpdatedDate = System.DateTime.Now;
                        _feeDueRepository.Update(feeDue);
                    }
                }

                await _paymentRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}
