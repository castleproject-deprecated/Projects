using System.Collections.Generic;

namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	public class WithComponentAndSections : AspViewBase
	{
		public override void Render()
		{
			Output(@"Parent
");
			InvokeViewComponent("MyComponent", null,
				new KeyValuePair<string, ViewComponentSectionRendereDelegate>[] { new KeyValuePair<string, ViewComponentSectionRendereDelegate>("section1", MyComponent1_section1), new KeyValuePair<string, ViewComponentSectionRendereDelegate>("section2", MyComponent1_section2) });
			Output(@"
Parent");
		}


		internal void MyComponent1_section1()
		{
			Output(@"section1");
		}

		internal void MyComponent1_section2()
		{
			Output(@"section2");
		}

		protected override string ViewDirectory
		{
			get { return ""; }
		}

		protected override string ViewName
		{
			get { throw new System.Exception("The method or operation is not implemented."); }
		}
	}
}
