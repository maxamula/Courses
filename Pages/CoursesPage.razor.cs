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
    public partial class CoursesPage
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

        protected IEnumerable<Courses.Models.Database.Course> courses;

        protected RadzenDataGrid<Courses.Models.Database.Course> grid0;

        protected override async Task OnInitializedAsync()
        {
            var all = await DatabaseService.GetCourses(new Query { Expand = "Department, Teacher" });
            courses = all.Where(x => !x.Archived);
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddCourse>("Add Course", null);
            await grid0.Reload();
        }

        protected async Task EditRow(Courses.Models.Database.Course args)
        {
            await DialogService.OpenAsync<EditCourse>("Edit Course", new Dictionary<string, object> { {"ID", args.ID} });
            await grid0.Reload();
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Courses.Models.Database.Course course)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to DELETE this record?") == true)
                {
                    var deleteResult = await DatabaseService.DeleteCourse(course.ID);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Course"
                });
            }
        }

        protected async Task GridArchiveButtonClick(MouseEventArgs args, Courses.Models.Database.Course course)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to ARCHIVE this record?") == true)
                {
                    course.Archived = true;
                    var result = await DatabaseService.UpdateCourse(course.ID, course);
                    if(result != null)
                        await grid0.Reload();
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
            }
        }

        protected Courses.Models.Database.Course course;
        protected async Task GetChildData(Courses.Models.Database.Course args)
        {
            course = args;
            var LessonsResult = await DatabaseService.GetLessons(new Query { Filter = $@"i => i.CourseID == {args.ID}", Expand = "Course" });
            if (LessonsResult != null)
            {
                args.Lessons = LessonsResult.OrderBy(x => x.Sequence).ToList();
            }
        }

        protected RadzenDataGrid<Courses.Models.Database.Lesson> LessonsDataGrid;

        protected async Task LessonsAddButtonClick(MouseEventArgs args, Courses.Models.Database.Course data)
        {
            var dialogResult = await DialogService.OpenAsync<AddLesson>("Add Lessons", new Dictionary<string, object> { {"CourseID" , data.ID} });
            await GetChildData(data);
            await LessonsDataGrid.Reload();
        }

        protected async Task LessonsRowSelect(Courses.Models.Database.Lesson args, Courses.Models.Database.Course data)
        {
            var dialogResult = await DialogService.OpenAsync<EditLesson>("Edit Lessons", new Dictionary<string, object> { {"ID", args.ID} });
            await GetChildData(data);
            await LessonsDataGrid.Reload();
        }

        protected async Task LessonsDeleteButtonClick(MouseEventArgs args, Courses.Models.Database.Lesson lesson)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await DatabaseService.DeleteLesson(lesson.ID);

                    await GetChildData(course);

                    if (deleteResult != null)
                    {
                        await LessonsDataGrid.Reload();
                    }
                }
            }
            catch (System.Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Lesson"
                });
            }
        }
    }
}