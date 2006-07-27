using System;
using Castle.MonoRail.Framework;
using Castle.MonoRail.ActiveRecordSupport;
using <%= HelpersNamespace %>;

namespace <%= Namespace %>
{
	/// <summary>
	/// Application's base class of every controller.
	/// </summary>
	[Helper(typeof(DisplayHelper))]
	public class ApplicationController : ARSmartDispatcherController
	{
		
	}
}
