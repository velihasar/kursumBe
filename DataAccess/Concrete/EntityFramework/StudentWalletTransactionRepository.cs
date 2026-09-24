using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete.Project;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;

namespace DataAccess.Concrete.EntityFramework
{
    public class StudentWalletTransactionRepository : EfEntityRepositoryBase<StudentWalletTransaction, ProjectDbContext>, IStudentWalletTransactionRepository
    {
        public StudentWalletTransactionRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
