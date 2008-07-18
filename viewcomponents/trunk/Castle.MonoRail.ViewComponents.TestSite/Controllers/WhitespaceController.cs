
namespace Castle.MonoRail.ViewComponents.TestSite.Controllers
{
    using System;
    using System.Collections.Generic;
    using Castle.MonoRail.ViewComponents;
    using Castle.MonoRail.Framework;
    using Castle.MonoRail.Framework.ViewComponents;
	using Castle.MonoRail.Framework.TransformFilters;

    [Layout("default")]
    public class WhitespaceController : SmartDispatcherController
    {
        [TransformFilter(typeof(WhitespaceTransformFilter))]
        public void Index()
        {
            ChartProperties chart;

            chart = new ChartProperties(SalesPerRegion);
            chart.GridUnit = 100;
            chart.BarWidthPixels = 60;
            chart.BarSpacingPixels = 15;
            chart.Title = "Sales Per Region";
            chart.YUnitLabel = "Sales";
            chart.XUnitLabel = "Region";
            chart.ShowDataLabels = true;
            chart.DataFormat = "$#,##0";
            chart.PlotHeightPixels = 300;
            PropertyBag["sales1"] = chart;

            chart = new ChartProperties(ViewsPerWeekday);
            chart.Title = "Views Per Day Of The Week";
            chart.PlotHeightPixels = 150;
            chart.YUnitLabel = "Views";
            chart.XUnitLabel = "Day";
            chart.CssClass = "chart";
            chart.ShowDataLabels = true;
            PropertyBag["views1"] = chart;

            chart = new ChartProperties(GetTimeScaleDataSource(30, DateTime.Today, 0, 10000));
            chart.BarWidthPixels = 40;
            chart.GridUnit = 1000;
            chart.YUnitLabel = "Amount";
            chart.XUnitLabel = "Time";
            chart.LabelFormat = "M/d/yy";
            chart.LabelInterval = 2;
            chart.DataFormat = "#,##0.00";
            PropertyBag["timescale1"] = chart;

            chart = new ChartProperties(GetTimeScaleDataSource(30, DateTime.Today, 0, 10000));
            chart.BarWidthPixels = 20;
            chart.GridUnit = 150;
            chart.LabelFormat = "M/d/yy";
            chart.ShowDataLabels = true;
            chart.DataFormat = "#,##0";
            chart.ShowGridlines = false;
            PropertyBag["timescale2"] = chart;

            chart = new ChartProperties(new List<ChartDataItem>());
            chart.Title = "Empty Chart";
            PropertyBag["empty"] = chart;

            chart = new ChartProperties(ViewsPerWeekday);
            chart.XUnitLabel = null;
            chart.YUnitLabel = null;
            chart.BarWidthPixels = 70;
            PropertyBag["viewsNoAxisUnitLabelsNorTitle"] = chart;

            chart = new ChartProperties(SalesPerRegion);
            chart.GridUnit = 100;
            chart.PlotHeightPixels = 600;
            chart.BarWidthPixels = 250;
            chart.BarSpacingPixels = 20;
            chart.Title = "Sales Per Region";
            chart.YUnitLabel = "Sales";
            chart.XUnitLabel = "Region";
            chart.DataFormat = "$#,##0";
            PropertyBag["salesBig"] = chart;
        }

        private static IList<ChartDataItem> SalesPerRegion
        {
            get
            {
                IList<ChartDataItem> result = new List<ChartDataItem>();
                result.Add(new ChartDataItem("East", 900m));
                result.Add(new ChartDataItem("Central", 680m));
                result.Add(new ChartDataItem("South", 100m));
                result.Add(new ChartDataItem("West", 154m));
                return result;
            }
        }

        private static IList<ChartDataItem> ViewsPerWeekday
        {
            get
            {
                IList<ChartDataItem> result = new List<ChartDataItem>();
                result.Add(new ChartDataItem("S", 3m));
                result.Add(new ChartDataItem("M", 3m));
                result.Add(new ChartDataItem("T", 0m));
                result.Add(new ChartDataItem("W", 12m));
                result.Add(new ChartDataItem("T", 18m));
                result.Add(new ChartDataItem("F", 6m));
                result.Add(new ChartDataItem("S", 24m));
                return result;
            }
        }

        private static IDictionary<object, decimal> GetTimeScaleDataSource(int count, DateTime start, int min, int max)
        {
            IDictionary<object, decimal> result = new Dictionary<object, decimal>();
            Random rnd = new Random(5);
            for (int i = 0; i < count; i++)
            {
                DateTime date = start.AddDays(i);
                result.Add(date, rnd.Next(min, max));
            }
            return result;
        }
    }
}
