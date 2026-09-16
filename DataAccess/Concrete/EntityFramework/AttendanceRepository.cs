
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using Core.Entities.Concrete.Project;
namespace DataAccess.Concrete.EntityFramework
{
    public class AttendanceRepository : EfEntityRepositoryBase<Attendance, ProjectDbContext>, IAttendanceRepository
    {
        public AttendanceRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
