using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.AnnouncementDto;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Announcements.Queries
{
    public class GetAnnouncementQuery : IRequest<IDataResult<AnnouncementGetByIdDto>>
    {
        public int Id { get; set; }

        public class GetAnnouncementQueryHandler : IRequestHandler<GetAnnouncementQuery, IDataResult<AnnouncementGetByIdDto>>
        {
            private readonly IAnnouncementRepository _announcementRepository;
            private readonly IMediator _mediator;

            public GetAnnouncementQueryHandler(IAnnouncementRepository announcementRepository, IMediator mediator)
            {
                _announcementRepository = announcementRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<AnnouncementGetByIdDto>> Handle(GetAnnouncementQuery request, CancellationToken cancellationToken)
            {
                var x = await _announcementRepository.Query()
                    .Include(t => t.Tenant)
                    .Include(t => t.Branch)
                    .FirstOrDefaultAsync(p => p.Id == request.Id && p.IsDeleted == false, cancellationToken);

                if (x == null)
                    return new ErrorDataResult<AnnouncementGetByIdDto>("Duyuru bulunamadı.");

                var dto = new AnnouncementGetByIdDto
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
                };

                return new SuccessDataResult<AnnouncementGetByIdDto>(dto);
            }
        }
    }
}
