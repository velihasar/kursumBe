using System;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.StudentWallets.Commands
{
    public class DeleteStudentWalletTransactionCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteStudentWalletTransactionCommandHandler : IRequestHandler<DeleteStudentWalletTransactionCommand, IResult>
        {
            private readonly IStudentWalletTransactionRepository _transactionRepository;
            private readonly IStudentWalletRepository _studentWalletRepository;

            public DeleteStudentWalletTransactionCommandHandler(
                IStudentWalletTransactionRepository transactionRepository,
                IStudentWalletRepository studentWalletRepository)
            {
                _transactionRepository = transactionRepository;
                _studentWalletRepository = studentWalletRepository;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteStudentWalletTransactionCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();

                var transaction = await _transactionRepository.GetAsync(t => t.Id == request.Id && t.IsDeleted == false);
                if (transaction == null)
                {
                    return new ErrorResult("İşlem kaydı bulunamadı.");
                }

                var wallet = await _studentWalletRepository.GetAsync(w => w.Id == transaction.StudentWalletId && w.IsDeleted == false);
                if (wallet != null)
                {
                    // Rollback balance adjustment
                    if (transaction.TransactionType == 2) // Harcama iptali -> bakiye geri yüklenir
                    {
                        wallet.Balance += transaction.Amount;
                        wallet.TotalSpent = Math.Max(0, wallet.TotalSpent - transaction.Amount);
                    }
                    else if (transaction.TransactionType == 1) // Yükleme iptali -> bakiye düşülür
                    {
                        if (wallet.Balance < transaction.Amount)
                        {
                            return new ErrorResult($"Bakiye yüklemesi iptal edilemez çünkü mevcut bakiye (₺{wallet.Balance:N2}) bu tutardan daha azdır.");
                        }
                        wallet.Balance -= transaction.Amount;
                        wallet.TotalDeposited = Math.Max(0, wallet.TotalDeposited - transaction.Amount);
                    }

                    wallet.UpdatedBy = userId > 0 ? userId : null;
                    wallet.UpdatedDate = DateTime.Now;
                    _studentWalletRepository.Update(wallet);
                }

                transaction.IsDeleted = true;
                transaction.UpdatedBy = userId > 0 ? userId : null;
                transaction.UpdatedDate = DateTime.Now;

                _transactionRepository.Update(transaction);
                await _transactionRepository.SaveChangesAsync();

                return new SuccessResult("İşlem başarıyla iptal edildi.");
            }
        }
    }
}
