using Business.Constants;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Validation;
using Business.Handlers.Payments.ValidationRules;
using Core.Entities.Dtos.PaymentDto;
using Core.Extensions;

namespace Business.Handlers.Payments.Commands
{
    public class UpdatePaymentCommand : IRequest<IDataResult<PaymentUpdateResponseDto>>
    {
        public int Id { get; set; }
        public int? FeeDueId { get; set; }
        public int StudentId { get; set; }
        public int? ParentId { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime PaymentDate { get; set; }
        public int PaymentType { get; set; }
        public string ReceiptNo { get; set; }
        public string TransactionId { get; set; }
        public string Notes { get; set; }

        public class UpdatePaymentCommandHandler : IRequestHandler<UpdatePaymentCommand, IDataResult<PaymentUpdateResponseDto>>
        {
            private readonly IPaymentRepository _paymentRepository;
            private readonly IMediator _mediator;

            public UpdatePaymentCommandHandler(IPaymentRepository paymentRepository, IMediator mediator)
            {
                _paymentRepository = paymentRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdatePaymentValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<PaymentUpdateResponseDto>> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();
                var isTherePaymentRecord = await _paymentRepository.GetAsync(u => u.Id == request.Id && u.IsDeleted == false);

                if (isTherePaymentRecord == null)
                    return new ErrorDataResult<PaymentUpdateResponseDto>("Kayıt bulunamadı.");

                isTherePaymentRecord.FeeDueId = request.FeeDueId;
                isTherePaymentRecord.StudentId = request.StudentId;
                isTherePaymentRecord.ParentId = request.ParentId;
                isTherePaymentRecord.Amount = request.Amount;
                isTherePaymentRecord.PaymentDate = request.PaymentDate;
                isTherePaymentRecord.PaymentType = request.PaymentType;
                isTherePaymentRecord.ReceiptNo = request.ReceiptNo;
                isTherePaymentRecord.TransactionId = request.TransactionId;
                isTherePaymentRecord.Notes = request.Notes;
                isTherePaymentRecord.UpdatedBy = userId > 0 ? userId : null;
                isTherePaymentRecord.UpdatedDate = System.DateTime.Now;

                _paymentRepository.Update(isTherePaymentRecord);
                await _paymentRepository.SaveChangesAsync();

                var dto = new PaymentUpdateResponseDto
                {
                    Id = isTherePaymentRecord.Id,
                    FeeDueId = isTherePaymentRecord.FeeDueId,
                    StudentId = isTherePaymentRecord.StudentId,
                    ParentId = isTherePaymentRecord.ParentId,
                    Amount = isTherePaymentRecord.Amount,
                    PaymentDate = isTherePaymentRecord.PaymentDate,
                    PaymentType = isTherePaymentRecord.PaymentType,
                    ReceiptNo = isTherePaymentRecord.ReceiptNo,
                    TransactionId = isTherePaymentRecord.TransactionId,
                    Notes = isTherePaymentRecord.Notes
                };

                return new SuccessDataResult<PaymentUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}
