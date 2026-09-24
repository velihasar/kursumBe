using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.StudentWalletDto;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.StudentWallets.Queries
{
    public class GetStudentWalletTransactionsQuery : IRequest<IDataResult<IEnumerable<StudentWalletTransactionGetAllDto>>>
    {
        public int? TenantId { get; set; }
        public int? StudentId { get; set; }
        public int? StudentWalletId { get; set; }
        public int? TransactionType { get; set; }
        public string Category { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public class GetStudentWalletTransactionsQueryHandler : IRequestHandler<GetStudentWalletTransactionsQuery, IDataResult<IEnumerable<StudentWalletTransactionGetAllDto>>>
        {
            private readonly IStudentWalletTransactionRepository _transactionRepository;

            public GetStudentWalletTransactionsQueryHandler(IStudentWalletTransactionRepository transactionRepository)
            {
                _transactionRepository = transactionRepository;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<StudentWalletTransactionGetAllDto>>> Handle(GetStudentWalletTransactionsQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                var query = _transactionRepository.Query()
                    .Include(t => t.Tenant)
                    .Include(t => t.Student).ThenInclude(s => s.Person)
                    .Where(t => t.IsDeleted == false);

                if (targetTenantId > 0)
                {
                    query = query.Where(t => t.TenantId == targetTenantId);
                }

                if (request.StudentId.HasValue && request.StudentId.Value > 0)
                {
                    query = query.Where(t => t.StudentId == request.StudentId.Value);
                }

                if (request.StudentWalletId.HasValue && request.StudentWalletId.Value > 0)
                {
                    query = query.Where(t => t.StudentWalletId == request.StudentWalletId.Value);
                }

                if (request.TransactionType.HasValue && request.TransactionType.Value > 0)
                {
                    query = query.Where(t => t.TransactionType == request.TransactionType.Value);
                }

                if (!string.IsNullOrWhiteSpace(request.Category))
                {
                    query = query.Where(t => t.Category == request.Category);
                }

                if (request.StartDate.HasValue)
                {
                    query = query.Where(t => t.TransactionDate >= request.StartDate.Value);
                }

                if (request.EndDate.HasValue)
                {
                    query = query.Where(t => t.TransactionDate <= request.EndDate.Value);
                }

                var list = await query.OrderByDescending(t => t.TransactionDate).ToListAsync(cancellationToken);

                var dtos = list.Select(t => new StudentWalletTransactionGetAllDto
                {
                    Id = t.Id,
                    TenantId = t.TenantId,
                    TenantName = t.Tenant != null ? t.Tenant.Name : null,
                    StudentWalletId = t.StudentWalletId,
                    StudentId = t.StudentId,
                    StudentName = t.Student != null && t.Student.Person != null ? $"{t.Student.Person.FirstName} {t.Student.Person.LastName}".Trim() : null,
                    StudentNumber = t.Student != null ? t.Student.StudentNumber : null,
                    TransactionType = t.TransactionType,
                    TransactionTypeName = t.TransactionType == 1 ? "Bakiye Yükleme" : (t.TransactionType == 2 ? "Harcama" : "İade"),
                    Amount = t.Amount,
                    BalanceBefore = t.BalanceBefore,
                    BalanceAfter = t.BalanceAfter,
                    Category = t.Category,
                    Description = t.Description,
                    PaymentType = t.PaymentType,
                    PaymentTypeName = t.PaymentType == 1 ? "Nakit" : (t.PaymentType == 2 ? "Kredi Kartı" : (t.PaymentType == 3 ? "Havale/EFT" : "Diğer")),
                    ReceiptNo = t.ReceiptNo,
                    TransactionDate = t.TransactionDate,
                    IsActive = t.IsActive
                });

                return new SuccessDataResult<IEnumerable<StudentWalletTransactionGetAllDto>>(dtos);
            }
        }
    }
}
