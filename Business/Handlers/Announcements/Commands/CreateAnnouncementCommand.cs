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
using Business.Handlers.Announcements.ValidationRules;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.AnnouncementDto;
using Core.Extensions;
using Business.Services;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Business.Handlers.Announcements.Commands
{
    public class CreateAnnouncementCommand : IRequest<IDataResult<AnnouncementCreateResponseDto>>
    {
        public int? TenantId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string Tag { get; set; }
        public string Icon { get; set; }
        public string ImageUrl { get; set; }
        public bool IsImportant { get; set; }
        public bool IsPublished { get; set; } = true;
        public System.DateTime? PublishDate { get; set; }
        public System.DateTime? ExpireDate { get; set; }
        public int TargetAudience { get; set; }
        public int? BranchId { get; set; }

        public class CreateAnnouncementCommandHandler : IRequestHandler<CreateAnnouncementCommand, IDataResult<AnnouncementCreateResponseDto>>
        {
            private readonly IAnnouncementRepository _announcementRepository;
            private readonly IMediator _mediator;
            private readonly IPushNotificationService _pushNotificationService;
            private readonly ITenantUserRepository _tenantUserRepository;

            public CreateAnnouncementCommandHandler(
                IAnnouncementRepository announcementRepository,
                IMediator mediator,
                IPushNotificationService pushNotificationService,
                ITenantUserRepository tenantUserRepository)
            {
                _announcementRepository = announcementRepository;
                _mediator = mediator;
                _pushNotificationService = pushNotificationService;
                _tenantUserRepository = tenantUserRepository;
            }

            [ValidationAspect(typeof(CreateAnnouncementValidator), Priority = 1)]
            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<AnnouncementCreateResponseDto>> Handle(CreateAnnouncementCommand request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();
                int targetTenantId = userTenantId > 0 ? userTenantId : (request.TenantId ?? 0);

                var addedAnnouncement = new Announcement
                {
                    TenantId = targetTenantId,
                    Title = request.Title,
                    Summary = request.Summary,
                    Content = request.Content,
                    Author = request.Author,
                    Tag = request.Tag,
                    Icon = request.Icon,
                    ImageUrl = request.ImageUrl,
                    IsImportant = request.IsImportant,
                    IsPublished = request.IsPublished,
                    PublishDate = request.PublishDate ?? System.DateTime.Now,
                    ExpireDate = request.ExpireDate,
                    TargetAudience = request.TargetAudience,
                    BranchId = request.BranchId,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = System.DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                };

                _announcementRepository.Add(addedAnnouncement);
                await _announcementRepository.SaveChangesAsync();

                // Tenant kullanıcılarına otomatik push bildirim gönder (Arka planda)
                if (targetTenantId > 0 && addedAnnouncement.IsPublished)
                {
                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            var tenantUsers = await _tenantUserRepository.GetListAsync(tu => tu.TenantId == targetTenantId && tu.IsActive == true && tu.IsDeleted != true);
                            var userIds = tenantUsers.Select(tu => tu.UserId).Distinct().ToList();
                            var pushTitle = $"📢 Yeni Duyuru: {addedAnnouncement.Title}";
                            var pushBody = string.IsNullOrWhiteSpace(addedAnnouncement.Summary) ? addedAnnouncement.Title : addedAnnouncement.Summary;
                            var pushData = new Dictionary<string, string> { { "type", "announcement" }, { "id", addedAnnouncement.Id.ToString() } };

                            foreach (var uid in userIds)
                            {
                                await _pushNotificationService.SendToUserAsync(uid, pushTitle, pushBody, pushData);
                            }
                        }
                        catch
                        {
                            // Bildirim hatası duyuru kaydını engellemez
                        }
                    });
                }

                var dto = new AnnouncementCreateResponseDto
                {
                    Id = addedAnnouncement.Id,
                    Title = addedAnnouncement.Title,
                    Summary = addedAnnouncement.Summary,
                    Author = addedAnnouncement.Author,
                    Tag = addedAnnouncement.Tag,
                    IsImportant = addedAnnouncement.IsImportant
                };

                return new SuccessDataResult<AnnouncementCreateResponseDto>(dto, Messages.Added);
            }
        }
    }
}