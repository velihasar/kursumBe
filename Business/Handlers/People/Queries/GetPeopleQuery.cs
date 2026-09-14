
using Business.BusinessAspects;
using Business.Handlers.People.FilterPerson;
using Core.Aspects.Autofac.Performance;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Aspects.Autofac.Caching;
using Core.Entities.Dtos.PersonDto;

namespace Business.Handlers.People.Queries
{
    public class GetPeopleQuery : IRequest<IDataResult<IEnumerable<PersonGetAllDto>>>
    {
        public class GetPeopleQueryHandler : IRequestHandler<GetPeopleQuery, IDataResult<IEnumerable<PersonGetAllDto>>>
        {
            private readonly IPersonRepository _personRepository;
            private readonly IMediator _mediator;

            public GetPeopleQueryHandler(IPersonRepository personRepository, IMediator mediator)
            {
                _personRepository = personRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<PersonGetAllDto>>> Handle(GetPeopleQuery request, CancellationToken cancellationToken)
            {
                var list = await _personRepository.GetListAsync(PersonFiltersHelper.GetPeopleQueryFilter(request));
                var dtos = list.Select(x => new PersonGetAllDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    UserId = x.UserId,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    DateOfBirth = x.DateOfBirth,
                    Phone = x.Phone,
                    Email = x.Email,
                    PhotoUrl = x.PhotoUrl
                });
                return new SuccessDataResult<IEnumerable<PersonGetAllDto>>(dtos);
            }
        }
    }
}