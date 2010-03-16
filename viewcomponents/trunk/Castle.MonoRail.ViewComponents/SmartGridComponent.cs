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
	using Castle.MonoRail.Framework.Helpers;
	using System.Data;
	using System.Linq;
	using Castle.MonoRail.Framework;

	class ColumnInfo
	{
		public Type Type { get; set; }
		public string Name { get; set; }
		private Func<object, object> getter;

		public object Value(object item)
		{
			return getter(item);
		}

		public ColumnInfo(PropertyInfo pi)
		{
			Type = pi.PropertyType;
			Name = pi.Name;
			getter = (obj => pi.GetValue(obj, null));
		}

		public ColumnInfo(DataColumn dc)
		{
			Type = dc.DataType;
			Name = dc.Caption ?? dc.ColumnName;
			getter = (dr => (dr as DataRow)[dc]);
		}
	}

	/// <summary>
	/// 
	/// </summary>
    public class SmartGridComponent : GridComponent
    {
		private readonly Dictionary<Type, bool> validTypesCache = new Dictionary<Type, bool>();

		private List<ColumnInfo> Columns = new List<ColumnInfo>();
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
			if (Columns.Count== 0) //there are no rows, if this is the case
                return false;

            bool isAlternate = false;
			bool hasStartAlternateRow = Context.HasSection("startAlternateRow");
			bool hasEndAlternateRow = Context.HasSection("endAlternateRow");
			string ItemCss = (ComponentParams["ItemCssClass"] as string) ?? "grid_item";
			string AltItemCss = (ComponentParams["AltItemCssClass"] as string) ?? "grid_alternateItem"; // ItemCss;
			bool raw = string.Compare(ComponentParams["RawText"] as string, "true", true) == 0;
            string nullText = ComponentParams["nulltext"] as string ?? "null";

			int rowNum = 0;
            RenderText("<tbody>");
            foreach (object item in source)
            {
                PropertyBag["item"] = item;
				PropertyBag["RowNum"] = ++rowNum;

				if (hasStartAlternateRow && isAlternate)
                {
					RenderOptionalSection("startAlternateRow", "<tr class='" + AltItemCss + "'>");
                }
                else
                {
					RenderOptionalSection("startRow", "<tr class='" + ItemCss + "'>");
                }

                RenderOptionalSection("columnStartRow");
				foreach (var property in Columns)
				{
					if (Context.HasSection(property.Name))
					{
						PropertyBag["value"] = property.Value(item);
						Context.RenderSection(property.Name);
					}
					else
					{
						RenderOptionalSection("startCell", "<td>");

						object val = property.Value(item) ?? nullText;

						if (raw)
							RenderText(val.ToString());
						else
							RenderText(System.Web.HttpUtility.HtmlEncode(val.ToString()));
						RenderOptionalSection("endCell", "</td>");
					}
				}
                RenderOptionalSection("more");

				if (hasEndAlternateRow && isAlternate)
                {
                    RenderOptionalSection("endAlternateRow", "</tr>");
                }
                else
                {
                    RenderOptionalSection("endRow", "</tr>");
                }
                isAlternate = !isAlternate;
            }
            RenderText("</tbody>");

            return true;
        }

		/// <summary>
		///  Validate the source object, and
		/// if necessaary, convert it into a IEnumerable type (each item in the enumeration
		/// corresponding to one row).
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		protected override IEnumerable AnalyzeSource(Object obj)
		{
			if ((obj == null)  || !(obj is IEnumerable || obj is DataTable))
			{
				throw new ViewComponentException(
					"The grid requires a view component parameter named 'source' which should be an 'IEnumerable' or a DataTable");
			}

			IEnumerable<string> columns;
			object objColumn = ComponentParams["columns"];
			if (objColumn is string)
				columns = objColumn.ToString().Split(',', '|');
			else
			{
				if (objColumn is IEnumerable)
				{
					columns = (objColumn as IEnumerable).Cast<string>();
				}
				else
					columns = null;
			}


			if (obj is IEnumerable)
			{
				var source = obj as IEnumerable;
				IEnumerator enumerator = source.GetEnumerator();

				bool hasItem = enumerator.MoveNext();

				if (hasItem)
				{
					object first = enumerator.Current;
					InitializeProperties(first, columns);
				}
				return source;
			}
			else // obj is DataTable
			{
				var dt = obj as DataTable;
				InitializeColumns(dt.Columns, columns);
				return dt.Rows;
			}
		}

		private void InitializeColumns(DataColumnCollection dataColumnCollection, IEnumerable<string> columns)
		{
			if (columns == null)
			{
				Columns.AddRange(
					dataColumnCollection.Cast<DataColumn>().Select(dc => new ColumnInfo(dc)));
			}
			else
			{
				Columns.AddRange(
					columns.Select(columnName => new ColumnInfo(dataColumnCollection[columnName])));
			}
		}

        /// <summary>
        /// Shows the header.
        /// </summary>
        /// <param name="source">The source.</param>
		protected override void ShowHeader()
		{

			TextHelper text = PropertyBag["TextHelper"] as TextHelper;
			RenderOptionalSection("caption");
			RenderOptionalSection("preHeaderRow", "<thead><tr>");
			RenderOptionalSection("columnStartRowHeader");

			string sortBy = ComponentParams["sortBy"] as string ?? string.Empty;

			bool? sortDirection = ComponentParams["sortAsc"] as bool?;

			bool sortEnabled = Convert.ToBoolean(ComponentParams["enableSort"]);


			_sortFunction = ComponentParams["sortFunction"] as string;

			foreach (var property in Columns)
			{
				string overrideSection = property.Name + "Header";
				if (Context.HasSection(overrideSection))
				{
					Context.RenderSection(overrideSection);
				}
				else
				{
					overrideSection = string.Empty;
					if (sortEnabled)
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

						RenderText(System.Web.HttpUtility.HtmlEncode(TextHelper.PascalCaseToWord(property.Name)));
						RenderOptionalSection("endHeaderSortCell", "</a></th>");
					}
					else
					{
						overrideSection = property.Name + "Header";
						if (Context.HasSection(overrideSection))
						{
							Context.RenderSection(overrideSection);
						}
						else
						{
							RenderOptionalSection("startHeaderCell", "<th class='grid_header'>");
							RenderText(System.Web.HttpUtility.HtmlEncode(TextHelper.PascalCaseToWord(property.Name)));
							RenderOptionalSection("endHeaderCell", "</th>");
						}
					}
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

        private void InitializeProperties(object first, IEnumerable<string> columns)
        {
            Type type = first.GetType();

			if (columns == null)
			{
				Columns.AddRange(
					type.GetProperties()
						.Where(prop => ValidPropertyToAutoGenerate(prop))
						.Select(prop => new ColumnInfo(prop)));
				return;
			}
			
			else      // "columns" defined
			{
				Columns.AddRange(
					columns.Select(columnName => type.GetProperty(columnName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase))
						.Where(pi => pi != null && ValidPropertyToAutoGenerate(pi))
						.Select(pi => new ColumnInfo(pi)));
			}
        }

        private bool ValidPropertyToAutoGenerate(PropertyInfo property)
        {
            if (false.Equals(ComponentParams["Display" + property.Name]))
                return false;
            if (Context.HasSection(property.Name))
                return true;
            return IsValidType(property.PropertyType);
        }

        private  bool IsValidType(Type typeToCheck)
        {
            if (validTypesCache.ContainsKey(typeToCheck))
                return validTypesCache[typeToCheck];

			bool result = true;
            if (typeof(ICollection).IsAssignableFrom(typeToCheck))
            {
                result = false;
            }
            else if (typeToCheck.IsGenericType)
            {
                result = !typeof(ICollection<>).IsAssignableFrom(typeToCheck.GetGenericTypeDefinition());
            }
            
			if (result)   // if we haven't already eliminated it.
            {
				// Make sure it not using Object.ToString()
				MethodInfo methToString = typeToCheck.GetMethod("ToString", Type.EmptyTypes);
				result = (methToString.DeclaringType != typeof(System.Object));
            }
            validTypesCache[typeToCheck] = result;
            return result;
        }
    }
}
