using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.Constants;
using Business.Services.Authentication;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Caching;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Entities.Concrete.Project;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using Core.Utilities.Security.Jwt;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Authorizations.Commands
{
    public class RegisterParentCommand : IRequest<IDataResult<AccessToken>>
    {
        public string AccessCode { get; set; } = null!;
        public string EmailOrPhone { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? FullName { get; set; }

        public class RegisterParentCommandHandler : IRequestHandler<RegisterParentCommand, IDataResult<AccessToken>>
        {
            private readonly IUserRepository _userRepository;
            private readonly IStudentRepository _studentRepository;
            private readonly IPersonRepository _personRepository;
            private readonly IParentRepository _parentRepository;
            private readonly IStudentParentRepository _studentParentRepository;
            private readonly ITenantUserRepository _tenantUserRepository;
            private readonly ITokenHelper _tokenHelper;
            private readonly ICacheManager _cacheManager;

            public RegisterParentCommandHandler(
                IUserRepository userRepository,
                IStudentRepository studentRepository,
                IPersonRepository personRepository,
                IParentRepository parentRepository,
                IStudentParentRepository studentParentRepository,
                ITenantUserRepository tenantUserRepository,
                ITokenHelper tokenHelper,
                ICacheManager cacheManager)
            {
                _userRepository = userRepository;
                _studentRepository = studentRepository;
                _personRepository = personRepository;
                _parentRepository = parentRepository;
                _studentParentRepository = studentParentRepository;
                _tenantUserRepository = tenantUserRepository;
                _tokenHelper = tokenHelper;
                _cacheManager = cacheManager;
            }

            [CacheRemoveAspect]
            [LogAspect(typeof(FileLogger))]
            public async Task<IDataResult<AccessToken>> Handle(RegisterParentCommand request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.AccessCode))
                {
                    return new ErrorDataResult<AccessToken>("Lütfen geçerli bir veli giriş kodu giriniz.");
                }

                if (string.IsNullOrWhiteSpace(request.EmailOrPhone))
                {
                    return new ErrorDataResult<AccessToken>("Lütfen e-posta adresi veya telefon numarası giriniz.");
                }

                if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 4)
                {
                    return new ErrorDataResult<AccessToken>("Şifreniz en az 4 karakter uzunluğunda olmalıdır.");
                }

                var code = request.AccessCode.Trim().ToUpperInvariant();

                // 1. Find student by ParentAccessCode
                var student = await _studentRepository.Query()
                    .Include(s => s.Person)
                    .Include(s => s.Parents).ThenInclude(sp => sp.Parent).ThenInclude(p => p.Person)
                    .FirstOrDefaultAsync(s => s.ParentAccessCode != null && s.ParentAccessCode.ToUpper() == code && s.IsDeleted == false, cancellationToken);

                if (student == null)
                {
                    return new ErrorDataResult<AccessToken>("Geçersiz veya bulunamayan veli giriş kodu. Lütfen kurumunuzdan aldığınız kodu kontrol ediniz.");
                }

                // 2. Check if this student already has an activated Parent User
                var existingLinkedParent = student.Parents?
                    .Select(sp => sp.Parent)
                    .FirstOrDefault(p => p != null && p.Person != null && p.Person.UserId.HasValue && p.Person.UserId.Value > 0);

                if (existingLinkedParent != null)
                {
                    return new ErrorDataResult<AccessToken>("Bu giriş kodu ile daha önce kayıt oluşturulmuştur. Lütfen e-posta veya telefon numaranız ve şifreniz ile giriş yapınız.");
                }

                // 3. Format email and phone
                var rawContact = request.EmailOrPhone.Trim();
                bool isEmail = rawContact.Contains("@");
                string email = isEmail ? rawContact : $"{rawContact}@kursum.local";
                string phone = !isEmail ? rawContact : "";

                // 4. Check if User already exists
                var existingUser = await _userRepository.GetAsync(u => u.Email == email || (!string.IsNullOrEmpty(phone) && u.MobilePhones == phone));
                if (existingUser != null)
                {
                    return new ErrorDataResult<AccessToken>("Bu iletişim bilgisi ile kayıtlı bir kullanıcı zaten mevcut. Lütfen Giriş Yap ekranından giriş yapınız.");
                }

                // 5. Create Password Hash and User
                HashingHelper.CreatePasswordHash(request.Password, out var passwordSalt, out var passwordHash);

                string parentFullName = !string.IsNullOrWhiteSpace(request.FullName)
                    ? request.FullName.Trim()
                    : (student.Parents?.FirstOrDefault(sp => sp.Parent?.Person != null)?.Parent?.Person != null
                        ? $"{student.Parents.First().Parent.Person.FirstName} {student.Parents.First().Parent.Person.LastName}".Trim()
                        : $"{student.Person?.LastName ?? "Öğrenci"} Velisi");

                var user = new User
                {
                    Email = email,
                    MobilePhones = phone,
                    FullName = parentFullName,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    Status = true,
                    RecordDate = DateTime.Now,
                    UpdateContactDate = DateTime.Now
                };

                _userRepository.Add(user);
                await _userRepository.SaveChangesAsync();

                // 6. Link with TenantUser
                var tenantUser = new TenantUser
                {
                    TenantId = student.TenantId,
                    UserId = user.UserId,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedDate = DateTime.Now
                };
                _tenantUserRepository.Add(tenantUser);
                await _tenantUserRepository.SaveChangesAsync();

                // 7. Attach or Create Parent & Person
                var existingParent = student.Parents?.FirstOrDefault(sp => sp.Parent != null)?.Parent;
                if (existingParent != null && existingParent.Person != null)
                {
                    existingParent.Person.UserId = user.UserId;
                    if (!string.IsNullOrEmpty(phone)) existingParent.Person.Phone = phone;
                    if (isEmail) existingParent.Person.Email = email;
                    _personRepository.Update(existingParent.Person);
                    await _personRepository.SaveChangesAsync();
                }
                else
                {
                    var names = parentFullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string fName = names.Length > 0 ? names[0] : "Veli";
                    string lName = names.Length > 1 ? string.Join(' ', names.Skip(1)) : (student.Person?.LastName ?? "");

                    var newPerson = new Person
                    {
                        TenantId = student.TenantId,
                        UserId = user.UserId,
                        FirstName = fName,
                        LastName = lName,
                        Email = isEmail ? email : null,
                        Phone = phone,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedDate = DateTime.Now
                    };
                    _personRepository.Add(newPerson);
                    await _personRepository.SaveChangesAsync();

                    var newParent = new Parent
                    {
                        TenantId = student.TenantId,
                        PersonId = newPerson.Id,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedDate = DateTime.Now
                    };
                    _parentRepository.Add(newParent);
                    await _parentRepository.SaveChangesAsync();

                    var studentParent = new StudentParent
                    {
                        TenantId = student.TenantId,
                        StudentId = student.Id,
                        ParentId = newParent.Id,
                        Relationship = "Veli",
                        IsPrimary = true,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedDate = DateTime.Now
                    };
                    _studentParentRepository.Add(studentParent);
                    await _studentParentRepository.SaveChangesAsync();
                }

                // 8. Generate Claims & AccessToken
                var claims = _userRepository.GetClaims(user.UserId);
                claims.Add(new OperationClaim { Name = "Parent" });
                claims.Add(new OperationClaim { Name = $"TenantId:{student.TenantId}" });
                claims.Add(new OperationClaim { Name = $"StudentId:{student.Id}" });

                var accessToken = _tokenHelper.CreateToken<DArchToken>(user, claims);
                accessToken.Claims = claims.Select(x => x.Name).ToList();

                user.RefreshToken = accessToken.RefreshToken;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                _cacheManager.Add($"{CacheKeys.UserIdForClaim}={user.UserId}", claims.Select(x => x.Name));

                return new SuccessDataResult<AccessToken>(accessToken, "Veli kaydı ve aktivasyonu başarıyla tamamlandı.");
            }
        }
    }
}
