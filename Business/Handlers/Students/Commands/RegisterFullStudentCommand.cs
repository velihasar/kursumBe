using System;
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
    public class RegisterFullStudentCommand : IRequest<IDataResult<RegisterFullStudentResponseDto>>
    {
        public RegisterFullStudentRequestDto Model { get; set; } = new();

        public class RegisterFullStudentCommandHandler : IRequestHandler<RegisterFullStudentCommand, IDataResult<RegisterFullStudentResponseDto>>
        {
            private readonly IStudentRepository _studentRepository;
            private readonly IPersonRepository _personRepository;
            private readonly IBranchRepository _branchRepository;
            private readonly IStudentBranchRepository _studentBranchRepository;
            private readonly IParentRepository _parentRepository;
            private readonly IStudentParentRepository _studentParentRepository;

            public RegisterFullStudentCommandHandler(
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
            public async Task<IDataResult<RegisterFullStudentResponseDto>> Handle(RegisterFullStudentCommand request, CancellationToken cancellationToken)
            {
                var req = request.Model;
                var userTenantId = UserInfoExtensions.GetTenantIdOrZero();
                var userId = UserInfoExtensions.GetUserIdOrZero();
                int targetTenantId = userTenantId > 0 ? userTenantId : (req.TenantId ?? 0);

                if (targetTenantId <= 0)
                {
                    return new ErrorDataResult<RegisterFullStudentResponseDto>("Geçerli bir kurum (Tenant) belirtilmelidir.");
                }

                if (string.IsNullOrWhiteSpace(req.FirstName) || string.IsNullOrWhiteSpace(req.LastName))
                {
                    return new ErrorDataResult<RegisterFullStudentResponseDto>("Öğrenci adı ve soyadı zorunludur.");
                }

                // 1. Person: Mevcut veya Yeni
                Person studentPerson;
                if (req.ExistingPersonId.HasValue && req.ExistingPersonId.Value > 0)
                {
                    studentPerson = await _personRepository.GetAsync(p => p.Id == req.ExistingPersonId.Value && p.TenantId == targetTenantId);
                    if (studentPerson == null)
                    {
                        return new ErrorDataResult<RegisterFullStudentResponseDto>("Belirtilen kişi kaydı bulunamadı.");
                    }
                }
                else
                {
                    studentPerson = new Person
                    {
                        TenantId = targetTenantId,
                        FirstName = req.FirstName.Trim(),
                        LastName = req.LastName.Trim(),
                        Email = string.IsNullOrWhiteSpace(req.Email) ? null : req.Email.Trim(),
                        Phone = string.IsNullOrWhiteSpace(req.Phone) ? null : req.Phone.Trim(),
                        DateOfBirth = req.DateOfBirth,
                        PhotoUrl = req.PhotoUrl,
                        IsActive = true,
                        IsDeleted = false,
                        CreatedBy = userId > 0 ? userId : null,
                        CreatedDate = DateTime.Now
                    };
                    _personRepository.Add(studentPerson);
                    await _personRepository.SaveChangesAsync();
                }

                // 2. Student Numarası ve ParentAccessCode
                string finalStudentNumber = req.StudentNumber?.Trim();
                if (string.IsNullOrWhiteSpace(finalStudentNumber))
                {
                    var count = _studentRepository.Query().Count(s => s.TenantId == targetTenantId && s.IsDeleted == false);
                    int nextNum = 1001 + count;
                    while (_studentRepository.Query().Any(s => s.TenantId == targetTenantId && s.StudentNumber == nextNum.ToString() && s.IsDeleted == false))
                    {
                        nextNum++;
                    }
                    finalStudentNumber = nextNum.ToString();
                }

                string accessCode = GenerateAccessCode(_studentRepository);

                var student = new Student
                {
                    TenantId = targetTenantId,
                    PersonId = studentPerson.Id,
                    StudentNumber = finalStudentNumber,
                    ParentAccessCode = accessCode,
                    EnrollmentDate = req.EnrollmentDate != default ? req.EnrollmentDate : DateTime.Now,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = userId > 0 ? userId : null,
                    CreatedDate = DateTime.Now
                };
                _studentRepository.Add(student);
                await _studentRepository.SaveChangesAsync();

                // 3. Şube (Branch) Bağlantısı
                string? branchName = null;
                if (req.BranchId.HasValue && req.BranchId.Value > 0)
                {
                    var branch = await _branchRepository.GetAsync(b => b.Id == req.BranchId.Value && b.TenantId == targetTenantId && b.IsDeleted == false);
                    if (branch != null)
                    {
                        branchName = branch.Name;
                        var studentBranch = new StudentBranch
                        {
                            TenantId = targetTenantId,
                            StudentId = student.Id,
                            BranchId = branch.Id,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = userId > 0 ? userId : null,
                            CreatedDate = DateTime.Now
                        };
                        _studentBranchRepository.Add(studentBranch);
                        await _studentBranchRepository.SaveChangesAsync();
                    }
                }

                // 4. Veli (Parent) ve StudentParent Bağlantısı
                int? parentId = null;
                string? parentFullName = null;

                if (req.IncludeParent || !string.IsNullOrWhiteSpace(req.ParentFirstName) || (req.ExistingParentPersonId.HasValue && req.ExistingParentPersonId.Value > 0))
                {
                    Parent? parent = null;
                    if (req.ExistingParentPersonId.HasValue && req.ExistingParentPersonId.Value > 0)
                    {
                        var parentPerson = await _personRepository.GetAsync(p => p.Id == req.ExistingParentPersonId.Value && p.TenantId == targetTenantId);
                        if (parentPerson != null)
                        {
                            parentFullName = $"{parentPerson.FirstName} {parentPerson.LastName}";
                            parent = await _parentRepository.GetAsync(p => p.PersonId == parentPerson.Id && p.TenantId == targetTenantId);
                            if (parent == null)
                            {
                                parent = new Parent
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
                            }
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(req.ParentFirstName))
                    {
                        var parentPerson = new Person
                        {
                            TenantId = targetTenantId,
                            FirstName = req.ParentFirstName.Trim(),
                            LastName = (req.ParentLastName ?? string.Empty).Trim(),
                            Phone = string.IsNullOrWhiteSpace(req.ParentPhone) ? null : req.ParentPhone.Trim(),
                            Email = string.IsNullOrWhiteSpace(req.ParentEmail) ? null : req.ParentEmail.Trim(),
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = userId > 0 ? userId : null,
                            CreatedDate = DateTime.Now
                        };
                        _personRepository.Add(parentPerson);
                        await _personRepository.SaveChangesAsync();

                        parentFullName = $"{parentPerson.FirstName} {parentPerson.LastName}".Trim();

                        parent = new Parent
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
                    }

                    if (parent != null)
                    {
                        parentId = parent.Id;
                        var studentParent = new StudentParent
                        {
                            TenantId = targetTenantId,
                            StudentId = student.Id,
                            ParentId = parent.Id,
                            Relationship = string.IsNullOrWhiteSpace(req.Relationship) ? "Veli" : req.Relationship.Trim(),
                            IsPrimary = req.IsPrimary,
                            IsActive = true,
                            IsDeleted = false,
                            CreatedBy = userId > 0 ? userId : null,
                            CreatedDate = DateTime.Now
                        };
                        _studentParentRepository.Add(studentParent);
                        await _studentParentRepository.SaveChangesAsync();
                    }
                }

                var response = new RegisterFullStudentResponseDto
                {
                    StudentId = student.Id,
                    PersonId = studentPerson.Id,
                    StudentNumber = student.StudentNumber,
                    FullName = $"{studentPerson.FirstName} {studentPerson.LastName}",
                    ParentId = parentId,
                    ParentFullName = parentFullName,
                    BranchId = req.BranchId,
                    BranchName = branchName,
                    Message = "Öğrenci kaydı tüm ilişkileriyle birlikte başarıyla tamamlandı."
                };

                return new SuccessDataResult<RegisterFullStudentResponseDto>(response, Messages.Added);
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
