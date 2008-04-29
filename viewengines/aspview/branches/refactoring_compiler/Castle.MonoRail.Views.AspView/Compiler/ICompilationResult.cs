namespace Castle.MonoRail.Views.AspView.Compiler
{
	public interface ICompilationResult<T> : ICompilationResult
	{
		T Product { get; }
	}
	public interface ICompilationResult { }
}
