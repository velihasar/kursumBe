using Business.BusinessAspects;
using Core.Aspects.Autofac.Performance;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Aspects.Autofac.Caching;
using Core.Entities.Dtos.PaymentDto;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Payments.Queries
{
    public class GetPaymentsQuery : IRequest<IDataResult<IEnumerable<PaymentGetAllDto>>>
    {
        public int? TenantId { get; set; }

        public class GetPaymentsQueryHandler : IRequestHandler<GetPaymentsQuery, IDataResult<IEnumerable<PaymentGetAllDto>>>
        {
            private readonly IPaymentRepository _paymentRepository;
            private readonly IMediator _mediator;

            public GetPaymentsQueryHandler(IPaymentRepository paymentRepository, IMediator mediator)
            {
                _paymentRepository = paymentRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<PaymentGetAllDto>>> Handle(GetPaymentsQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var query = _paymentRepository.Query()
                    .Include(x => x.Tenant)
                    .Include(x => x.Student).ThenInclude(s => s.Person)
                    .Include(x => x.Parent).ThenInclude(p => p.Person)
                    .Where(x => x.IsDeleted == false);

                if (userTenantId > 0)
                {
                    query = query.Where(x => x.TenantId == userTenantId);
                }
                else if (request.TenantId.HasValue && request.TenantId.Value > 0)
                {
                    query = query.Where(x => x.TenantId == request.TenantId.Value);
                }

                var list = await query.ToListAsync(cancellationToken);
                var dtos = list.Select(x => new PaymentGetAllDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    TenantName = x.Tenant != null ? x.Tenant.Name : null,
                    FeeDueId = x.FeeDueId,
                    StudentId = x.StudentId,
                    StudentName = x.Student != null && x.Student.Person != null ? $"{x.Student.Person.FirstName} {x.Student.Person.LastName}".Trim() : null,
                    ParentId = x.ParentId,
                    ParentName = x.Parent != null && x.Parent.Person != null ? $"{x.Parent.Person.FirstName} {x.Parent.Person.LastName}".Trim() : null,
                    Amount = x.Amount,
                    PaymentDate = x.PaymentDate,
                    PaymentType = x.PaymentType,
                    ReceiptNo = x.ReceiptNo,
                    TransactionId = x.TransactionId,
                    Notes = x.Notes,
                    IsActive = x.IsActive
                });

                return new SuccessDataResult<IEnumerable<PaymentGetAllDto>>(dtos);
            }
        }
    }
}