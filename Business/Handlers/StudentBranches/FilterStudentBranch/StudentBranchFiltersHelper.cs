using Business.Handlers.StudentBranches.Commands;
using Business.Handlers.StudentBranches.Queries;
using Core.Entities.Concrete.Project;
using Core.Extensions;
using System;
using System.Linq.Expressions;

namespace Business.Handlers.StudentBranches.FilterStudentBranch
{
    public static class StudentBranchFiltersHelper
    {
        public static Expression<Func<StudentBranch, bool>> GetStudentBranchQueryFilter(GetStudentBranchQuery request)
        {
            return c =>
                (c.IsDeleted == null || c.IsDeleted == false) &&
                c.StudentId == request.StudentId &&
                c.BranchId == request.BranchId;
        }

        public static Expression<Func<StudentBranch, bool>> GetStudentBranchesQueryFilter(GetStudentBranchesQuery request)
        {
            var tenantId = UserInfoExtensions.GetTenantIdOrZero();
            return c =>
                (c.IsDeleted == null || c.IsDeleted == false) &&
                (tenantId <= 0 || c.TenantId == tenantId) &&
                (!request.StudentId.HasValue || c.StudentId == request.StudentId.Value) &&
                (!request.BranchId.HasValue || c.BranchId == request.BranchId.Value);
        }

        public static Expression<Func<StudentBranch, bool>> CreateStudentBranchCommandFilter(CreateStudentBranchCommand request)
        {
            var tenantId = UserInfoExtensions.GetTenantIdOrZero();
            return c =>
                (c.IsDeleted == null || c.IsDeleted == false) &&
                c.StudentId == request.StudentId &&
                c.BranchId == request.BranchId &&
                (tenantId <= 0 || c.TenantId == tenantId);
        }

        public static Expression<Func<StudentBranch, bool>> UpdateStudentBranchCommandFilter(UpdateStudentBranchCommand request)
        {
            return c =>
                (c.IsDeleted == null || c.IsDeleted == false) &&
                c.StudentId == request.StudentId &&
                c.BranchId == request.BranchId;
        }
    }
}
