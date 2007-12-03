using System;
using System.IO;
using Castle.MonoRail.Framework;

namespace Castle.MonoRail.Views.AspView
{
	public interface IViewBaseInternal : IViewBase
	{
		IViewBaseInternal ContentView { get; set;}
		void SetOutputWriter(TextWriter newWriter);
		IDisposable SetDisposeableOutputWriter(TextWriter newWriter);
		void SetContent(string content);
		IViewEngine ViewEngine { get; }
		void OutputSubView(string subViewName);
	}
}
