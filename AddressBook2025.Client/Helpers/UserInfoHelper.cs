using System.Security.Claims;
using AddressBook2025.Client.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace AddressBook2025.Client.Helpers
{
    public static class UserInfoHelper
    {
        //called server-side
        public static UserInfo GetUserInfo(AuthenticationState authState)
        {
            var userId = authState.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = authState.User.FindFirst(ClaimTypes.Email)?.Value;
            var firstName = authState.User.FindFirst("FirstName")?.Value;
            var lastName = authState.User.FindFirst("LastName")?.Value;
            var profilePictureUrl = authState.User.FindFirst("ProfilePictureUrl")?.Value;

            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(email) || 
                string.IsNullOrEmpty(firstName) || 
                string.IsNullOrEmpty(lastName) || 
                string.IsNullOrEmpty(profilePictureUrl) )
                return null!;

            UserInfo userInfo = new()
            {
                UserId = userId,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                ProfilePictureUrl = profilePictureUrl
            };
            return userInfo;

        }
        //called client-side
        public static async Task<UserInfo?> GetUserInfoAsync(Task<AuthenticationState> authStateTask)
        {
            if (authStateTask is null) 
            {
                return null;
            }
            else
            {
                AuthenticationState authState = await authStateTask;
                UserInfo userInfo = GetUserInfo(authState);
                return userInfo;
            }
         
        }
    }
}
