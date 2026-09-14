using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using Core.Entities.Concrete;
using Core.Entities.Concrete.Project;
using Core.Extensions;
using Core.Utilities.Security.Encyption;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;


namespace Core.Utilities.Security.Jwt
{
    public class JwtHelper : ITokenHelper
    {
        private readonly TokenOptions _tokenOptions;
        private DateTime _accessTokenExpiration;

        public JwtHelper(IConfiguration configuration)
        {
            Configuration = configuration;
            _tokenOptions = Configuration.GetSection("TokenOptions").Get<TokenOptions>();
        }

        public IConfiguration Configuration { get; }

        public static string DecodeToken(string input)
        {
            var handler = new JwtSecurityTokenHandler();
            if (input.StartsWith("Bearer "))
            {
                input = input["Bearer ".Length..];
            }

            return handler.ReadJwtToken(input).ToString();
        }

        public TAccessToken CreateToken<TAccessToken>(User user, IEnumerable<OperationClaim> operationClaims = null)
            where TAccessToken : IAccessToken, new()
        {
            _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
            var securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);
            var jwt = CreateJwtSecurityToken(_tokenOptions, user, operationClaims, signingCredentials);
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.WriteToken(jwt);

            return new TAccessToken()
            {
                Token = token,
                Expiration = _accessTokenExpiration,
                RefreshToken = GenerateRefreshToken()
            };
        }

        public JwtSecurityToken CreateJwtSecurityToken(
            TokenOptions tokenOptions,
            User user,
            IEnumerable<OperationClaim> operationClaims,
            SigningCredentials signingCredentials)
        {
            var jwt = new JwtSecurityToken(
                tokenOptions.Issuer,
                tokenOptions.Audience,
                expires: _accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: SetClaims(user, operationClaims),
                signingCredentials: signingCredentials);
            return jwt;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }

        private static IEnumerable<Claim> SetClaims(User user, IEnumerable<OperationClaim> operationClaims)
        {
            var claims = new List<Claim>();
            claims.AddNameIdentifier(user.UserId.ToString());
            if (user.CitizenId > 0)
            {
                claims.AddNameUniqueIdentifier(user.CitizenId.ToString());
            }

            if (!string.IsNullOrEmpty(user.FullName))
            {
                claims.AddName($"{user.FullName}");
            }

            string role = "";

            if (operationClaims != null)
            {
                foreach (var claim in operationClaims)
                {
                    if (claim.Name != null && claim.Name.StartsWith("TenantId:", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = claim.Name.Split(':');
                        if (parts.Length > 1)
                        {
                            claims.Add(new Claim("TenantId", parts[1]));
                            claims.Add(new Claim("tenantid", parts[1]));
                        }
                    }
                    else if (claim.Name != null)
                    {
                        if (claim.Name.Equals("SuperAdmin", StringComparison.OrdinalIgnoreCase) ||
                            claim.Name.Equals("SUPER_ADMIN", StringComparison.OrdinalIgnoreCase) ||
                            claim.Name.Equals("Super Admin", StringComparison.OrdinalIgnoreCase))
                        {
                            role = "SUPER_ADMIN";
                        }
                        else if (claim.Name.Equals("KurumSahibi", StringComparison.OrdinalIgnoreCase) ||
                                 claim.Name.Equals("Kurum Sahibi", StringComparison.OrdinalIgnoreCase))
                        {
                            if (role != "SUPER_ADMIN")
                            {
                                role = "KurumSahibi";
                            }
                        }
                        else if (claim.Name.Equals("SubeYonetici", StringComparison.OrdinalIgnoreCase) ||
                                 claim.Name.Equals("OKUL_ADMIN", StringComparison.OrdinalIgnoreCase) ||
                                 claim.Name.Equals("EDITOR", StringComparison.OrdinalIgnoreCase))
                        {
                            if (string.IsNullOrEmpty(role))
                            {
                                role = claim.Name;
                            }
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(role))
            {
                role = (!string.IsNullOrEmpty(user.AuthenticationProviderType) && user.AuthenticationProviderType != "Person" && user.AuthenticationProviderType != "Unknown")
                    ? user.AuthenticationProviderType
                    : "OKUL_ADMIN";
            }

            claims.Add(new Claim(ClaimTypes.Role, role));

            return claims;
        }
    }
}