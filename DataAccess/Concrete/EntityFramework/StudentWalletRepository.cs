using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete.Project;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;

namespace DataAccess.Concrete.EntityFramework
{
    public class StudentWalletRepository : EfEntityRepositoryBase<StudentWallet, ProjectDbContext>, IStudentWalletRepository
    {
        public StudentWalletRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
