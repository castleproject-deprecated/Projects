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
using System.Reflection;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Framework.Helpers;
using System.Text;

namespace Castle.MonoRail.ViewComponents
{
    public class CheckboxListComponent : ViewComponent
    {
        private static readonly string[] sections = new string[]
            {
                "containerStart", "containerEnd", "itemStart", "itemEnd"
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

        private bool isHorizontal;
        private IEnumerable source;
        private string target;
        private string valueMemberName;
        private string displayMemberName;
        private string style;
        private string labelStyle;
        private string cssClass;
        private bool splitPascalCase;
        
        public override void Render()
        {
            GetParameters();
            RenderStart();
            RenderItems();
            RenderEnd();
        }

        private void GetParameters()
        {
            source = ComponentParams["source"] as IEnumerable;
            if (source == null)
            {
                throw new ViewComponentException("The checkbox list component requires a parameter named 'source' that implements 'IEnumerable'");
            }

            target = ComponentParams["target"] as string;
            if (target == null)
            {
                throw new ViewComponentException("The checkbox list component requires a view component parameter named 'target' that is a string");
            }
            valueMemberName = ComponentParams["valueMember"] as string;
            isHorizontal = GetBoolParamValue("horizontal", false);
            displayMemberName = ComponentParams["displayMember"] as string;
            style = ComponentParams["style"] as string;
            labelStyle = ComponentParams["labelStyle"] as string;
            cssClass = GetCssClass();
            splitPascalCase = GetBoolParamValue("splitPascalCase", true);
        }

        private void RenderStart()
        {
            if (Context.HasSection("containerStart"))
            {
                RenderSection("containerStart");
            }
            else
            {
                RenderText(string.Format("<div class='{0}' style='{1}'>", cssClass,
                    style ?? "white-space:nowrap;'"));
            }
        }

        private void RenderItems()
        {
            IDictionary attributes = new Hashtable();
            if (!string.IsNullOrEmpty(valueMemberName))
            {
                attributes.Add("value", valueMemberName);
            }

            FormHelper helper = (FormHelper)Context.ContextVars["FormHelper"];

            FormHelper.CheckboxList list = helper.CreateCheckboxList(
                target, source, attributes);

            int index = 0;
            foreach (object item in list)
            {
                RenderItemStart();
                RenderItem(list, item, index);
                RenderItemEnd();
                index++;
            }
        }


        private void RenderEnd()
        {
            if (Context.HasSection("containerEnd"))
            {
                RenderSection("containerEnd");
            }
            else
            {
                RenderText("</div>");
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

        private void RenderItem(FormHelper.CheckboxList list, object item, int index)
        {
            PropertyBag["item"] = item;
            RenderText(list.Item());
            string checkboxId = string.Format("{0}_{1}_", target.Replace('.', '_'), index);
            RenderLabel(item, checkboxId);
        }

        private void RenderLabel(object item, string forId)
        {
            object itemValue;
            if (string.IsNullOrEmpty(displayMemberName))
            {
                itemValue = item;
            }
            else
            {
                Type type = item.GetType();
                PropertyInfo pi = type.GetProperty(displayMemberName,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (pi == null)
                {
                    throw new ViewComponentException(string.Format("Invalid 'displayMember' specified for the checkbox list component.  The source class '{0}' does not contain a property '{1}'", type, displayMemberName));
                }
                itemValue = pi.GetValue(item, null);
            }
            string labelText = splitPascalCase
                ? PascalCaseToPhrase(itemValue.ToString())
                : itemValue.ToString();
            RenderText(string.Format("<label for='{0}' style='{1}'>{2}</label>",
                forId,
                labelStyle ?? "padding-left:0.4em; padding-right:1em;",
                labelText));
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

        private string GetCssClass()
        {
            string parmValue = ComponentParams["cssClass"] as string;
            if (!string.IsNullOrEmpty(parmValue))
            {
                return parmValue;
            }
            string cssPrefix = isHorizontal ? "horizontal" : "vertical";
            return string.Format("{0}{1}", cssPrefix, "CheckboxList");
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
            StringBuilder result = new StringBuilder();
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
                    result.Append(" ");
                }
                result.Append(letters[i].ToString());
            }
            return result.ToString();
        }
    }
}