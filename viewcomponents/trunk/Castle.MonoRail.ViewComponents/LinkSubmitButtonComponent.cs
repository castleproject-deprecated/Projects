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
	using System;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;
	/// <summary>
	/// ViewComponent to create a text link, which functions like a Submit button.
	/// </summary>
	/// <remarks>
	/// <list type="table">
	/// <listheader><term>Parameter</term><description>Description</description>
	/// </listheader>
	/// <item>
	/// <term>                   FormToSubmit                        </term>
	/// <description>  string, specifying the Html element id of the form which
	///                will be submitted by this link, required.
	/// </description>
	/// </item>
	/// <item>
	/// <term>                   LinkText                        </term>
	/// <description>  string, text displayed in the link. Optional, default to "Submit"
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>

	/// <example><code><![CDATA[
	///#component(LinkSubmitButtonComponent with "linkText=Search" "formToSubmit=$searchFormName)
	/// 
	/// ]]></code> </example>
	public class LinkSubmitButtonComponent : ViewComponentEx
	{

		[ViewComponentParam(Required=true)]
		public string FormToSubmit { get; set; }

		[ViewComponentParam(Default="Submit")]
		public string LinkText { get; set; }

		private string JavascriptToSubmitForm()
		{
			const string FormSubmissionJavascriptFormat = "javascript: $('{0}').submit()";
			return string.Format(FormSubmissionJavascriptFormat, FormToSubmit);
		}

		private string LinkToRender()
		{
			return new HtmlHelper().Link(JavascriptToSubmitForm(), LinkText, 
				DictHelper.CreateN("class", "linkButton"));
		}
		/// <summary>
		/// Called by the framework so the component can
		/// render its content
		/// </summary>
		public override void Render()
		{
			base.Render();
			JavascriptHelper javascript = new JavascriptHelper(Context, EngineContext,"LinkSubmitButtonComponent");
			javascript.IncludeStandardScripts("Ajax");

			RenderText(LinkToRender());
			CancelView();
		}
	}
}
