using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.AttendanceDto;
using Core.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Attendances.Commands
{
    public class BulkCreateAttendanceCommand : IRequest<IDataResult<BulkAttendanceResponseDto>>
    {
        public BulkAttendanceCreateDto Model { get; set; } = new BulkAttendanceCreateDto();

        public class BulkCreateAttendanceCommandHandler : IRequestHandler<BulkCreateAttendanceCommand, IDataResult<BulkAttendanceResponseDto>>
        {
            private readonly IAttendanceRepository _attendanceRepository;
            private readonly IMediator _mediator;

            public BulkCreateAttendanceCommandHandler(IAttendanceRepository attendanceRepository, IMediator mediator)
            {
                _attendanceRepository = attendanceRepository;
                _mediator = mediator;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<BulkAttendanceResponseDto>> Handle(BulkCreateAttendanceCommand request, CancellationToken cancellationToken)
            {
                var dto = request.Model;
                if (dto == null || dto.CourseId <= 0)
                {
                    return new ErrorDataResult<BulkAttendanceResponseDto>("Geçersiz yoklama isteği.");
                }

                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();

                int targetTenantId = userTenantId > 0 ? userTenantId : (dto.TenantId ?? 0);
                if (targetTenantId <= 0)
                {
                    return new ErrorDataResult<BulkAttendanceResponseDto>("SuperAdmin olarak işlem yapmaktasınız. Lütfen geçerli bir kurum (okul) seçiniz.");
                }

                if (dto.Records == null || dto.Records.Count == 0)
                {
                    return new ErrorDataResult<BulkAttendanceResponseDto>("Kaydedilecek öğrenci yoklama kaydı bulunamadı.");
                }

                var targetDate = dto.AttendanceDate.Date;
                var studentIds = dto.Records.Select(r => r.StudentId).ToList();

                // Get existing records for this tenant, course, date and students in a single query
                var existingRecords = await _attendanceRepository.Query()
                    .Where(a => a.IsDeleted == false &&
                                a.TenantId == targetTenantId &&
                                a.CourseId == dto.CourseId &&
                                a.AttendanceDate.Date == targetDate &&
                                studentIds.Contains(a.StudentId))
                    .ToListAsync(cancellationToken);

                var existingMap = existingRecords.ToDictionary(a => a.StudentId);

                int createdCount = 0;
                int updatedCount = 0;

                foreach (var item in dto.Records)
                {
                    if (existingMap.TryGetValue(item.StudentId, out var existing))
                    {
                        // Update
                        existing.IsPresent = item.IsPresent;
                        existing.Reason = item.Reason;
                        existing.UpdatedBy = userId > 0 ? userId : null;
                        existing.UpdatedDate = DateTime.Now;
                        _attendanceRepository.Update(existing);
                        updatedCount++;
                    }
                    else
                    {
                        // Create
                        var newRecord = new Attendance
                        {
                            TenantId = targetTenantId,
                            CourseId = dto.CourseId,
                            StudentId = item.StudentId,
                            AttendanceDate = dto.AttendanceDate,
                            IsPresent = item.IsPresent,
                            Reason = item.Reason,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = userId > 0 ? userId : null,
                            CreatedDate = DateTime.Now
                        };
                        _attendanceRepository.Add(newRecord);
                        createdCount++;
                    }
                }

                await _attendanceRepository.SaveChangesAsync();

                var response = new BulkAttendanceResponseDto
                {
                    TotalProcessed = dto.Records.Count,
                    CreatedCount = createdCount,
                    UpdatedCount = updatedCount,
                    Message = $"{createdCount} yeni yoklama kaydedildi, {updatedCount} kayıt güncellendi."
                };

                return new SuccessDataResult<BulkAttendanceResponseDto>(response, response.Message);
            }
        }
    }
}
