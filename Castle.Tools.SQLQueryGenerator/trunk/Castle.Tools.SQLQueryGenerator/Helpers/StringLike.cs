namespace Castle.Tools.SQLQueryGenerator.Helpers
{
	public class StringLike
	{
		public static implicit operator string(StringLike stringLike)
		{
			return stringLike.ToString();
		}
	}
}