using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Business.Handlers.StudentBranches.Commands
{
    public class DeleteStudentBranchCommand : IRequest<IResult>
    {
        public int StudentId { get; set; }
        public int BranchId { get; set; }

        public class DeleteStudentBranchCommandHandler : IRequestHandler<DeleteStudentBranchCommand, IResult>
        {
            private readonly IStudentBranchRepository _studentBranchRepository;
            private readonly IMediator _mediator;

            public DeleteStudentBranchCommandHandler(IStudentBranchRepository studentBranchRepository, IMediator mediator)
            {
                _studentBranchRepository = studentBranchRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteStudentBranchCommand request, CancellationToken cancellationToken)
            {
                var record = await _studentBranchRepository.GetAsync(p => p.StudentId == request.StudentId && p.BranchId == request.BranchId);
                if (record == null)
                {
                    return new ErrorResult("Kayıt bulunamadı.");
                }

                _studentBranchRepository.Delete(record);
                await _studentBranchRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}
