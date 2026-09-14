using System.Collections.Generic;
using System.Linq;
using System.Security;
using Business.Constants;
using Castle.DynamicProxy;
using Core.CrossCuttingConcerns.Caching;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using DataAccess.Abstract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Business.BusinessAspects
{
    /// <summary>
    /// This Aspect control the user's roles in HttpContext by inject the IHttpContextAccessor.
    /// It is checked by writing as [SecuredOperation] on the handler.
    /// If a valid authorization cannot be found in aspect, it throws an exception.
    /// </summary>
    public class SecuredOperation : MethodInterception
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICacheManager _cacheManager;


        public SecuredOperation()
        {
            _httpContextAccessor = ServiceTool.ServiceProvider.GetService<IHttpContextAccessor>();
            _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
        }

        protected override void OnBefore(IInvocation invocation)
        {
            var userId = _httpContextAccessor.HttpContext?.User.Claims
                .FirstOrDefault(x => x.Type.EndsWith("nameidentifier"))?.Value;

            if (userId == null)
            {
                throw new SecurityException(Messages.AuthorizationsDenied);
            }

            var operationName = invocation.TargetType.ReflectedType.Name;
            var oprClaims = _cacheManager.Get<IEnumerable<string>>($"{CacheKeys.UserIdForClaim}={userId}");

            if (oprClaims != null && oprClaims.Contains(operationName))
            {
                return;
            }

            // Önbellekte yoksa veritabanından güncel yetkileri çekip tazele
            if (int.TryParse(userId, out int uId))
            {
                var userRepository = ServiceTool.ServiceProvider.GetService<IUserRepository>();
                if (userRepository != null)
                {
                    var freshClaims = userRepository.GetClaims(uId).Select(x => x.Name).ToList();
                    _cacheManager.Add($"{CacheKeys.UserIdForClaim}={userId}", freshClaims);
                    if (freshClaims.Contains(operationName))
                    {
                        return;
                    }
                    var claimsStr = freshClaims.Any() ? string.Join(", ", freshClaims) : "HİÇ YETKİ YOK";
                    throw new SecurityException($"{Messages.AuthorizationsDenied} -> İstenen Yetki: '{operationName}', Kullanıcının DB Yetkileri: [{claimsStr}]");
                }
            }

            throw new SecurityException(Messages.AuthorizationsDenied);
        }
    }
}