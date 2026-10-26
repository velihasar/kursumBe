using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Business.BusinessAspects;
using Business.Constants;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Concrete.Project;
using Core.Entities.Dtos.StudentDto;
using Core.Extensions;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Business.Handlers.Students.Commands
{
    public class ImportStudentsFromExcelCommand : IRequest<IDataResult<ExcelBulkImportResponseDto>>
    {
        public ExcelBulkImportRequestDto Model { get; set; } = new();

        public class ImportStudentsFromExcelCommandHandler : IRequestHandler<ImportStudentsFromExcelCommand, IDataResult<ExcelBulkImportResponseDto>>
        {
            private readonly IStudentRepository _studentRepository;
            private readonly IPersonRepository _personRepository;
            private readonly IBranchRepository _branchRepository;
            private readonly IStudentBranchRepository _studentBranchRepository;
            private readonly IParentRepository _parentRepository;
            private readonly IStudentParentRepository _studentParentRepository;

            public ImportStudentsFromExcelCommandHandler(
                IStudentRepository studentRepository,
                IPersonRepository personRepository,
                IBranchRepository branchRepository,
                IStudentBranchRepository studentBranchRepository,
                IParentRepository parentRepository,
                IStudentParentRepository studentParentRepository)
            {
                _studentRepository = studentRepository;
                _personRepository = personRepository;
                _branchRepository = branchRepository;
                _studentBranchRepository = studentBranchRepository;
                _parentRepository = parentRepository;
                _studentParentRepository = studentParentRepository;
            }

            [CacheRemoveAspect("Get")]
            [LogAspect(typeof(FileLogger))]
            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<ExcelBulkImportResponseDto>> Handle(ImportStudentsFromExcelCommand request, CancellationToken cancellationToken)
            {
                var req = request.Model;
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();
                int targetTenantId = userTenantId > 0 ? userTenantId : (req.TenantId ?? 0);

                if (targetTenantId <= 0)
                {
                    return new ErrorDataResult<ExcelBulkImportResponseDto>("Geçerli bir kurum (Tenant) belirtilmelidir.");
                }

                if (req.Rows == null || req.Rows.Count == 0)
                {
                    return new ErrorDataResult<ExcelBulkImportResponseDto>("İçe aktarılacak satır bulunamadı.");
                }

                var response = new ExcelBulkImportResponseDto
                {
                    TotalProcessed = req.Rows.Count
                };

                // Cache branches for tenant
                var branches = await _branchRepository.Query()
                    .Where(b => b.TenantId == targetTenantId && b.IsDeleted == false)
                    .ToListAsync(cancellationToken);

                // Pre-count existing students for auto numbering
                var studentCount = await _studentRepository.Query()
                    .CountAsync(s => s.TenantId == targetTenantId && s.IsDeleted == false, cancellationToken);
                int nextAutoNumber = 1001 + studentCount;

                int rowIndex = 0;
                foreach (var row in req.Rows)
                {
                    rowIndex++;
                    if (string.IsNullOrWhiteSpace(row.FirstName) || string.IsNullOrWhiteSpace(row.LastName))
                    {
                        response.FailedCount++;
                        response.ErrorMessages.Add($"Satır {rowIndex}: Öğrenci adı ve soyadı zorunludur.");
                        continue;
                    }

                    try
                    {
                        string trimmedFirstName = row.FirstName.Trim();
                        string trimmedLastName = row.LastName.Trim();
                        string? trimmedPhone = string.IsNullOrWhiteSpace(row.Phone) ? null : row.Phone.Trim();
                        string? trimmedEmail = string.IsNullOrWhiteSpace(row.Email) ? null : row.Email.Trim();

                        // 1. Duplicate check if requested
                        if (req.ImportMode == "SkipDuplicates")
                        {
                            bool alreadyExists = await _personRepository.Query()
                                .AnyAsync(p => p.TenantId == targetTenantId && p.IsDeleted == false
                                    && p.FirstName.ToLower() == trimmedFirstName.ToLower()
                                    && p.LastName.ToLower() == trimmedLastName.ToLower()
                                    && (trimmedPhone == null || p.Phone == trimmedPhone), cancellationToken);

                            if (alreadyExists)
                            {
                                response.FailedCount++;
                                response.ErrorMessages.Add($"Satır {rowIndex}: {trimmedFirstName} {trimmedLastName} zaten kayıtlı (Atlandı).");
                                continue;
                            }
                        }

                        // 2. Create Student Person
                        var studentPerson = new Person
                        {
                            TenantId = targetTenantId,
                            FirstName = trimmedFirstName,
                            LastName = trimmedLastName,
                            Phone = trimmedPhone,
                            Email = trimmedEmail,
                            DateOfBirth = row.DateOfBirth,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = userId > 0 ? userId : null,
                            CreatedDate = DateTime.Now
                        };
                        _personRepository.Add(studentPerson);
                        await _personRepository.SaveChangesAsync();

                        // 3. Determine student number & access code
                        string studentNumber = row.StudentNumber?.Trim();
                        if (string.IsNullOrWhiteSpace(studentNumber))
                        {
                            studentNumber = nextAutoNumber.ToString();
                            nextAutoNumber++;
                        }

                        string accessCode = GenerateAccessCode(_studentRepository);

                        var student = new Student
                        {
                            TenantId = targetTenantId,
                            PersonId = studentPerson.Id,
                            StudentNumber = studentNumber,
                            ParentAccessCode = accessCode,
                            EnrollmentDate = row.EnrollmentDate ?? DateTime.Now,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = userId > 0 ? userId : null,
                            CreatedDate = DateTime.Now
                        };
                        _studentRepository.Add(student);
                        await _studentRepository.SaveChangesAsync();

                        // 4. Branch assignment
                        Branch? matchedBranch = null;
                        if (row.BranchId.HasValue && row.BranchId.Value > 0)
                        {
                            matchedBranch = branches.FirstOrDefault(b => b.Id == row.BranchId.Value);
                        }
                        else if (!string.IsNullOrWhiteSpace(row.BranchName))
                        {
                            var targetName = row.BranchName.Trim().ToLower();
                            matchedBranch = branches.FirstOrDefault(b => b.Name.ToLower() == targetName);
                        }

                        if (matchedBranch != null)
                        {
                            var sb = new StudentBranch
                            {
                                TenantId = targetTenantId,
                                StudentId = student.Id,
                                BranchId = matchedBranch.Id,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = userId > 0 ? userId : null,
                                CreatedDate = DateTime.Now
                            };
                            _studentBranchRepository.Add(sb);
                            await _studentBranchRepository.SaveChangesAsync();
                        }

                        // 5. Parent assignment
                        if (!string.IsNullOrWhiteSpace(row.ParentFirstName))
                        {
                            string pFirst = row.ParentFirstName.Trim();
                            string pLast = (row.ParentLastName ?? trimmedLastName).Trim();
                            string? pPhone = string.IsNullOrWhiteSpace(row.ParentPhone) ? null : row.ParentPhone.Trim();
                            string? pEmail = string.IsNullOrWhiteSpace(row.ParentEmail) ? null : row.ParentEmail.Trim();

                            var parentPerson = new Person
                            {
                                TenantId = targetTenantId,
                                FirstName = pFirst,
                                LastName = pLast,
                                Phone = pPhone,
                                Email = pEmail,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = userId > 0 ? userId : null,
                                CreatedDate = DateTime.Now
                            };
                            _personRepository.Add(parentPerson);
                            await _personRepository.SaveChangesAsync();

                            var parent = new Parent
                            {
                                TenantId = targetTenantId,
                                PersonId = parentPerson.Id,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = userId > 0 ? userId : null,
                                CreatedDate = DateTime.Now
                            };
                            _parentRepository.Add(parent);
                            await _parentRepository.SaveChangesAsync();

                            var studentParent = new StudentParent
                            {
                                TenantId = targetTenantId,
                                StudentId = student.Id,
                                ParentId = parent.Id,
                                Relationship = string.IsNullOrWhiteSpace(row.Relationship) ? "Veli" : row.Relationship.Trim(),
                                IsPrimary = row.IsPrimary,
                                IsActive = true,
                                IsDeleted = false,
                                CreatedBy = userId > 0 ? userId : null,
                                CreatedDate = DateTime.Now
                            };
                            _studentParentRepository.Add(studentParent);
                            await _studentParentRepository.SaveChangesAsync();
                        }

                        response.SuccessCount++;
                        response.ImportedStudentNames.Add($"{trimmedFirstName} {trimmedLastName}");
                    }
                    catch (Exception ex)
                    {
                        response.FailedCount++;
                        response.ErrorMessages.Add($"Satır {rowIndex}: Beklenmeyen hata - {ex.Message}");
                    }
                }

                return new SuccessDataResult<ExcelBulkImportResponseDto>(response, $"{response.SuccessCount} öğrenci başarıyla içe aktarıldı.");
            }

            private static string GenerateAccessCode(IStudentRepository studentRepository)
            {
                var random = new Random();
                const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
                string code;
                do
                {
                    var codeStr = new string(Enumerable.Repeat(chars, 6)
                        .Select(s => s[random.Next(s.Length)]).ToArray());
                    code = $"KRS-{codeStr}";
                } while (studentRepository.Query().Any(s => s.ParentAccessCode == code));

                return code;
            }
        }
    }
}
