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
            [PerformanceAspect(5)]
            [LogAspect(typeof(ElasticSearchLogger))]
            public async Task<IDataResult<IEnumerable<UserDto>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
            {
                var userTenantId = Core.Extensions.UserInfoExtensions.GetTenantIdOrZero();
                var userList = await _userRepository.GetListAsync();
                var tenantUsers = _tenantUserRepository.Query().Where(tu => tu.IsActive == true && tu.IsDeleted == false).ToList();
                var tenants = _tenantRepository.Query().Where(t => t.IsDeleted == false).ToList();
                var userGroups = _userGroupRepository.Query().ToList();
                var groups = _groupRepository.Query().ToList();

                var userDtoList = userList
                    .Where(user =>
                    {
                        if (userTenantId == 0) return true;
                        var tu = tenantUsers.FirstOrDefault(x => x.UserId == user.UserId);
                        return tu != null && tu.TenantId == userTenantId;
                    })
                    .Select(user =>
                {
                    var dto = _mapper.Map<UserDto>(user);
                    var tu = tenantUsers.FirstOrDefault(x => x.UserId == user.UserId);
                    if (tu != null)
                    {
                        dto.TenantId = tu.TenantId;
                        var t = tenants.FirstOrDefault(x => x.Id == tu.TenantId);
                        if (t != null)
                        {
                            dto.TenantName = t.Name;
                        }
                    }

                    var uGroups = userGroups.Where(ug => ug.UserId == user.UserId).ToList();
                    dto.UserGroups = uGroups.Select(ug =>
                    {
                        var g = groups.FirstOrDefault(x => x.Id == ug.GroupId);
                        return new SelectionItem
                        {
                            Id = ug.GroupId.ToString(),
                            Label = g != null ? g.GroupName : $"Grup #{ug.GroupId}"
                        };
                    }).ToList();

                    return dto;
                }).ToList();

                return new SuccessDataResult<IEnumerable<UserDto>>(userDtoList);
            }
        }
    }
}