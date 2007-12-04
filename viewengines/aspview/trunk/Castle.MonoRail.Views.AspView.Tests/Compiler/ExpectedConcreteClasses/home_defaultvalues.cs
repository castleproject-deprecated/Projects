using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
	public class home_defaultvalues : AspViewBase
	{
		protected override string ViewName { get { return "\\Home\\DefaultValues.aspx"; } }
		protected override string ViewDirectory { get { return "\\Home"; } }

		private string s { get { return (string)GetParameter("s", "default"); } }
		private int i { get { return (int)GetParameter("i", 1); } }
		private DateTime? dt { get { return (DateTime?)GetParameter("dt", null); } }

		public override void Render()
		{
Output(s);
Output(@"
");
Output(i);
Output(@"
");
Output(dt==null);
Output(@"
");

		}

	}
}
