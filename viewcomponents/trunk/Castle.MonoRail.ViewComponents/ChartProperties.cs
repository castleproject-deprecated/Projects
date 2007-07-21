// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System.Collections.Generic;
using System.Collections;

namespace Castle.MonoRail.ViewComponents
{
    /// <summary>
    /// Contains the properties used be ColumnChart view component
    /// </summary>
    public class ChartProperties
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The data to display on the chart</param>
        public ChartProperties(IList<ChartDataItem> data)
        {
            _data = data;
            _count = data.Count;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">The data to display on the chart</param>
        public ChartProperties(IDictionary<object, decimal> data)
        {
            _data = new List<ChartDataItem>();
            foreach (KeyValuePair<object, decimal> entry in data)
            {
                _data.Add(new ChartDataItem(entry.Key, entry.Value));
            }
            _count = data.Count;
        }

        private IList<ChartDataItem> _data;
        private string _title;
        private decimal? _plotHeightPixels;
        private decimal? _barWidthPixels;
        private string _labelFormat;
        private string _dataFormat;
        private string _emptyMessage;
        private bool? _showDataLabels;
        private int? _gridUnit;
        private string _xUnitLabel;
        private string _yUnitLabel;
        private decimal? _barSpacingPixels;
        private int _count;
        private string _cssClass;
        private bool? _showGridlines;
        private int? _labelInterval;

        /// <summary>
        /// The data to display on the chart (read only)
        /// </summary>
        public IList<ChartDataItem> Data
        {
            get { return _data; }
        }

        /// <summary>
        /// The title of the chart
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        /// <summary>
        /// The format used to display the 'X' axis labels.  Uses 
        /// standard .NET formatting notation.  Default is no formatting
        /// </summary>
        public string LabelFormat
        {
            get { return _labelFormat; }
            set { _labelFormat = value; }
        }

        /// <summary>
        /// The text of the message to show if the chart contains no data.
        /// Defaults is 'No chart data to display'
        /// </summary>
        public string EmptyMessage
        {
            get { return _emptyMessage ?? "No chart data to display"; }
            set { _emptyMessage = value; }
        }

        /// <summary>
        /// The width of each bar in pixels.  Default is 30
        /// </summary>
        public decimal BarWidthPixels
        {
            get { return _barWidthPixels ?? 30m; }
            set { _barWidthPixels = value; }
        }

        /// <summary>
        /// Indicates whether to show data labels.  Default is FALSE
        /// </summary>
        public bool ShowDataLabels
        {
            get { return _showDataLabels ?? false; }
            set { _showDataLabels = value; }
        }

        /// <summary>
        /// Format applied to data labels and 'Y' axis labels.  Uses 
        /// standard .NET formatting notation
        /// </summary>
        public string DataFormat
        {
            get { return _dataFormat; }
            set { _dataFormat = value; }
        }

        /// <summary>
        /// The height of the plot area of the chart in pixels.  
        /// Default is 300
        /// </summary>
        public decimal PlotHeightPixels
        {
            get { return _plotHeightPixels ?? 300m; }
            set { _plotHeightPixels = value; }
        }

        /// <summary>
        /// Determines at what interval gridlines are shown from 
        /// the 'Y' axis.  Defaut is 10
        /// </summary>
        public int GridUnit
        {
            get { return _gridUnit ?? 10; }
            set { _gridUnit = value; }
        }

        /// <summary>
        /// Text indicating the unit that is represented by 
        /// 'X' axis.  Applies only if the chart does not contain
        /// a XUnitLabel section
        /// </summary>
        public string XUnitLabel
        {
            get { return _xUnitLabel; }
            set { _xUnitLabel = value; }
        }

        /// <summary>
        /// Text indicating the unit that is represented by 
        /// 'Y' axis.  Applies only if the chart does not contain
        /// a YUnitLabel section.
        /// </summary>
        public string YUnitLabel
        {
            get { return _yUnitLabel; }
            set { _yUnitLabel = value; }
        }

        /// <summary>
        /// The number of pixels padded on the left and right of each bar
        /// and also added the sides of the plot area.  To 'spread out' 
        /// the bars more, increase this value.  Default is 5
        /// </summary>
        public decimal BarSpacingPixels
        {
            get { return _barSpacingPixels ?? 5m; }
            set { _barSpacingPixels = value; }
        }

        /// <summary>
        /// The number of items in the chart data (read only)
        /// </summary>
        public int ItemCount
        {
            get { return _count; }
        }

        /// <summary>
        /// CSS class applied to the chart container.  Default is 
        /// "chart".  Applies only if the chart does not contain
        /// a title section
        /// </summary>
        public string CssClass
        {
            get { return _cssClass ?? "chart"; }
            set { _cssClass = value; }
        }

        /// <summary>
        /// Indicates whether to show gridlines. Default
        /// is TRUE
        /// </summary>
        public bool ShowGridlines
        {
            get { return _showGridlines ?? true; }
            set { _showGridlines = value; }
        }

        /// <summary>
        /// Interval at which to display 'X' axis labels. Default
        /// is 1
        /// </summary>
        public int LabelInterval
        {
            get { return _labelInterval ?? 1; }
            set { _labelInterval = value; }
        }
    }
}