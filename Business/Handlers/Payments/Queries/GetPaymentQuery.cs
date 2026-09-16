using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.PaymentDto;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Payments.Queries
{
    public class GetPaymentQuery : IRequest<IDataResult<PaymentGetByIdDto>>
    {
        public int Id { get; set; }

        public class GetPaymentQueryHandler : IRequestHandler<GetPaymentQuery, IDataResult<PaymentGetByIdDto>>
        {
            private readonly IPaymentRepository _paymentRepository;
            private readonly IMediator _mediator;

            public GetPaymentQueryHandler(IPaymentRepository paymentRepository, IMediator mediator)
            {
                _paymentRepository = paymentRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<PaymentGetByIdDto>> Handle(GetPaymentQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var query = _paymentRepository.Query()
                    .Include(x => x.Tenant)
                    .Include(x => x.Student).ThenInclude(s => s.Person)
                    .Include(x => x.Parent).ThenInclude(p => p.Person);

                var payment = await query.FirstOrDefaultAsync(p => p.Id == request.Id && p.IsDeleted == false && (userTenantId <= 0 || p.TenantId == userTenantId), cancellationToken);

                if (payment == null)
                    return new ErrorDataResult<PaymentGetByIdDto>("Kayıt bulunamadı.");

                var dto = new PaymentGetByIdDto
                {
                    Id = payment.Id,
                    TenantId = payment.TenantId,
                    TenantName = payment.Tenant != null ? payment.Tenant.Name : null,
                    FeeDueId = payment.FeeDueId,
                    StudentId = payment.StudentId,
                    StudentName = payment.Student != null && payment.Student.Person != null ? $"{payment.Student.Person.FirstName} {payment.Student.Person.LastName}".Trim() : null,
                    ParentId = payment.ParentId,
                    ParentName = payment.Parent != null && payment.Parent.Person != null ? $"{payment.Parent.Person.FirstName} {payment.Parent.Person.LastName}".Trim() : null,
                    Amount = payment.Amount,
                    PaymentDate = payment.PaymentDate,
                    PaymentType = payment.PaymentType,
                    ReceiptNo = payment.ReceiptNo,
                    TransactionId = payment.TransactionId,
                    Notes = payment.Notes,
                    IsActive = payment.IsActive
                };

                return new SuccessDataResult<PaymentGetByIdDto>(dto);
            }
        }
    }
}
