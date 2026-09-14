
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


namespace Business.Handlers.Teachers.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteTeacherCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteTeacherCommandHandler : IRequestHandler<DeleteTeacherCommand, IResult>
        {
            private readonly ITeacherRepository _teacherRepository;
            private readonly IMediator _mediator;

            public DeleteTeacherCommandHandler(ITeacherRepository teacherRepository, IMediator mediator)
            {
                _teacherRepository = teacherRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteTeacherCommand request, CancellationToken cancellationToken)
            {
                var teacherToDelete = _teacherRepository.Get(p => p.Id == request.Id);

                _teacherRepository.Delete(teacherToDelete);
                await _teacherRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

