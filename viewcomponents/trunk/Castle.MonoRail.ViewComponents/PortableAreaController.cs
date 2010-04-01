using System;
using System.IO;
using System.Linq;
using Castle.MonoRail.Framework;


namespace Castle.MonoRail.ViewComponents
{
	/// <summary>
	/// Base class for implementing a Portable Class
	/// 
	/// </summary>
	/// <remarks>
	/// <list type="bullet">
	/// <item><description> Add view templates in subfolder named same as controller, and mark as "EmbeddedREsource".</description></item>
	/// <item><description> Add layout templates in "layouts" subfolder, and mark as "EmbeddedResource".</description></item>
	/// <item><description> Add individual file in project root, and mark as "EmbeddedResource".</description></item>
	/// </list>
	/// </remarks>
	public class PortableAreaController : SmartDispatcherController
	{
		private string[] resourceNames;
		private string asmName;

		[DefaultAction]
		public void _DefaultAction()
		{
			string filename = asmName + "." + Action;
			var resourceName = resourceNames.FirstOrDefault(rn=> rn.Equals(filename,StringComparison.InvariantCultureIgnoreCase));
			if (resourceName != null)
			{
				string ext = Path.GetExtension(filename);
				this.Response.ContentType = GetContentTypeFromExt(ext);

				Stream contents = this.GetType().Assembly.GetManifestResourceStream(resourceName);
				this.Response.BinaryWrite(contents);
				CancelView();
			}
			else
				DefaultAction();
		}

		/// <summary>
		/// When overridden in derived class, called for each undeclared action.
		/// </summary>
		protected virtual void DefaultAction()
		{
			// do nothng here.
		}


        private string GetContentTypeFromExt(string ext)
		{
			string contentType = null;
#if !MONO
			var reg = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
			if (reg != null)
				contentType = 	reg.GetValue("Content Type") as string;
#endif
			if (contentType == null)
			{
				// put some slow, platform-independent, method here....

				contentType = "text/plain";
			}
			return contentType;
		}

		/// <summary>
		/// Initializes this instance. Implementors
		/// can use this method to perform initialization
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
			var asm = this.GetType().Assembly;
			resourceNames = asm.GetManifestResourceNames();
			asmName = asm.GetName().Name;
			var asminfo = new AssemblySourceInfo(asm, asmName.ToLower());
			if (!this.Context.Services.ViewSourceLoader.AssemblySources.Cast<AssemblySourceInfo>().Any(asi=>asi.AssemblyName==asminfo.AssemblyName))
				this.Context.Services.ViewSourceLoader.AddAssemblySource(asminfo);
		}
	}
}
