using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
using Castle.MonoRail.Framework.Internal;
namespace CompiledViews
{
	public class withresource_index : AspViewBase
	{
		protected override string ViewName { get { return "\\WithResource\\Index.aspx"; } }
		protected override string ViewDirectory { get { return "\\WithResource"; } }

		private ResourceFacade MyResource { get { return (ResourceFacade)GetParameter("MyResource"); } }

		public override void Render()
		{
Output(MyResource["name"]);
Output(@"
");

		}

	}
}
