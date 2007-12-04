using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
	public class usingviewcomponents_usingcomponentwithasinglelettername : AspViewBase
	{
		protected override string ViewName { get { return "\\UsingViewComponents\\UsingComponentWithASingleLetterName.aspx"; } }
		protected override string ViewDirectory { get { return "\\UsingViewComponents"; } }


		public override void Render()
		{
Output(@"some text before viewcomponent
");
InvokeViewComponent("A", null, new KeyValuePair<string, object>[] {  } );
Output(@"
some text after viewcomponent");

		}

	}
}
