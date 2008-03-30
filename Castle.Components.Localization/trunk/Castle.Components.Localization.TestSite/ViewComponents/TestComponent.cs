
namespace Castle.Components.Localization.TestSite.ViewComponents
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Text;

	using Castle.MonoRail.Framework;

	#endregion Using Directives

	[Resource( "TestResources", "Castle.Components.Localization.TestSite.Resources.TestComponent, Castle.Components.Localization.TestSite" )]
	public class TestComponent : ViewComponent
	{
		public override void Render()
		{
			base.RenderBody();
		}
	}
}
