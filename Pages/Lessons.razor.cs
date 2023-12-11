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
    public partial class Lessons
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

        protected IEnumerable<Courses.Models.Database.Lesson> lessons;

        protected RadzenDataGrid<Courses.Models.Database.Lesson> grid0;
        protected override async Task OnInitializedAsync()
        {
            lessons = await DatabaseService.GetLessons(new Query { Expand = "Course" });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await DialogService.OpenAsync<AddLesson>("Add Lesson", null);
            await grid0.Reload();
        }

        protected async Task EditRow(Courses.Models.Database.Lesson args)
        {
            await DialogService.OpenAsync<EditLesson>("Edit Lesson", new Dictionary<string, object> { {"ID", args.ID} });
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Courses.Models.Database.Lesson lesson)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await DatabaseService.DeleteLesson(lesson.ID);

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
                    Detail = $"Unable to delete Lesson"
                });
            }
        }

        protected Courses.Models.Database.Lesson lesson;
        protected async Task GetChildData(Courses.Models.Database.Lesson args)
        {
            lesson = args;
        }
    }
}