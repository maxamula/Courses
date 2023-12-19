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

        protected RadzenScheduler<Appointment> scheduler;

        protected RadzenDataList<Models.Database.Group> dataList;

        protected Student Self { get => (AuthenticationStateProvider as AuthStateProvider).Account.Student; }

        protected Appointment SelectedApointment { get; set; }

        protected IEnumerable<Appointment> StudentAppointments { get; set; }

        protected IQueryable<Models.Database.Group> Groups { get; set; }

        protected Course CourseOfTheDay { get; set; }

        protected List<string> studentGroups { get; set; } 

        protected async void Reload()
        {
            studentGroups = DatabaseContext.Studentsgroups
            .Where(sg => sg.StudentID == Self.ID)
            .Select(sg => sg.GroupName)
            .ToList();
            Groups = (await DatabaseService.GetGroups()).Where(x => !x.Course.Archived && !studentGroups.Contains(x.Name));
            StudentAppointments = DatabaseContext.Appointments.Include(i => i.Lesson).Include(i => i.Group)
            .Where(appointment => studentGroups.Contains(appointment.GroupName)).OrderBy(x => x.LessonStart).ToList();
            await dataList.Reload();
            await scheduler.Reload();
        }

        protected override async Task OnInitializedAsync()
        {
            studentGroups = DatabaseContext.Studentsgroups
            .Where(sg => sg.StudentID == Self.ID)
            .Select(sg => sg.GroupName)
            .ToList();
            
            Groups = (await DatabaseService.GetGroups()).Where(x => !x.Course.Archived && !studentGroups.Contains(x.Name));

            StudentAppointments = DatabaseContext.Appointments.Include(i => i.Lesson).Include(i => i.Group)
            .Where(appointment => studentGroups.Contains(appointment.GroupName)).OrderBy(x => x.LessonStart).ToList();

            Random random = new Random((int)DateTime.Now.Ticks);

            CourseOfTheDay = DatabaseContext.Courses.Where(x => !x.Archived)
                .Skip(random.Next(0, DatabaseContext.Courses.Where(x => !x.Archived).Count() - 1))
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

        private string GetDays(int num)
        {
            string days = "";
            var flags = (DaysOfWeek)num;
            foreach (DaysOfWeek day in Enum.GetValues(typeof(DaysOfWeek)))
            {
                if (flags.HasFlag(day) && day != 0)
                {
                    days += day + ", ";
                }
            }
            days = days.TrimEnd(',', ' ');
            return days;
        }

        protected async Task EnrollButtonClick(MouseEventArgs args, Courses.Models.Database.Group group)
        {
            try
            {
                await DatabaseService.CreateStudentsgroup(new Studentsgroup(){ StudentID = Self.ID, GroupName = group.Name });
                Reload();
            }
            catch(Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
            }
        }
    }
}