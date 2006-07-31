using System;
using System.IO;
using System.Web;
using System.Configuration;
using System.Reflection;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;

namespace <%= ClassName %>
{
	/// <summary>
	/// Bootstrap class for the application.
	/// </summary>
	public class Boot : HttpApplication
	{
		private static bool isInitialized = false;
		
		protected void Application_Start(Object sender, EventArgs e)
		{
			InitializeActiveRecord("development");
		}
		
		public static void InitializeActiveRecord(string database)
		{
			InitializeActiveRecord(database, true);
		}
		
		public static void InitializeActiveRecord(string database, bool isWeb)
		{
			if (isInitialized) return;
			
			XmlConfigurationSource config = null;
			
			if(isWeb)
			{
				config = new XmlConfigurationSource(
					string.Format("../config/databases/{0}.xml", database));
			}
			else
			{
				config = new XmlConfigurationSource(string.Format("../../config/databases/{0}.xml", database));
 				config.ThreadScopeInfoImplementation = null;	
			}			
			isInitialized = true;
		}
	}
}

