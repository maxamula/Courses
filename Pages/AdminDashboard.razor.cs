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
using DocumentFormat.OpenXml.Office2010.Excel;
using Courses.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Courses.Pages
{

    class ChartItem
    {
        public string Type { get; set; }
        public double Count { get; set; }
    }

    public partial class AdminDashboard
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected DatabaseService DatabaseService { get; set; }

        [Inject]
        protected DatabaseContext DatabaseContext { get; set; }

        [Inject]
        protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        ChartItem[] typeDistr;

        protected override async Task OnInitializedAsync()
        {
            typeDistr = new ChartItem[]
            {
                new ChartItem{ Type="Lecture", Count=DatabaseContext.Courses.Where(x => x.CourseType == "Lecture").Count()},
                new ChartItem{ Type="Practice", Count=DatabaseContext.Courses.Where(x => x.CourseType == "Practice").Count()},
                new ChartItem{ Type="Combined", Count=DatabaseContext.Courses.Where(x => x.CourseType == "Combined").Count()},
            };
        }
    }
}