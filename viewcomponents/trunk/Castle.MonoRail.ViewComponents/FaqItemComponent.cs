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

#region References
using System;
using System.Collections.Generic;
using System.Text;
using Castle.MonoRail.Framework;
using System.IO;
#endregion

namespace Castle.MonoRail.ViewComponents
{
    /// <summary>
    /// ViewComponent to build one item of a Frequently Asked Questions list. 
    /// The generated markup displays the question, and using DHTML, displays &amp; hides 
    /// the answer when the question text is clicked.
    /// </summary>
    /// <remarks><para>
    /// FaqItemComponent is one of two different components for creating FAQ pages.
    /// </para><para>
    /// It is intended for it format FAQ entries where the text in hard-coded in the view.
    /// To format FAQ entries where the text is comes from an external data source, see <seealso cref="FaqListComponent"/>.
    /// </para><para>
    /// FaqItem is a block component which has two required sections, 
    /// <c>"question"</c> and <c>"answer"</c>, and four optional parameters.
    /// </para>
    /// <list type="table">
    /// <listheader>
    /// <term>Section</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>        #question          </term>
    /// <description>
    /// 
    /// Contains the text of a Frequently asked question.  The text is 
    /// always displayed on the page.  Clicking this text will display the answer.
    /// 
    /// 
    /// </description>
    /// </item>
    /// <item>
    /// <term>        #answer             </term>
    /// <description>
    /// 
    /// Contains the text of the answer to the FAQ.  The text is initially hidden,
    /// and only displayed when the question is clicked.
    /// 
    /// </description>
    /// </item>
    /// </list>
    /// <list type="table">
    /// <listheader>
    /// <term>Parameter</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>       QuestionCssClass      </term>
    /// <description>
    /// 
    /// CSS Class used for the DIV block holding the question.
    /// (Default: <b>Question</b>)
    /// 
    /// </description>
    /// </item>
    /// <item>
    /// <term>       AnswerCssClass        </term>
    /// <description>
    /// 
    /// CSS Class used for the DIV block holding the answer. 
    /// (Default: <b>Answer</b>)</description>
    /// 
    /// </item>
    /// <item>
    /// <term>       WrapItems             </term>
    /// <description>
    /// 
    /// If set to "true", each question/answer block will be wrapped in a LI tags, 
    /// so that a series of FaqItemComponents can be made a ordered (OL) or unordered (UL) list. 
    /// (Default:<b>False</b>)
    /// 
    /// </description>
    /// </item>
    /// <item>
    /// <term>       Sticky                 </term>
    /// <description>
    /// 
    /// If set to "True", the values given for the other parameters will be used for all
    /// subsequent FaqItemComponents on this page. If <i>explicitly</i> set to "False", 
    /// previously save values are forgotten.(Default:<b>False</b>)</description>
    /// 
    /// </item>
    /// </list>
    /// 
    /// <b>NOTE:</b> This ViewComponent makes use of the prototype.js javascript library, and therefore
    /// requires the following line appears in either the view which FaqItemComponent is used, or the layout 
    /// template used by that view:
    /// <code>
    /// $AjaxHelper.GetJavascriptFunctions()
    /// </code>
    /// 
    /// Copyright &#169; 2007, James M. Curran  <br/>
    /// Licensed under the Apache License, Version 2.0
    /// </remarks>
    /// <example>
    /// <code><![CDATA[
    /// #blockcomponent (FaqItemComponent) 
    /// #question 
    ///  Is MonoRail stable? Why it's not 1.0? 
    /// #end 
    ///
    /// #answer 
    ///     Yes, very stable, albeit there's always room for improvements. 
    ///     Check our issue tracker. 
    /// 
    ///      We are not 1.0 because there are important features not 
    ///      implemented yet, like Caching and Logging support.
    /// #end
    /// #end
    /// ]]></code>
    /// will generate the following markup:
    /// <code><![CDATA[
    /// <div id="Faq_Q1" onclick="Element.toggle('Faq_A1')" class="Question">
    /// Is MonoRail stable? Why it's not 1.0?  
    /// </div>
    /// <div id="Faq_A1" style="display:none" class="Answer">
    ///      <br/>
    ///     Yes, very stable, albeit there's always room for improvements. 
    ///     Check our issue tracker. 
    /// 
    ///      We are not 1.0 because there are important features not 
    ///      implemented yet, like Caching and Logging support. 
    /// <hr/>
    /// </div>
    /// ]]></code>
    /// 
    /// </example>
 
    public class FaqItemComponent : ViewComponent
    {
        #region Private Variables
        private int? faqNum;
        private string questionCssClass;
        private string answerCssClass;
        private bool wrapItems;
        #endregion

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // FaqNum is used to assure that the ids used for the Question & Answer DIV blocks
            // are unique across a page, which is expected to have multiple FaqItemComponents.
            faqNum = (int?) Context.ContextVars["FaqNum"];
            if (!faqNum.HasValue)
                faqNum = 0;
            faqNum++;
            Context.ContextVars["FaqNum"] = faqNum;

            // First retrieve saved "sticky" parameters, if any, or set defaults if not.
            questionCssClass = Context.ContextVars["QuestionCssClass"] as string ?? "Question";
            answerCssClass = Context.ContextVars["AnswerCssClass"] as string ?? "Answer";
            string strWrap = Context.ContextVars["WrapItems"] as string ?? "false";

            questionCssClass = Context.ComponentParameters["QuestionCssClass"] as string ?? questionCssClass;
            answerCssClass = Context.ComponentParameters["AnswerCssClass"] as string ?? answerCssClass;
            strWrap = Context.ComponentParameters["WrapItems"] as string ?? strWrap;
            if (!Boolean.TryParse(strWrap, out wrapItems))
                wrapItems = false;

            string strSticky = Context.ComponentParameters["Sticky"] as string;
            bool sticky = false;
            if (strSticky != null && Boolean.TryParse(strSticky, out sticky))
            {
                if (sticky)
                {
                    // If "Sticky" is given, and it's parsable, 
                    // and it's value is True, save values
                    Context.ContextVars["QuestionCssClass"] = questionCssClass;
                    Context.ContextVars["AnswerCssClass"] = answerCssClass;
                    Context.ContextVars["WrapItems"] = strWrap;
                }
                else
                {
                    // If "Sticky" is given, and it's parsable, 
                    // and it's value is False, remove saved values
                    Context.ContextVars.Remove("QuestionCssClass");
                    Context.ContextVars.Remove("AnswerCssClass");
                    Context.ContextVars.Remove("WrapItems");
                }
            }
        }

        /// <summary>
        /// Renders this instance.
        /// </summary>
        public override void Render()
        {
            if (!Context.HasSection("question"))
                throw new ViewComponentException("FaqItemComponent: you must supply a 'question' section");

            if (!Context.HasSection("answer"))
                throw new ViewComponentException("FaqItemComponent: you must supply a 'answer' section");

            StringWriter sw = new StringWriter();
            Context.RenderSection("question", sw);
            string question = sw.ToString();
            
            sw = new StringWriter();
            Context.RenderSection("answer", sw);
            string answer = sw.ToString();

            FaqParameters param = new FaqParameters();
            param.AnswerCssClass = answerCssClass;
            param.QuestionCssClass = questionCssClass;
            param.Number = faqNum.Value;
            param.WrapItems = wrapItems;

            RenderText(FaqItemHelper.BuildItem(question, answer, param));

            CancelView();
        }

        /// <summary>
        /// Indicates if the section is supported by the FaqViewComponent.
        /// </summary>
        /// <param name="name">The name of the section.</param>
        /// <returns>Returns true for "question" or "answer"; and false otherwise.</returns>
        /// <remarks>Called by the framework.</remarks>
        public override bool SupportsSection(string name)
        {
            return name == "question" || name == "answer";
        }
    }


    /// <summary>
    /// Simple class to hold a Question &amp; Answer pair.
    /// </summary>
    /// <remarks>
    /// <list type="table">
    /// <listheader>
    /// <term>Section</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>        Question          </term>
    /// <description>
    /// 
    /// Contains the text of a Frequently asked question.  The text is 
    /// always displayed on the page.  Clicking this text will display the answer.
    /// 
    /// 
    /// </description>
    /// </item>
    /// <item>
    /// <term>        Answer             </term>
    /// <description>
    /// 
    /// Contains the text of the answer to the FAQ.  The text is initially hidden,
    /// and only displayed when the question is clicked.
    /// 
    /// </description>
    /// </item>
    /// </list> 
    /// </remarks>
    public class QnA
    {
        public string Question;     //  { get; set; }
        public string Answer;       //  { get; set; }
        /// <summary>
        /// Initializes a new instance of the QnA class.
        /// </summary>
        /// <param name="question"></param>
        /// <param name="answer"></param>
        public QnA(string question, string answer)
        {
            Question = question;
            Answer = answer;
        }
    }

    /// <summary>
    /// ViewComponent to build a list of Frequently Asked Questions.
    /// The generated markup displays the question, and using DHTML, displays &amp; hides 
    /// the answer when the question text is clicked.
    /// </summary>
    /// <remarks><para>
    /// FaqItemComponent is one of two different components for creating FAQ pages.
    /// </para><para>
    /// It is intended to format FAQ entries where the text is comes from an external data source. <br/>
    /// To format FAQ entries where the text in hard-coded in the view, see <seealso cref="FaqItemComponent"/>.
    /// </para><para>
    /// FaqItem is a line component which has no subsections, one required and three optional parameters.
    /// </para>
    /// <list type="table">
    /// <listheader>
    /// <term>Parameter</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>       Elements      </term>
    /// <description>
    /// 
    /// An IEnumerable collection of <see cref="QnA"/> objects, holding the
    /// text of the FAQs to display.  (Required, no default)
    /// 
    /// </description>
    /// </item>
    /// <item>
    /// <term>       QuestionCssClass      </term>
    /// <description>
    /// 
    /// CSS Class used for the DIV block holding the question.
    /// (Default: <b>Question</b>)
    /// 
    /// </description>
    /// </item>
    /// <item>
    /// <term>       AnswerCssClass        </term>
    /// <description>
    /// 
    /// CSS Class used for the DIV block holding the answer. 
    /// (Default: <b>Answer</b>)</description>
    /// 
    /// </item>
    /// <item>
    /// <term>       ListType             </term>
    /// <description>
    /// Indicates how the items should be formatted into alist.  
    /// Must be one of values in the table below. (Default: <b>None</b>)
    /// 
    /// </description>
    /// </item>
    /// </list>
    /// 
    /// <list type="table">
    /// <listheader>
    /// <term>ListType</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>        None                 </term>
    /// <description>
    /// 
    /// Items are rendered without any form of list structure.
    /// 
    /// </description>
    /// </item>
    /// <item>
    /// <term>        Ordered             </term>
    /// <description>
    /// 
    /// Items are numbered.  ("OL" and "Numbered" are acceptable alternatives)
    /// 
    /// </description>
    /// </item>
    /// <item>
    /// <term>        Unordered            </term>
    /// <description>
    /// 
    /// Items are bulleted.   ("UL" and "bullet" are acceptable alternatives)
    /// 
    /// </description>
    /// </item>
    /// </list>
    /// <b>NOTE:</b> This ViewComponent makes use of the prototype.js javascript library, and therefore
    /// requires the following line appears in either the view which FaqItemComponent is used, or the layout 
    /// template used by that view:
    /// <code>
    /// $AjaxHelper.GetJavascriptFunctions()
    /// </code>
    /// 
    /// Copyright &#169; 2007, James M. Curran  <br/>
    /// Licensed under the Apache License, Version 2.0
    /// </remarks>
    /// <example><code><![CDATA[
    /// #component (FaqListComponent with "Elements=$faqItems")
    /// ]]></code>
    /// See <see cref="FaqItemComponent"/> for example of markup generated.
    /// </example>
    public class FaqListComponent : ViewComponent
    {
        #region    Private fields
        IEnumerable<QnA> faqList;
        String ListTag;
        FaqParameters param;
        #endregion

        #region Overridden Methods
        /// <summary>
        /// Called by the framework once the component instance
        /// is initialized
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            faqList = Context.ComponentParameters["elements"] as IEnumerable<QnA>;

            if (faqList == null)
                throw new ViewComponentException("FaqListComponent: required 'elements' parameter missing or empty.");

            param = new FaqParameters();
            param.QuestionCssClass = Context.ComponentParameters["QuestionCssClass"] as string ?? "Question";
            param.AnswerCssClass = Context.ComponentParameters["AnswerCssClass"] as string ?? "Answer";
            string listType = Context.ComponentParameters["ListType"] as string ?? "none";
            switch (listType.ToLower())
            {
                case "none":
                    param.WrapItems = false;
                    ListTag = "";
                    break;

                case "ordered":
                case "numbered":
                case "ol":
                    param.WrapItems = true;
                    ListTag = "ol";
                    break;

                case "unordered":
                case "ul":
                case "bullet":
                    param.WrapItems = true;
                    ListTag = "ul";
                    break;

                default:
                    throw new ViewComponentException("FaqListComponent: '"+listType+"' is not a acceptable ListType");
            }

        }

        /// <summary>
        /// Called by the framework so the component can
        /// render its content
        /// </summary>
        public override void Render()
        {
            base.Render();
            if (ListTag.Length > 0)
                RenderText("<" + ListTag + ">" + Environment.NewLine);

            foreach (QnA faq in faqList)
            {
                param.Number++;
                RenderText(FaqItemHelper.BuildItem(faq.Question, faq.Answer, param));
            }
            if (ListTag.Length > 0)
                RenderText("</" + ListTag + ">" + Environment.NewLine);

            Context.ContextVars["FaqNum"] = new int?(param.Number);

            CancelView();

        }
        #endregion
    }


    /// <summary>
    /// Private class used to pass around parameter easily.
    /// </summary>
    internal class FaqParameters
    {
        public int Number;
        public string QuestionCssClass;
        public string AnswerCssClass;
        public bool WrapItems;
    }

    /// <summary>
    /// Private class to do the really work of FaqItemComponent &amp; FaqListComponent.
    /// </summary>
    internal static class FaqItemHelper
    {
        public static string BuildItem(string question, string answer, FaqParameters param)
        {
            StringBuilder sb = new StringBuilder(250);

            if (param.WrapItems)
                sb.Append("<li>");

            sb.AppendFormat(@"<div id=""Faq_Q{0}"" onclick=""Element.toggle('Faq_A{0}')"" class=""{1}"">" + Environment.NewLine, 
                param.Number, param.QuestionCssClass);
            sb.Append(question);
            sb.Append("</div>");

            sb.AppendFormat(@"<div id=""Faq_A{0}"" style=""display:none"" class=""{1}"">" + Environment.NewLine, 
                param.Number, param.AnswerCssClass);

            sb.Append("<br/>");
            sb.Append(answer);
            sb.Append("<hr/>");
            sb.Append("</div>");

            if (param.WrapItems)
                sb.Append("</li>");

            return sb.ToString();
        }
    }

}

