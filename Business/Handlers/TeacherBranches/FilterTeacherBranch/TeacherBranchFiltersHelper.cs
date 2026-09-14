using Business.Handlers.TeacherBranches.Commands;
using Business.Handlers.TeacherBranches.Queries;
using Core.Entities.Concrete.Project;
using Core.Extensions;
using System;
using System.Linq.Expressions;

namespace Business.Handlers.TeacherBranches.FilterTeacherBranch
{
    public static class TeacherBranchFiltersHelper
    {
        public static Expression<Func<TeacherBranch, bool>> GetTeacherBranchQueryFilter(GetTeacherBranchQuery request)
        {
            return c =>
                (c.IsDeleted == null || c.IsDeleted == false) &&
                c.TeacherId == request.TeacherId &&
                c.BranchId == request.BranchId;
        }

        public static Expression<Func<TeacherBranch, bool>> GetTeacherBranchesQueryFilter(GetTeacherBranchesQuery request)
        {
            var tenantId = UserInfoExtensions.GetTenantIdOrZero();
            return c =>
                (c.IsDeleted == null || c.IsDeleted == false) &&
                (tenantId <= 0 || c.TenantId == tenantId) &&
                (!request.TeacherId.HasValue || c.TeacherId == request.TeacherId.Value) &&
                (!request.BranchId.HasValue || c.BranchId == request.BranchId.Value);
        }

        public static Expression<Func<TeacherBranch, bool>> CreateTeacherBranchCommandFilter(CreateTeacherBranchCommand request)
        {
            var tenantId = UserInfoExtensions.GetTenantIdOrZero();
            return c =>
                (c.IsDeleted == null || c.IsDeleted == false) &&
                c.TeacherId == request.TeacherId &&
                c.BranchId == request.BranchId &&
                (tenantId <= 0 || c.TenantId == tenantId);
        }

        public static Expression<Func<TeacherBranch, bool>> UpdateTeacherBranchCommandFilter(UpdateTeacherBranchCommand request)
        {
            return c =>
                (c.IsDeleted == null || c.IsDeleted == false) &&
                c.TeacherId == request.TeacherId &&
                c.BranchId == request.BranchId;
        }
    }
}
