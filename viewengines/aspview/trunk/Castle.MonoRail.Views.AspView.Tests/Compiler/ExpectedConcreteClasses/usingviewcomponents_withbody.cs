using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
	public class usingviewcomponents_withbody : AspViewBase
	{
		protected override string ViewName { get { return "\\UsingViewComponents\\WithBody.aspx"; } }
		protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }


		public override void Render()
		{
Output(@"A simple viewcomponent, without a body and sections
");
InvokeViewComponent("Bold", Bold0_body, null);
Output(@"
I was supposed to be rendered after the viewcomponent");

		}

		internal void Bold0_body ()
		{
			Output(@"I was supposed to be rendered in the viewcomponent");

		}

	}
}
