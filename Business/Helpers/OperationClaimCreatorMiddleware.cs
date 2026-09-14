using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Fakes.Handlers.Authorizations;
using Business.Fakes.Handlers.OperationClaims;
using Business.Fakes.Handlers.UserClaims;
using Core.Entities.Concrete;
using Core.Utilities.IoC;
using DataAccess.Abstract;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Business.Helpers
{
    public static class OperationClaimCreatorMiddleware
    {
        public static async Task UseDbOperationClaimCreator(this IApplicationBuilder app)
        {
            var mediator = ServiceTool.ServiceProvider.GetService<IMediator>();
            foreach (var operationName in GetOperationNames())
            {
                await mediator.Send(new CreateOperationClaimInternalCommand
                {
                    ClaimName = operationName
                });
            }

            var operationClaims = (await mediator.Send(new GetOperationClaimsInternalQuery())).Data;
            var user = await mediator.Send(new RegisterUserInternalCommand
            {
                FullName = "System Admin",
                Password = "Q1w212*_*",
                Email = "admin@adminmail.com",
            });
            await mediator.Send(new CreateUserClaimsInternalCommand
            {
                UserId = 1,
                OperationClaims = operationClaims
            });

            // Otomatik SuperAdmin Grubu ve Yetki Ataması
            var groupRepository = ServiceTool.ServiceProvider.GetService<IGroupRepository>();
            var groupClaimRepository = ServiceTool.ServiceProvider.GetService<IGroupClaimRepository>();
            var userGroupRepository = ServiceTool.ServiceProvider.GetService<IUserGroupRepository>();

            if (groupRepository != null)
            {
                var superAdminGroup = await groupRepository.GetAsync(g => g.GroupName == "SuperAdmin" || g.GroupName == "SUPER_ADMIN");
                if (superAdminGroup == null)
                {
                    superAdminGroup = new Group { GroupName = "SuperAdmin" };
                    groupRepository.Add(superAdminGroup);
                    await groupRepository.SaveChangesAsync();
                }

                var tenantOwnerGroup = await groupRepository.GetAsync(g => g.GroupName == "KurumSahibi" || g.GroupName == "TenantAdmin" || g.GroupName == "Kurum Sahibi");
                if (tenantOwnerGroup == null)
                {
                    tenantOwnerGroup = new Group { GroupName = "KurumSahibi" };
                    groupRepository.Add(tenantOwnerGroup);
                    await groupRepository.SaveChangesAsync();
                }

                if (userGroupRepository != null)
                {
                    var userGroup = await userGroupRepository.GetAsync(ug => ug.UserId == 1 && ug.GroupId == superAdminGroup.Id);
                    if (userGroup == null)
                    {
                        userGroupRepository.Add(new UserGroup { UserId = 1, GroupId = superAdminGroup.Id });
                        await userGroupRepository.SaveChangesAsync();
                    }
                }

                if (groupClaimRepository != null && operationClaims != null && operationClaims.Any())
                {
                    var claimsList = operationClaims.Select(c => new GroupClaim { GroupId = superAdminGroup.Id, ClaimId = c.Id }).ToList();
                    await groupClaimRepository.BulkInsert(superAdminGroup.Id, claimsList);
                    await groupClaimRepository.SaveChangesAsync();
                }
            }
        }

        private static IEnumerable<string> GetOperationNames()
        {
            var assemblies = Assembly.GetExecutingAssembly().GetTypes()
                .Where(x =>
                    // runtime generated anonmous type'larin assemblysi olmadigi icin null cek yap
                    x.Namespace != null && x.Namespace.StartsWith("Business.Handlers") &&
                    (x.Name.EndsWith("Command") || x.Name.EndsWith("Query")));

            return (from assembly in assemblies
                    from nestedType in assembly.GetNestedTypes()
                    from method in nestedType.GetMethods()
                    where method.CustomAttributes.Any(u => u.AttributeType == typeof(SecuredOperation))
                    select assembly.Name).ToList();
        }
    }
}
