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
            private readonly IMediator _mediator;

            public DeletePaymentCommandHandler(IPaymentRepository paymentRepository, IMediator mediator)
            {
                _paymentRepository = paymentRepository;
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
                await _paymentRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}
