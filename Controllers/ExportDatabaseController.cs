using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using Courses.Data;

namespace Courses.Controllers
{
    public partial class ExportDatabaseController : ExportController
    {
        private readonly DatabaseContext context;
        private readonly DatabaseService service;

        public ExportDatabaseController(DatabaseContext context, DatabaseService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/Database/accounts/csv")]
        [HttpGet("/export/Database/accounts/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAccountsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAccounts(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/accounts/excel")]
        [HttpGet("/export/Database/accounts/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAccountsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAccounts(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/appointments/csv")]
        [HttpGet("/export/Database/appointments/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAppointmentsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetAppointments(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/appointments/excel")]
        [HttpGet("/export/Database/appointments/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportAppointmentsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetAppointments(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/courses/csv")]
        [HttpGet("/export/Database/courses/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCoursesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetCourses(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/courses/excel")]
        [HttpGet("/export/Database/courses/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportCoursesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetCourses(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/departments/csv")]
        [HttpGet("/export/Database/departments/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDepartmentsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetDepartments(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/departments/excel")]
        [HttpGet("/export/Database/departments/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportDepartmentsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetDepartments(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/groups/csv")]
        [HttpGet("/export/Database/groups/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGroupsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetGroups(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/groups/excel")]
        [HttpGet("/export/Database/groups/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportGroupsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetGroups(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/lessons/csv")]
        [HttpGet("/export/Database/lessons/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportLessonsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetLessons(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/lessons/excel")]
        [HttpGet("/export/Database/lessons/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportLessonsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetLessons(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/students/csv")]
        [HttpGet("/export/Database/students/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportStudentsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetStudents(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/students/excel")]
        [HttpGet("/export/Database/students/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportStudentsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetStudents(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/studentsgroups/csv")]
        [HttpGet("/export/Database/studentsgroups/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportStudentsgroupsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetStudentsgroups(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/studentsgroups/excel")]
        [HttpGet("/export/Database/studentsgroups/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportStudentsgroupsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetStudentsgroups(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/teachers/csv")]
        [HttpGet("/export/Database/teachers/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTeachersToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTeachers(), Request.Query), fileName);
        }

        [HttpGet("/export/Database/teachers/excel")]
        [HttpGet("/export/Database/teachers/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTeachersToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTeachers(), Request.Query), fileName);
        }
    }
}
