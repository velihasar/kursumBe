using Business.BusinessAspects;
using Core.Aspects.Autofac.Performance;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Aspects.Autofac.Caching;
using Core.Entities.Dtos.AnnouncementDto;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Announcements.Queries
{
    public class GetAnnouncementsQuery : IRequest<IDataResult<IEnumerable<AnnouncementGetAllDto>>>
    {
        public int? TenantId { get; set; }
        public int? BranchId { get; set; }

        public class GetAnnouncementsQueryHandler : IRequestHandler<GetAnnouncementsQuery, IDataResult<IEnumerable<AnnouncementGetAllDto>>>
        {
            private readonly IAnnouncementRepository _announcementRepository;
            private readonly IMediator _mediator;

            public GetAnnouncementsQueryHandler(IAnnouncementRepository announcementRepository, IMediator mediator)
            {
                _announcementRepository = announcementRepository;
                _mediator = mediator;
            }

            [PerformanceAspect(5)]
            [CacheAspect(10)]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<AnnouncementGetAllDto>>> Handle(GetAnnouncementsQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var query = _announcementRepository.Query()
                    .Include(x => x.Tenant)
                    .Include(x => x.Branch)
                    .Where(x => x.IsDeleted == false);

                if (userTenantId > 0)
                {
                    query = query.Where(x => x.TenantId == userTenantId);
                }
                else if (request.TenantId.HasValue && request.TenantId.Value > 0)
                {
                    query = query.Where(x => x.TenantId == request.TenantId.Value);
                }

                if (request.BranchId.HasValue && request.BranchId.Value > 0)
                {
                    query = query.Where(x => x.BranchId == request.BranchId.Value || x.BranchId == null);
                }

                var list = await query.OrderByDescending(x => x.PublishDate ?? x.CreatedDate).ToListAsync(cancellationToken);
                var dtos = list.Select(x => new AnnouncementGetAllDto
                {
                    Id = x.Id,
                    TenantId = x.TenantId,
                    TenantName = x.Tenant != null ? x.Tenant.Name : null,
                    Title = x.Title,
                    Summary = x.Summary,
                    Content = x.Content,
                    Author = x.Author,
                    Tag = x.Tag,
                    Icon = x.Icon,
                    ImageUrl = x.ImageUrl,
                    IsImportant = x.IsImportant,
                    IsPublished = x.IsPublished,
                    PublishDate = x.PublishDate,
                    ExpireDate = x.ExpireDate,
                    TargetAudience = x.TargetAudience,
                    BranchId = x.BranchId,
                    BranchName = x.Branch != null ? x.Branch.Name : null,
                    IsActive = x.IsActive
                });

                return new SuccessDataResult<IEnumerable<AnnouncementGetAllDto>>(dtos);
            }
        }
    }
}