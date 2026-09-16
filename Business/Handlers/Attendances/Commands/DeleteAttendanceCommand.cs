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

namespace Business.Handlers.Attendances.Commands
{
    public class DeleteAttendanceCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteAttendanceCommandHandler : IRequestHandler<DeleteAttendanceCommand, IResult>
        {
            private readonly IAttendanceRepository _attendanceRepository;
            private readonly IMediator _mediator;

            public DeleteAttendanceCommandHandler(IAttendanceRepository attendanceRepository, IMediator mediator)
            {
                _attendanceRepository = attendanceRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteAttendanceCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();
                var attendanceToDelete = await _attendanceRepository.GetAsync(p => p.Id == request.Id && p.IsDeleted == false);

                if (attendanceToDelete == null)
                    return new ErrorResult("Kayıt bulunamadı.");

                attendanceToDelete.IsDeleted = true;
                attendanceToDelete.DeletedBy = userId > 0 ? userId : null;
                attendanceToDelete.DeletedDate = System.DateTime.Now;

                _attendanceRepository.Update(attendanceToDelete);
                await _attendanceRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}
