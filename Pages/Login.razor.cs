using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Courses.Pages
{
    public partial class Login
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected ProtectedSessionStorage ProtectedSessionStorage { get; set; }

        [Inject]
        protected AuthenticationStateProvider Authentication { get; set; }

        protected async System.Threading.Tasks.Task Submit(Radzen.LoginArgs args)
        {
            try
            {
                await (Authentication as AuthStateProvider).MakeAuth(args.Username, args.Password);
                NavigationManager.NavigateTo("/", false);
            }
            catch (Exception ex)
            {
                JSRuntime.InvokeVoidAsync("alert", ex.Message);
            }
        }

        protected async System.Threading.Tasks.Task Register() => NavigationManager.NavigateTo("/register");
    }
}