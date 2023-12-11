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

namespace Courses.Pages
{
    public partial class EditCourse
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

        [Parameter]
        public int ID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            course = await DatabaseService.GetCourseById(ID);
            

            departmentsForDepartmentName = await DatabaseService.GetDepartments();

            teachersForTeacherID = await DatabaseService.GetTeachers();

            List<int> loadedDays = new List<int>();
            var flags = (DaysOfWeek)course.DaysOfWeek;
            foreach (DaysOfWeek day in Enum.GetValues(typeof(DaysOfWeek)))
            {
                if (flags.HasFlag(day) && day != 0)
                {
                    loadedDays.Add((int)day);
                }
            }
            editDays = loadedDays.ToArray();
        }
        protected bool errorVisible;
        protected Courses.Models.Database.Course course;

        protected IEnumerable<Courses.Models.Database.Department> departmentsForDepartmentName;

        protected IEnumerable<Courses.Models.Database.Teacher> teachersForTeacherID;

        protected IEnumerable<int> editDays = new int[]{1, 32};

        protected async Task FormSubmit()
        {
            try
            {
                course.DaysOfWeek = 0;
                foreach(int flag in editDays)
                    course.DaysOfWeek |= (byte)flag;
                await DatabaseService.UpdateCourse(ID, course);
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