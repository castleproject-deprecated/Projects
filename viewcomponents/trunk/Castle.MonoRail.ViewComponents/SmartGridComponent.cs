// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.ViewComponents
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text.RegularExpressions;

	/// <summary>
	/// 
	/// </summary>
    public class SmartGridComponent : GridComponent
    {
        private static readonly Hashtable validTypesCache = Hashtable.Synchronized(new Hashtable());
        private static readonly Hashtable propertiesCache = Hashtable.Synchronized(new Hashtable());

        private PropertyInfo[] properties;
        private string _sortFunction;

		/// <summary>
		/// Implementor should return true only if the
		/// <c>name</c> is a known section the view component
		/// supports.
		/// </summary>
		/// <param name="name">section being added</param>
		/// <returns>
		/// 	<see langword="true"/> always, as any column can be a section.
		/// </returns>

        public override bool SupportsSection(string name)
        {
            return true;
        }

		/// <summary>
		/// Render the rows from the source, return true if there are rows to render,
		/// false otherwise.
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
        protected override bool ShowRows(IEnumerable source)
        {
            if (properties == null) //there are no rows, if this is the case
                return false;

            bool isAlternate = false;

            foreach (object item in source)
            {
                PropertyBag["item"] = item;

                if (isAlternate)
                {
                    RenderOptionalSection("startAlternateRow", "<tr class='grid_alternateItem'>");
                }
                else
                {
                    RenderOptionalSection("startRow", "<tr class='grid_item'>");
                }

                RenderOptionalSection("columnStartRow");

                foreach (PropertyInfo property in properties)
                {
                    if (ValidPropertyToAutoGenerate(property) == false) continue;

                    if (Context.HasSection(property.Name))
                    {
                        PropertyBag["value"] = property.GetValue(item, null);
                        Context.RenderSection(property.Name);
                        continue;
                    }

                    RenderOptionalSection("startCell", "<td>");

                    object val = property.GetValue(item, null) ?? "null";

                    RenderText(System.Web.HttpUtility.HtmlEncode(val.ToString()));
                    RenderOptionalSection("endCell", "</td>");

                }
                RenderOptionalSection("more");

                if (isAlternate)
                {
                    RenderOptionalSection("endAlternateRow", "</tr>");
                }
                else
                {
                    RenderOptionalSection("endRow", "</tr>");
                }

                isAlternate = !isAlternate;

            }

            return true;
        }


        /// <summary>
        /// Shows the header.
        /// </summary>
        /// <param name="source">The source.</param>
        protected override void ShowHeader(IEnumerable source)
        {
            IEnumerator enumerator = source.GetEnumerator();

            bool hasItem = enumerator.MoveNext();

            if (hasItem == false)
            {
                return;
            }

            object first = enumerator.Current;
            InitializeProperties(first);

            RenderOptionalSection("caption");
            RenderOptionalSection("preHeaderRow", "<thead><tr>");
            RenderOptionalSection("columnStartRowHeader");

            string sortBy = ComponentParams["sortBy"] as string ?? string.Empty;

            bool? sortDirection = ComponentParams["sortAsc"] as bool?;

            bool? sortEnabled = ComponentParams["enableSort"] as bool?;
            if (!sortEnabled.HasValue)
                sortEnabled = false;

            _sortFunction = ComponentParams["sortFunction"] as string;

            foreach (PropertyInfo property in properties)
            {
                if (ValidPropertyToAutoGenerate(property) == false) continue;
                string overrideSection = property.Name + "Header";
                if (Context.HasSection(overrideSection))
                {
                    Context.RenderSection(overrideSection);
                    continue;
                }

                overrideSection = string.Empty;
                if (sortEnabled.Value)
                {
                    overrideSection = property.Name + "SortHeader";
                    if (Context.HasSection(overrideSection))
                    {
                        Context.RenderSection(overrideSection);
                        continue;
                    }
                    if (sortBy == property.Name)
                        RenderHeaderSortCellStart(property.Name, sortDirection.HasValue ? !sortDirection.Value : true, true);
                    else
                        RenderHeaderSortCellStart(property.Name, true, false);

                    RenderText(System.Web.HttpUtility.HtmlEncode(SplitPascalCase(property.Name)));
                    RenderOptionalSection("endHeaderSortCell", "</a></th>");
                }
                else
                {
                    overrideSection = property.Name + "Header";
                    if (Context.HasSection(overrideSection))
                    {
                        Context.RenderSection(overrideSection);
                        continue;
                    }
                    RenderOptionalSection("startHeaderCell", "<th class='grid_header'>");
                    RenderText(System.Web.HttpUtility.HtmlEncode(SplitPascalCase(property.Name)));
                    RenderOptionalSection("endHeaderCell", "</th>");
                }
            }
            RenderOptionalSection("moreHeader");

            RenderOptionalSection("postHeaderRow", "</tr></thead>");

            RenderOptionalSection("tFoot");
        }

        private void RenderHeaderSortCellStart(string fieldName, bool sortAsc, bool showArrow)
        {
            if (Context.HasSection("startHeaderSortCell"))
            {
                PropertyBag["fieldName"] = fieldName;
                PropertyBag["sortAsc"] = sortAsc;

                Context.RenderSection("startHeaderSortAscCell");
                return;
            }

            string href;
            if (!String.IsNullOrEmpty(_sortFunction))
            {
                href = "javascript:" + _sortFunction + "('" + fieldName + "', '" + sortAsc + "');void(0);";
            }
            else
            {
                string url = EngineContext.Request.FilePath;
                string separator = "?";
                if (url.IndexOf('?') > 0)
                    separator = "&";

                href = url + separator + "sortBy=" + fieldName + "&sortAsc=" + sortAsc;
            }

            string style = string.Empty;
            if (showArrow)
                style = !sortAsc ? " sortAscHeader" : " sortDescHeader";

            RenderText(String.Format("<th class=\"grid_header{0}\"><a href=\"{1}\">", style, href));
        }

        /// <summary>
        /// Split a PascalCase string into Pascal Case words.
        /// Note that if the string contains spaces, we assume it is already formatted
        /// http://weblogs.asp.net/jgalloway/archive/2005/09/27/426087.aspx
        /// </summary>
        private static string SplitPascalCase(string input)
        {
            if (input.Contains(" ")) return input;
            return Regex.Replace(input, "([A-Z])", "$1", RegexOptions.Compiled);
        }

        private void InitializeProperties(object first)
        {
            Type type = first.GetType();
            if (ComponentParams.Contains("columns") == false)
            {
                if (propertiesCache.Contains(type))
                    properties = (PropertyInfo[])propertiesCache[type];
                else
                    propertiesCache[type] = properties = type.GetProperties();
                return;
            }
            List<PropertyInfo> props = new List<PropertyInfo>();
            IEnumerable columns = (IEnumerable)ComponentParams["columns"];
            foreach (string columnName in columns)
            {
                string key = type.FullName + "." + columnName;
                PropertyInfo propertyInfo;
                if (propertiesCache.Contains(key))
                    propertyInfo = (PropertyInfo)propertiesCache[key];
                else
                    propertiesCache[key] =
                        propertyInfo = type.GetProperty(columnName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
                if (propertyInfo != null)
                    props.Add(propertyInfo);
            }
            properties = props.ToArray();
        }

        private bool ValidPropertyToAutoGenerate(PropertyInfo property)
        {
            if (false.Equals(ComponentParams["Display" + property.Name]))
                return false;
            if (Context.HasSection(property.Name))
                return true;
            return IsValidType(property.PropertyType);
        }

        private static bool IsValidType(Type typeToCheck)
        {
            if (validTypesCache.ContainsKey(typeToCheck))
                return (bool)validTypesCache[typeToCheck];
            bool result;
            if (typeof(ICollection).IsAssignableFrom(typeToCheck))
            {
                result = false;
            }
            else if (typeToCheck.IsGenericType)
            {
                result = typeof(ICollection<>).IsAssignableFrom(typeToCheck.GetGenericTypeDefinition());
            }
            else
            {
                result = true;
            }
            validTypesCache[typeToCheck] = result;
            return result;
        }
    }
}