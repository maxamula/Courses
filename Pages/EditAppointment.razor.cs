using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Courses.Data;

namespace Courses.Pages
{
    public partial class EditAppointment
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
        public DatabaseService DatabaseService { get; set; }

        [Inject]
        public DatabaseContext Context { get; set; }

        [Parameter]
        public int ID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            appointment = await DatabaseService.GetAppointmentById(ID);
        }
        protected bool errorVisible;
        protected Courses.Models.Database.Appointment appointment;

        protected async Task FormSubmit()
        {
            await DatabaseService.UpdateAppointment(ID, appointment);
            DialogService.Close(appointment);
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}