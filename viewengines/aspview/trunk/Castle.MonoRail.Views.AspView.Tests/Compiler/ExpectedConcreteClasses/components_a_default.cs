using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
	public class components_a_default : AspViewBase
	{
		protected override string ViewName { get { return "\\Components\\A\\Default.aspx"; } }
		protected override string ViewDirectory { get { return "\\Components\\A"; } }


		public override void Render()
		{
Output(@"<p>
<strong>A ViewComponent</strong>
</p>");

		}

	}
}
