﻿using AddressBook2025.Client.Helpers;
using AddressBook2025.Client.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace AddressBook2025.Client.Components.BaseComponents
{
    public abstract class AuthenticatedComponentBase : ComponentBase
    {
        [CascadingParameter]
        public Task<AuthenticationState> AuthStateTask { get; set; } = default!;

        protected UserInfo? UserInfo { get;  set; }

        protected override async Task OnInitializedAsync()
        {
            UserInfo = await UserInfoHelper.GetUserInfoAsync(AuthStateTask);
            await OnInitializedWithAuthAsync();
        }

        /// <summary>
        /// Override this method instead of OnInitializedAsync when inheriting.
        /// </summary>
        protected virtual Task OnInitializedWithAuthAsync() => Task.CompletedTask;
        
            
    }
}
