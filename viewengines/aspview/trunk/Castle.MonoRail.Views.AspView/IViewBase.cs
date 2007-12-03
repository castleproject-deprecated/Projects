using System;
using System.Collections.Generic;
using System.IO;
using Castle.MonoRail.Framework;

namespace Castle.MonoRail.Views.AspView
{
	public interface IViewBase
	{
		/// <summary>
		/// Gets the output writer for the current view rendering
		/// </summary>
		TextWriter OutputWriter { get; }

		void Process();

		/// <summary>
		/// Used only in layouts. Gets the view contents
		/// </summary>
		string ViewContents { get; }

		/// <summary>
		/// Gets the properties container. Based on current property containers that was sent from the controller, such us PropertyBag, Flash, etc.
		/// </summary>
		Dictionary<string, object> Properties { get; }

		/// <summary>
		/// Gets the current Rails context
		/// </summary>
		IRailsEngineContext Context { get; }

		/// <summary>
		/// Gets the calling controller
		/// </summary>
		IController Controller { get; }

		/// <summary>
		/// Gets a reference to the view's parent view
		/// </summary>
		AspViewBase ParentView { get; }

		void Initialize(AspViewEngine newViewEngine, TextWriter output, IRailsEngineContext newContext,
		                IController newController);

		/// <summary>
		/// When overriden in a concrete view class, renders the view content to the output writer
		/// </summary>
		void Render();

	}
}