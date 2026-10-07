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
using Core.Extensions;
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
            private readonly IGroupRepository _groupRepository;
            private readonly IUserGroupRepository _userGroupRepository;
            private readonly ITokenHelper _tokenHelper;
            private readonly ICacheManager _cacheManager;

            public RegisterParentCommandHandler(
                IUserRepository userRepository,
                IStudentRepository studentRepository,
                IPersonRepository personRepository,
                IParentRepository parentRepository,
                IStudentParentRepository studentParentRepository,
                ITenantUserRepository tenantUserRepository,
                IGroupRepository groupRepository,
                IUserGroupRepository userGroupRepository,
                ITokenHelper tokenHelper,
                ICacheManager cacheManager)
            {
                _userRepository = userRepository;
                _studentRepository = studentRepository;
                _personRepository = personRepository;
                _parentRepository = parentRepository;
                _studentParentRepository = studentParentRepository;
                _tenantUserRepository = tenantUserRepository;
                _groupRepository = groupRepository;
                _userGroupRepository = userGroupRepository;
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

                // 2. Format and normalize contact (email or phone)
                var rawContact = request.EmailOrPhone.Trim();
                bool isEmail = rawContact.Contains("@");
                string email = isEmail ? rawContact.ToLower() : null;

                string rawPhone = !isEmail ? rawContact : "";
                string phone = "";
                string phoneWithoutZero = "";
                if (!string.IsNullOrEmpty(rawPhone))
                {
                    var digits = new string(rawPhone.Where(char.IsDigit).ToArray());
                    if (digits.StartsWith("90") && digits.Length == 12) digits = digits.Substring(2);
                    if (digits.Length == 10 && digits.StartsWith("5")) digits = "0" + digits;
                    phone = digits;
                    phoneWithoutZero = digits.StartsWith("0") ? digits.Substring(1) : digits;
                }

                // Helper to normalize any phone string
                string NormalizeDbPhone(string p)
                {
                    if (string.IsNullOrWhiteSpace(p)) return "";
                    var d = new string(p.Where(char.IsDigit).ToArray());
                    if (d.StartsWith("90") && d.Length == 12) d = d.Substring(2);
                    if (d.Length == 10 && d.StartsWith("5")) d = "0" + d;
                    return d;
                }

                // 3. Check if a User already exists with this phone or email
                var existingUser = await _userRepository.GetAsync(u => 
                    (!string.IsNullOrEmpty(email) && u.Email == email) || 
                    (!string.IsNullOrEmpty(phone) && (u.MobilePhones == phone || u.MobilePhones == phoneWithoutZero)));

                if (existingUser != null)
                {
                    return new ErrorDataResult<AccessToken>("Bu iletişim bilgisi ile kayıtlı bir kullanıcı zaten mevcut. Lütfen Giriş Yap ekranından şifreniz ile giriş yapınız.");
                }

                // 4. Check matching parent from student's enrolled parent list
                var matchedStudentParent = student.Parents?.FirstOrDefault(sp =>
                    sp.Parent?.Person != null && (
                        (isEmail && !string.IsNullOrWhiteSpace(sp.Parent.Person.Email) && sp.Parent.Person.Email.Trim().ToLower() == email) ||
                        (!isEmail && !string.IsNullOrWhiteSpace(sp.Parent.Person.Phone) && NormalizeDbPhone(sp.Parent.Person.Phone) == phone)
                    ));

                // If matched parent already has an activated account
                if (matchedStudentParent?.Parent?.Person != null &&
                    matchedStudentParent.Parent.Person.UserId.HasValue &&
                    matchedStudentParent.Parent.Person.UserId.Value > 0)
                {
                    return new ErrorDataResult<AccessToken>("Bu veli için zaten hesap oluşturulmuştur. Lütfen şifreniz ile Giriş Yap ekranından giriş yapınız.");
                }

                // If no exact phone/email match, look for an unlinked parent record without contact info
                var targetStudentParent = matchedStudentParent ?? student.Parents?.FirstOrDefault(sp =>
                    sp.Parent?.Person != null &&
                    (!sp.Parent.Person.UserId.HasValue || sp.Parent.Person.UserId == 0) &&
                    string.IsNullOrWhiteSpace(sp.Parent.Person.Phone) &&
                    string.IsNullOrWhiteSpace(sp.Parent.Person.Email));

                var existingPerson = targetStudentParent?.Parent?.Person;

                // Determine final email and phone for user (use entered value, or fallback to existing parent data)
                string finalEmail = !string.IsNullOrWhiteSpace(email) 
                    ? email 
                    : (!string.IsNullOrWhiteSpace(existingPerson?.Email) ? existingPerson.Email.Trim().ToLower() : null);

                string finalPhone = !string.IsNullOrWhiteSpace(phone) 
                    ? phone 
                    : (!string.IsNullOrWhiteSpace(existingPerson?.Phone) ? NormalizeDbPhone(existingPerson.Phone) : "");

                // 5. Create Password Hash and User
                HashingHelper.CreatePasswordHash(request.Password, out var passwordSalt, out var passwordHash);

                string parentFullName = !string.IsNullOrWhiteSpace(request.FullName)
                    ? request.FullName.ToTurkishTitleCase()
                    : (existingPerson != null && !string.IsNullOrWhiteSpace(existingPerson.FirstName)
                        ? $"{existingPerson.FirstName} {existingPerson.LastName}".ToTurkishTitleCase()
                        : $"{student.Person?.LastName?.ToTurkishTitleCase() ?? "Öğrenci"} Velisi");

                var user = new User
                {
                    Email = finalEmail?.Trim().ToLowerInvariant(),
                    MobilePhones = finalPhone,
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

                // 6.1 Assign to 'Veli' Group
                var veliGroup = await _groupRepository.GetAsync(g => g.GroupName == "Veli" || g.GroupName == "Parent");
                if (veliGroup == null)
                {
                    veliGroup = new Group { GroupName = "Veli" };
                    _groupRepository.Add(veliGroup);
                    await _groupRepository.SaveChangesAsync();
                }
                _userGroupRepository.Add(new UserGroup { GroupId = veliGroup.Id, UserId = user.UserId });
                await _userGroupRepository.SaveChangesAsync();

                int createdOrUpdatedParentId = 0;

                // 7. Attach or Create Parent & Person
                if (targetStudentParent?.Parent != null && existingPerson != null)
                {
                    createdOrUpdatedParentId = targetStudentParent.Parent.Id;
                    existingPerson.UserId = user.UserId;
                    if (!string.IsNullOrEmpty(finalPhone)) existingPerson.Phone = finalPhone;
                    if (!string.IsNullOrEmpty(finalEmail)) existingPerson.Email = finalEmail.Trim().ToLowerInvariant();
                    if (!string.IsNullOrWhiteSpace(request.FullName))
                    {
                        var names = request.FullName.ToTurkishTitleCase().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                        existingPerson.FirstName = names.Length > 0 ? names[0] : existingPerson.FirstName;
                        existingPerson.LastName = names.Length > 1 ? string.Join(' ', names.Skip(1)) : existingPerson.LastName;
                    }
                    _personRepository.Update(existingPerson);
                    await _personRepository.SaveChangesAsync();
                }
                else
                {
                    var names = parentFullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    string fName = names.Length > 0 ? names[0].ToTurkishTitleCase() : "Veli";
                    string lName = names.Length > 1 ? string.Join(' ', names.Skip(1)).ToTurkishTitleCase() : (student.Person?.LastName?.ToTurkishTitleCase() ?? "");

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
                    createdOrUpdatedParentId = newParent.Id;

                    var studentParent = new StudentParent
                    {
                        TenantId = student.TenantId,
                        StudentId = student.Id,
                        ParentId = newParent.Id,
                        Relationship = "Veli",
                        IsPrimary = student.Parents?.Count == 0,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedDate = DateTime.Now
                    };
                    _studentParentRepository.Add(studentParent);
                    await _studentParentRepository.SaveChangesAsync();
                }

                // 8. Generate Claims & AccessToken
                var claims = _userRepository.GetClaims(user.UserId);
                if (!claims.Any(c => c.Name == "Parent" || c.Name == "Veli"))
                {
                    claims.Add(new OperationClaim { Name = "Parent" });
                    claims.Add(new OperationClaim { Name = "Veli" });
                }
                claims.Add(new OperationClaim { Name = $"ParentId:{createdOrUpdatedParentId}" });
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
