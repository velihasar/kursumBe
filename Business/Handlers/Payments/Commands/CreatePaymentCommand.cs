using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Business.Handlers.Payments.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.PaymentDto;
using Core.Extensions;

namespace Business.Handlers.Payments.Commands
{
    public class CreatePaymentCommand : IRequest<IDataResult<PaymentCreateResponseDto>>
    {
        public int? TenantId { get; set; }
        public int? FeeDueId { get; set; }
        public int StudentId { get; set; }
        public int? ParentId { get; set; }
        public decimal Amount { get; set; }
        public System.DateTime PaymentDate { get; set; }
        public int PaymentType { get; set; }
        public string ReceiptNo { get; set; }
        public string TransactionId { get; set; }
        public string Notes { get; set; }

        public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, IDataResult<PaymentCreateResponseDto>>
        {
            private readonly IPaymentRepository _paymentRepository;
            private readonly IFeeDueRepository _feeDueRepository;
            private readonly IMediator _mediator;

            public CreatePaymentCommandHandler(IPaymentRepository paymentRepository, IFeeDueRepository feeDueRepository, IMediator mediator)
            {
                _paymentRepository = paymentRepository;
                _feeDueRepository = feeDueRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreatePaymentValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<PaymentCreateResponseDto>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();

                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                if (targetTenantId <= 0)
                {
                    return new ErrorDataResult<PaymentCreateResponseDto>("SuperAdmin olarak işlem yapmaktasınız. Lütfen geçerli bir kurum (okul) seçiniz.");
                }

                var addedPayment = new Payment
                {
                    TenantId = targetTenantId,
                    FeeDueId = request.FeeDueId,
                    StudentId = request.StudentId,
                    ParentId = request.ParentId,
                    Amount = request.Amount,
                    PaymentDate = request.PaymentDate,
                    PaymentType = request.PaymentType,
                    ReceiptNo = request.ReceiptNo,
                    TransactionId = request.TransactionId,
                    Notes = request.Notes,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = System.DateTime.Now
                };

                _paymentRepository.Add(addedPayment);

                // If payment is linked to a FeeDue (Tahakkuk), update FeeDue balance and status
                if (request.FeeDueId.HasValue && request.FeeDueId.Value > 0)
                {
                    var feeDue = await _feeDueRepository.GetAsync(f => f.Id == request.FeeDueId.Value && f.IsDeleted == false);
                    if (feeDue != null)
                    {
                        feeDue.PaidAmount += request.Amount;
                        feeDue.RemainingAmount = System.Math.Max(0, feeDue.Amount - feeDue.PaidAmount);
                        feeDue.Status = feeDue.RemainingAmount <= 0 ? 2 : (feeDue.PaidAmount > 0 ? 1 : 0);
                        feeDue.UpdatedBy = userId > 0 ? userId : null;
                        feeDue.UpdatedDate = System.DateTime.Now;
                        _feeDueRepository.Update(feeDue);
                    }
                }

                await _paymentRepository.SaveChangesAsync();

                var dto = new PaymentCreateResponseDto
                {
                    Id = addedPayment.Id,
                    FeeDueId = addedPayment.FeeDueId,
                    StudentId = addedPayment.StudentId,
                    ParentId = addedPayment.ParentId,
                    Amount = addedPayment.Amount,
                    PaymentDate = addedPayment.PaymentDate,
                    PaymentType = addedPayment.PaymentType,
                    ReceiptNo = addedPayment.ReceiptNo,
                    TransactionId = addedPayment.TransactionId,
                    Notes = addedPayment.Notes
                };

                return new SuccessDataResult<PaymentCreateResponseDto>(dto, Messages.Added);
            }
        }
    }
}