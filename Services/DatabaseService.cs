using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Radzen;

using Courses.Data;

namespace Courses
{
    public partial class DatabaseService
    {
        DatabaseContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly DatabaseContext context;
        private readonly NavigationManager navigationManager;

        public DatabaseService(DatabaseContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportAccountsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/accounts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/accounts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAccountsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/accounts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/accounts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAccountsRead(ref IQueryable<Courses.Models.Database.Account> items);

        public async Task<IQueryable<Courses.Models.Database.Account>> GetAccounts(Query query = null)
        {
            var items = Context.Accounts.AsQueryable();

            items = items.Include(i => i.Student);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAccountsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAccountGet(Courses.Models.Database.Account item);
        partial void OnGetAccountByName(ref IQueryable<Courses.Models.Database.Account> items);


        public async Task<Courses.Models.Database.Account> GetAccountByName(string name)
        {
            var items = Context.Accounts
                              .AsNoTracking()
                              .Where(i => i.Name == name);

            items = items.Include(i => i.Student);
 
            OnGetAccountByName(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAccountGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAccountCreated(Courses.Models.Database.Account item);
        partial void OnAfterAccountCreated(Courses.Models.Database.Account item);

        public async Task<Courses.Models.Database.Account> CreateAccount(Courses.Models.Database.Account account)
        {
            OnAccountCreated(account);

            var existingItem = Context.Accounts
                              .Where(i => i.Name == account.Name)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Accounts.Add(account);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(account).State = EntityState.Detached;
                throw;
            }

            OnAfterAccountCreated(account);

            return account;
        }

        public async Task<Courses.Models.Database.Account> CancelAccountChanges(Courses.Models.Database.Account item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAccountUpdated(Courses.Models.Database.Account item);
        partial void OnAfterAccountUpdated(Courses.Models.Database.Account item);

        public async Task<Courses.Models.Database.Account> UpdateAccount(string name, Courses.Models.Database.Account account)
        {
            OnAccountUpdated(account);

            var itemToUpdate = Context.Accounts
                              .Where(i => i.Name == account.Name)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(account);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAccountUpdated(account);

            return account;
        }

        partial void OnAccountDeleted(Courses.Models.Database.Account item);
        partial void OnAfterAccountDeleted(Courses.Models.Database.Account item);

        public async Task<Courses.Models.Database.Account> DeleteAccount(string name)
        {
            var itemToDelete = Context.Accounts
                              .Where(i => i.Name == name)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAccountDeleted(itemToDelete);


            Context.Accounts.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAccountDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportAppointmentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/appointments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/appointments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportAppointmentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/appointments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/appointments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnAppointmentsRead(ref IQueryable<Courses.Models.Database.Appointment> items);

        public async Task<IQueryable<Courses.Models.Database.Appointment>> GetAppointments(Query query = null)
        {
            var items = Context.Appointments.AsQueryable();

            items = items.Include(i => i.Group);
            items = items.Include(i => i.Lesson);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnAppointmentsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnAppointmentGet(Courses.Models.Database.Appointment item);
        partial void OnGetAppointmentById(ref IQueryable<Courses.Models.Database.Appointment> items);


        public async Task<Courses.Models.Database.Appointment> GetAppointmentById(int id)
        {
            var items = Context.Appointments
                              .AsNoTracking()
                              .Where(i => i.ID == id);

            items = items.Include(i => i.Group);
            items = items.Include(i => i.Lesson);
 
            OnGetAppointmentById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnAppointmentGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnAppointmentCreated(Courses.Models.Database.Appointment item);
        partial void OnAfterAppointmentCreated(Courses.Models.Database.Appointment item);

        public async Task<Courses.Models.Database.Appointment> CreateAppointment(Courses.Models.Database.Appointment appointment)
        {
            OnAppointmentCreated(appointment);

            var existingItem = Context.Appointments
                              .Where(i => i.ID == appointment.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Appointments.Add(appointment);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(appointment).State = EntityState.Detached;
                throw;
            }

            OnAfterAppointmentCreated(appointment);

            return appointment;
        }

        public async Task<Courses.Models.Database.Appointment> CancelAppointmentChanges(Courses.Models.Database.Appointment item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnAppointmentUpdated(Courses.Models.Database.Appointment item);
        partial void OnAfterAppointmentUpdated(Courses.Models.Database.Appointment item);

        public async Task<Courses.Models.Database.Appointment> UpdateAppointment(int id, Courses.Models.Database.Appointment appointment)
        {
            OnAppointmentUpdated(appointment);

            var itemToUpdate = Context.Appointments
                              .Where(i => i.ID == appointment.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(appointment);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterAppointmentUpdated(appointment);

            return appointment;
        }

        partial void OnAppointmentDeleted(Courses.Models.Database.Appointment item);
        partial void OnAfterAppointmentDeleted(Courses.Models.Database.Appointment item);

        public async Task<Courses.Models.Database.Appointment> DeleteAppointment(int id)
        {
            var itemToDelete = Context.Appointments
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnAppointmentDeleted(itemToDelete);


            Context.Appointments.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterAppointmentDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportCoursesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/courses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/courses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportCoursesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/courses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/courses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnCoursesRead(ref IQueryable<Courses.Models.Database.Course> items);

        public async Task<IQueryable<Courses.Models.Database.Course>> GetCourses(Query query = null)
        {
            var items = Context.Courses.AsQueryable();

            items = items.Include(i => i.Department);
            items = items.Include(i => i.Teacher);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnCoursesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnCourseGet(Courses.Models.Database.Course item);
        partial void OnGetCourseById(ref IQueryable<Courses.Models.Database.Course> items);


        public async Task<Courses.Models.Database.Course> GetCourseById(int id)
        {
            var items = Context.Courses
                              .AsNoTracking()
                              .Where(i => i.ID == id);

            items = items.Include(i => i.Department);
            items = items.Include(i => i.Teacher);
 
            OnGetCourseById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnCourseGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnCourseCreated(Courses.Models.Database.Course item);
        partial void OnAfterCourseCreated(Courses.Models.Database.Course item);

        public async Task<Courses.Models.Database.Course> CreateCourse(Courses.Models.Database.Course course)
        {
            OnCourseCreated(course);

            var existingItem = Context.Courses
                              .Where(i => i.ID == course.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Courses.Add(course);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(course).State = EntityState.Detached;
                throw;
            }

            OnAfterCourseCreated(course);

            return course;
        }

        public async Task<Courses.Models.Database.Course> CancelCourseChanges(Courses.Models.Database.Course item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnCourseUpdated(Courses.Models.Database.Course item);
        partial void OnAfterCourseUpdated(Courses.Models.Database.Course item);

        public async Task<Courses.Models.Database.Course> UpdateCourse(int id, Courses.Models.Database.Course course)
        {
            OnCourseUpdated(course);

            var itemToUpdate = Context.Courses
                              .Where(i => i.ID == course.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(course);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterCourseUpdated(course);

            return course;
        }

        partial void OnCourseDeleted(Courses.Models.Database.Course item);
        partial void OnAfterCourseDeleted(Courses.Models.Database.Course item);

        public async Task<Courses.Models.Database.Course> DeleteCourse(int id)
        {
            var itemToDelete = Context.Courses
                              .Where(i => i.ID == id)
                              .Include(i => i.Groups)
                              .Include(i => i.Lessons)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnCourseDeleted(itemToDelete);


            Context.Courses.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterCourseDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportDepartmentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/departments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/departments/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportDepartmentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/departments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/departments/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnDepartmentsRead(ref IQueryable<Courses.Models.Database.Department> items);

        public async Task<IQueryable<Courses.Models.Database.Department>> GetDepartments(Query query = null)
        {
            var items = Context.Departments.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnDepartmentsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnDepartmentGet(Courses.Models.Database.Department item);
        partial void OnGetDepartmentByDepartmentName(ref IQueryable<Courses.Models.Database.Department> items);


        public async Task<Courses.Models.Database.Department> GetDepartmentByDepartmentName(string departmentname)
        {
            var items = Context.Departments
                              .AsNoTracking()
                              .Where(i => i.DepartmentName == departmentname);

 
            OnGetDepartmentByDepartmentName(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnDepartmentGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnDepartmentCreated(Courses.Models.Database.Department item);
        partial void OnAfterDepartmentCreated(Courses.Models.Database.Department item);

        public async Task<Courses.Models.Database.Department> CreateDepartment(Courses.Models.Database.Department department)
        {
            OnDepartmentCreated(department);

            var existingItem = Context.Departments
                              .Where(i => i.DepartmentName == department.DepartmentName)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Departments.Add(department);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(department).State = EntityState.Detached;
                throw;
            }

            OnAfterDepartmentCreated(department);

            return department;
        }

        public async Task<Courses.Models.Database.Department> CancelDepartmentChanges(Courses.Models.Database.Department item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnDepartmentUpdated(Courses.Models.Database.Department item);
        partial void OnAfterDepartmentUpdated(Courses.Models.Database.Department item);

        public async Task<Courses.Models.Database.Department> UpdateDepartment(string departmentname, Courses.Models.Database.Department department)
        {
            OnDepartmentUpdated(department);

            var itemToUpdate = Context.Departments
                              .Where(i => i.DepartmentName == department.DepartmentName)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(department);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterDepartmentUpdated(department);

            return department;
        }

        partial void OnDepartmentDeleted(Courses.Models.Database.Department item);
        partial void OnAfterDepartmentDeleted(Courses.Models.Database.Department item);

        public async Task<Courses.Models.Database.Department> DeleteDepartment(string departmentname)
        {
            var itemToDelete = Context.Departments
                              .Where(i => i.DepartmentName == departmentname)
                              .Include(i => i.Courses)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnDepartmentDeleted(itemToDelete);


            Context.Departments.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterDepartmentDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportGroupsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/groups/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/groups/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportGroupsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/groups/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/groups/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnGroupsRead(ref IQueryable<Courses.Models.Database.Group> items);

        public async Task<IQueryable<Courses.Models.Database.Group>> GetGroups(Query query = null)
        {
            var items = Context.Groups.AsQueryable();

            items = items.Include(i => i.Course);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnGroupsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnGroupGet(Courses.Models.Database.Group item);
        partial void OnGetGroupByName(ref IQueryable<Courses.Models.Database.Group> items);


        public async Task<Courses.Models.Database.Group> GetGroupByName(string name)
        {
            var items = Context.Groups
                              .AsNoTracking()
                              .Where(i => i.Name == name);

            items = items.Include(i => i.Course);
 
            OnGetGroupByName(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnGroupGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnGroupCreated(Courses.Models.Database.Group item);
        partial void OnAfterGroupCreated(Courses.Models.Database.Group item);

        public async Task<Courses.Models.Database.Group> CreateGroup(Courses.Models.Database.Group _group)
        {
            OnGroupCreated(_group);

            var existingItem = Context.Groups
                              .Where(i => i.Name == _group.Name)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Groups.Add(_group);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(_group).State = EntityState.Detached;
                throw;
            }

            OnAfterGroupCreated(_group);

            return _group;
        }

        public async Task<Courses.Models.Database.Group> CancelGroupChanges(Courses.Models.Database.Group item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnGroupUpdated(Courses.Models.Database.Group item);
        partial void OnAfterGroupUpdated(Courses.Models.Database.Group item);

        public async Task<Courses.Models.Database.Group> UpdateGroup(string name, Courses.Models.Database.Group _group)
        {
            OnGroupUpdated(_group);

            var itemToUpdate = Context.Groups
                              .Where(i => i.Name == _group.Name)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(_group);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterGroupUpdated(_group);

            return _group;
        }

        partial void OnGroupDeleted(Courses.Models.Database.Group item);
        partial void OnAfterGroupDeleted(Courses.Models.Database.Group item);

        public async Task<Courses.Models.Database.Group> DeleteGroup(string name)
        {
            var itemToDelete = Context.Groups
                              .Where(i => i.Name == name)
                              .Include(i => i.Appointments)
                              .Include(i => i.Studentsgroups)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnGroupDeleted(itemToDelete);


            Context.Groups.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterGroupDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportLessonsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/lessons/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/lessons/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportLessonsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/lessons/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/lessons/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnLessonsRead(ref IQueryable<Courses.Models.Database.Lesson> items);

        public async Task<IQueryable<Courses.Models.Database.Lesson>> GetLessons(Query query = null)
        {
            var items = Context.Lessons.AsQueryable();

            items = items.Include(i => i.Course);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnLessonsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnLessonGet(Courses.Models.Database.Lesson item);
        partial void OnGetLessonById(ref IQueryable<Courses.Models.Database.Lesson> items);


        public async Task<Courses.Models.Database.Lesson> GetLessonById(int id)
        {
            var items = Context.Lessons
                              .AsNoTracking()
                              .Where(i => i.ID == id);

            items = items.Include(i => i.Course);
 
            OnGetLessonById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnLessonGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnLessonCreated(Courses.Models.Database.Lesson item);
        partial void OnAfterLessonCreated(Courses.Models.Database.Lesson item);

        public async Task<Courses.Models.Database.Lesson> CreateLesson(Courses.Models.Database.Lesson lesson)
        {
            OnLessonCreated(lesson);

            var existingItem = Context.Lessons
                              .Where(i => i.ID == lesson.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Lessons.Add(lesson);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(lesson).State = EntityState.Detached;
                throw;
            }

            OnAfterLessonCreated(lesson);

            return lesson;
        }

        public async Task<Courses.Models.Database.Lesson> CancelLessonChanges(Courses.Models.Database.Lesson item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnLessonUpdated(Courses.Models.Database.Lesson item);
        partial void OnAfterLessonUpdated(Courses.Models.Database.Lesson item);

        public async Task<Courses.Models.Database.Lesson> UpdateLesson(int id, Courses.Models.Database.Lesson lesson)
        {
            OnLessonUpdated(lesson);

            var itemToUpdate = Context.Lessons
                              .Where(i => i.ID == lesson.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(lesson);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterLessonUpdated(lesson);

            return lesson;
        }

        partial void OnLessonDeleted(Courses.Models.Database.Lesson item);
        partial void OnAfterLessonDeleted(Courses.Models.Database.Lesson item);

        public async Task<Courses.Models.Database.Lesson> DeleteLesson(int id)
        {
            var itemToDelete = Context.Lessons
                              .Where(i => i.ID == id)
                              .Include(i => i.Appointments)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnLessonDeleted(itemToDelete);


            Context.Lessons.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterLessonDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportStudentsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/students/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/students/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportStudentsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/students/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/students/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnStudentsRead(ref IQueryable<Courses.Models.Database.Student> items);

        public async Task<IQueryable<Courses.Models.Database.Student>> GetStudents(Query query = null)
        {
            var items = Context.Students.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnStudentsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnStudentGet(Courses.Models.Database.Student item);
        partial void OnGetStudentById(ref IQueryable<Courses.Models.Database.Student> items);


        public async Task<Courses.Models.Database.Student> GetStudentById(int id)
        {
            var items = Context.Students
                              .AsNoTracking()
                              .Where(i => i.ID == id);

 
            OnGetStudentById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnStudentGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnStudentCreated(Courses.Models.Database.Student item);
        partial void OnAfterStudentCreated(Courses.Models.Database.Student item);

        public async Task<Courses.Models.Database.Student> CreateStudent(Courses.Models.Database.Student student)
        {
            OnStudentCreated(student);

            var existingItem = Context.Students
                              .Where(i => i.ID == student.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Students.Add(student);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(student).State = EntityState.Detached;
                throw;
            }

            OnAfterStudentCreated(student);

            return student;
        }

        public async Task<Courses.Models.Database.Student> CancelStudentChanges(Courses.Models.Database.Student item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnStudentUpdated(Courses.Models.Database.Student item);
        partial void OnAfterStudentUpdated(Courses.Models.Database.Student item);

        public async Task<Courses.Models.Database.Student> UpdateStudent(int id, Courses.Models.Database.Student student)
        {
            OnStudentUpdated(student);

            var itemToUpdate = Context.Students
                              .Where(i => i.ID == student.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(student);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterStudentUpdated(student);

            return student;
        }

        partial void OnStudentDeleted(Courses.Models.Database.Student item);
        partial void OnAfterStudentDeleted(Courses.Models.Database.Student item);

        public async Task<Courses.Models.Database.Student> DeleteStudent(int id)
        {
            var itemToDelete = Context.Students
                              .Where(i => i.ID == id)
                              .Include(i => i.Accounts)
                              .Include(i => i.Studentsgroups)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnStudentDeleted(itemToDelete);


            Context.Students.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterStudentDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportStudentsgroupsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/studentsgroups/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/studentsgroups/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportStudentsgroupsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/studentsgroups/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/studentsgroups/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnStudentsgroupsRead(ref IQueryable<Courses.Models.Database.Studentsgroup> items);

        public async Task<IQueryable<Courses.Models.Database.Studentsgroup>> GetStudentsgroups(Query query = null)
        {
            var items = Context.Studentsgroups.AsQueryable();

            items = items.Include(i => i.Group);
            items = items.Include(i => i.Student);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnStudentsgroupsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnStudentsgroupGet(Courses.Models.Database.Studentsgroup item);
        partial void OnGetStudentsgroupById(ref IQueryable<Courses.Models.Database.Studentsgroup> items);


        public async Task<Courses.Models.Database.Studentsgroup> GetStudentsgroupById(int id)
        {
            var items = Context.Studentsgroups
                              .AsNoTracking()
                              .Where(i => i.ID == id);

            items = items.Include(i => i.Group);
            items = items.Include(i => i.Student);
 
            OnGetStudentsgroupById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnStudentsgroupGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnStudentsgroupCreated(Courses.Models.Database.Studentsgroup item);
        partial void OnAfterStudentsgroupCreated(Courses.Models.Database.Studentsgroup item);

        public async Task<Courses.Models.Database.Studentsgroup> CreateStudentsgroup(Courses.Models.Database.Studentsgroup studentsgroup)
        {
            OnStudentsgroupCreated(studentsgroup);

            var existingItem = Context.Studentsgroups
                              .Where(i => i.ID == studentsgroup.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Studentsgroups.Add(studentsgroup);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(studentsgroup).State = EntityState.Detached;
                throw;
            }

            OnAfterStudentsgroupCreated(studentsgroup);

            return studentsgroup;
        }

        public async Task<Courses.Models.Database.Studentsgroup> CancelStudentsgroupChanges(Courses.Models.Database.Studentsgroup item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnStudentsgroupUpdated(Courses.Models.Database.Studentsgroup item);
        partial void OnAfterStudentsgroupUpdated(Courses.Models.Database.Studentsgroup item);

        public async Task<Courses.Models.Database.Studentsgroup> UpdateStudentsgroup(int id, Courses.Models.Database.Studentsgroup studentsgroup)
        {
            OnStudentsgroupUpdated(studentsgroup);

            var itemToUpdate = Context.Studentsgroups
                              .Where(i => i.ID == studentsgroup.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(studentsgroup);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterStudentsgroupUpdated(studentsgroup);

            return studentsgroup;
        }

        partial void OnStudentsgroupDeleted(Courses.Models.Database.Studentsgroup item);
        partial void OnAfterStudentsgroupDeleted(Courses.Models.Database.Studentsgroup item);

        public async Task<Courses.Models.Database.Studentsgroup> DeleteStudentsgroup(int id)
        {
            var itemToDelete = Context.Studentsgroups
                              .Where(i => i.ID == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnStudentsgroupDeleted(itemToDelete);


            Context.Studentsgroups.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterStudentsgroupDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportTeachersToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/teachers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/teachers/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTeachersToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/database/teachers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/database/teachers/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTeachersRead(ref IQueryable<Courses.Models.Database.Teacher> items);

        public async Task<IQueryable<Courses.Models.Database.Teacher>> GetTeachers(Query query = null)
        {
            var items = Context.Teachers.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTeachersRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTeacherGet(Courses.Models.Database.Teacher item);
        partial void OnGetTeacherById(ref IQueryable<Courses.Models.Database.Teacher> items);


        public async Task<Courses.Models.Database.Teacher> GetTeacherById(int id)
        {
            var items = Context.Teachers
                              .AsNoTracking()
                              .Where(i => i.ID == id);

 
            OnGetTeacherById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTeacherGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTeacherCreated(Courses.Models.Database.Teacher item);
        partial void OnAfterTeacherCreated(Courses.Models.Database.Teacher item);

        public async Task<Courses.Models.Database.Teacher> CreateTeacher(Courses.Models.Database.Teacher teacher)
        {
            OnTeacherCreated(teacher);

            var existingItem = Context.Teachers
                              .Where(i => i.ID == teacher.ID)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Teachers.Add(teacher);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(teacher).State = EntityState.Detached;
                throw;
            }

            OnAfterTeacherCreated(teacher);

            return teacher;
        }

        public async Task<Courses.Models.Database.Teacher> CancelTeacherChanges(Courses.Models.Database.Teacher item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTeacherUpdated(Courses.Models.Database.Teacher item);
        partial void OnAfterTeacherUpdated(Courses.Models.Database.Teacher item);

        public async Task<Courses.Models.Database.Teacher> UpdateTeacher(int id, Courses.Models.Database.Teacher teacher)
        {
            OnTeacherUpdated(teacher);

            var itemToUpdate = Context.Teachers
                              .Where(i => i.ID == teacher.ID)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(teacher);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTeacherUpdated(teacher);

            return teacher;
        }

        partial void OnTeacherDeleted(Courses.Models.Database.Teacher item);
        partial void OnAfterTeacherDeleted(Courses.Models.Database.Teacher item);

        public async Task<Courses.Models.Database.Teacher> DeleteTeacher(int id)
        {
            var itemToDelete = Context.Teachers
                              .Where(i => i.ID == id)
                              .Include(i => i.Courses)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnTeacherDeleted(itemToDelete);


            Context.Teachers.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTeacherDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}