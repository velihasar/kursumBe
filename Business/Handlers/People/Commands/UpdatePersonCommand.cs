
using Business.Constants;
using Business.BusinessAspects;
using Business.Handlers.People.FilterPerson;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Core.Aspects.Autofac.Validation;
using Business.Handlers.People.ValidationRules;
using Core.Entities.Dtos.PersonDto;
using Core.Extensions;

namespace Business.Handlers.People.Commands
{
    public class UpdatePersonCommand : IRequest<IDataResult<PersonUpdateResponseDto>>
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public int? UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public System.DateTime? DateOfBirth { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsActive { get; set; }

        public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, IDataResult<PersonUpdateResponseDto>>
        {
            private readonly IPersonRepository _personRepository;
            private readonly IMediator _mediator;

            public UpdatePersonCommandHandler(IPersonRepository personRepository, IMediator mediator)
            {
                _personRepository = personRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdatePersonValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<PersonUpdateResponseDto>> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
            {
                var isTherePersonRecord = await _personRepository.GetAsync(PersonFiltersHelper.UpdatePersonCommandFilter(request));
                if (isTherePersonRecord == null)
                    return new ErrorDataResult<PersonUpdateResponseDto>("Kayıt bulunamadı.");

                var tenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();
                if (tenantId > 0)
                {
                    isTherePersonRecord.TenantId = tenantId;
                }
                else if (request.TenantId.HasValue && request.TenantId.Value > 0)
                {
                    isTherePersonRecord.TenantId = request.TenantId.Value;
                }
                if (userId > 0)
                {
                    isTherePersonRecord.UpdatedBy = userId;
                }
                isTherePersonRecord.UpdatedDate = System.DateTime.Now;

                var cleanPhone = !string.IsNullOrWhiteSpace(request.Phone) ? request.Phone.Replace(" ", "").Trim() : null;

                isTherePersonRecord.UserId = request.UserId;
                isTherePersonRecord.FirstName = request.FirstName;
                isTherePersonRecord.LastName = request.LastName;
                isTherePersonRecord.DateOfBirth = request.DateOfBirth;
                isTherePersonRecord.Phone = cleanPhone;
                isTherePersonRecord.Email = request.Email;
                isTherePersonRecord.PhotoUrl = request.PhotoUrl;
                isTherePersonRecord.IsActive = request.IsActive;

                _personRepository.Update(isTherePersonRecord);
                await _personRepository.SaveChangesAsync();

                var dto = new PersonUpdateResponseDto
                {
                    Id = isTherePersonRecord.Id,
                    UserId = isTherePersonRecord.UserId,
                    FirstName = isTherePersonRecord.FirstName,
                    LastName = isTherePersonRecord.LastName,
                    DateOfBirth = isTherePersonRecord.DateOfBirth,
                    Phone = isTherePersonRecord.Phone,
                    Email = isTherePersonRecord.Email,
                    PhotoUrl = isTherePersonRecord.PhotoUrl
                };

                return new SuccessDataResult<PersonUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}

