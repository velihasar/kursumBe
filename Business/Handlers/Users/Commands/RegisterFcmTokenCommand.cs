using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.Users.Commands
{
    public class RegisterFcmTokenCommand : IRequest<IResult>
    {
        public string FcmToken { get; set; }
        public string Platform { get; set; } = "android";
        public string AppVersion { get; set; } = "1.0.0";

        public class RegisterFcmTokenCommandHandler : IRequestHandler<RegisterFcmTokenCommand, IResult>
        {
            private readonly IUserDeviceRepository _userDeviceRepository;
            private readonly IUserRepository _userRepository;

            public RegisterFcmTokenCommandHandler(IUserDeviceRepository userDeviceRepository, IUserRepository userRepository)
            {
                _userDeviceRepository = userDeviceRepository;
                _userRepository = userRepository;
            }

            [SecuredOperation(Priority = 1)]
            [LogAspect(typeof(FileLogger))]
            public async Task<IResult> Handle(RegisterFcmTokenCommand request, CancellationToken cancellationToken)
            {
                if (string.IsNullOrWhiteSpace(request.FcmToken))
                {
                    return new ErrorResult("FCM token gereklidir.");
                }

                var userId = UserInfoExtensions.GetUserIdOrZero();
                if (userId <= 0)
                {
                    return new ErrorResult(Messages.AuthorizationsDenied);
                }

                // Update User table
                var user = await _userRepository.GetAsync(u => u.UserId == userId);
                if (user != null)
                {
                    user.FcmToken = request.FcmToken;
                    _userRepository.Update(user);
                    await _userRepository.SaveChangesAsync();
                }

                // Update UserDevices table
                var existingDevice = await _userDeviceRepository.GetAsync(x => x.UserId == userId && x.DeviceToken == request.FcmToken);
                if (existingDevice != null)
                {
                    existingDevice.IsActive = true;
                    existingDevice.UpdatedDate = DateTime.Now;
                    existingDevice.Platform = request.Platform ?? existingDevice.Platform;
                    existingDevice.AppVersion = request.AppVersion ?? existingDevice.AppVersion;
                    _userDeviceRepository.Update(existingDevice);
                }
                else
                {
                    var newDevice = new UserDevice
                    {
                        UserId = userId,
                        DeviceToken = request.FcmToken,
                        Platform = request.Platform ?? "android",
                        AppVersion = request.AppVersion ?? "1.0.0",
                        IsActive = true,
                        CreatedDate = DateTime.Now
                    };
                    _userDeviceRepository.Add(newDevice);
                }

                await _userDeviceRepository.SaveChangesAsync();
                return new SuccessResult("FCM token başarıyla kaydedildi.");
            }
        }
    }
}
