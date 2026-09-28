using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Business.BusinessAspects;
using Core.Aspects.Autofac.Caching;
using Core.Aspects.Autofac.Logging;
using Core.Aspects.Autofac.Performance;
using Core.CrossCuttingConcerns.Logging.Serilog.Loggers;
using Core.Entities.Dtos;
using Core.Utilities.Results;
using DataAccess.Abstract;
using MediatR;

namespace Business.Handlers.Users.Queries
{
    public class GetUsersQuery : IRequest<IDataResult<IEnumerable<UserDto>>>
    {
        public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IDataResult<IEnumerable<UserDto>>>
        {
            private readonly IUserRepository _userRepository;
            private readonly ITenantUserRepository _tenantUserRepository;
            private readonly ITenantRepository _tenantRepository;
            private readonly IUserGroupRepository _userGroupRepository;
            private readonly IGroupRepository _groupRepository;
            private readonly IMapper _mapper;

            public GetUsersQueryHandler(
                IUserRepository userRepository,
                ITenantUserRepository tenantUserRepository,
                ITenantRepository tenantRepository,
                IUserGroupRepository userGroupRepository,
                IGroupRepository groupRepository,
                IMapper mapper)
            {
                _userRepository = userRepository;
                _tenantUserRepository = tenantUserRepository;
                _tenantRepository = tenantRepository;
                _userGroupRepository = userGroupRepository;
                _groupRepository = groupRepository;
                _mapper = mapper;
            }

            [SecuredOperation(Priority = 1)]
            public async Task<IDataResult<IEnumerable<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var userTenantId = Core.Extensions.UserInfoExtensions.GetTenantIdOrZero();
                    var userList = (await _userRepository.GetListAsync())?.ToList() ?? new List<User>();

                    List<Core.Entities.Concrete.Project.TenantUser> tenantUsers = new();
                    try
                    {
                        tenantUsers = _tenantUserRepository.Query().Where(tu => tu.IsActive != false && tu.IsDeleted != true).ToList();
                    }
                    catch
                    {
                        // Fallback if tenantUser query encounters issues
                    }

                    List<Core.Entities.Concrete.Project.Tenant> tenants = new();
                    try
                    {
                        tenants = _tenantRepository.Query().Where(t => t.IsDeleted != true).ToList();
                    }
                    catch
                    {
                        // Fallback if tenant query encounters issues
                    }

                    List<UserGroup> userGroups = new();
                    try
                    {
                        userGroups = _userGroupRepository.Query().ToList();
                    }
                    catch
                    {
                    }

                    List<Group> groups = new();
                    try
                    {
                        groups = _groupRepository.Query().ToList();
                    }
                    catch
                    {
                    }

                    var userDtoList = userList
                        .Where(user =>
                        {
                            if (userTenantId == 0) return true;
                            var tu = tenantUsers.FirstOrDefault(x => x.UserId == user.UserId);
                            return tu != null && tu.TenantId == userTenantId;
                        })
                        .Select(user =>
                        {
                            var tu = tenantUsers.FirstOrDefault(x => x.UserId == user.UserId);
                            var t = tu != null ? tenants.FirstOrDefault(x => x.Id == tu.TenantId) : null;
                            var uGroups = userGroups.Where(ug => ug.UserId == user.UserId).ToList();

                            var dto = new UserDto
                            {
                                UserId = user.UserId,
                                FullName = user.FullName ?? "",
                                Email = user.Email ?? "",
                                MobilePhones = user.MobilePhones ?? "",
                                Status = user.Status,
                                TenantId = tu?.TenantId,
                                TenantName = t?.Name,
                                UserGroups = uGroups.Select(ug =>
                                {
                                    var g = groups.FirstOrDefault(x => x.Id == ug.GroupId);
                                    return new SelectionItem
                                    {
                                        Id = ug.GroupId.ToString(),
                                        Label = g != null ? g.GroupName : $"Grup #{ug.GroupId}"
                                    };
                                }).ToList()
                            };

                            return dto;
                        }).ToList();

                    return new SuccessDataResult<IEnumerable<UserDto>>(userDtoList);
                }
                catch (System.Exception ex)
                {
                    System.Console.WriteLine($"[GetUsersQuery Error]: {ex}");
                    // Fallback to basic user list if relations fail
                    var basicUsers = (await _userRepository.GetListAsync())?.Select(u => new UserDto
                    {
                        UserId = u.UserId,
                        FullName = u.FullName ?? "",
                        Email = u.Email ?? "",
                        MobilePhones = u.MobilePhones ?? "",
                        Status = u.Status
                    }).ToList() ?? new List<UserDto>();

                    return new SuccessDataResult<IEnumerable<UserDto>>(basicUsers);
                }
            }
        }
    }
}