using Business.Handlers.StudentParents.Commands;
using Business.Handlers.StudentParents.Queries;
using Core.Entities.Concrete.Project;
using Core.Extensions;
using System;
using System.Linq.Expressions;

namespace Business.Handlers.StudentParents.FilterStudentParent
{
    public static class StudentParentFiltersHelper
    {
        public static Expression<Func<StudentParent, bool>> GetStudentParentQueryFilter(GetStudentParentQuery request)
        {
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.Id == request.Id;
        }

        public static Expression<Func<StudentParent, bool>> GetStudentParentsQueryFilter(GetStudentParentsQuery request)
        {
            var tenantId = UserInfoExtensions.GetTenantIdOrZero();
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                (tenantId <= 0 || c.TenantId == tenantId);
        }

        public static Expression<Func<StudentParent, bool>> CreateStudentParentCommandFilter(CreateStudentParentCommand request)
        {
            var tenantId = UserInfoExtensions.GetTenantIdOrZero();
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.StudentId == request.StudentId &&
                c.ParentId == request.ParentId &&
                (tenantId <= 0 || c.TenantId == tenantId);
        }

        public static Expression<Func<StudentParent, bool>> UpdateStudentParentCommandFilter(UpdateStudentParentCommand request)
        {
            return c =>
                c.IsDeleted == false &&
                (request.Id > 0 ? c.Id == request.Id : (c.StudentId == request.StudentId && c.ParentId == request.ParentId));
        }
    }
}
