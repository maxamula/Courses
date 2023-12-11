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
    public partial class Departments
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

        protected IEnumerable<Courses.Models.Database.Department> departments;

        protected RadzenDataGrid<Courses.Models.Database.Department> grid0;

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            departments = await DatabaseService.GetDepartments(new Query { Filter = $@"i => i.DepartmentName.Contains(@0)", FilterParameters = new object[] { search } });
        }
        protected override async Task OnInitializedAsync()
        {
            departments = await DatabaseService.GetDepartments(new Query { Filter = $@"i => i.DepartmentName.Contains(@0)", FilterParameters = new object[] { search } });
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await grid0.InsertRow(new Courses.Models.Database.Department());
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, Courses.Models.Database.Department department)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await DatabaseService.DeleteDepartment(department.DepartmentName);

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
                    Detail = $"Unable to delete Department"
                });
            }
        }

        protected async Task GridRowUpdate(Courses.Models.Database.Department args)
        {
            await DatabaseService.UpdateDepartment(args.DepartmentName, args);
        }

        protected async Task GridRowCreate(Courses.Models.Database.Department args)
        {
            await DatabaseService.CreateDepartment(args);
            await grid0.Reload();
        }

        protected async Task EditButtonClick(MouseEventArgs args, Courses.Models.Database.Department data)
        {
            await grid0.EditRow(data);
        }

        protected async Task SaveButtonClick(MouseEventArgs args, Courses.Models.Database.Department data)
        {
            await grid0.UpdateRow(data);
        }

        protected async Task CancelButtonClick(MouseEventArgs args, Courses.Models.Database.Department data)
        {
            grid0.CancelEditRow(data);
            await DatabaseService.CancelDepartmentChanges(data);
        }
    }
}