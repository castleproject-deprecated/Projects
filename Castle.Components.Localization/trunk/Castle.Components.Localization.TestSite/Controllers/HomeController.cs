
namespace Castle.Components.Localization.TestSite.Controllers
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Text;
	using System.Threading;

	using Castle.Components.Localization.MonoRail;
	using Castle.Components.Localization.MonoRail.Helpers;
	using Castle.MonoRail.Framework;

	#endregion Using Directives

	[Layout( "Default" )]
	[Helper( typeof( LocalizedFormHelper ), "LocalizedForm" )]
	[Helper( typeof( ResourceHelper ), "Resource" )]
	[Resource( "GhoticPepper", "Castle.Components.Localization.TestSite.Resources.Images", ResourceEntry = "GhoticPepper", IsStream = true )] 
	public class HomeController : SmartDispatcherController
	{

		public void Default()
		{
			PropertyBag[ "Culture" ] = Thread.CurrentThread.CurrentCulture.DisplayName;
			PropertyBag[ "UICulture" ] = Thread.CurrentThread.CurrentUICulture.DisplayName;
		}

		public void ShowAllNETCultures()
		{
			PropertyBag[ "Culture" ] = Thread.CurrentThread.CurrentCulture.DisplayName;
			PropertyBag[ "UICulture" ] = Thread.CurrentThread.CurrentUICulture.DisplayName;
			PropertyBag[ "Cultures" ] = CultureInfo.GetCultures( CultureTypes.NeutralCultures );
		}

		public void Enumerations()
		{
			PropertyBag[ "Culture" ] = Thread.CurrentThread.CurrentCulture.DisplayName;
			PropertyBag[ "UICulture" ] = Thread.CurrentThread.CurrentUICulture.DisplayName;
			PropertyBag[ "MyEnumerationValue" ] = MyEnumeration.Value2;
			PropertyBag[ "MyEnumerationType" ] = typeof( MyEnumeration );
		}

	}
}
