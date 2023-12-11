using System.Net.Http;
using Courses.Data;
using Courses.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace Courses.Pages
{
    public partial class About
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
        protected DatabaseService DatabaseService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Parameter]
        public string CourseId { get; set; }

        protected Course Course { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                Course = await DatabaseService.GetCourseById(int.Parse(CourseId));
            }
            catch (Exception ex)
            {
                JSRuntime.InvokeVoidAsync("alert", ex.Message);
            }
        }
    }
}