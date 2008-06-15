using System.Collections;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView.Compiler;

namespace Castle.MonoRail.Views.AspView
{
	public interface IAspViewEngineTestAccess
	{
		Hashtable Compilations { get; }
		void SetViewSourceLoader(IViewSourceLoader viewSourceLoader);
		void SetCompilationContext(ICompilationContext context);
	}
}
