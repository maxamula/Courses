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
    public partial class AddLesson
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

            coursesForCourseID = await DatabaseService.GetCourses();
        }
        protected bool errorVisible;
        protected Courses.Models.Database.Lesson lesson;

        protected IEnumerable<Courses.Models.Database.Course> coursesForCourseID;

        protected async Task FormSubmit()
        {
            try
            {
                await DatabaseService.CreateLesson(lesson);
                DialogService.Close(lesson);
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





        bool hasCourseIDValue;

        [Parameter]
        public int CourseID { get; set; }
        public override async Task SetParametersAsync(ParameterView parameters)
        {
            lesson = new Courses.Models.Database.Lesson();

            hasCourseIDValue = parameters.TryGetValue<int>("CourseID", out var hasCourseIDResult);

            if (hasCourseIDValue)
            {
                lesson.CourseID = hasCourseIDResult;
            }
            await base.SetParametersAsync(parameters);
        }
    }
}