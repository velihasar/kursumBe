
using Business.BusinessAspects;
using Business.Handlers.StudentParents.FilterStudentParent;
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
using Core.Entities.Dtos.StudentParentDto;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.StudentParents.Queries
{
    public class GetStudentParentsQuery : IRequest<IDataResult<IEnumerable<StudentParentGetAllDto>>>
    {
        public class GetStudentParentsQueryHandler : IRequestHandler<GetStudentParentsQuery, IDataResult<IEnumerable<StudentParentGetAllDto>>>
        {
            private readonly IStudentParentRepository _studentParentRepository;
            private readonly IMediator _mediator;

            public GetStudentParentsQueryHandler(IStudentParentRepository studentParentRepository, IMediator mediator)
            {
                _studentParentRepository = studentParentRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<StudentParentGetAllDto>>> Handle(GetStudentParentsQuery request, CancellationToken cancellationToken)
            {
                var list = await _studentParentRepository.Query()
                    .Include(x => x.Student).ThenInclude(s => s.Person)
                    .Include(x => x.Student).ThenInclude(s => s.StudentBranches).ThenInclude(sb => sb.Branch)
                    .Include(x => x.Parent).ThenInclude(p => p.Person)
                    .Where(x => x.IsDeleted == false)
                    .ToListAsync(cancellationToken);

                var dtos = list.Select(x => new StudentParentGetAllDto
                {
                    Id = x.Id,
                    StudentId = x.StudentId,
                    ParentId = x.ParentId,
                    Relationship = x.Relationship,
                    IsPrimary = x.IsPrimary,
                    StudentName = x.Student?.Person != null ? $"{x.Student.Person.FirstName} {x.Student.Person.LastName}".Trim() : null,
                    StudentNumber = x.Student?.StudentNumber,
                    BranchName = x.Student?.StudentBranches?.FirstOrDefault(sb => sb.IsDeleted == false)?.Branch?.Name,
                    ParentName = x.Parent?.Person != null ? $"{x.Parent.Person.FirstName} {x.Parent.Person.LastName}".Trim() : null,
                    ParentPhone = x.Parent?.Person?.Phone,
                    ParentEmail = x.Parent?.Person?.Email,
                });
                return new SuccessDataResult<IEnumerable<StudentParentGetAllDto>>(dtos);
            }
        }
    }
}