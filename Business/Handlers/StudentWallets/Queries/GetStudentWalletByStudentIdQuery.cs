using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.StudentWalletDto;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.StudentWallets.Queries
{
    public class GetStudentWalletByStudentIdQuery : IRequest<IDataResult<StudentWalletGetByIdDto>>
    {
        public int StudentId { get; set; }

        public class GetStudentWalletByStudentIdQueryHandler : IRequestHandler<GetStudentWalletByStudentIdQuery, IDataResult<StudentWalletGetByIdDto>>
        {
            private readonly IStudentWalletRepository _studentWalletRepository;
            private readonly IStudentWalletTransactionRepository _transactionRepository;
            private readonly IStudentRepository _studentRepository;

            public GetStudentWalletByStudentIdQueryHandler(
                IStudentWalletRepository studentWalletRepository,
                IStudentWalletTransactionRepository transactionRepository,
                IStudentRepository studentRepository)
            {
                _studentWalletRepository = studentWalletRepository;
                _transactionRepository = transactionRepository;
                _studentRepository = studentRepository;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<StudentWalletGetByIdDto>> Handle(GetStudentWalletByStudentIdQuery request, CancellationToken cancellationToken)
            {
                var student = await _studentRepository.Query()
                    .Include(s => s.Person)
                    .Include(s => s.Tenant)
                    .FirstOrDefaultAsync(s => s.Id == request.StudentId && s.IsDeleted == false, cancellationToken);

                if (student == null)
                {
                    return new ErrorDataResult<StudentWalletGetByIdDto>("Öğrenci bulunamadı.");
                }

                var wallet = await _studentWalletRepository.Query()
                    .Include(w => w.Tenant)
                    .FirstOrDefaultAsync(w => w.StudentId == request.StudentId && w.IsDeleted == false, cancellationToken);

                var transactions = await _transactionRepository.Query()
                    .Include(t => t.Tenant)
                    .Where(t => t.StudentId == request.StudentId && t.IsDeleted == false)
                    .OrderByDescending(t => t.TransactionDate)
                    .ToListAsync(cancellationToken);

                var transactionDtos = transactions.Select(t => new StudentWalletTransactionGetAllDto
                {
                    Id = t.Id,
                    TenantId = t.TenantId,
                    TenantName = t.Tenant != null ? t.Tenant.Name : null,
                    StudentWalletId = t.StudentWalletId,
                    StudentId = t.StudentId,
                    StudentName = student.Person != null ? $"{student.Person.FirstName} {student.Person.LastName}".Trim() : null,
                    StudentNumber = student.StudentNumber,
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
                }).ToList();

                var dto = new StudentWalletGetByIdDto
                {
                    Id = wallet?.Id ?? 0,
                    TenantId = student.TenantId,
                    TenantName = student.Tenant?.Name,
                    StudentId = student.Id,
                    StudentName = student.Person != null ? $"{student.Person.FirstName} {student.Person.LastName}".Trim() : null,
                    StudentNumber = student.StudentNumber,
                    Balance = wallet?.Balance ?? 0,
                    TotalDeposited = wallet?.TotalDeposited ?? 0,
                    TotalSpent = wallet?.TotalSpent ?? 0,
                    LastTransactionDate = wallet?.LastTransactionDate,
                    IsActive = wallet?.IsActive ?? true,
                    Transactions = transactionDtos
                };

                return new SuccessDataResult<StudentWalletGetByIdDto>(dto);
            }
        }
    }
}
