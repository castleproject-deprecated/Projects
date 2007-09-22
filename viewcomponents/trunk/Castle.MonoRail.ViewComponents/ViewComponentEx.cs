
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
    }
}
