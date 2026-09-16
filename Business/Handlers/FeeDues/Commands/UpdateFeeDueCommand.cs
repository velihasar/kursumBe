using Business.Constants;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Validation;
using Business.Handlers.FeeDues.ValidationRules;
using Core.Entities.Dtos.FeeDueDto;
using Core.Extensions;

namespace Business.Handlers.FeeDues.Commands
{
    public class UpdateFeeDueCommand : IRequest<IDataResult<FeeDueUpdateResponseDto>>
    {
        public int Id { get; set; }
        public int CourseEnrollmentId { get; set; }
        public int StudentId { get; set; }
        public string Period { get; set; }
        public string Title { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public System.DateTime DueDate { get; set; }
        public int Status { get; set; }
        public string Description { get; set; }

        public class UpdateFeeDueCommandHandler : IRequestHandler<UpdateFeeDueCommand, IDataResult<FeeDueUpdateResponseDto>>
        {
            private readonly IFeeDueRepository _feeDueRepository;
            private readonly IMediator _mediator;

            public UpdateFeeDueCommandHandler(IFeeDueRepository feeDueRepository, IMediator mediator)
            {
                _feeDueRepository = feeDueRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateFeeDueValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<FeeDueUpdateResponseDto>> Handle(UpdateFeeDueCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();
                var isThereFeeDueRecord = await _feeDueRepository.GetAsync(u => u.Id == request.Id && u.IsDeleted == false);

                if (isThereFeeDueRecord == null)
                    return new ErrorDataResult<FeeDueUpdateResponseDto>("Kayıt bulunamadı.");

                isThereFeeDueRecord.CourseEnrollmentId = request.CourseEnrollmentId;
                isThereFeeDueRecord.StudentId = request.StudentId;
                isThereFeeDueRecord.Period = request.Period;
                isThereFeeDueRecord.Title = request.Title;
                isThereFeeDueRecord.Amount = request.Amount;
                isThereFeeDueRecord.PaidAmount = request.PaidAmount;
                isThereFeeDueRecord.RemainingAmount = request.RemainingAmount;
                isThereFeeDueRecord.DueDate = request.DueDate;
                isThereFeeDueRecord.Status = request.Status;
                isThereFeeDueRecord.Description = request.Description;
                isThereFeeDueRecord.UpdatedBy = userId > 0 ? userId : null;
                isThereFeeDueRecord.UpdatedDate = System.DateTime.Now;

                _feeDueRepository.Update(isThereFeeDueRecord);
                await _feeDueRepository.SaveChangesAsync();

                var dto = new FeeDueUpdateResponseDto
                {
                    Id = isThereFeeDueRecord.Id,
                    CourseEnrollmentId = isThereFeeDueRecord.CourseEnrollmentId,
                    StudentId = isThereFeeDueRecord.StudentId,
                    Period = isThereFeeDueRecord.Period,
                    Title = isThereFeeDueRecord.Title,
                    Amount = isThereFeeDueRecord.Amount,
                    PaidAmount = isThereFeeDueRecord.PaidAmount,
                    RemainingAmount = isThereFeeDueRecord.RemainingAmount,
                    DueDate = isThereFeeDueRecord.DueDate,
                    Status = isThereFeeDueRecord.Status,
                    Description = isThereFeeDueRecord.Description
                };

                return new SuccessDataResult<FeeDueUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}
