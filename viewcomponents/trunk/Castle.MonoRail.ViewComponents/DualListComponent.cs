#region License
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

// Original implementation Copyright (c) 2007, Shawn Carr 

#endregion

namespace Castle.MonoRail.ViewComponents
{
    using System;
	using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Framework;

    /// <summary>
    /// The DualListComponent is a control designed for the passing items from 
    /// an available list of items to a "chosen" list of items via javascript.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader><term>Parameter</term><description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>                   SourceColumns                        </term>
	/// <description>  An IEnumerable&lt;string> which is the list of available
	///                names for the source listbox.
    /// </description>
    /// </item>
	/// <item>
	/// <term>                   SourceColumnsType                        </term>
	/// <description>  Type field which builds a list of available fields 
	///                based off of the Public Properties of the Type.  <br/>
	///                                   <i>or</i>                            <br/>
	///                An enum, which builds a list of available fields
	///                based off the values of the enum.                <br />
	///                                   <i>or</i>                            <br/>
	///                An Object, which builds a list of available fields 
	///                based off of the Public Properties of the object.
	/// </description>
	/// </item>
	/// <item>
    /// <term>                   SelectedColumns                          </term>
    /// <description>  An IEnumerable&lt;string> which is the list of property 
	///                names that are to go in the already choosen fields listbox. 
	///                (<b>Required</b>)
    /// </description>
    /// </item>
	/// <item>
	/// <term>                   Id                          </term>
	/// <description>  String, used to form Html element ids.  Optional, random Ids used 
	///                if not specified.
	/// </description>
	/// </item>    /// </list>
	/// 
	/// Either SourceColumns or SourceColumnsType must be specified.
	/// <br/>
    /// <br/>
    /// <list type="table">
    /// <listheader><term>Section</term><description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>                   preList                                  </term>
    /// <description>  The start of the view. Default is table tr
    /// </description>
    /// </item>
    /// <item>
    /// <term>                   preListBox1                                  </term>
    /// <description>   Before the first list box. Default is td
    /// </description>
    /// </item>
    /// <item>
    /// <term>                   postListBox1                                  </term>
    /// <description>   After the first list box. Default is /td
    /// </description>
    /// </item>
    /// <item>
    /// <term>                   center                                  </term>
    /// <description>   The code between the listboxes 
    /// </description>
    /// </item>
    /// <item>
    /// <term>                   preListBox2                                  </term>
    /// <description>Before the second list box. Default is td
    /// </description>
    /// </item>
    /// <item>
    /// <term>                   postListBox2                                  </term>
    /// <description>After the second list box. Default is /td
    /// </description>
    /// </item>
    /// <item>
    /// <term>                   postList                                  </term>
    /// <description>The end of the view. Default is /tr /table
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <example> Usage:
    /// <code><![CDATA[
    /// <?brail component DualListComponent, {@sourceColumnsType: sourceColumnsType, 
    ///                                        @selectedColumns: selectedColumns, } ?>
    /// ]]></code>
    /// </example>
    [ViewComponentDetails("DualListComponent", Sections = "preList,preListBox1,postListBox1,center,preListBox2,postListBox2,postList")]
    public class DualListComponent : ViewComponentEx
    {
		/// <summary>
		/// Gets or sets the selected columns.
		/// </summary>
		/// <remarks>
		/// An IEnumerable&lt;string> which is the list of property 
		///                names that are to go in the already choosen fields listbox. 
		///                (<b>Required</b>)
		///</remarks>
		/// <value>The selected columns.</value>
        [ViewComponentParam(Required=true)]
        public IEnumerable<string> SelectedColumns
        {
            get { return _SelectedColumns; }
            set { _SelectedColumns = value; }
        }
        private IEnumerable<string> _SelectedColumns;


		/// <summary>
		/// Gets or sets the source columns.
		/// </summary>
		/// <remarks>An IEnumerable&lt;string> which is the list of available
		///                names for the source listbox.
		///</remarks>
		/// <value>The source columns.</value>
        [ViewComponentParam]
        public IEnumerable<string> SourceColumns
        {
            get { return _SourceColumns; }
            set { _SourceColumns = value; }
        }
        private IEnumerable<string> _SourceColumns;

		/// <summary>
		/// Gets or sets the id.
		/// </summary>
		/// <value>The id.</value>
		[ViewComponentParam]
		public string Id
		{
			get { return _Id; }
			set { _Id = value; }
		}
		private string _Id;

        /// <summary>
        /// Called by the framework so the component can
        /// render its content
        /// </summary>
        public override void Render()
        {
            if (SourceColumns == null)
            {
				object sourceColumnsType = ComponentParams["sourceColumnsType"];

                if (sourceColumnsType == null)
                {
                    throw new ViewComponentException(
                        "The grid requires a view component parameter named 'SourceColumns' which should a IEnumerable<string> \n\r" +
                        " or a 'sourceColumnsType' which should contain a  object");
                }
				if (sourceColumnsType is Type)
					SourceColumns = InitializeProperties(sourceColumnsType as Type);

				else if (sourceColumnsType is Enum)
					SourceColumns = Enum.GetNames(sourceColumnsType.GetType());
				else
					SourceColumns = InitializeProperties(sourceColumnsType.GetType());


            }

			if (Id == null)
			{
				Id = base.MakeUniqueId("Dual");
			}

            BuildPreList();

            BuildPreListBox1();

            BuildListBox1();

            BuildPostListBox1();

            BuildCenter();

            BuildPreListBox2();

            BuildListBox2();

            BuildPostListBox2();

            BuildPostList();

            BuildScript();
        }

        private void BuildScript()
        {
			JavascriptHelper helper = new JavascriptHelper(Context, EngineContext, "DualList");
			helper.IncludeStandardScripts("Ajax");
			helper.IncludeScriptText(
                @"function moveItem(sourceControl, targetControl) 
{
    var source = $(sourceControl);
    var target = $(targetControl);
    if ((source != null) && (target != null)) 
    {
        while ( source.options.selectedIndex >= 0 ) 
        {
            var newOption = new Option();		          // Create a new instance of ListItem
            newOption.text = source.options[source.options.selectedIndex].text;
            newOption.value = source.options[source.options.selectedIndex].value;
            target.options[target.length] = newOption;	  //Append the item in target
            source.remove(source.options.selectedIndex);  //Remove the item from Source
        }
    }
}

// moveOptionsUp
//
// move the selected options up one location in the select list
//
function moveOptionsUp(sourceControl)
{
    var selectList = $(sourceControl);
    var selectOptions = selectList.getElementsByTagName('option');
    for (var i = 1; i < selectOptions.length; i++)
    {
        var opt = selectOptions[i];
        if (opt.selected)
        {
            selectList.removeChild(opt);
            selectList.insertBefore(opt, selectOptions[i - 1]);
        }
     }
}

// moveOptionsDown
//
// move the selected options down one location in the select list
//

function moveOptionsDown(sourceControl)
{
    var selectList = $(sourceControl);
    var selectOptions = selectList.getElementsByTagName('option');
    for (var i = selectOptions.length - 2; i >= 0; i--)
    { 
        var opt = selectOptions[i];
        if (opt.selected)
        {
            var nextOpt = selectOptions[i + 1];   
            opt = selectList.removeChild(opt); 
            nextOpt = selectList.replaceChild(opt, nextOpt); 
            selectList.insertBefore(nextOpt, opt);
         } 
     }
}
");
        }

        private IEnumerable<string> InitializeProperties(Type type)
        {
            PropertyInfo[] properties = type.GetProperties();
            string[] sourceColumns = new string[properties.Length];
            for(int i =0; i < properties.Length; ++i)
            {
                sourceColumns[i] = properties[i].Name;
            }
            return sourceColumns;
        }


        private void BuildListBox1()
        {
            StartListBox(Id+"listBox1");

            List<string> selectedColumns2 = new List<string>(SelectedColumns);

            foreach (string  field in SourceColumns)
            {
                if (!selectedColumns2.Contains(field))
                {
                    CreateOptionItem(field, SplitPascalCase(field));
                }
            }
            EndListBox();
        }

        private void BuildListBox2()
        {
			StartListBox(Id + "listBox2");

            foreach (string s in SelectedColumns)
            {
                CreateOptionItem(s, SplitPascalCase(s));
            }

            EndListBox();

            RenderText("<br/>");

            RenderText("<input onclick=\"javascript:moveOptionsDown('"+Id+"listBox2');\" type=\"button\" value=\"&dArr;\" />"+
					   "<input onclick=\"javascript:moveOptionsUp('" + Id + "listBox2');\" type=\"button\" value=\"&uArr;\" />");
        }

        private void StartListBox(string id)
        {
            RenderText("<select id=\"" + id + "\" class=\"" + id + "\" size=\"7\" multiple=\"multiple\">");
        }

        private void EndListBox()
        {
            RenderText("</select>");
        }

        private void CreateOptionItem(string itemValue, string itemText)
        {
            RenderText("<option value=\"" + itemValue + "\">" + itemText + "</option>");
        }

        private void BuildPreList()
        {
            RenderOptionalSection("preList", "<table class=\"manageColumnList\" cellspacing=\"0\" cellpadding=\"0\"><tr>");
        }

        private void BuildPreListBox1()
        {
            RenderOptionalSection("preListBox1", "<td class=\"listbox1Col\">");
        }

        private void BuildPostListBox1()
        {
            RenderOptionalSection("postListBox1", "</td>");
        }

        private void BuildCenter()
        {
            RenderOptionalSection("center", "<td><p>"+
											"<input onclick=\"javascript:moveItem('" + Id + "listBox1', '" + Id + "listBox2');\" type=\"button\" value=\"&rArr;\" /></p><p>" +
											"<input onclick=\"javascript:moveItem('" + Id + "listBox2', '" + Id + "listBox1');\" type=\"button\" value=\"&lArr;\" /></p></td>");
        }

        private void BuildPreListBox2()
        {
            RenderOptionalSection("preListBox2", "<td class=\"listbox2Col\">");
        }

        private void BuildPostListBox2()
        {
            RenderOptionalSection("postListBox2", "</td>");
        }

        private void BuildPostList()
        {
            RenderOptionalSection("postList", "</tr></table>");
        }

        private static string SplitPascalCase(string input)
        {
            if (input.Contains(" ")) return input;
            return Regex.Replace(input, "([A-Z])", " $1", RegexOptions.Compiled);
        }
    }
}
