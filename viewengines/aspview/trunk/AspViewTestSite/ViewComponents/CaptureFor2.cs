
using Castle.MonoRail.Framework;

namespace AspViewTestSite.ViewComponents
{
	using System;
	using System.IO;
	using System.Text;

	/// <summary>
	/// Renders the inner content and stores it in the IViewEngineContext
	/// <code>
	/// #blockcomponent(CaptureFor with "id=someId" ["append=before"])
	///		content to be captured
	/// #end
	///
	/// ${someId}
	/// </code>
	/// id - the key to be used to retrieve the captured contents
	/// append - when present will append component content into the current
	///			 content, if append = "before" will append before the current content
	/// </summary>
	public class CaptureFor2 : ViewComponent
	{
		/// <summary>
		/// Render component's content and stores it in the view engine ContextVars
		/// so it can be reference and included in other places
		/// </summary>
		public override void Render()
		{
			String id = (String)Context.ComponentParameters["id"];

			if (id == null || id.Trim().Length == 0)
			{
				throw new RailsException("CaptureFor requires an id attribute use #blockcomponent(CaptureFor with \"id=someid\")...#end");
			}

			StringWriter buffer = new StringWriter();

			Context.RenderBody(buffer);


			String currentContent = Context.ContextVars[id] as string;
			StringBuilder sb = buffer.GetStringBuilder();
			String appendAtt = Context.ComponentParameters["append"] as string;

			if (appendAtt != null)
			{
				if (appendAtt == "before")
				{
					sb.Append(currentContent);
				}
				else
				{
					sb.Insert(0, currentContent);
				}
			}

			Context.ContextVars[id] = sb.ToString();

			//This makes sure that Brail would bubble the content
			//up to a parent view if called from a sub view.
			Context.ContextVars[id + ".@bubbleUp"] = true;
		}
	}
}
