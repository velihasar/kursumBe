using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Business.Handlers.FeeDues.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.FeeDueDto;
using Core.Extensions;

namespace Business.Handlers.FeeDues.Commands
{
    public class CreateFeeDueCommand : IRequest<IDataResult<FeeDueCreateResponseDto>>
    {
        public int? TenantId { get; set; }
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

        public class CreateFeeDueCommandHandler : IRequestHandler<CreateFeeDueCommand, IDataResult<FeeDueCreateResponseDto>>
        {
            private readonly IFeeDueRepository _feeDueRepository;
            private readonly IMediator _mediator;

            public CreateFeeDueCommandHandler(IFeeDueRepository feeDueRepository, IMediator mediator)
            {
                _feeDueRepository = feeDueRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(CreateFeeDueValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<FeeDueCreateResponseDto>> Handle(CreateFeeDueCommand request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();

                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                if (targetTenantId <= 0)
                {
                    return new ErrorDataResult<FeeDueCreateResponseDto>("SuperAdmin olarak işlem yapmaktasınız. Lütfen geçerli bir kurum (okul) seçiniz.");
                }

                var isThereFeeDueRecord = _feeDueRepository.Query().Any(u => u.IsDeleted == false && u.TenantId == targetTenantId && u.CourseEnrollmentId == request.CourseEnrollmentId && u.Period == request.Period);

                if (isThereFeeDueRecord)
                    return new ErrorDataResult<FeeDueCreateResponseDto>(Messages.NameAlreadyExist);

                var addedFeeDue = new FeeDue
                {
                    TenantId = targetTenantId,
                    CourseEnrollmentId = request.CourseEnrollmentId,
                    StudentId = request.StudentId,
                    Period = request.Period,
                    Title = request.Title,
                    Amount = request.Amount,
                    PaidAmount = request.PaidAmount,
                    RemainingAmount = request.RemainingAmount,
                    DueDate = request.DueDate,
                    Status = request.Status,
                    Description = request.Description,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = System.DateTime.Now
                };

                _feeDueRepository.Add(addedFeeDue);
                await _feeDueRepository.SaveChangesAsync();

                var dto = new FeeDueCreateResponseDto
                {
                    Id = addedFeeDue.Id,
                    CourseEnrollmentId = addedFeeDue.CourseEnrollmentId,
                    StudentId = addedFeeDue.StudentId,
                    Period = addedFeeDue.Period,
                    Title = addedFeeDue.Title,
                    Amount = addedFeeDue.Amount,
                    PaidAmount = addedFeeDue.PaidAmount,
                    RemainingAmount = addedFeeDue.RemainingAmount,
                    DueDate = addedFeeDue.DueDate,
                    Status = addedFeeDue.Status,
                    Description = addedFeeDue.Description
                };

                return new SuccessDataResult<FeeDueCreateResponseDto>(dto, Messages.Added);
            }
        }
    }
}