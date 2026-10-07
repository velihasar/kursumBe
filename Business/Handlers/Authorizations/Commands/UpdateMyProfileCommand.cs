using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Extensions;
using Core.Utilities.Results;
using Core.Utilities.Security.Hashing;
using DataAccess.Abstract;
using MediatR;
using System;

namespace Business.Handlers.Authorizations.Commands
{
    public class UpdateMyProfileCommand : IRequest<IDataResult<UpdateMyProfileResponseDto>>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }

        public class UpdateMyProfileCommandHandler : IRequestHandler<UpdateMyProfileCommand, IDataResult<UpdateMyProfileResponseDto>>
        {
            private readonly IUserRepository _userRepository;
            private readonly IPersonRepository _personRepository;
            private readonly IMediator _mediator;

            public UpdateMyProfileCommandHandler(
                IUserRepository userRepository,
                IPersonRepository personRepository,
                IMediator mediator)
            {
                _userRepository = userRepository;
                _personRepository = personRepository;
                _mediator = mediator;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            public async Task<IDataResult<UpdateMyProfileResponseDto>> Handle(UpdateMyProfileCommand request, CancellationToken cancellationToken)
            {
                var currentUserId = UserInfoExtensions.GetUserIdOrZero();
                if (currentUserId <= 0)
                {
                    return new ErrorDataResult<UpdateMyProfileResponseDto>("Oturum bilgisi doğrulanamadı.");
                }

                var user = await _userRepository.GetAsync(u => u.UserId == currentUserId);
                if (user == null)
                {
                    return new ErrorDataResult<UpdateMyProfileResponseDto>("Kullanıcı bulunamadı.");
                }

                // Password change validation if NewPassword is provided
                if (!string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                    {
                        return new ErrorDataResult<UpdateMyProfileResponseDto>("Şifrenizi değiştirmek için mevcut şifrenizi girmeniz gerekmektedir.");
                    }

                    if (user.PasswordHash != null && user.PasswordSalt != null && user.PasswordHash.Length > 0)
                    {
                        if (!HashingHelper.VerifyPasswordHash(request.CurrentPassword, user.PasswordHash, user.PasswordSalt))
                        {
                            return new ErrorDataResult<UpdateMyProfileResponseDto>("Mevcut şifreniz hatalı.");
                        }
                    }

                    if (request.NewPassword.Length < 4)
                    {
                        return new ErrorDataResult<UpdateMyProfileResponseDto>("Yeni şifre en az 4 karakter olmalıdır.");
                    }

                    HashingHelper.CreatePasswordHash(request.NewPassword, out var newSalt, out var newHash);
                    user.PasswordHash = newHash;
                    user.PasswordSalt = newSalt;
                }

                // Update Name & Contact
                string formattedFirstName = request.FirstName.ToTurkishTitleCase();
                string formattedLastName = request.LastName.ToTurkishTitleCase();
                string combinedFullName = $"{formattedFirstName} {formattedLastName}".Trim();

                if (!string.IsNullOrWhiteSpace(combinedFullName))
                {
                    user.FullName = combinedFullName;
                }

                if (!string.IsNullOrWhiteSpace(request.Email))
                {
                    user.Email = request.Email.Trim().ToLowerInvariant();
                }

                if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
                {
                    user.MobilePhones = request.PhoneNumber.Trim();
                }

                user.UpdateContactDate = DateTime.Now;
                _userRepository.Update(user);
                await _userRepository.SaveChangesAsync();

                // Update linked Person record if exists
                var person = await _personRepository.GetAsync(p => p.UserId == currentUserId && p.IsDeleted == false);
                if (person != null)
                {
                    if (!string.IsNullOrWhiteSpace(formattedFirstName)) person.FirstName = formattedFirstName;
                    if (!string.IsNullOrWhiteSpace(formattedLastName)) person.LastName = formattedLastName;
                    if (!string.IsNullOrWhiteSpace(request.Email)) person.Email = request.Email.Trim().ToLowerInvariant();
                    if (!string.IsNullOrWhiteSpace(request.PhoneNumber)) person.Phone = request.PhoneNumber.Trim();

                    _personRepository.Update(person);
                    await _personRepository.SaveChangesAsync();
                }

                var response = new UpdateMyProfileResponseDto
                {
                    UserId = user.UserId,
                    FullName = user.FullName,
                    FirstName = !string.IsNullOrWhiteSpace(formattedFirstName) ? formattedFirstName : (person?.FirstName ?? ""),
                    LastName = !string.IsNullOrWhiteSpace(formattedLastName) ? formattedLastName : (person?.LastName ?? ""),
                    Email = user.Email,
                    PhoneNumber = user.MobilePhones ?? person?.Phone ?? "",
                };

                return new SuccessDataResult<UpdateMyProfileResponseDto>(response, "Profil bilgileriniz başarıyla güncellendi.");
            }
        }
    }

    public class UpdateMyProfileResponseDto
    {
        public int UserId { get; set; }
        public string FullName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
