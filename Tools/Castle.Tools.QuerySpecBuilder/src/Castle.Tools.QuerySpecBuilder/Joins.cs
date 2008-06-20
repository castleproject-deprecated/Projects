namespace Castle.Tools.QuerySpecBuilder
{
	using Builder.Expressions;

	public static class Joins
	{
		public static JoinExpression Table(string table)
		{
			return new JoinExpression().Table(table);
		}

		public static JoinExpression Left
		{
			get { return new JoinExpression().Left(); }
		}

		public static JoinExpression Right
		{
			get { return new JoinExpression().Right(); }
		}
	}
}