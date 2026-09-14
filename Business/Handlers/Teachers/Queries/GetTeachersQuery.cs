
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Handlers.Teachers.FilterTeacher;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.TeacherDto;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Teachers.Queries
{
    public class GetTeachersQuery : IRequest<IDataResult<IEnumerable<TeacherGetAllDto>>>
    {
        public class GetTeachersQueryHandler : IRequestHandler<GetTeachersQuery, IDataResult<IEnumerable<TeacherGetAllDto>>>
        {
            private readonly ITeacherRepository _teacherRepository;
            private readonly IMediator _mediator;

            public GetTeachersQueryHandler(ITeacherRepository teacherRepository, IMediator mediator)
            {
                _teacherRepository = teacherRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<TeacherGetAllDto>>> Handle(GetTeachersQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = Core.Extensions.UserInfoExtensions.GetTenantIdOrZero();
                var query = _teacherRepository.Query()
                    .Include(x => x.Person)
                    .Where(x => x.IsDeleted == false);

                if (userTenantId > 0)
                {
                    query = query.Where(x => x.TenantId == userTenantId);
                }

                var list = await query.ToListAsync(cancellationToken);

                var dtos = list.Select(x => new TeacherGetAllDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    PersonId = x.PersonId,
                    StartDate = x.StartDate,
                    FirstName = x.Person != null ? x.Person.FirstName : null,
                    LastName = x.Person != null ? x.Person.LastName : null,
                    DateOfBirth = x.Person != null ? x.Person.DateOfBirth : null,
                    Phone = x.Person != null ? x.Person.Phone : null,
                    Email = x.Person != null ? x.Person.Email : null,
                    PhotoUrl = x.Person != null ? x.Person.PhotoUrl : null,
                });
                return new SuccessDataResult<IEnumerable<TeacherGetAllDto>>(dtos);
            }
        }
    }
}