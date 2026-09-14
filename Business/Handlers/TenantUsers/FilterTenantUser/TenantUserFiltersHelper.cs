using Business.Handlers.TenantUsers.Commands;
using Business.Handlers.TenantUsers.Queries;
using Core.Entities.Concrete.Project;
using Core.Extensions;
using System;
using System.Linq.Expressions;

namespace Business.Handlers.TenantUsers.FilterTenantUser
{
    public static class TenantUserFiltersHelper
    {
        public static Expression<Func<TenantUser, bool>> GetTenantUserQueryFilter(GetTenantUserQuery request)
        {
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.Id == request.Id;
        }

        public static Expression<Func<TenantUser, bool>> GetTenantUsersQueryFilter(GetTenantUsersQuery request)
        {
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true;
        }

        public static Expression<Func<TenantUser, bool>> CreateTenantUserCommandFilter(CreateTenantUserCommand request)
        {
            var tenantId = UserInfoExtensions.GetTenantIdOrZero();
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.UserId == request.UserId &&
                (tenantId <= 0 || c.TenantId == tenantId);
        }

        public static Expression<Func<TenantUser, bool>> UpdateTenantUserCommandFilter(UpdateTenantUserCommand request)
        {
            return c =>
                c.IsDeleted == false &&
                c.IsActive == true &&
                c.Id == request.Id;
        }
    }
}
