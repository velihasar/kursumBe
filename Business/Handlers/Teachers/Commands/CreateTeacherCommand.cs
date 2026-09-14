
using Business.BusinessAspects;
using Business.Constants;
using Business.Handlers.Teachers.FilterTeacher;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Business.Handlers.Teachers.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.TeacherDto;
using Core.Extensions;

namespace Business.Handlers.Teachers.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class CreateTeacherCommand : IRequest<IDataResult<TeacherCreateResponseDto>>
    {
        public int? TenantId { get; set; }
        public int PersonId { get; set; }
        public System.DateTime StartDate { get; set; }


        public class CreateTeacherCommandHandler : IRequestHandler<CreateTeacherCommand, IDataResult<TeacherCreateResponseDto>>
        {
            private readonly ITeacherRepository _teacherRepository;
            private readonly IMediator _mediator;
            public CreateTeacherCommandHandler(ITeacherRepository teacherRepository, IMediator mediator)
            {
                _teacherRepository = teacherRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateTeacherValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<TeacherCreateResponseDto>> Handle(CreateTeacherCommand request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();

                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                if (targetTenantId <= 0)
                {
                    return new ErrorDataResult<TeacherCreateResponseDto>("SuperAdmin olarak işlem yapmaktasınız. Lütfen geçerli bir kurum (okul) seçiniz.");
                }



                var addedTeacher = new Teacher
                {
                    TenantId = targetTenantId,
                    PersonId = request.PersonId,
                    StartDate = request.StartDate,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = System.DateTime.Now
                };

                _teacherRepository.Add(addedTeacher);
                await _teacherRepository.SaveChangesAsync();

                var dto = new TeacherCreateResponseDto
                {
                    Id = addedTeacher.Id,
                    PersonId = addedTeacher.PersonId,
                    StartDate = addedTeacher.StartDate
                };

                return new SuccessDataResult<TeacherCreateResponseDto>(dto, Messages.Added);
            }
        }
    }
}