using System.Security.Claims;
using AddressBook2025.Client.Models;
using AddressBook2025.Data;
using AddressBook2025.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AddressBook2025.Components.Account
{
    public class CustomUserClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> options) : UserClaimsPrincipalFactory<ApplicationUser>(userManager, options)
    {
        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);

            string profilePictureUrl = user.ProfilePictureId.HasValue ?
                                        $"/api/uploads/{user.ProfilePictureId}"
                                        : ImageHelper.DefaultProfilePictureUrl; // Default placeholder image URL

            List<Claim> customClaims =
            // Create and add custom claims individually
           [
            new Claim(nameof(UserInfo.ProfilePictureUrl), profilePictureUrl),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName)
            ];
            identity.AddClaims(customClaims);
            return identity;
        }
    }

}
