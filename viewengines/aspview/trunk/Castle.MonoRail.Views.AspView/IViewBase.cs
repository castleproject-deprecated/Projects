using System.Collections;
using System.IO;
using Castle.MonoRail.Framework;

namespace Castle.MonoRail.Views.AspView
{
	public interface IViewBase
	{
		/// <summary>
		/// Gets the output writer to which the view is rendered.
		/// </summary>
		TextWriter OutputWriter { get; }

		/// <summary>
		/// Processes the view.
		/// Will first render a ContentView if present (on layouts), to the ViewContents property.
		/// </summary>
		void Process();

		/// <summary>
		/// (For layouts) - gets the ContentView's contents.
		/// </summary>
		string ViewContents { get; }

		/// <summary>
		/// Gets the properties container. Based on current property containers that was sent from the controller, such us PropertyBag, Flash, etc.
		/// </summary>
		IDictionary Properties { get; }

		/// <summary>
		/// Gets the current Rails context.
		/// </summary>
		IRailsEngineContext Context { get; }

		/// <summary>
		/// Gets the controller which have asked for this view to be rendered.
		/// </summary>
		IController Controller { get; }

		/// <summary>
		/// (For subviews) - gets a reference to the view's parent view.
		/// </summary>
		IViewBase ParentView { get; }

		/// <summary>
		/// Would initialize a view instance, prepearing it to be processed.
		/// </summary>
		/// <param name="viewEngine">The view engine.</param>
		/// <param name="output">The writer to which the view would be rendered.</param>
		/// <param name="context">The rails engine content.</param>
		/// <param name="controller">The controller which have asked for this view to be rendered.</param>
		void Initialize(AspViewEngine viewEngine, TextWriter output, IRailsEngineContext context,
		                IController controller);

		/// <summary>
		/// When overriden in a concrete view class, renders the view content to the output writer.
		/// </summary>
		void Render();

	}
}