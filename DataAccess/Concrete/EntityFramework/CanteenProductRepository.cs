using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete.Project;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;

namespace DataAccess.Concrete.EntityFramework
{
    public class CanteenProductRepository : EfEntityRepositoryBase<CanteenProduct, ProjectDbContext>, ICanteenProductRepository
    {
        public CanteenProductRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
