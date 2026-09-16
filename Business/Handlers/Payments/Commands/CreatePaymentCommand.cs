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
            private readonly IMediator _mediator;

            public CreatePaymentCommandHandler(IPaymentRepository paymentRepository, IMediator mediator)
            {
                _paymentRepository = paymentRepository;
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