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

using System;
using System.Collections;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;

namespace Castle.MonoRail.ViewComponents
{
    public class CheckboxListComponent : ViewComponent
    {
        private bool useInline;
        private bool splitPascalCase;
        private bool isHorizontal;

        private static readonly string[] sections = new string[]
            {
                "label", "containerStart", "containerEnd", "itemStart", "itemEnd"
            };

        public override bool SupportsSection(string name)
        {
            foreach (string section in sections)
            {
                if (section.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public override void Render()
        {
            IEnumerable source = ComponentParams["source"] as IEnumerable;
            if (source == null)
            {
                throw new ViewComponentException("The checkbox list component requires a parameter named 'source' that implements 'IEnumerable'");
            }

            string target = ComponentParams["target"] as string;
            if (target == null)
            {
                throw new ViewComponentException("The checkbox list component requires a view component parameter named 'target' that is a string");
            }
            
            isHorizontal = GetBoolParamValue("horizontal", false);
            useInline = GetBoolParamValue("useInline", true);
            splitPascalCase = GetBoolParamValue("splitPascalCase", true);

            FormHelper helper = (FormHelper)Context.ContextVars["FormHelper"];

            FormHelper.CheckboxList list = helper.CreateCheckboxList(
                target, source, null);

            RenderStart();

            int index = 0;
            foreach (object item in list)
            {
                PropertyBag["item"] = item;
                RenderItemStart();
                RenderText(list.Item());
                string checkboxId = string.Format("{0}_{1}_", target.Replace('.', '_'), index);
                RenderLabel(item, checkboxId);
                RenderItemEnd();
                index++;
            }

            RenderEnd();
        }

        private void RenderStart()
        {
            if (Context.HasSection("containerEnd"))
            {
                RenderSection("containerEnd");
            }
            else
            {
                string inlineStyle = useInline
                    ? " style='white-space:nowrap;'"
                    : string.Empty;
                RenderText(string.Format("<div class='{0}'{1}>", CssClass, inlineStyle));
            }
        }

        private void RenderEnd()
        {
            if (Context.HasSection("containerStart"))
            {
                RenderSection("containerStart");
            }
            else
            {
                RenderText("</div>");
            }
        }

        private void RenderLabel(object item, string forId)
        {
            if (Context.HasSection("label"))
            {
                RenderSection("label");
            }
            else
            {
                string inlineStyle = useInline
                    ? " style='padding-left:0.4em; padding-right:1em;'"
                    : string.Empty;
                string itemValue = splitPascalCase
                    ? PascalCaseToPhrase(item.ToString())
                    : item.ToString();
                RenderText(string.Format("<label for='{0}' {1}>{2}</label>",
                    forId, inlineStyle, itemValue));
            }
        }

        private void RenderItemStart()
        {
            if (Context.HasSection("itemStart"))
            {
                RenderSection("itemStart");
            }
            else
            {
                RenderText(string.Format("<{0}>",
                    isHorizontal ? "span" : "div"));
            }
        }

        private void RenderItemEnd()
        {
            if (Context.HasSection("itemEnd"))
            {
                RenderSection("itemEnd");
            }
            else
            {
                RenderText(string.Format("</{0}>",
                    isHorizontal ? "span" : "div"));
            }
        }

        private string CssClass
        {
            get
            {
                string cssClass = ComponentParams["cssClass"] as string;
                if (!string.IsNullOrEmpty(cssClass))
                {
                    return cssClass;
                }
                string cssPrefix = isHorizontal ? "horizontal" : "vertical";
                return string.Format("{0}{1}", cssPrefix, "CheckboxList");
            }
        }

        private bool GetBoolParamValue(string paramName, bool defaultValue)
        {
            bool? paramValue = ComponentParams[paramName] as bool?;
            if (defaultValue)
            {
                return !paramValue.HasValue || paramValue.Value;
            }
            else
            {
                return paramValue.HasValue && paramValue.Value;
            }
        }

        /// <summary>
        /// Converts pascal case string into a phrase,  Treats consecutive capital 
        /// letters as a word.  For example, 'HasABCDAcronym' would be tranformed to
        /// 'Has ABCD Acronym'
        /// </summary>
        /// <param name="input">A string containing pascal case words</param>
        /// <returns>string</returns>
        private static string PascalCaseToPhrase(string input)
        {
            string result = null;
            char[] letters = input.ToCharArray();
            for (int i = 0; i < letters.Length; i++)
            {
                bool isFirstLetter = i == 0;
                bool isLastLetter = i == letters.Length;
                bool isCurrentLetterUpper = Char.IsUpper(letters[i]);
                bool isPrevLetterLower = i > 0 && Char.IsLower(letters[i - 1]);
                bool isNextLetterLower = i + 1 < letters.Length && Char.IsLower(letters[i + 1]);
                bool isPrevCharIsSpace = i > 0 && letters[i - 1] == ' ';
                if (!isFirstLetter &&
                    isCurrentLetterUpper &&
                    !isPrevCharIsSpace &&
                    (isPrevLetterLower || isNextLetterLower || isLastLetter))
                {
                    result += " ";
                }
                result += letters[i].ToString();
            }
            return result;
        }
    }
}