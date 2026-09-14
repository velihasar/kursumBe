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

namespace Business.Handlers.TeacherBranches.Commands
{
    public class DeleteTeacherBranchCommand : IRequest<IResult>
    {
        public int TeacherId { get; set; }
        public int BranchId { get; set; }

        public class DeleteTeacherBranchCommandHandler : IRequestHandler<DeleteTeacherBranchCommand, IResult>
        {
            private readonly ITeacherBranchRepository _teacherBranchRepository;
            private readonly IMediator _mediator;

            public DeleteTeacherBranchCommandHandler(ITeacherBranchRepository teacherBranchRepository, IMediator mediator)
            {
                _teacherBranchRepository = teacherBranchRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IResult> Handle(DeleteTeacherBranchCommand request, CancellationToken cancellationToken)
            {
                var record = await _teacherBranchRepository.GetAsync(p => p.TeacherId == request.TeacherId && p.BranchId == request.BranchId);
                if (record == null)
                {
                    return new ErrorResult("Kayıt bulunamadı.");
                }

                _teacherBranchRepository.Delete(record);
                await _teacherBranchRepository.SaveChangesAsync();
                return new SuccessResult(Messages.Deleted);
            }
        }
    }
}
