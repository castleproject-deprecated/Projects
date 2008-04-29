namespace Castle.MonoRail.Views.AspView.Compiler
{
	using System.Reflection;

	public class InMemoryCompilationResult : AbstractCompilationResult<Assembly>
	{
		public InMemoryCompilationResult(Assembly product) : base(product)
		{
		}
	}
}
