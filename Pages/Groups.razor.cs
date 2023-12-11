using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using DocumentFormat.OpenXml.InkML;
using MySqlConnector;
using Courses.Data;
using Microsoft.EntityFrameworkCore;
using Courses.Models.Database;

namespace Courses.Pages
{
    public partial class Groups
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
        public DatabaseContext DatabaseContext { get; set; }

        RadzenScheduler<Appointment> scheduler;

        protected IEnumerable<Courses.Models.Database.Group> groups;

        protected IEnumerable<Courses.Models.Database.Appointment> appointments;

        protected RadzenDataGrid<Courses.Models.Database.Group> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            groups = await DatabaseService.GetGroups(new Query { Filter = $@"i => i.Name.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Course" });
        }

        protected IEnumerable<Courses.Models.Database.Course> coursesForCourseID;
        protected override async Task OnInitializedAsync()
        {
            groups = await DatabaseService.GetGroups(new Query { Filter = $@"i => i.Name.Contains(@0)", FilterParameters = new object[] { search }, Expand = "Course" });
            appointments = await DatabaseService.GetAppointments();
            coursesForCourseID = (await DatabaseService.GetCourses()).Where(x => !x.Archived);
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await grid0.InsertRow(new Courses.Models.Database.Group());
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Courses.Models.Database.Group _group)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await DatabaseService.DeleteGroup(_group.Name);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                        appointments = await DatabaseService.GetAppointments();
                        await scheduler.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Group"
                });
            }
        }

        protected async Task GridRowUpdate(Courses.Models.Database.Group args)
        {
            await DatabaseService.UpdateGroup(args.Name, args);
        }

        protected async Task GridRowCreate(Courses.Models.Database.Group args)
        {
            await DatabaseService.CreateGroup(args);
            await grid0.Reload();
        }

        protected async Task EditButtonClick(MouseEventArgs args, Courses.Models.Database.Group data)
        {
            await grid0.EditRow(data);
        }

        protected async Task SaveButtonClick(MouseEventArgs args, Courses.Models.Database.Group data)
        {
            await grid0.UpdateRow(data);
            appointments = await DatabaseService.GetAppointments();
            await scheduler.Reload();
        }

        protected async Task CancelButtonClick(MouseEventArgs args, Courses.Models.Database.Group data)
        {
            grid0.CancelEditRow(data);
            await DatabaseService.CancelGroupChanges(data);
        }

        protected async Task ArrangeButtonClick(MouseEventArgs args, Courses.Models.Database.Group data)
        {
            try
            {
                string date = await JSRuntime.InvokeAsync<string>("prompt", "Enter starting date:");
                string time = await JSRuntime.InvokeAsync<string>("prompt", "Enter time:");
                DatabaseContext.Database.ExecuteSqlRaw("CALL ArrangeAppointments({0}, {1}, {2})", data.Name, date, time);
                appointments = await DatabaseService.GetAppointments();
                await scheduler.Reload();
            }
            catch (Exception ex) 
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
            }
        }

        async Task OnAppointmentSelect(SchedulerAppointmentSelectEventArgs<Appointment> args)
        {        
            Appointment data = await DialogService.OpenAsync<EditAppointment>("Edit Appointment", new Dictionary<string, object> { { "Appointment", args.Data } });
            await DatabaseContext.SaveChangesAsync();
            DatabaseContext.Entry(data).Reload();
            await scheduler.Reload();
        }
    }
}