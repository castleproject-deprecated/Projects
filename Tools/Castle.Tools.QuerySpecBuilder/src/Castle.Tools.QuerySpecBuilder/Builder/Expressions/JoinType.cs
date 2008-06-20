namespace Castle.Tools.QuerySpecBuilder.Builder.Expressions
{
	public class JoinType
	{
		readonly static JoinType left = new LeftJoin();
		readonly static JoinType right = new RightJoin();
		readonly static JoinType inner = new InnerJoin();
		public static JoinType Left { get { return left; } }
		public static JoinType Right { get { return right; } }
		public static JoinType Inner { get { return inner; } }

		public class LeftJoin : JoinType
		{
			public override string ToString() { return "LEFT JOIN\t"; }
		}
		public class RightJoin : JoinType
		{
			public override string ToString() { return "RIGHT JOIN\t"; }
		}
		public class InnerJoin : JoinType
		{
			public override string ToString() { return "INNER JOIN\t"; }
		}
	}
}