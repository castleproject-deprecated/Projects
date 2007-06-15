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


namespace Castle.MonoRail.ViewComponents
{

    using System;
    using Castle.MonoRail.Framework;
    
    public class ColumnChartComponent : ViewComponent
    {

        #region constants

        private const int _dataLabelVerticalOffset = 18;
        private const int _defaultLineWidthPx = 1;
        
        #endregion constants

        #region private fields
        
        private static readonly string[] sections = new string[]
            {
                "title", "empty", "containerStart", "containerEnd",
                "xUnitLabel", "yUnitLabel"
            };
        private decimal _maxValue;
        private decimal _columnCount;
        private decimal _pxPerUnit;
        private int _gridlineCount;
        private int _ceilingValue;
        private decimal _plotAreaWidth;
        private bool _hasYUnitLabel;
        private bool _isBrowserIE;
        
        #endregion private fields

        private ChartProperties Props
        {
            get { return ComponentParams["properties"] as ChartProperties; }
        }

        public override bool SupportsSection(string name)
        {
            foreach (string section in sections)
            {
                if (section.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }

        public override void Render()
        {
            if (Props == null)
            {
                throw new ViewComponentException("The chart component requires a parameter named 'properties' of type ColumnChartProperties");
            }

            SetCachedValues();

            RenderContainerStart();
            RenderTitle();
            if (Props.ItemCount > 0)
            {
                RenderPlotArea();
                RenderXAxisLabels();
                RenderXUnitLabel();
            }
            else
            {
                RenderPlotAreaEmpty();
            }
            RenderContainerEnd();
        }

        private void SetCachedValues()
        {
            _maxValue = 0;
            foreach (ChartDataItem item in Props.Data)
            {
                if (item.Value > _maxValue)
                {
                    _maxValue = item.Value;
                }
            }

            decimal headRoomGridlinePercent = .4m;
            _gridlineCount = (int) Math.Ceiling(
                                       (_maxValue + (Props.GridUnit * headRoomGridlinePercent))/
                                       Props.GridUnit);
            _ceilingValue = _gridlineCount*Props.GridUnit;
            _pxPerUnit = Props.PlotHeightPixels/_ceilingValue;

            _columnCount = Props.ItemCount + (_hasYUnitLabel ? 5 : 4);

            decimal barCellWidth = Props.BarWidthPixels + (Props.BarSpacingPx * 2);

            decimal totalPlotMarginWidth = Props.BarSpacingPx * 2;

            _isBrowserIE = HttpContext.Request.Browser.Browser.ToLower() == "ie";

            _plotAreaWidth = (barCellWidth*Props.ItemCount) + totalPlotMarginWidth + 
                             (_isBrowserIE ? _defaultLineWidthPx : 0);

            _hasYUnitLabel = Context.HasSection("yUnitLabel") || 
                             !string.IsNullOrEmpty(Props.YUnitLabel);

        }

        private void RenderContainerStart()
        {
            if (Context.HasSection("containerStart"))
            {
                Context.RenderSection("containerStart");
            }
            else
            {
                RenderText(string.Format("<div class='{0}'>", Props.CssClass));
            }
            RenderText("<table style='border-collapse: collapse; border-spacing: 0;'>");
        }

        private void RenderTitle()
        {
            if (Context.HasSection("title") || !string.IsNullOrEmpty(Props.Title))
            {
                RenderText(string.Format("<tr><td colspan='{0}' class='title'>", _columnCount));
                if (Context.HasSection("title"))
                {
                    Context.RenderSection("title");
                }
                else if (!string.IsNullOrEmpty(Props.Title))
                {
                    RenderText(Props.Title);
                }
                RenderText("</td></tr>");
            }
        }

        private void RenderContainerEnd()
        {
            RenderText("</table>");
            if (Context.HasSection("containerEnd"))
            {
                Context.RenderSection("containerEnd");
            }
            else
            {
                RenderText("</div>");
            }
        }

        private void RenderPlotAreaEmpty()
        {
            if (Context.HasSection("empty"))
            {
                Context.RenderSection("empty");
            }
            else
            {
                RenderText(string.Format(@"
                      <tr style='height: {0}px;'>
                        <td colspan='4' style='text-align: center; padding:20px; 
                                border: solid {1}px #999'>{2}</td></tr>",
                    Props.PlotHeightPixels, _defaultLineWidthPx, Props.EmptyMessage));
            }
        }

        private void RenderPlotArea()
        {
            RenderText("<tr>");
            RenderYUnitLabel();
            RenderYAxis();
            RenderColumns();
            RenderText("</tr>");
        }

        private void RenderYAxis()
        {
            RenderText(string.Format(@"
                  <td style='height: {0};'>
                    <div class='left-line' style='height: {0}px; right: -1px; text-align: right; 
                    position: relative;' />",
                Props.PlotHeightPixels));

            for (int i = 0; i <= _gridlineCount; i++)
            {
                string labelAlignmentAttribute;
                decimal unit = Props.GridUnit * (_gridlineCount - i);
                decimal top = _pxPerUnit*Props.GridUnit*i;

                if (i == 0)
                {
                    labelAlignmentAttribute = "top: 0;";
                }
                else if (i == _gridlineCount)
                {
                    labelAlignmentAttribute = "bottom: 0;";
                }
                else
                {
                    labelAlignmentAttribute = string.Format("top: {0}px;", top);

                    if (Props.ShowGridlines)
                    {
                        RenderText(string.Format(@"
                            <div class='gridline' style='width: {0}px; margin-right: -{0}px; 
                                border-width:{1}px; position: absolute; top: {2}px; right: 0; 
                                height: 0;'></div>",
                            _plotAreaWidth, _defaultLineWidthPx, top));
                    }
                }

                RenderText(string.Format(@"
                    <div class='y-label' style='position: absolute; {0} right: 4px; margin-top: -1ex;'>
                       {1:" + Props.DataFormat + @"}
                    </div>",
                    labelAlignmentAttribute, unit));
            }

            RenderText("</div></td>");
        }

        private void RenderColumns()
        {
            RenderText(string.Format(@"
                  <td class='plot-area-left-margin' style='padding:0; height: {0}px; width: {1}px;
                        border-left-width: {2}px; border-left-style: solid; border-bottom-width: {2}px;
                        border-bottom-style: solid;'>
                    <div style='width: {1}px; height 1px;'></div>
                  </td>",
                Props.PlotHeightPixels, Props.BarSpacingPx, _defaultLineWidthPx));

            foreach (ChartDataItem item in Props.Data)
            {
                decimal height;
                if (_isBrowserIE)
                {
                    height = (item.Value * _pxPerUnit);
                    height = height == 0m ? 1m : height;
                }
                else
                {
                    height = (item.Value*_pxPerUnit) - _defaultLineWidthPx;
                    height = height < 0m ? 0m : height;
                }

                decimal top = Props.PlotHeightPixels - height;
                decimal barWidth = Props.BarWidthPixels;
                
                string html = (string.Format(@"
                    <td title='{0}: {1:" + Props.DataFormat + @"}' class='plot-area-column' 
                            style='height: {2}px; border-bottom-width: {7}px; border-bottom-style: solid;
                            border-top-width: {7}px; vertical-align: bottom; padding: 0 {3}px; 
                            overflow:hidden;'>
                        <div class='bar' style='height: {4}px; width: {5}px; position: relative; 
                                border-style: none;'>" +
                          (Props.ShowDataLabels ?
                          @"
                            <div class='data-label' style='position: absolute; top: -{8}px; 
                                    width: 100%; text-align:center;'>
                              {1:" + Props.DataFormat + @"}
                            </div>" : null) +
                          @"<div style='width: {5}px; height: 1px; font-size: 1px;'></div>
                        </div>
                     </td>",
                    item.Label, item.Value, Props.PlotHeightPixels, Props.BarSpacingPx,
                    height, barWidth, top, _defaultLineWidthPx, _dataLabelVerticalOffset
                    ));

                RenderText(html);
            }

            RenderText(string.Format(@"
                  <td class='plot-area-right-margin' style='padding:0; width:{0}px; 
                        height: {1}px; border-right-width: 1px; border-bottom-width: {2}px; 
                        border-bottom-style: solid;'>
                    <div style='width: {0}px; height 1px;'></div>
                  </td>",
                Props.BarSpacingPx, Props.PlotHeightPixels, _defaultLineWidthPx));
        }

        private void RenderXAxisLabels()
        {
            RenderText(string.Format(@"
                <tr><td colspan='{0}'></td><td style='width:{1}px; padding:0;'></td>",
                _hasYUnitLabel ? 2 : 1,  Props.BarSpacingPx));

            foreach (ChartDataItem item in Props.Data)
            {
                decimal columnCellWidth = Props.BarWidthPixels + (Props.BarSpacingPx * 2);
                RenderText(string.Format(@"
                      <td class='x-label' style='padding: 1px; overflow:hidden;'>
                        <div style='position:absolute; width: {1}px; margin-right: 0; text-align:center;'>
                          {0:" + Props.LabelFormat + @"}
                        </div>
                      </td>", 
                      item.Label, columnCellWidth));
            }

            RenderText(string.Format(@"
                    <td style='width:{0}px; padding:0;'></td></tr>",
                Props.BarSpacingPx));
        }

        private void RenderYUnitLabel()
        {
            if (_hasYUnitLabel)
            {
                RenderText(string.Format(@"
                    <td class='y-unit-label' style='vertical-align: middle; height: {0}px'>",
                    Props.PlotHeightPixels));
                if (Context.HasSection("yunitlabel"))
                {
                    Context.RenderSection("yunitlabel");
                }
                else
                {
                    RenderText(Props.YUnitLabel);
                }
                RenderText("</td>");
            }
        }

        private void RenderXUnitLabel()
        {
            RenderText(string.Format(@"
                  <tr>
                    <td colspan='{0}'></td>
                    <td colspan='{1}' class='x-unit-label' style='text-align: center;'>",
                _hasYUnitLabel ? 2 : 1, Props.ItemCount + 2));

            if (Context.HasSection("xunitlabel"))
            {
                Context.RenderSection("xunitlabel");
            }
            else
            {
                RenderText(Props.XUnitLabel);
            }

            RenderText("</td></tr>");
        }
    }
}