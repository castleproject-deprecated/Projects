using System;
using AspViewTestSite.Interfaces.UsingDictionaryAdapter.Nested;
using Castle.MonoRail.Framework;
using TestModel;

namespace AspViewTestSite.Controllers.UsingDictionaryAdapter
{
	[ControllerDetails(Area = "UsingDictionaryAdapter")]
	public class NestedController : Controller<IAlsoWithTypedPropertiesView>
	{
		public void AlsoWithTypedProperties()
		{
			TypedPropertyBag.Id = 32;
			TypedPropertyBag.Name = "McHale, Kevin";
			TypedPropertyBag.Post = new Post();
			TypedPropertyBag.Post.Content = "Some content";
			TypedPropertyBag.Post.PublishDate = new DateTime(2007, 10, 16);
			TypedPropertyBag.IsImportant = true;
		}
	}
}