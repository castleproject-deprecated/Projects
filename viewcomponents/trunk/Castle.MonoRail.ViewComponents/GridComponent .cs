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
	using System.Collections;
	using System.IO;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.Components.Pagination;

    /// <summary>
    /// The grid component is simple component, similar to the WebForms Repeater in style.
    /// It can take an IEnumerable data source and render it on the view. 
    /// The Grid Component is intentionally very simple and bare bones components, 
    /// it has very few capabilities, but it is very flexible.                                                     <para/>
    /// It handles rendering of sections, alternate lines, and pagination (using IPaginatedPage) 
    /// automatically, but it leaves all the markup generation to the user.             <para/>
    /// 
    /// Full documentation and examples at:
    /// http://using.castleproject.org/display/Contrib/Grid+Component
    /// 
    /// </summary>
    [ViewComponentDetails("Grid",Sections="header,footer,pagination,empty,item,alternateItem,tablestart,tableend,paginationLink")]
	public class GridComponent : ViewComponentEx
	{
        /// <summary>
        /// Renders this instance.
        /// </summary>
		public override void Render()
		{
			 var source =  AnalyzeSource(ComponentParams["source"]);

			string id = ComponentParams["id"] as string ?? MakeUniqueId("");

			if (! RenderOptionalSection("tablestart"))
			{
				RenderTextFormat("<table id='{0}' class='{1}'>", id, ComponentParams["gridCssClass"] ?? "grid" );
			}

            ShowHeader();

			bool hasItems = ShowRows(source);

            RenderOptionalSection("footer");
            RenderOptionalSection("tableend", "</table>");

			if ( ! hasItems )
			{
                RenderOptionalSection("empty", "Grid has no data");
            }

			IPaginatedPage page = source as IPaginatedPage;
			
			if (page != null)
			{
				ShowPagination(page);
			}
		}

		/// <summary>
		/// When implemented in a derived class, should validate the source object, and
		/// if necessaary, convert it into a IEnumerable type (each item in the enumeration 
		/// corresponding to one row).
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		protected virtual IEnumerable AnalyzeSource(object obj)
		{
			var source = obj as IEnumerable;
			if (source == null)
			{
				throw new ViewComponentException(
					"The grid requires a view component parameter named 'source' which should contain 'IEnumerable' instance");
			}
			return source;
		}

		/// <summary>
		/// Render the rows from the source, return true if there are rows to render,
		/// false otherwise.
		/// </summary>
		protected virtual bool ShowRows(IEnumerable source)
		{
			bool hasAlternate = Context.HasSection("alternateItem");
			bool isAlternate = false;
			bool hasItems = false;
			string ItemCss = (ComponentParams["ItemCssClass"] as string)  ?? "ListItem";
			string AltItemCss = (ComponentParams["AltItemCssClass"] as string) ?? ItemCss;

			foreach(object item in source)
			{
				hasItems = true;
				PropertyBag["item"] = item;

				if (hasAlternate && isAlternate)
				{
					Context.RenderSection("alternateItem");
				}
				else
				{
					PropertyBag["gridItemCssClass"] = isAlternate ? AltItemCss : ItemCss;
					Context.RenderSection("item");
				}

				isAlternate = !isAlternate;
			}
			
			return hasItems;
		}

		private void ShowPagination(IPaginatedPage page)
		{
			PropertyBag["currentPage"] = page;
			
			if (Context.HasSection("pagination"))
			{
				Context.RenderSection("pagination");
				return;
			}
			
			PaginationHelper paginationHelper = (PaginationHelper) Context.ContextVars["PaginationHelper"];
			StringWriter output = new StringWriter();
			output.Write(
				@"<div class='pagination'><span class='paginationLeft'>
Showing {0} - {1} of {2} 
</span><span class='paginationRight'>",
				page.FirstItem, page.LastItem, page.TotalItems);
			
			//if (page.HasFirst)
			//{
				CreateLink(output, paginationHelper, 1, "first");
			//}
			//else
			//{
			//    output.Write("first");
			//}

			output.Write(" | ");
			
			if (page.HasPreviousPage)
			{
				CreateLink(output, paginationHelper, page.PreviousPageIndex, "prev");
			}
			else
			{
				output.Write("prev");
			}

			output.Write(" | ");

			if (page.HasNextPage)
			{
				CreateLink(output, paginationHelper, page.NextPageIndex, "next");
			}
			else
			{
				output.Write("next");
			}

			output.Write(" | ");
			
			//if (page.HasLast)
			//{
				CreateLink(output, paginationHelper, page.LastItemIndex, "last");
			//}
			//else
			//{
			//    output.Write("last");
			//}

			output.Write(@"</span></div>");

			RenderText(output.ToString());
		}

		private void CreateLink(TextWriter output, PaginationHelper paginationHelper, int pageIndex, string title)
		{
			if (Context.HasSection("paginationLink"))
			{
				PropertyBag["pageIndex"] = pageIndex;
				PropertyBag["title"] = title;
				Context.RenderSection("paginationLink", output);
			}
			else
			{
				output.Write(paginationHelper.CreatePageLink(pageIndex, title, null, QueryStringAsDictionary()));
			}
		}

		private IDictionary QueryStringAsDictionary()
		{
			Hashtable table = new Hashtable();

			foreach(string key in Request.QueryString)
			{
				if (key == null) continue;
				
				table[key] = Request.QueryString[key];
			}

			return table;
		}

        /// <summary>
        /// Shows the header.
        /// </summary>
        /// <param name="source">The source.</param>
		protected virtual void ShowHeader()
		{
            ConfirmSectionPresent("header");
			Context.RenderSection("header");
		}
	}
}