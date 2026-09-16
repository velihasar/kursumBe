using Business.BusinessAspects;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos.FeeDueDto;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.FeeDues.Queries
{
    public class GetFeeDueQuery : IRequest<IDataResult<FeeDueGetByIdDto>>
    {
        public int Id { get; set; }

        public class GetFeeDueQueryHandler : IRequestHandler<GetFeeDueQuery, IDataResult<FeeDueGetByIdDto>>
        {
            private readonly IFeeDueRepository _feeDueRepository;
            private readonly IMediator _mediator;

            public GetFeeDueQueryHandler(IFeeDueRepository feeDueRepository, IMediator mediator)
            {
                _feeDueRepository = feeDueRepository;
                _mediator = mediator;
            }

            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<FeeDueGetByIdDto>> Handle(GetFeeDueQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var query = _feeDueRepository.Query()
                    .Include(x => x.Tenant)
                    .Include(x => x.Student).ThenInclude(s => s.Person);

                var feeDue = await query.FirstOrDefaultAsync(p => p.Id == request.Id && p.IsDeleted == false && (userTenantId <= 0 || p.TenantId == userTenantId), cancellationToken);

                if (feeDue == null)
                    return new ErrorDataResult<FeeDueGetByIdDto>("Kayıt bulunamadı.");

                var dto = new FeeDueGetByIdDto
                {
                    Id = feeDue.Id,
                    TenantId = feeDue.TenantId,
                    TenantName = feeDue.Tenant != null ? feeDue.Tenant.Name : null,
                    CourseEnrollmentId = feeDue.CourseEnrollmentId,
                    StudentId = feeDue.StudentId,
                    StudentName = feeDue.Student != null && feeDue.Student.Person != null ? $"{feeDue.Student.Person.FirstName} {feeDue.Student.Person.LastName}".Trim() : null,
                    Period = feeDue.Period,
                    Title = feeDue.Title,
                    Amount = feeDue.Amount,
                    PaidAmount = feeDue.PaidAmount,
                    RemainingAmount = feeDue.RemainingAmount,
                    DueDate = feeDue.DueDate,
                    Status = feeDue.Status,
                    Description = feeDue.Description,
                    IsActive = feeDue.IsActive
                };

                return new SuccessDataResult<FeeDueGetByIdDto>(dto);
            }
        }
    }
}
