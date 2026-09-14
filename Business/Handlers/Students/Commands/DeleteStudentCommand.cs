
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


namespace Business.Handlers.Students.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public class DeleteStudentCommand : IRequest<IResult>
    {
        public int Id { get; set; }

        public class DeleteStudentCommandHandler : IRequestHandler<DeleteStudentCommand, IResult>
        {
            private readonly IStudentRepository _studentRepository;
            private readonly IMediator _mediator;

            public DeleteStudentCommandHandler(IStudentRepository studentRepository, IMediator mediator)
            {
                _studentRepository = studentRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
            {
                var studentToDelete = _studentRepository.Get(p => p.Id == request.Id);

                _studentRepository.Delete(studentToDelete);
                await _studentRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}

