
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
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<StudentParentGetAllDto>>> Handle(GetStudentParentsQuery request, CancellationToken cancellationToken)
            {
                var list = await _studentParentRepository.GetListAsync(StudentParentFiltersHelper.GetStudentParentsQueryFilter(request));
                var dtos = list.Select(x => new StudentParentGetAllDto
                {
                    Id = x.Id,
                    StudentId = x.StudentId,
                    ParentId = x.ParentId,
                    Relationship = x.Relationship,
                    IsPrimary = x.IsPrimary
                });
                return new SuccessDataResult<IEnumerable<StudentParentGetAllDto>>(dtos);
            }
        }
    }
}