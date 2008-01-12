#region License
// Copyright (c) 2007, James M. Curran
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
#endregion

namespace Castle.MonoRail.ViewComponents
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;

    using Castle.MonoRail.Framework;
    using Castle.MonoRail.Framework.Helpers;
    using System.IO;
    #endregion

    /// <summary>
    /// ViewComponent to section text into a lede that is always display, with the remainder 
    /// in a hidden block which is revealed by clicking a link.
    /// </summary>
    /// <remarks>
    /// The component uses methods from the prototype javascript library which it automatically
    /// loads, and adds it's own javascript method, using the <see cref="JavascriptComponent"/>.
    /// It requires that the <see cref="InsertJavascriptComponent"/> be included in the HEAD section 
    /// of the HTML page (probably best placed in the layout).
    /// <para/>
    /// The Html generated has the following features:  <ul>
    ///  <li>Full HTML formating available to all portions of the text.</li>
    ///  <li>Full text available in the body of the page (not just in a tooltip)</li>
    ///  <li>Full text visible when Javascript unavailable/disabled.</li>
    ///   <li>Minimal disurption when CSS unavailable/disabled.</li>
    ///   </ul>
    /// <list type="table">
    /// <listheader>
    /// <term>Parameter</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>       Text        </term>
    /// <description>The string to be displayed. <para/>
    /// <b>Required</b> </description>
    /// </item>
    /// <item>
    /// <term>       MaxLength      </term>
    /// <description>Integer value which gives the maximum number of characters of <see cref="Text"/> that will always 
    /// be displayed. <para />
    /// <b>Required</b>
    /// </description>
    /// </item>
    /// <item>
    /// <term>       MoreText        </term>
    /// <description>The string to be displayed. <para/>
    /// <b>Optional</b>, defaults to "(more)" </description>
    /// </item>
    /// <item>
    /// <term>       LessText        </term>
    /// <description>The string to be displayed. <para/>
    /// <b>Optional</b>, defaults to "(less)" </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <example>
    /// Given the following code:
    /// <code> <![CDATA[
    ///  #set ($text="Congress shall make no law respecting an establishment of religion, or prohibiting the free exercise thereof; or abridging the freedom of speech, or of the press; or the right of the people peaceably to assemble, and to petition the Government for a redress of grievances.")
    /// 
    /// #component (MoreLessText with "Text=$text" "MaxLength=100")
    /// ]]></code>
    /// Would generate code similar to this
    /// <code><![CDATA[
    /// Congress shall make no law respecting an establishment of religion, or prohibiting the free exercise&nbsp;
    /// <span id="extraText:2548fd60"> thereof; or abridging the freedom of speech, or of the press; or the right of the people 
    /// peaceably to assemble, and to petition the Government for a redress of grievances.
    /// <a href="#" onclick="moreless('2548fd60');">(less)</a></span>
    /// <span id="more:2548fd60" style="display:none">
    /// <a href="#" onclick="moreless('2548fd60');">(more)</a>
    /// </span>
    /// <script type="text/javascript">moreless('2548fd60');</script>
    /// ]]></code>
    /// </example>
    [ViewComponentDetails("MoreLessText", Sections = "")]
    public class MoreLessTextComponent : ViewComponentEx
    {
        private int _MaxLength;
        /// <summary>
        /// Gets or sets the max length the text can be before spliting.
        /// </summary>
        /// <value>The length of the max.</value>
        [ViewComponentParam(Required = true)]
        public int MaxLength
        {
            get { return _MaxLength; }
            set { _MaxLength = value; }
        }

        private string _Text;
        /// <summary>
        /// Gets or sets the text to be sectioned.
        /// </summary>
        /// <value>The text.</value>
        [ViewComponentParam(Required = true)]
        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        private string _MoreText;
        /// <summary>
        /// Gets or sets the text used for the link which displays the hidden text.
        /// Defaults to "(more)"
        /// </summary>
        /// <value>The text.</value>
        [ViewComponentParam(Default="(more)")]
        public string MoreText
        {
            get { return _MoreText; }
            set { _MoreText = value; }
        }

        private string _LessText;
        /// <summary>
        /// Gets or sets the text used for the link which hids (again)  the revealed text.
        /// Defaults to "(less)"
        /// </summary>
        /// <value>The text.</value>
        [ViewComponentParam(Default="(less)")]
        public string LessText
        {
            get { return _LessText; }
            set { _LessText = value; }
        }


        /// <summary>
        /// Called by the framework so the component can
        /// render its content
        /// </summary>
        public override void Render()
        {
            base.Render();

            if (Text.Length <= MaxLength)
            {
                RenderText(Text);
            }
            else
            {
                int inx = Text.LastIndexOf(' ', MaxLength);
                RenderText(Text.Substring(0, inx));

				JavascriptHelper helper = new JavascriptHelper(Context, EngineContext, "MoreLessComponent");
                helper.IncludeStandardScripts("Ajax");
                helper.IncludeScriptText("function moreless(key) {Element.toggle('extraText:'+key);Element.toggle('more:'+key);}");
                string key = base.MakeUniqueId("");
                RenderTextFormat(@"&nbsp;<span id=""extraText:{0}"">", key);
                RenderText(Text.Substring(inx));
                RenderTextFormat(@"<a onclick=""moreless('{0}');"">{1}</a></span>" +
                                                @"<span id=""more:{0}"" style=""display:none""><a onclick=""moreless('{0}');"">{2}</a></span>"+
                                                @"<script type=""text/javascript"">moreless('{0}');</script>",
                                                key, LessText, MoreText);
            }
            CancelView();
        }
    }
}
