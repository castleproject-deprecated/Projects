
namespace Castle.Components.Localization.MonoRail
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Web;
	using System.Threading;
	using System.Globalization;

	#endregion Using Directives

	public class GlobalApplication : HttpApplication
	{
		protected virtual void Application_OnBeginRequest( object sender, EventArgs e )
		{
			// For each request initialize the culture values with
			// the culture specified in the culture cookie if it exists, 
			// other sets the culture as specified by the browser.

			try
			{
				CultureInfo culture = null;
				HttpCookie cultureCookie = HttpContext.Current.Request.Cookies[ "culture" ];

				if ( cultureCookie != null )
					culture = CultureInfo.CreateSpecificCulture( cultureCookie.Value );

				Thread.CurrentThread.CurrentCulture = culture;
			}
			catch ( Exception )
			{
				try
				{
					Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture( Request.UserLanguages[ 0 ] );
				}
				catch
				{
					// provide fallback for not supported languages.
					Thread.CurrentThread.CurrentCulture = new CultureInfo( "en-US" );
				}

			}

			Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
		}

	}
}
