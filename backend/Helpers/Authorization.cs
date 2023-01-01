using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.Security;
using static OfficeOpenXml.ExcelErrorValue;

namespace backend.Helpers
{
    internal class PermissionAttribute : AuthorizeAttribute
    {
        private Permissao _permission;
        public Permissao Permission
        {
            get => _permission;
            set
            {
                _permission = value;
                Policy = $"{nameof(Permissao)}.{value}";
            }
        }

        public PermissionAttribute(Permissao permission)
        {
            Permission = permission;
        }
    }

    internal class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private DefaultAuthorizationPolicyProvider BackupPolicyProvider { get; }

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            BackupPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => BackupPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() => BackupPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith($"{nameof(Permissao)}.") &&
                Enum.TryParse(policyName.Split('.').Last(), out Permissao permission))
            {
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(permission));
                return Task.FromResult<AuthorizationPolicy?>(policy.Build());
            }

            return BackupPolicyProvider.GetPolicyAsync(policyName);
        }
    }

    internal class PermissionRequirement : IAuthorizationRequirement
    {
        public Permissao Permission { get; private set; }

        public PermissionRequirement(Permissao permission)
        {
            Permission = permission;
        }
    }

    internal class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (Enum.TryParse(context.User.FindFirst(x => x.Type == nameof(Usuario.Perfil))?.Value, out Perfil role)
                && Papel.Permissoes[role].Contains(requirement.Permission))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
