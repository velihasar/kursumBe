using System;
using System.Linq;
using Core.Extensions;
using Core.Utilities.IoC;
using DataAccess.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Helpers
{
    public static class SecurityHelper
    {
        public static bool IsSuperAdmin()
        {
            try
            {
                var httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
                var httpContext = httpContextAccessor?.HttpContext;
                if (httpContext == null || httpContext.User == null)
                {
                    return false;
                }

                // 1. Check Claims from Token
                var claims = httpContext.User.Claims.ToList();
                var hasSuperClaim = claims.Any(c =>
                    c.Value.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) ||
                    c.Value.Equals("SUPER_ADMIN", StringComparison.OrdinalIgnoreCase) ||
                    (c.Type.EndsWith("role", StringComparison.OrdinalIgnoreCase) &&
                     (c.Value.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) || c.Value.Equals("SUPER_ADMIN", StringComparison.OrdinalIgnoreCase))) ||
                    (c.Type.Equals("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", StringComparison.OrdinalIgnoreCase) &&
                     (c.Value.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) || c.Value.Equals("SUPER_ADMIN", StringComparison.OrdinalIgnoreCase)))
                );

                if (hasSuperClaim)
                {
                    return true;
                }

                // 2. Check Database Claims
                var userId = UserInfoExtensions.GetUserIdOrZero();
                if (userId > 0)
                {
                    var userRepo = ServiceTool.ServiceProvider.GetService<IUserRepository>();
                    if (userRepo != null)
                    {
                        var userClaims = userRepo.GetClaims(userId);
                        if (userClaims != null && userClaims.Any(c =>
                            c.Name != null && (c.Name.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) || c.Name.Equals("SUPER_ADMIN", StringComparison.OrdinalIgnoreCase))))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsSuperAdminGroup(int groupId)
        {
            try
            {
                var groupRepo = ServiceTool.ServiceProvider.GetService<IGroupRepository>();
                if (groupRepo != null)
                {
                    var group = groupRepo.Get(g => g.Id == groupId);
                    if (group != null && (group.GroupName.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) || group.GroupName.Equals("SUPER_ADMIN", StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool ContainsSuperAdminGroup(int[] groupIds)
        {
            if (groupIds == null || groupIds.Length == 0) return false;
            try
            {
                var groupRepo = ServiceTool.ServiceProvider.GetService<IGroupRepository>();
                if (groupRepo != null)
                {
                    return groupRepo.Query().Any(g =>
                        groupIds.Contains(g.Id) &&
                        (g.GroupName.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) || g.GroupName.Equals("SUPER_ADMIN", StringComparison.OrdinalIgnoreCase))
                    );
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool IsSuperAdminClaim(int claimId)
        {
            try
            {
                var claimRepo = ServiceTool.ServiceProvider.GetService<IOperationClaimRepository>();
                if (claimRepo != null)
                {
                    var claim = claimRepo.Get(c => c.Id == claimId);
                    if (claim != null && (claim.Name.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) || claim.Name.Equals("SUPER_ADMIN", StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool ContainsSuperAdminClaim(int[] claimIds)
        {
            if (claimIds == null || claimIds.Length == 0) return false;
            try
            {
                var claimRepo = ServiceTool.ServiceProvider.GetService<IOperationClaimRepository>();
                if (claimRepo != null)
                {
                    return claimRepo.Query().Any(c =>
                        claimIds.Contains(c.Id) &&
                        (c.Name.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) || c.Name.Equals("SUPER_ADMIN", StringComparison.OrdinalIgnoreCase))
                    );
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}
