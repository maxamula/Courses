using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Courses.Pages
{
    public partial class AddCourse
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

        protected override async Task OnInitializedAsync()
        {
            course = new Courses.Models.Database.Course();

            departmentsForDepartmentName = await DatabaseService.GetDepartments();

            teachersForTeacherID = await DatabaseService.GetTeachers();
        }
        protected IEnumerable<int> editDays = new int[] { 2, 8 };
        protected bool errorVisible;
        protected Courses.Models.Database.Course course;

        protected IEnumerable<Courses.Models.Database.Department> departmentsForDepartmentName;

        protected IEnumerable<Courses.Models.Database.Teacher> teachersForTeacherID;

        protected async Task FormSubmit()
        {
            try
            {
                course.Archived = false;
                course.DaysOfWeek = 0;
                foreach (int flag in editDays)
                    course.DaysOfWeek |= (byte)flag;
                await DatabaseService.CreateCourse(course);
                DialogService.Close(course);
            }
            catch (Exception ex)
            {
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }
    }
}