
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using Core.Entities.Concrete.Project;
namespace DataAccess.Concrete.EntityFramework
{
    public class TeacherRepository : EfEntityRepositoryBase<Teacher, ProjectDbContext>, ITeacherRepository
    {
        public TeacherRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
