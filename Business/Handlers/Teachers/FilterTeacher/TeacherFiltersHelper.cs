using Business.Handlers.Teachers.Commands;
using Business.Handlers.Teachers.Queries;
using Core.Entities.Concrete.Project;
using Core.Extensions;
using System;
using System.Linq.Expressions;

namespace Business.Handlers.Teachers.FilterTeacher
{
    public static class TeacherFiltersHelper
    {
        public static Expression<Func<Teacher, bool>> GetTeacherQueryFilter(GetTeacherQuery request)
        {
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.Id == request.Id;
        }

        public static Expression<Func<Teacher, bool>> GetTeachersQueryFilter(GetTeachersQuery request)
        {
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true;
        }

        public static Expression<Func<Teacher, bool>> CreateTeacherCommandFilter(CreateTeacherCommand request)
        {
            var tenantId = UserInfoExtensions.GetTenantIdOrZero();
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.PersonId == request.PersonId &&
                (tenantId <= 0 || c.TenantId == tenantId);
        }

        public static Expression<Func<Teacher, bool>> UpdateTeacherCommandFilter(UpdateTeacherCommand request)
        {
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.Id == request.Id;
        }
    }
}
