using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;

namespace DataAccess.Concrete.EntityFramework
{
    public class UserDeviceRepository : EfEntityRepositoryBase<UserDevice, ProjectDbContext>, IUserDeviceRepository
    {
        public UserDeviceRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
