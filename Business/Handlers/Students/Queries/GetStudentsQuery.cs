
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Handlers.Students.FilterStudent;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.StudentDto;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Students.Queries
{
    public class GetStudentsQuery : IRequest<IDataResult<IEnumerable<StudentGetAllDto>>>
    {
        public class GetStudentsQueryHandler : IRequestHandler<GetStudentsQuery, IDataResult<IEnumerable<StudentGetAllDto>>>
        {
            private readonly IStudentRepository _studentRepository;
            private readonly IMediator _mediator;

            public GetStudentsQueryHandler(IStudentRepository studentRepository, IMediator mediator)
            {
                _studentRepository = studentRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<StudentGetAllDto>>> Handle(GetStudentsQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = Core.Extensions.UserInfoExtensions.GetTenantIdOrZero();
                var query = _studentRepository.Query()
                    .Include(x => x.Person)
                    .Where(x => x.IsDeleted == false);

                if (userTenantId > 0)
                {
                    query = query.Where(x => x.TenantId == userTenantId);
                }

                var list = await query.ToListAsync(cancellationToken);

                var dtos = list.Select(x => new StudentGetAllDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    PersonId = x.PersonId,
                    StudentNumber = x.StudentNumber,
                    EnrollmentDate = x.EnrollmentDate,
                    FirstName = x.Person != null ? x.Person.FirstName : null,
                    LastName = x.Person != null ? x.Person.LastName : null,
                    DateOfBirth = x.Person != null ? x.Person.DateOfBirth : null,
                    Phone = x.Person != null ? x.Person.Phone : null,
                    Email = x.Person != null ? x.Person.Email : null,
                    PhotoUrl = x.Person != null ? x.Person.PhotoUrl : null,
                });
                return new SuccessDataResult<IEnumerable<StudentGetAllDto>>(dtos);
            }
        }
    }
}