using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
	public class layouts_usingcapturefor : AspViewBase
	{
		protected override string ViewName { get { return "\\Layouts\\UsingCaptureFor.aspx"; } }
		protected override string ViewDirectory { get { return "\\Layouts"; } }

		private object capturedContent { get { return (object)GetParameter("capturedContent"); } }

		public override void Render()
		{
Output(@"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">

<html xmlns=""http://www.w3.org/1999/xhtml"" >
<head>
    <title>AspView layout test</title>
</head>
<body>
    <div>
        hello from UsingCaptureFor layout
    </div>
    <div>
		<h1>Under me should appear the regular content of the view</h1>
        ");
Output(ViewContents);
Output(@"
    </div>
    <div>
		<h1>Under me should appear the contents of a CaptureFor component, with id=""capturedContent""</h1>
		");
Output(capturedContent);
Output(@"
    </div>
</body>
</html>");

		}

	}
}
