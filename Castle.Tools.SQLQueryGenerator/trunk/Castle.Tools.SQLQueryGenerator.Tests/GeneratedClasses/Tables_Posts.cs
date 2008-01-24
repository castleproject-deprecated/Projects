namespace Castle.Tools.SQLQueryGenerator.Tests.GeneratedClasses
{
	public class Tables_Posts : Runtime.Model.Table.AbstractTable
	{
		public Tables_Posts(string alias) : base("dbo", "Posts", alias)
		{
			Id = new Tables_Posts_Id(this);
			BlogId = new Tables_Posts_BlogId(this);
			Title = new Tables_Posts_Title(this);
		}

		public Tables_Posts() : base("dbo", "Posts")
		{
			Id = new Tables_Posts_Id(this);
			BlogId = new Tables_Posts_BlogId(this);
			Title = new Tables_Posts_Title(this);
		}

		public readonly Tables_Posts_Id Id;
		public readonly Tables_Posts_BlogId BlogId;
		public readonly Tables_Posts_Title Title;

		public Tables_Posts As(string alias)
		{
			return new Tables_Posts(alias);
		}
	}
}
