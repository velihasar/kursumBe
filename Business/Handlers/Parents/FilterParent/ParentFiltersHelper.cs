using Business.Handlers.Parents.Commands;
using Business.Handlers.Parents.Queries;
using Core.Entities.Concrete.Project;
using Core.Extensions;
using System;
using System.Linq.Expressions;

namespace Business.Handlers.Parents.FilterParent
{
    public static class ParentFiltersHelper
    {
        public static Expression<Func<Parent, bool>> GetParentQueryFilter(GetParentQuery request)
        {
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.Id == request.Id;
        }

        public static Expression<Func<Parent, bool>> GetParentsQueryFilter(GetParentsQuery request)
        {
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true;
        }

        public static Expression<Func<Parent, bool>> CreateParentCommandFilter(CreateParentCommand request)
        {
            var tenantId = UserInfoExtensions.GetTenantIdOrZero();
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.PersonId == request.PersonId &&
                (tenantId <= 0 || c.TenantId == tenantId);
        }

        public static Expression<Func<Parent, bool>> UpdateParentCommandFilter(UpdateParentCommand request)
        {
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.Id == request.Id;
        }
    }
}
