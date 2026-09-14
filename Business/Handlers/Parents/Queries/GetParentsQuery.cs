
using Business.BusinessAspects;
using Business.Handlers.Parents.FilterParent;
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
using Core.Entities.Dtos.ParentDto;

using Microsoft.EntityFrameworkCore;
using Core.Extensions;

namespace Business.Handlers.Parents.Queries
{
    public class GetParentsQuery : IRequest<IDataResult<IEnumerable<ParentGetAllDto>>>
    {
        public class GetParentsQueryHandler : IRequestHandler<GetParentsQuery, IDataResult<IEnumerable<ParentGetAllDto>>>
        {
            private readonly IParentRepository _parentRepository;
            private readonly IMediator _mediator;

            public GetParentsQueryHandler(IParentRepository parentRepository, IMediator mediator)
            {
                _parentRepository = parentRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<ParentGetAllDto>>> Handle(GetParentsQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var query = _parentRepository.Query()
                    .Include(x => x.Person)
                    .Where(x => x.IsDeleted == false);

                if (userTenantId > 0)
                {
                    query = query.Where(x => x.TenantId == userTenantId);
                }

                var list = await query.ToListAsync(cancellationToken);
                var dtos = list.Select(x => new ParentGetAllDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    PersonId = x.PersonId,
                    FirstName = x.Person != null ? x.Person.FirstName : null,
                    LastName = x.Person != null ? x.Person.LastName : null,
                    DateOfBirth = x.Person != null ? x.Person.DateOfBirth : null,
                    Phone = x.Person != null ? x.Person.Phone : null,
                    Email = x.Person != null ? x.Person.Email : null,
                    PhotoUrl = x.Person != null ? x.Person.PhotoUrl : null,
                });
                return new SuccessDataResult<IEnumerable<ParentGetAllDto>>(dtos);
            }
        }
    }
}