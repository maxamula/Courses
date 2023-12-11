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
    public partial class Teachers
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

        protected IEnumerable<Courses.Models.Database.Teacher> teachers;

        protected RadzenDataGrid<Courses.Models.Database.Teacher> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            teachers = await DatabaseService.GetTeachers(new Query { Filter = $@"i => (i.FullName.Contains(@0) || i.Email.Contains(@0) || i.Phone.Contains(@0)) && !i.Archived", FilterParameters = new object[] { search } });
        }
        
        protected override async Task OnInitializedAsync()
        {
            teachers = await DatabaseService.GetTeachers(new Query { Filter = $@"i => !i.Archived", FilterParameters = new object[] { search } });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await grid0.InsertRow(new Courses.Models.Database.Teacher());
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Courses.Models.Database.Teacher teacher)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await DatabaseService.DeleteTeacher(teacher.ID);

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
                    Detail = $"Unable to delete Teacher"
                });
            }
        }

        protected async Task GridRowUpdate(Courses.Models.Database.Teacher args)
        {
            await DatabaseService.UpdateTeacher(args.ID, args);
        }

        protected async Task GridRowCreate(Courses.Models.Database.Teacher args)
        {
            await DatabaseService.CreateTeacher(args);
            await grid0.Reload();
        }

        protected async Task EditButtonClick(MouseEventArgs args, Courses.Models.Database.Teacher data)
        {
            await grid0.EditRow(data);
        }

        protected async Task GridArchiveButtonClick(MouseEventArgs args, Courses.Models.Database.Teacher data)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to ARCHIVE this record?") == true)
                {
                    data.Archived = true;
                    var result = await DatabaseService.UpdateTeacher(data.ID, data);
                    if(result != null)
                        await grid0.Reload();
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", ex.Message);
            }
        }

        protected async Task SaveButtonClick(MouseEventArgs args, Courses.Models.Database.Teacher data)
        {
            await grid0.UpdateRow(data);
        }

        protected async Task CancelButtonClick(MouseEventArgs args, Courses.Models.Database.Teacher data)
        {
            grid0.CancelEditRow(data);
            await DatabaseService.CancelTeacherChanges(data);
        }
    }
}