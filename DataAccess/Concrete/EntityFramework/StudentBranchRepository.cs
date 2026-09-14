
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using Core.Entities.Concrete.Project;
namespace DataAccess.Concrete.EntityFramework
{
    public class StudentBranchRepository : EfEntityRepositoryBase<StudentBranch, ProjectDbContext>, IStudentBranchRepository
    {
        public StudentBranchRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
