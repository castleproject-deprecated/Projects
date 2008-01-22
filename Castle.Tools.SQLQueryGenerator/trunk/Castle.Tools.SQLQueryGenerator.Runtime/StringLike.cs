namespace Castle.Tools.SQLQueryGenerator.Runtime
{
	public class StringLike
	{
		public static implicit operator string(StringLike stringLike)
		{
			return stringLike.ToString();
		}
	}
}
