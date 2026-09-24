using System;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.StudentWallets.ValidationRules;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.StudentWalletDto;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.StudentWallets.Commands
{
    public class DepositStudentWalletCommand : IRequest<IDataResult<StudentWalletGetAllDto>>
    {
        public int? TenantId { get; set; }
        public int StudentId { get; set; }
        public decimal Amount { get; set; }
        public int PaymentType { get; set; } = 1; // 1: Nakit, 2: Kredi Kartı, 3: Havale/EFT, 4: Diğer
        public string Description { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime? TransactionDate { get; set; }

        public class DepositStudentWalletCommandHandler : IRequestHandler<DepositStudentWalletCommand, IDataResult<StudentWalletGetAllDto>>
        {
            private readonly IStudentWalletRepository _studentWalletRepository;
            private readonly IStudentWalletTransactionRepository _transactionRepository;
            private readonly IStudentRepository _studentRepository;

            public DepositStudentWalletCommandHandler(
                IStudentWalletRepository studentWalletRepository,
                IStudentWalletTransactionRepository transactionRepository,
                IStudentRepository studentRepository)
            {
                _studentWalletRepository = studentWalletRepository;
                _transactionRepository = transactionRepository;
                _studentRepository = studentRepository;
            }

            [ValidationAspect(typeof(DepositStudentWalletValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<StudentWalletGetAllDto>> Handle(DepositStudentWalletCommand request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();
                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                var student = await _studentRepository.Query()
                    .Include(s => s.Person)
                    .FirstOrDefaultAsync(s => s.Id == request.StudentId && s.IsDeleted == false, cancellationToken);

                if (student == null)
                {
                    return new ErrorDataResult<StudentWalletGetAllDto>("Öğrenci bulunamadı.");
                }

                if (targetTenantId <= 0)
                {
                    targetTenantId = student.TenantId;
                }

                var wallet = await _studentWalletRepository.Query()
                    .FirstOrDefaultAsync(w => w.StudentId == request.StudentId && w.IsDeleted == false, cancellationToken);

                var txDate = request.TransactionDate ?? DateTime.Now;

                if (wallet == null)
                {
                    wallet = new StudentWallet
                    {
                        TenantId = targetTenantId,
                        StudentId = request.StudentId,
                        Balance = 0,
                        TotalDeposited = 0,
                        TotalSpent = 0,
                        LastTransactionDate = txDate,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = userId > 0 ? userId : null,
                        CreatedDate = DateTime.Now
                    };
                    _studentWalletRepository.Add(wallet);
                    await _studentWalletRepository.SaveChangesAsync();
                }

                decimal balanceBefore = wallet.Balance;
                wallet.Balance += request.Amount;
                wallet.TotalDeposited += request.Amount;
                wallet.LastTransactionDate = txDate;
                wallet.UpdatedBy = userId > 0 ? userId : null;
                wallet.UpdatedDate = DateTime.Now;

                _studentWalletRepository.Update(wallet);

                var transaction = new StudentWalletTransaction
                {
                    TenantId = targetTenantId,
                    StudentWalletId = wallet.Id,
                    StudentId = request.StudentId,
                    TransactionType = 1, // 1: Bakiye Yükleme
                    Amount = request.Amount,
                    BalanceBefore = balanceBefore,
                    BalanceAfter = wallet.Balance,
                    Category = "Bakiye Yükleme",
                    Description = string.IsNullOrWhiteSpace(request.Description) ? "Kantin/Dolap bakiye yüklemesi" : request.Description,
                    PaymentType = request.PaymentType,
                    ReceiptNo = request.ReceiptNo,
                    TransactionDate = txDate,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = DateTime.Now
                };

                _transactionRepository.Add(transaction);
                await _transactionRepository.SaveChangesAsync();

                var dto = new StudentWalletGetAllDto
                {
                    Id = wallet.Id,
                    TenantId = wallet.TenantId,
                    StudentId = wallet.StudentId,
                    StudentName = student.Person != null ? $"{student.Person.FirstName} {student.Person.LastName}".Trim() : null,
                    StudentNumber = student.StudentNumber,
                    Balance = wallet.Balance,
                    TotalDeposited = wallet.TotalDeposited,
                    TotalSpent = wallet.TotalSpent,
                    LastTransactionDate = wallet.LastTransactionDate,
                    IsActive = wallet.IsActive
                };

                return new SuccessDataResult<StudentWalletGetAllDto>(dto, "Bakiye başarıyla yüklendi.");
            }
        }
    }
}
