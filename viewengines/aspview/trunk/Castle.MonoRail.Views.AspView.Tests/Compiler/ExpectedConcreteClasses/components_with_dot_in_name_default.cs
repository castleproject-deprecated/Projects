using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
	public class components_with_dot_in_name_default : AspViewBase
	{
		protected override string ViewName { get { return "\\Components\\With.Dot.In.Name\\Default.aspx"; } }
		protected override string ViewDirectory { get { return "\\Components\\With.Dot.In.Name"; } }


		public override void Render()
		{
Output(@"<p>
<strong>With.Dot.In.Name ViewComponent</strong>
</p>");

		}

	}
}
