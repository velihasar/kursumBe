
using System;
using System.Linq;
using Core.DataAccess.EntityFramework;
using Entities.Concrete;
using DataAccess.Concrete.EntityFramework.Contexts;
using DataAccess.Abstract;
using Core.Entities.Concrete.Project;
namespace DataAccess.Concrete.EntityFramework
{
    public class ParentRepository : EfEntityRepositoryBase<Parent, ProjectDbContext>, IParentRepository
    {
        public ParentRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
