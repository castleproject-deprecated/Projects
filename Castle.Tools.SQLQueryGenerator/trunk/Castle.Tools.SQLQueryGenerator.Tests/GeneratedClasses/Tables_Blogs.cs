namespace Castle.Tools.SQLQueryGenerator.Tests.GeneratedClasses
{
	public class Tables_Blogs : Runtime.Model.Table.AbstractTable
	{
		public Tables_Blogs(string alias) : base("dbo", "Blogs", alias)
		{
			Id = new Tables_Blogs_Id(this);
			Name = new Tables_Blogs_Name(this);
		}

		public Tables_Blogs() : this(null)
		{
		}

		public readonly Tables_Blogs_Id Id;
		public readonly Tables_Blogs_Name Name;

		public Tables_Blogs As(string alias)
		{
			return new Tables_Blogs(alias);
		}
	}
}