// Copyright 2006-2007 Ken Egozi http://www.kenegozi.com/
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


namespace AspViewTestSite.ViewComponents
{
	using System;
	using System.Collections;
	using Castle.MonoRail.Framework;

	public class RepeaterComponent : ViewComponent
	{
		static readonly string[] sections = new string[]
			{
				"header", "footer", "empty",
				"item", "alternateItem"
			};

		public override bool SupportsSection(string name)
		{
			foreach (string section in sections)
			{
				if (section.Equals(name, StringComparison.InvariantCultureIgnoreCase))
					return true;
			}
			return false;
		}

		private void AssertRequiredSectionsExists()
		{
			if (!Context.HasSection("header"))
			{
				throw new ViewComponentException("A RepeaterComponent must has a header section");
			}
			if (!Context.HasSection("item"))
			{
				throw new ViewComponentException("A RepeaterComponent must has an item section");
			}
			if (!Context.HasSection("footer"))
			{
				throw new ViewComponentException("A RepeaterComponent must has a footer section");
			}
		}

		private void AssertRequiredParametersExists()
		{
			if (ComponentParams["source"] as IEnumerable == null)
			{
				throw new ViewComponentException(
					"A RepeaterComponent requires a view component parameter named 'source' which should contain an 'IEnumerable' instance");
			}
		}

		public override void Render()
		{
			AssertRequiredSectionsExists();

			AssertRequiredParametersExists();

			IEnumerable source = ComponentParams["source"] as IEnumerable;

			ShowHeader();

			ShowItems(source);

			ShowFooter();
		}

		private void ShowHeader()
		{
			Context.RenderSection("header");
		}

		private void ShowItems(IEnumerable source)
		{
			bool hasAlternate = Context.HasSection("alternateItem");
			bool isAlternate = false;
			bool hasItems = false;

			foreach (object item in source)
			{
				hasItems = true;
				PropertyBag["item"] = item;

				if (hasAlternate && isAlternate)
					Context.RenderSection("alternateItem");
				else
					Context.RenderSection("item");

				isAlternate = !isAlternate;
			}
			if (!hasItems)
				ShowEmpty();
		}

		private void ShowEmpty()
		{
			if (Context.HasSection("empty"))
			{
				Context.RenderSection("empty");
			}
			else
			{
				RenderText("no data");
			}
		}

		private void ShowFooter()
		{
			Context.RenderSection("footer");
		}
	}
}