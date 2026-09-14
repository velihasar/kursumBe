
using Business.BusinessAspects;
using Business.Handlers.People.FilterPerson;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.PersonDto;

namespace Business.Handlers.People.Queries
{
    public class GetPersonQuery : IRequest<IDataResult<PersonGetByIdDto>>
    {
        public int Id { get; set; }

        public class GetPersonQueryHandler : IRequestHandler<GetPersonQuery, IDataResult<PersonGetByIdDto>>
        {
            private readonly IPersonRepository _personRepository;
            private readonly IMediator _mediator;

            public GetPersonQueryHandler(IPersonRepository personRepository, IMediator mediator)
            {
                _personRepository = personRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<PersonGetByIdDto>> Handle(GetPersonQuery request, CancellationToken cancellationToken)
            {
                var person = await _personRepository.GetAsync(PersonFiltersHelper.GetPersonQueryFilter(request));
                if (person == null)
                    return new ErrorDataResult<PersonGetByIdDto>("Kayıt bulunamadı.");

                var dto = new PersonGetByIdDto
                {
                    Id = person.Id,
                    TenantId = person.TenantId,
                    UserId = person.UserId,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    DateOfBirth = person.DateOfBirth,
                    Phone = person.Phone,
                    Email = person.Email,
                    PhotoUrl = person.PhotoUrl
                };
                return new SuccessDataResult<PersonGetByIdDto>(dto);
            }
        }
    }
}
