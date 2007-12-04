using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
	public class layouts_usingmultipleviewcomponents : AspViewBase
	{
		protected override string ViewName { get { return "\\Layouts\\UsingMultipleViewComponents.aspx"; } }
		protected override string ViewDirectory { get { return "\\Layouts"; } }

		private object capturedContent1 { get { return (object)GetParameter("capturedContent1"); } }
		private object capturedContent2 { get { return (object)GetParameter("capturedContent2"); } }

		public override void Render()
		{
Output(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">

<html xmlns=""http://www.w3.org/1999/xhtml"" >
<head>
    <title>AspView layout test</title>
</head>
<body>
    <div>
        hello from UsingMultipleViewComponents layout
    </div>
    <div>
		<h1>Under me should appear the regular content of the view</h1>
        ");
Output(ViewContents);
Output(@"
    </div>
    <div>
		<h1>Under me should appear the contents of a CaptureFor component, with id=""capturedContent1""</h1>
		");
Output(capturedContent1);
Output(@"
    </div>
    <div>
		<h1>Under me should appear the contents of a CaptureFor component, with id=""capturedContent2""</h1>
		");
Output(capturedContent2);
Output(@"
    </div>
</body>
</html>");

		}

	}
}
