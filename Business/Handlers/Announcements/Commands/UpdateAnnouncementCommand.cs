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
using Business.Handlers.Announcements.ValidationRules;
using Core.Entities.Dtos.AnnouncementDto;
using Core.Extensions;

namespace Business.Handlers.Announcements.Commands
{
    public class UpdateAnnouncementCommand : IRequest<IDataResult<AnnouncementUpdateResponseDto>>
    {
        public int Id { get; set; }
        public int? TenantId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string Tag { get; set; }
        public string Icon { get; set; }
        public string ImageUrl { get; set; }
        public bool IsImportant { get; set; }
        public bool IsPublished { get; set; }
        public System.DateTime? PublishDate { get; set; }
        public System.DateTime? ExpireDate { get; set; }
        public int TargetAudience { get; set; }
        public int? BranchId { get; set; }

        public class UpdateAnnouncementCommandHandler : IRequestHandler<UpdateAnnouncementCommand, IDataResult<AnnouncementUpdateResponseDto>>
        {
            private readonly IAnnouncementRepository _announcementRepository;
            private readonly IMediator _mediator;

            public UpdateAnnouncementCommandHandler(IAnnouncementRepository announcementRepository, IMediator mediator)
            {
                _announcementRepository = announcementRepository;
                _mediator = mediator;
            }

            [ValidationAspect(typeof(UpdateAnnouncementValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<AnnouncementUpdateResponseDto>> Handle(UpdateAnnouncementCommand request, CancellationToken cancellationToken)
            {
                var userId = UserInfoExtensions.GetUserIdOrZero();
                var isThereAnnouncementRecord = await _announcementRepository.GetAsync(u => u.Id == request.Id && u.IsDeleted == false);

                if (isThereAnnouncementRecord == null)
                    return new ErrorDataResult<AnnouncementUpdateResponseDto>("Duyuru bulunamadı.");

                if (request.TenantId.HasValue && request.TenantId.Value > 0)
                {
                    isThereAnnouncementRecord.TenantId = request.TenantId.Value;
                }

                isThereAnnouncementRecord.Title = request.Title;
                isThereAnnouncementRecord.Summary = request.Summary;
                isThereAnnouncementRecord.Content = request.Content;
                isThereAnnouncementRecord.Author = request.Author;
                isThereAnnouncementRecord.Tag = request.Tag;
                isThereAnnouncementRecord.Icon = request.Icon;
                isThereAnnouncementRecord.ImageUrl = request.ImageUrl;
                isThereAnnouncementRecord.IsImportant = request.IsImportant;
                isThereAnnouncementRecord.IsPublished = request.IsPublished;
                isThereAnnouncementRecord.PublishDate = request.PublishDate;
                isThereAnnouncementRecord.ExpireDate = request.ExpireDate;
                isThereAnnouncementRecord.TargetAudience = request.TargetAudience;
                isThereAnnouncementRecord.BranchId = request.BranchId;
                isThereAnnouncementRecord.UpdatedBy = userId > 0 ? userId : null;
                isThereAnnouncementRecord.UpdatedDate = System.DateTime.Now;

                _announcementRepository.Update(isThereAnnouncementRecord);
                await _announcementRepository.SaveChangesAsync();

                var dto = new AnnouncementUpdateResponseDto
                {
                    Id = isThereAnnouncementRecord.Id,
                    Title = isThereAnnouncementRecord.Title,
                    Summary = isThereAnnouncementRecord.Summary,
                    Author = isThereAnnouncementRecord.Author,
                    Tag = isThereAnnouncementRecord.Tag,
                    IsImportant = isThereAnnouncementRecord.IsImportant
                };

                return new SuccessDataResult<AnnouncementUpdateResponseDto>(dto, Messages.Updated);
            }
        }
    }
}
