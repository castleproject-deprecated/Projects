using System;
using AspViewTestSite.Interfaces.UsingDictionaryAdapter;
using TestModel;

namespace AspViewTestSite.Controllers
{
	public class UsingDictionaryAdapterController : Controller<IWithTypedPropertiesView>
	{
		public void WithTypedProperties()
		{
			TypedPropertyBag.Id = 33;
			TypedPropertyBag.Name = "Bird, Larry";
			TypedPropertyBag.Post = new Post();
			TypedPropertyBag.Post.Content = "Some content";
			TypedPropertyBag.Post.PublishDate = new DateTime(2007, 10, 16);
		}
	}
}