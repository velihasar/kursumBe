using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Extensions;

namespace Business.Handlers.CourseEnrollments.Commands
{
    public class DeleteCourseEnrollmentCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteCourseEnrollmentCommandHandler : IRequestHandler<DeleteCourseEnrollmentCommand, IResult>
        {
            private readonly ICourseEnrollmentRepository _courseEnrollmentRepository;
            private readonly IMediator _mediator;

            public DeleteCourseEnrollmentCommandHandler(ICourseEnrollmentRepository courseEnrollmentRepository, IMediator mediator)
            {
                _courseEnrollmentRepository = courseEnrollmentRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteCourseEnrollmentCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();
                var courseEnrollmentToDelete = await _courseEnrollmentRepository.GetAsync(p => p.Id == request.Id && p.IsDeleted == false);

                if (courseEnrollmentToDelete == null)
                    return new ErrorResult("Kayıt bulunamadı.");

                courseEnrollmentToDelete.IsDeleted = true;
                courseEnrollmentToDelete.DeletedBy = userId > 0 ? userId : null;
                courseEnrollmentToDelete.DeletedDate = System.DateTime.Now;

                _courseEnrollmentRepository.Update(courseEnrollmentToDelete);
                await _courseEnrollmentRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}
