
namespace Castle.MonoRail.ViewComponents
{
    using System;
    using System.Collections;
    using System.IO;
    using Castle.MonoRail.Framework;
    using Castle.MonoRail.Framework.Helpers;

    /// <summary>
    ///  Render a list into a table, one item per cell, in columns going first down, and then right.
    /// </summary>
    ///  <example>
    ///  Given a list like: 
    ///  <code>PropertyBag["list"] = string[] { "A", "B", "C", "D", "E", "F", "G"};</code> 
    ///  The command:
    ///  <code>#component(Columns with "source=$list" "columns=2")</code>
    ///  will produce a table like:
    ///  <list type="table">
    /// 
    /// <item><term>A</term><description>E</description></item>
    /// <item><term>B</term><description>F</description></item>
    /// <item><term>C</term><description>G</description></item> 
    /// <item><term>D</term><description> </description></item>
    /// </list>
    ///  </example>
    ///  <remarks>
    ///  The componet take one required and two optional parameters.
    ///  <list type="table">
    ///  <listheader>
    ///  <term>Parameter</term>
    ///  <description>Description</description>
    ///  </listheader>
    ///  <item><term>               source                       </term>
    ///  <description>   The data source that will be displayed in the grid. <b>Required. </b>
    ///                  Must implement IList.                   </description></item>
    ///  <item><term>               columns                      </term>
    ///  <description>   integer.  The number of columns in the output table.  Optional, 
    ///                  defaults to 3.                          </description></item>
    ///  <item><term>               id                           </term>
    ///  <description>   string.  The HTML id assign to the grid.  Optional.  
    ///                  Defaults to a random string.            </description></item>
    ///  <item><term>               gridCssClass                 </term>
    ///  <description>   string.  The name of the CSS class assigned to the grid. Optional, 
    ///                  Defaults to "grid".                     </description></item>
    ///  <item></item>
    ///  </list>
    ///  
    ///  ColumnComponent can be used as either a line component or a block component.  If used as a line component, each
    ///  item in source is render using it's ToString() method, with nothing rendered for items past the end.    <br/>
    ///  When used as a block component, up to five sections (all optional) may be specified.
    ///  
    ///  <list type="table">
    ///  <listheader>
    ///  <item><term>Section</term><description>Description</description></item>
    ///  </listheader>
    ///  <item><term>              item                         </term>
    ///  <description>    Specifies how each item in the list should be rendered.  
    ///                   The item itself is available as ${item}.</description></item>
    ///  <item><term>              emptycell                    </term>
    ///  <description>    Specifies what should be displayed in cells after the 
    ///                   end of the list.  If not given, nothing is displayed.</description></item>
    ///  <item><term>              empty                        </term>
    ///  <description>    Specifies what should be displayed if the list is empty.</description></item>
    ///  <item><term>              tablestart                   </term>
    ///  <description>    Allows user to specified their own &lt;table> element</description></item>
    ///  <item><term>               tableend                    </term>
    ///  <description>    Allows user to specified their own &lt;/table> element</description></item>
    ///  </list>
    ///  </remarks>
    [ViewComponentDetails("Columns", Sections = "tablestart,tableend,empty,item,emptycell")]
    public class ColumnsComponent : ViewComponentEx
    {
        /// <summary>
        /// Called by the framework so the component can
        /// render its content
        /// </summary>
        public override void Render()
        {
            IList source = ComponentParams["source"] as IList;

            if (source == null)
            {
                throw new ViewComponentException(
                    "The grid requires a view component parameter named 'source' which should contain 'IList' instance");
            }

            string id = ComponentParams["id"] as string ?? MakeUniqueId("");
            int numCols;
            if (!Int32.TryParse(ComponentParams["columns"] as string ?? "3", out numCols))
                throw new ViewComponentException(
                    "Invalid 'column' view component parameter. 'column' must be an integer.");

            if (!RenderOptionalSection("tablestart"))
            {
                RenderTextFormat("<table id='{0}' class='{1}'>", id, ComponentParams["gridCssClass"] ?? "grid");
            }
            if (source.Count >0)
            {
                int offset = (source.Count + numCols - 1) / numCols;
                for (int i = 0; i < offset; ++i)
                {
                    RenderText("<tr>");
                    for (int j = 0; j < numCols; j++)
                    {
                        RenderText("<td>");
                        int pos = j * offset + i;
                        if (pos < source.Count)
                        {
                            PropertyBag["item"] = source[pos];
                            RenderOptionalSection("item", source[pos].ToString());
                        }
                        else
                        {
                            RenderOptionalSection("emptycell");
                        }
                        RenderText("</td>");
                    }
                    RenderText("</tr>");

                }
            }

            RenderOptionalSection("tableend", "</table>");
        }
    }
}
