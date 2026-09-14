
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using Core.Entities.Concrete.Project;
namespace DataAccess.Concrete.EntityFramework
{
    public class BranchRepository : EfEntityRepositoryBase<Branch, ProjectDbContext>, IBranchRepository
    {
        public BranchRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
