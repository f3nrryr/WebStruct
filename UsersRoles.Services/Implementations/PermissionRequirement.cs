using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using UsersRoles.Services.Enums;
using UsersRoles.Services.Interfaces;

namespace UsersRoles.Services.Implementations
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionEnum Permission { get; }

        public PermissionRequirement(PermissionEnum permission)
        {
            Permission = permission;
        }
    }

    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IRolesService _roleService;

        public PermissionAuthorizationHandler(IRolesService roleService)
        {
            _roleService = roleService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId != null && await _roleService.UserHasPermissionAsync(userId, requirement.Permission))
            {
                context.Succeed(requirement);
            }
        }
    }

    public class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(PermissionEnum permission)
        {
            Policy = permission.GetName();
        }
    }
}
