using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Courses.Models.Database;
using DocumentFormat.OpenXml.Office2010.Excel;
using Courses.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using DocumentFormat.OpenXml.InkML;

namespace Courses.Pages
{
    public partial class Dashboard
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected DatabaseService DatabaseService { get; set; }

        [Inject]
        protected DatabaseContext DatabaseContext { get; set; }

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        protected Student Self { get => (AuthenticationStateProvider as AuthStateProvider).Account.Student; }

        protected Appointment SelectedApointment { get; set; }

        protected IEnumerable<Appointment> StudentAppointments { get; set; }

        protected IEnumerable<Models.Database.Group> StudentGroups { get; set; }

        protected Course CourseOfTheDay { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var studentGroups = DatabaseContext.Studentsgroups
            .Where(sg => sg.StudentID == Self.ID)
            .Select(sg => sg.GroupName)
            .ToList();

            StudentAppointments = DatabaseContext.Appointments.Include(i => i.Lesson).Include(i => i.Group)
            .Where(appointment => studentGroups.Contains(appointment.GroupName)).OrderBy(x => x.LessonStart).ToList();

            Random random = new Random((int)DateTime.Now.Ticks);

            CourseOfTheDay = DatabaseContext.Courses
                .Skip(random.Next(0, DatabaseContext.Courses.Count() - 1))
                .FirstOrDefault();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                await JSRuntime.InvokeAsync<object>("eval", "StudentDashboardMain();");
            }
        }

        async Task OnAppointmentSelect(SchedulerAppointmentSelectEventArgs<Appointment> args) => SelectedApointment = args.Data;
    }
}