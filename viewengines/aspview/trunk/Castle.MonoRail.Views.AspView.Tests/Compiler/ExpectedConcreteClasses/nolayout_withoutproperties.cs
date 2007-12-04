using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
	public class nolayout_withoutproperties : AspViewBase
	{
		protected override string ViewName { get { return "\\NoLayout\\WithoutProperties.aspx"; } }
		protected override string ViewDirectory { get { return "\\NoLayout"; } }


		public override void Render()
		{
Output(@"A View without any properties");

		}

	}
}
