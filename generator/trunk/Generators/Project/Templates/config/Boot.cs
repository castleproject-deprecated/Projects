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
			
			string basePath = new FileInfo(
				Assembly.GetExecutingAssembly().Location).Directory.FullName;
			
			XmlConfigurationSource config = new XmlConfigurationSource(
				string.Format("{0}/../../config/databases/{1}.xml", 
				basePath,  database));
			if (!isWeb) {
				config.ThreadScopeInfoImplementation = null;
			}
			
			ActiveRecordStarter.Initialize(Assembly.GetExecutingAssembly(),	config);
			
			isInitialized = true;
		}
	}
}

