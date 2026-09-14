
using Business.BusinessAspects;
using Business.Handlers.Teachers.FilterTeacher;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.TeacherDto;

namespace Business.Handlers.Teachers.Queries
{
    public class GetTeacherQuery : IRequest<IDataResult<TeacherGetByIdDto>>
    {
        public int Id { get; set; }

        public class GetTeacherQueryHandler : IRequestHandler<GetTeacherQuery, IDataResult<TeacherGetByIdDto>>
        {
            private readonly ITeacherRepository _teacherRepository;
            private readonly IMediator _mediator;

            public GetTeacherQueryHandler(ITeacherRepository teacherRepository, IMediator mediator)
            {
                _teacherRepository = teacherRepository;
                _mediator = mediator;
            }
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<TeacherGetByIdDto>> Handle(GetTeacherQuery request, CancellationToken cancellationToken)
            {
                var teacher = await _teacherRepository.GetAsync(TeacherFiltersHelper.GetTeacherQueryFilter(request));
                if (teacher == null)
                    return new ErrorDataResult<TeacherGetByIdDto>("Kayıt bulunamadı.");

                var dto = new TeacherGetByIdDto
                {
                    Id = teacher.Id,
                    PersonId = teacher.PersonId,
                    StartDate = teacher.StartDate
                };
                return new SuccessDataResult<TeacherGetByIdDto>(dto);
            }
        }
    }
}
