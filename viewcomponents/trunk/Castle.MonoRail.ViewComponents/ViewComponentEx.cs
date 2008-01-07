
#region License
// Copyright (c) 2007-2008, James M. Curran
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

#region Reference
using System;
using System.IO;
#endregion

namespace Castle.MonoRail.ViewComponents
{
    using Castle.MonoRail.Framework;
    /// <summary>
    /// Helper class to provide a some convenient methods for viewcomponents.
    /// </summary>
    /// <remarks>May one day be incorporated into base class.</remarks>
    public class ViewComponentEx : ViewComponent
    {
        /// <summary>
        /// Renders the text, formatted.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="args">The args.</param>
        public void RenderTextFormat(string format, params object[] args)
        {
            string content = String.Format(format, args);
            base.RenderText(content);
        }


        /// <summary>
        /// Renders the optional section, or the default text, if section not present.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <param name="defaultText">The default text.</param>
        protected bool RenderOptionalSection(string section, string defaultText)
        {
            if (Context.HasSection(section))
            {
                Context.RenderSection(section);
				return true;
            }
            RenderText(defaultText);
			return false;
        }

        /// <summary>
        /// Renders the optional section.
        /// </summary>
        /// <param name="section">The section.</param>
        protected bool RenderOptionalSection(string section)
        {
            if (Context.HasSection(section))
            {
                Context.RenderSection(section);
				return true;
            }
			return false;
        }

        /// <summary>
        /// Gets the section text.
        /// </summary>
        /// <param name="section">The section.</param>
        /// <returns></returns>
        public string GetSectionText(string section)
        {
            StringWriter sw = new StringWriter();
            Context.RenderSection(section, sw);
            return sw.ToString();
        }
        /// <summary>
        /// Confirms a section is present.
        /// </summary>
        /// <remarks>Throws an exception if the given section is not present.
        /// </remarks>
        /// <param name="section">The section.</param>
        /// <exception cref="ViewComponentException">If specified section is not present.</exception>
        public void ConfirmSectionPresent(string section)
        {
            if (!Context.HasSection(section))
            {
                string message = String.Format("{0}: you must supply a '{1}' section", Context.ComponentName, section);
                throw new ViewComponentException(message);
            }
        }

        /// <summary>
        /// Gets a boolean parameter value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="defaultValue">The value used if there is no parameter named key.</param>
        /// <returns></returns>
        public bool GetBoolParamValue(string key, bool defaultValue)
        {
            object parmValue = Context.ComponentParameters[key];
            bool value = defaultValue;
            if (parmValue is string)
            {
                if (!Boolean.TryParse(parmValue as string, out value))
                    value = defaultValue;
            }
            else if (parmValue is bool)
            {
                value = (bool)parmValue;
            }
            return value;
        }

        /// <summary>
        /// Makes an unique id.
        /// </summary>
        /// <remarks>The given prefix is prepended to the generated number.
        /// The ID isn't actual guarenteed to be unique (which would require
        /// using all 32 digits of the guid). But this produce ids sufficently
        /// distinctive to generate multiple controls on a page.
        /// </remarks>
        /// <param name="prefix">The prefix.</param>
        /// <returns>A string usable as a Html element Id</returns>
        public string MakeUniqueId(string prefix)
        {
            byte[] bytes = Guid.NewGuid().ToByteArray();
            int a = BitConverter.ToInt32(bytes, 0);
            int b = BitConverter.ToInt32(bytes, 4);
            int c = BitConverter.ToInt32(bytes, 8);
            int d = BitConverter.ToInt32(bytes, 12);
            return prefix + (a ^ b ^ c ^ d).ToString("x08");
        }
    }
}
