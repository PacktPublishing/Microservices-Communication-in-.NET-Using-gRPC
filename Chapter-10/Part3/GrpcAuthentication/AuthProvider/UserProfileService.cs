using AuthProvider.Models;
using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthProvider
{
    public class UserProfileService : IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory;
        private readonly UserManager<ApplicationUser> usersManager;
        private readonly RoleManager<IdentityRole> rolesManager;

        public UserProfileService(
            UserManager<ApplicationUser> usersManager,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            RoleManager<IdentityRole> rolesManager)
        {
            this.usersManager = usersManager;
            this.claimsFactory = claimsFactory;
            this.rolesManager = rolesManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subject = context.Subject.GetSubjectId();
            var user = await usersManager.FindByIdAsync(subject);
            var claimsPrincipal = await claimsFactory.CreateAsync(user);

            var claimsList = claimsPrincipal.Claims.ToList();
            claimsList = claimsList.Where(c => context.RequestedClaimTypes.Contains(c.Type)).ToList();

            if (usersManager.SupportsUserRole)
            {
                foreach (var role in rolesManager.Roles)
                {
                    claimsList.Add(new Claim(JwtClaimTypes.Role, role.Name));

                    if (rolesManager.SupportsRoleClaims)
                    {
                        claimsList.AddRange(await rolesManager.GetClaimsAsync(role));
                    }
                }

            }
            context.IssuedClaims = claimsList;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subject = context.Subject.GetSubjectId();
            var user = await usersManager.FindByIdAsync(subject);
            context.IsActive = user != null;
        }
    }
}
