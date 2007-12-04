using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
	public class usingviewcomponents_nested : AspViewBase
	{
		protected override string ViewName { get { return "\\UsingViewComponents\\Nested.aspx"; } }
		protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }


		public override void Render()
		{
Output(@"before the components
");
InvokeViewComponent("Bold", Bold0_body, null);
Output(@"
after all components");

		}

		internal void Bold1_body ()
		{
			Output(@"
					in secondary bolded
				");

		}

		internal void Bold0_body ()
		{
			Output(@"
				in inner bolded
				");
			InvokeViewComponent("Bold", Bold1_body, null);
			Output(@"
				back in outer bolded
			");

		}

	}
}
