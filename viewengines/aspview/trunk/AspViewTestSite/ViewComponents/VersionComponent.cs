#region Copyright © 2008 Andre Loker. All rights reserved.
/* 
  $Id$

  This file is subject to the licence agreement found in LICENSE.TXT. If you
  did not receive the licence agreement together with this file, send an e-mail 
  to mail@andreloker.de.
*/
#endregion

using System;
using Castle.MonoRail.Framework;

namespace AspViewTestSite.ViewComponents {
	/// <summary>
	/// A simple test component that displays the content of a 
	/// <see cref="Version"/> instance.
	/// </summary>
	public class VersionComponent : ViewComponent{
		private Version version;

		[ViewComponentParam]
		public Version Version 
		{
			get { return version; }
			set { version = value; }
		}


		public override void Render()
		{
			if (version != null)
			{
				RenderText(string.Format("Major: {0} - Minor: {1} - Build {2} - Revision {3}",
					version.Major, version.Minor, version.Build, version.Revision));
			}
			else 
			{
				RenderText("Version == null");
			}
		}
	}
}

