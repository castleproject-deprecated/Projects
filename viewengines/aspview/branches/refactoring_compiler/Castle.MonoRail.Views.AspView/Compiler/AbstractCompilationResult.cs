namespace Castle.MonoRail.Views.AspView.Compiler
{
	public abstract class AbstractCompilationResult<T> : ICompilationResult<T>
	{
		readonly T product;
		protected AbstractCompilationResult(T product)
		{
			this.product = product;
		}

		public T Product
		{
			get { return product; }
		}
	}
}
