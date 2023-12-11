using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Courses.Data;
using Courses.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace Courses.Pages
{
    public partial class EditLesson
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

        [Inject]
        public DatabaseContext Context { get; set; }

        [Parameter]
        public int ID { get; set; }

        protected override async Task OnInitializedAsync()
        {
            lesson = await DatabaseService.GetLessonById(ID);

            coursesForCourseID = await DatabaseService.GetCourses();
        }
        protected bool errorVisible;
        protected Courses.Models.Database.Lesson lesson;

        protected IEnumerable<Courses.Models.Database.Course> coursesForCourseID;

        protected async Task FormSubmit()
        {
            using (var transaction = Context.Database.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted))
            {
                try
                {
                    Context.Database.ExecuteSqlRaw("SELECT * FROM lessons FOR UPDATE;");
                    transaction.CreateSavepoint("lesson_savepoint");

                    int maxSeq = 0;
                    foreach (var lesson in Context.Lessons.Where(x => x.CourseID == lesson.CourseID))
                    {
                        maxSeq = Math.Max(lesson.Sequence, maxSeq);
                    }


                    if (lesson.Sequence > maxSeq || lesson.Sequence < 0)
                    {
                        transaction.RollbackToSavepoint("lesson_savepoint");
                        DialogService.Close(lesson);
                        return;
                    }

                    var target = Context.Lessons.Find(lesson.ID);
                    int oldSeq = target.Sequence;
                    target.Sequence = lesson.Sequence;

                    List<Lesson> lessonsToShift;
                    if (target.Sequence < oldSeq)
                    {
                        lessonsToShift = Context.Lessons
                        .Where(l => l.CourseID == lesson.CourseID && l.Sequence >= lesson.Sequence && l.Sequence < oldSeq)
                        .OrderBy(l => l.Sequence)
                        .ToList();
                    }
                    else
                    {
                        lessonsToShift = Context.Lessons
                        .Where(l => l.CourseID == lesson.CourseID && l.Sequence <= lesson.Sequence && l.Sequence > oldSeq)
                        .OrderBy(l => l.Sequence)
                        .ToList();
                    }

                    int shift = target.Sequence - oldSeq < 0 ? 1 : -1;

                    foreach (var lesson in lessonsToShift)
                    {
                        lesson.Sequence += shift;
                    }

                    Context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw;
                }
            }
            DialogService.Close(lesson);
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