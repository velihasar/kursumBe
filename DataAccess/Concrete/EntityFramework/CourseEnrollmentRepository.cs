
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using Core.Entities.Concrete.Project;
namespace DataAccess.Concrete.EntityFramework
{
    public class CourseEnrollmentRepository : EfEntityRepositoryBase<CourseEnrollment, ProjectDbContext>, ICourseEnrollmentRepository
    {
        public CourseEnrollmentRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
