
#region License
// Copyright (c) 2009, James M. Curran
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

namespace Castle.MonoRail.Framework.Extensions
{
	using System;
	using System.Reflection;
	using System.Web;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Configuration;
	using System.Collections;

//using System.CodeDom.Compiler;

	/// <summary>
	/// This extension appends the ASP.NET Tracing diagnostic information and custom 
	/// tracing messages to the output of the page and sends this information to the 
	/// requesting browser, just as if &lt;% @Page   trace="true" /&gt;  had been specified
	/// on an aspx page.
	/// </summary>
	/// <remarks>
	/// To install this extension you must register the extension on the extensions node
	/// and add the element <c>webtrace</c> under the <c>monoRail</c> configuration node.
	/// <code><![CDATA[
	///   <monoRail>
	///   	<extensions>
	///   	       <extension type="Castle.MonoRail.Framework.Extensions.WebTraceExtension, Castle.MonoRail.ViewComponents" />
	///   	</extensions>
	///   	    <webtrace enabled="true" includePropertyBag="true"  htmlOnly="true" />
	///   </monoRail>
	/// ]]></code>
	/// The <c>webtrace</c> tag accepts three attributes.
	/// <list type="table">
	/// <listheader><term>
	///                                                               Attribute                                                                   </term>
	/// <description>                                          Description                                                              </description>
	/// </listheader>
	/// <item>
	/// <term>                                                     enabled                                                                  </term>
	/// <description>Turns output on and off.  Must be set to <c> true</c> is see anything.  Defaults to <c>false</c></description>
	/// </item>
	/// <item>
	/// <term>                                                     includePropertyBag                                                                  </term>
	/// <description>If set to <c>true</c>, the PropertyBag and Flash dictionaries are displayed in the 
	///                     Application and Session variables sections, respectively, of the output.  Defaults to <c>false</c>
	///</description>
	/// </item>
	/// <item>
	/// <term>                                                     htmlOnly                                                                  </term>
	/// <description>If set to <c>true</c> (the default) trace output is only appended to Html pages.  
	/// Output will be supressed for all responses where the ContentType is not "text/html" 
	/// or where the Request headers include "x-requested-with: XMLHttpRequest".
	/// Since the responses where the output is suppressed are rarely displayed in a browser, or have
	/// very specific formatting (e.g. XML or JSON), there is rarely a need to set this to false.
	/// </description>
	/// </item>
	/// </list>
	/// </remarks>

	public class WebTraceExtension : IMonoRailExtension
	{

		#region Private Fields
		bool includePropertyBag;
		bool htmlOnly = true;
		#endregion

		#region IMonoRailExtension Members

		/// <summary>
		/// Gives to the extension implementor a chance to read
		/// attributes and child nodes of the extension node
		/// </summary>
		/// <param name="node">The node that defines the MonoRail extension</param>
		/// <remarks>Not used by the WebTrace Extension.</remarks>
		public void SetExtensionConfigNode(Castle.Core.Configuration.IConfiguration node)
		{
		}

		#endregion

		#region IMRServiceEnabled Members

		/// <summary>
		/// Configures the WebTrace extension based on value in the web.config.
		/// </summary>
		/// <param name="serviceProvider">The service provider.</param>
		public void Service(IMonoRailServices serviceProvider)
		{
			var config = serviceProvider.GetService<IMonoRailConfiguration>();
			var webtraceNode = config.ConfigurationSection.Children["webtrace"];
			string attr = webtraceNode.Attributes["enabled"];
			bool enabled = attr != null && System.Xml.XmlConvert.ToBoolean(attr);
			if (enabled)
			{
				var manager = serviceProvider.GetService<ExtensionManager>();
				manager.PostControllerProcess += manager_PostControllerProcess;

				attr = webtraceNode.Attributes["includePropertyBag"];
				includePropertyBag = (attr != null) && System.Xml.XmlConvert.ToBoolean(attr);

				attr = webtraceNode.Attributes["htmlOnly"];
				htmlOnly = (attr == null) || System.Xml.XmlConvert.ToBoolean(attr);
			}
		}

		private static void DictionaryToTable(System.Data.DataSet renderdata, string sectionName, IDictionary dict)
		{
			var applicationTable = renderdata.Tables[sectionName + "_State"];
			foreach (System.Collections.DictionaryEntry kv in dict)
			{
				var row = applicationTable.NewRow();
				var current = kv.Key as string;
				row[sectionName + "_Key"] = (current != null) ? current : "<null>";
				var obj2 = kv.Value;
				if (obj2 != null)
				{
					row["Trace_Type"] = obj2.GetType();
					row["Trace_Value"] = obj2.ToString();
				}
				else
				{
					row["Trace_Type"] = "<null>";
					row["Trace_Value"] = "<null>";
				}
				applicationTable.Rows.Add(row);
			}
		}
		void manager_PostControllerProcess(IEngineContext context)
		{
			// only insert on Html pages.
			if (htmlOnly && (!context.Response.ContentType.StartsWith("text/html") || context.Request.Headers["x-requested-with"] == "XMLHttpRequest"))
				return;

			TraceContext tc = context.UnderlyingContext.Trace ?? new TraceContext(context.UnderlyingContext);
			if (includePropertyBag)
			{
				var getdata = typeof(TraceContext).GetMethod("GetData", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod);
				var renderdata = getdata.Invoke(tc, null) as System.Data.DataSet;
				if (renderdata != null)
				{
					DictionaryToTable(renderdata, "Trace_Application", context.CurrentControllerContext.PropertyBag);
					DictionaryToTable(renderdata, "Trace_Session", context.Flash);
				}
			}

			var render = typeof(TraceContext).GetMethod("Render", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod);
			using (var htw = new System.Web.UI.HtmlTextWriter(context.UnderlyingContext.Response.Output))
			{
				render.Invoke(tc, new object[] { htw });
			}
		}
		#endregion
	
	}
}
