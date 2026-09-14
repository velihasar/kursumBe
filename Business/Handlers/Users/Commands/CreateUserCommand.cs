using System;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Users.Commands
{
    public class CreateUserCommand : IRequest<IResult>
    {
        public int UserId { get; set; }
        public long CitizenId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string MobilePhones { get; set; }
        public bool Status { get; set; }
        public DateTime RecordDate { get; set; }
        public DateTime UpdateContactDate { get; set; }
        public string Password { get; set; }
        public int? TenantId { get; set; }


        public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, IResult>
        {
            private readonly IUserRepository _userRepository;
            private readonly ITenantUserRepository _tenantUserRepository;

            public CreateUserCommandHandler(IUserRepository userRepository, ITenantUserRepository tenantUserRepository)
            {
                _userRepository = userRepository;
                _tenantUserRepository = tenantUserRepository;
            }

            [SecuredOperation(Priority = 1)]
            [CacheRemoveAspect("GetUsers")]
            [LogAspect(typeof(ElasticSearchLogger))]
            public async Task<IResult> Handle(CreateUserCommand request, CancellationToken cancellationToken)
            {
                var isThereAnyUser = await _userRepository.GetAsync(u => u.Email == request.Email);

                if (isThereAnyUser != null)
                {
                    return new ErrorResult(Messages.NameAlreadyExist);
                }

                var cleanMobile = !string.IsNullOrWhiteSpace(request.MobilePhones) ? request.MobilePhones.Replace(" ", "").Trim() : null;

                var user = new User
                {
                    Email = request.Email,
                    FullName = request.FullName,
                    Status = true,
                    CitizenId = request.CitizenId,
                    MobilePhones = cleanMobile
                };

                _userRepository.Add(user);
                await _userRepository.SaveChangesAsync();

                if (request.TenantId.HasValue && request.TenantId.Value > 0)
                {
                    var tenantUser = new Core.Entities.Concrete.Project.TenantUser
                    {
                        UserId = user.UserId,
                        TenantId = request.TenantId.Value,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedDate = DateTime.Now
                    };
                    _tenantUserRepository.Add(tenantUser);
                    await _tenantUserRepository.SaveChangesAsync();
                }

                return new SuccessResult(Messages.Added);
            }
        }
    }
}