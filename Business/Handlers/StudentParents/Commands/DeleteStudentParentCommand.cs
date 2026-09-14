
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


namespace Business.Handlers.StudentParents.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteStudentParentCommand : IRequest<IResult>
    {
        public int StudentId { get; set; }

        public class DeleteStudentParentCommandHandler : IRequestHandler<DeleteStudentParentCommand, IResult>
        {
            private readonly IStudentParentRepository _studentParentRepository;
            private readonly IMediator _mediator;

            public DeleteStudentParentCommandHandler(IStudentParentRepository studentParentRepository, IMediator mediator)
            {
                _studentParentRepository = studentParentRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteStudentParentCommand request, CancellationToken cancellationToken)
            {
                var studentParentToDelete = _studentParentRepository.Get(p => p.StudentId == request.StudentId);

                _studentParentRepository.Delete(studentParentToDelete);
                await _studentParentRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

