
using Business.Constants;
using Business.BusinessAspects;
using Business.Handlers.Teachers.FilterTeacher;
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
using Business.Handlers.Teachers.ValidationRules;
using Core.Entities.Dtos.TeacherDto;
using Core.Extensions;

namespace Business.Handlers.Teachers.Commands
{
    public class UpdateTeacherCommand : IRequest<IDataResult<TeacherUpdateResponseDto>>
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public System.DateTime StartDate { get; set; }
        public bool IsActive { get; set; }

        public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, IDataResult<TeacherUpdateResponseDto>>
        {
            private readonly ITeacherRepository _teacherRepository;
            private readonly IMediator _mediator;

            public UpdateTeacherCommandHandler(ITeacherRepository teacherRepository, IMediator mediator)
            {
                _teacherRepository = teacherRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateTeacherValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<TeacherUpdateResponseDto>> Handle(UpdateTeacherCommand request, CancellationToken cancellationToken)
            {
                var isThereTeacherRecord = await _teacherRepository.GetAsync(TeacherFiltersHelper.UpdateTeacherCommandFilter(request));
                if (isThereTeacherRecord == null)
                    return new ErrorDataResult<TeacherUpdateResponseDto>("Kayıt bulunamadı.");

                var tenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();
                if (tenantId > 0)
                {
                    isThereTeacherRecord.TenantId = tenantId;
                }
                if (userId > 0)
                {
                    isThereTeacherRecord.UpdatedBy = userId;
                }
                isThereTeacherRecord.UpdatedDate = System.DateTime.Now;

                isThereTeacherRecord.PersonId = request.PersonId;
                isThereTeacherRecord.StartDate = request.StartDate;
                isThereTeacherRecord.IsActive = request.IsActive;

                _teacherRepository.Update(isThereTeacherRecord);
                await _teacherRepository.SaveChangesAsync();

                var dto = new TeacherUpdateResponseDto
                {
                    Id = isThereTeacherRecord.Id,
                    PersonId = isThereTeacherRecord.PersonId,
                    StartDate = isThereTeacherRecord.StartDate
                };

                return new SuccessDataResult<TeacherUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}

