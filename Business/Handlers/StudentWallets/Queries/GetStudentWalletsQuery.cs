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
    public class GetStudentWalletsQuery : IRequest<IDataResult<IEnumerable<StudentWalletGetAllDto>>>
    {
        public int? TenantId { get; set; }
        public string SearchTerm { get; set; }

        public class GetStudentWalletsQueryHandler : IRequestHandler<GetStudentWalletsQuery, IDataResult<IEnumerable<StudentWalletGetAllDto>>>
        {
            private readonly IStudentWalletRepository _studentWalletRepository;
            private readonly IStudentRepository _studentRepository;

            public GetStudentWalletsQueryHandler(
                IStudentWalletRepository studentWalletRepository,
                IStudentRepository studentRepository)
            {
                _studentWalletRepository = studentWalletRepository;
                _studentRepository = studentRepository;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<StudentWalletGetAllDto>>> Handle(GetStudentWalletsQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                var query = _studentWalletRepository.Query()
                    .Include(x => x.Tenant)
                    .Include(x => x.Student).ThenInclude(s => s.Person)
                    .Where(x => x.IsDeleted == false);

                if (targetTenantId > 0)
                {
                    query = query.Where(x => x.TenantId == targetTenantId);
                }

                var list = await query.ToListAsync(cancellationToken);

                var dtos = list.Select(x => new StudentWalletGetAllDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    TenantName = x.Tenant != null ? x.Tenant.Name : null,
                    StudentId = x.StudentId,
                    StudentName = x.Student != null && x.Student.Person != null ? $"{x.Student.Person.FirstName} {x.Student.Person.LastName}".Trim() : null,
                    StudentNumber = x.Student != null ? x.Student.StudentNumber : null,
                    Balance = x.Balance,
                    TotalDeposited = x.TotalDeposited,
                    TotalSpent = x.TotalSpent,
                    LastTransactionDate = x.LastTransactionDate,
                    IsActive = x.IsActive
                });

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var term = request.SearchTerm.Trim().ToLower();
                    dtos = dtos.Where(x => (x.StudentName != null && x.StudentName.ToLower().Contains(term)) ||
                                           (x.StudentNumber != null && x.StudentNumber.ToLower().Contains(term)));
                }

                return new SuccessDataResult<IEnumerable<StudentWalletGetAllDto>>(dtos.OrderByDescending(x => x.LastTransactionDate ?? System.DateTime.MinValue));
            }
        }
    }
}
