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

// Original implementation Copyright (c) 2007, Joey Beninghove.
// http://joeydotnet.com/blog

// Revised and adapted by James M. Curran
// http://www.HonestIllusion.com

#endregion

namespace Castle.MonoRail.ViewComponents
{
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;

	/// <summary>
	/// Creates a simple form, with a text box and submit link, intended to request a 
	/// seach keyword.
	/// </summary>
	/// <remarks>
	/// <list type="table">
	/// <listheader><term>Parameter</term><description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>                   ActionToFire                        </term>
	/// <description>  string, specifying the Action (on the current controller) 
	///                that the form will be submitted to. Optional, defaults to "index"
	/// </description>
	/// </item>
	/// <item>
	/// <term>                   searchFormName                        </term>
	/// <description>  string, specifying the Html element id of the form.
	///                Optional, defaults to "searchCriteria".
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>
	public class SearchFormComponent : ViewComponentEx
	{
		HtmlHelper html;

		/// <summary>
		/// Gets or sets the action to fire.
		/// </summary>
		/// <value>The action to fire.</value>
		[ViewComponentParam(Default="index")]
		public string ActionToFire { get; set; }

		/// <summary>
		/// Gets or sets the name of the search form.
		/// </summary>
		/// <value>The name of the search form.</value>
		[ViewComponentParam(Default="searchForm")]
		public string searchFormName {get; set;}

		private string searchCriteria;
		private const string searchCriteriaKey = "searchCriteria";

		/// <summary>
		/// Initializes a new instance of the SearchFormComponent class.
		/// </summary>
        public override void Initialize()
		{
			html = new HtmlHelper(EngineContext);
			base.Initialize();

			searchCriteria = PropertyBag[searchCriteriaKey] as string ?? string.Empty;
		}

		/// <summary>
		/// Called by the framework so the component can
		/// render its content
		/// </summary>
		public override void Render()
		{
			base.Render();

			RenderText(html.Form(ActionToFire, searchFormName, "post"));

			RenderText(html.InputText(searchCriteriaKey, searchCriteria));
			RenderText("&nbsp;");
			RenderComponent<LinkSubmitButtonComponent>(DictHelper.CreateN("linkText","Search").N("formToSubmit", searchFormName));
			RenderText(html.EndForm());
			CancelView();
		}
	}
}
