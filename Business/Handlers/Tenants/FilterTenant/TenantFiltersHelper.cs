using Business.Handlers.Tenants.Commands;
using Business.Handlers.Tenants.Queries;
using Core.Entities.Concrete.Project;
using System;
using System.Linq.Expressions;

namespace Business.Handlers.Tenants.FilterTenant
{
    public static class TenantFiltersHelper
    {
        public static Expression<Func<Tenant, bool>> GetTenantQueryFilter(GetTenantQuery request)
        {
            return c =>
                c.IsDeleted == false &&
                c.Id == request.Id;
        }

        public static Expression<Func<Tenant, bool>> GetTenantsQueryFilter(GetTenantsQuery request)
        {
            var userTenantId = Core.Extensions.UserInfoExtensions.GetTenantIdOrZero();
            return c =>
                c.IsDeleted == false &&
                (userTenantId == 0 || c.Id == userTenantId);
        }

        public static Expression<Func<Tenant, bool>> CreateTenantCommandFilter(CreateTenantCommand request)
        {
            return c =>
                c.IsDeleted == false &&
                c.Name == request.Name;
        }

        public static Expression<Func<Tenant, bool>> UpdateTenantCommandFilter(UpdateTenantCommand request)
        {
            return c =>
                c.IsDeleted == false &&
                c.Id == request.Id;
        }
    }
}
