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

namespace Business.Handlers.Courses.Commands
{
    public class DeleteCourseCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteCourseCommandHandler : IRequestHandler<DeleteCourseCommand, IResult>
        {
            private readonly ICourseRepository _courseRepository;
            private readonly IMediator _mediator;

            public DeleteCourseCommandHandler(ICourseRepository courseRepository, IMediator mediator)
            {
                _courseRepository = courseRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();
                var courseToDelete = await _courseRepository.GetAsync(p => p.Id == request.Id && p.IsDeleted == false);

                if (courseToDelete == null)
                    return new ErrorResult("Kayıt bulunamadı.");

                courseToDelete.IsDeleted = true;
                courseToDelete.DeletedBy = userId > 0 ? userId : null;
                courseToDelete.DeletedDate = System.DateTime.Now;

                _courseRepository.Update(courseToDelete);
                await _courseRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}
