using Business.Handlers.Students.Commands;
using Business.Handlers.Students.Queries;
using Core.Entities.Concrete.Project;
using Core.Extensions;
using System;
using System.Linq.Expressions;

namespace Business.Handlers.Students.FilterStudent
{
    public static class StudentFiltersHelper
    {
        public static Expression<Func<Student, bool>> GetStudentQueryFilter(GetStudentQuery request)
        {
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.Id == request.Id;
        }

        public static Expression<Func<Student, bool>> GetStudentsQueryFilter(GetStudentsQuery request)
        {
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true;
        }

        public static Expression<Func<Student, bool>> CreateStudentCommandFilter(CreateStudentCommand request)
        {
            var tenantId = UserInfoExtensions.GetTenantIdOrZero();
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.StudentNumber == request.StudentNumber &&
                (tenantId <= 0 || c.TenantId == tenantId);
        }

        public static Expression<Func<Student, bool>> UpdateStudentCommandFilter(UpdateStudentCommand request)
        {
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.Id == request.Id;
        }
    }
}
