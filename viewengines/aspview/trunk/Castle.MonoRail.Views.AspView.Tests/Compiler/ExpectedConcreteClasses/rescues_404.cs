using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
	public class rescues_404 : AspViewBase
	{
		protected override string ViewName { get { return "\\Rescues\\404.aspx"; } }
		protected override string ViewDirectory { get { return "\\Rescues"; } }


		public override void Render()
		{
Output(@"I'm 404");

		}

	}
}
