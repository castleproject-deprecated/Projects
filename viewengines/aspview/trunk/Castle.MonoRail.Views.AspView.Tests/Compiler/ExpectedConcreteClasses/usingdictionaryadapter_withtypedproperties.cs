using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
using TestModel;
namespace CompiledViews
{
	public class usingdictionaryadapter_withtypedproperties : AspViewBase<AspViewTestSite.Interfaces.UsingDictionaryAdapter.IWithTypedPropertiesView>
	{
		protected override string ViewName { get { return "\\UsingDictionaryAdapter\\WithTypedProperties.aspx"; } }
		protected override string ViewDirectory { get { return "\\UsingDictionaryAdapter"; } }


		public override void Render()
		{
Output(@"<p id=""No_");
Output(view.Id);
Output(@""">Hello ");
Output(view.Name);
Output(@"</p>
<form action=""Save.rails"">
    <input type=""text"" name=""post.PublishDate"" value='");
Output(view.Post.PublishDate);
Output(@"' />
    <input type=""text"" name=""post.Content"" value='");
Output(view.Post.Content);
Output(@"' />
    <input type=""submit"" value=""Save"" />
</form>");

		}

	}
}
