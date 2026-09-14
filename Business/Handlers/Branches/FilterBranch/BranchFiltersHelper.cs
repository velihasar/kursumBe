using Business.Handlers.Branches.Commands;
using Business.Handlers.Branches.Queries;
using Core.Entities.Concrete.Project;
using Core.Extensions;
using System;
using System.Linq.Expressions;

namespace Business.Handlers.Branches.FilterBranch
{
    public static class BranchFiltersHelper
    {
        public static Expression<Func<Branch, bool>> GetBranchQueryFilter(GetBranchQuery request)
        {
            return c =>
                c.IsDeleted == false &&
                c.Id == request.Id;
        }

        public static Expression<Func<Branch, bool>> GetBranchesQueryFilter(GetBranchesQuery request)
        {
            return c =>
                c.IsDeleted == false;
        }

        public static Expression<Func<Branch, bool>> CreateBranchCommandFilter(CreateBranchCommand request)
        {
            var tenantId = UserInfoExtensions.GetTenantIdOrZero();
            return c =>
                c.IsDeleted == false &&
                c.Name == request.Name &&
                (tenantId <= 0 || c.TenantId == tenantId);
        }

        public static Expression<Func<Branch, bool>> UpdateBranchCommandFilter(UpdateBranchCommand request)
        {
            return c =>
                c.IsDeleted == false &&
                c.Id == request.Id;
        }
    }
}
