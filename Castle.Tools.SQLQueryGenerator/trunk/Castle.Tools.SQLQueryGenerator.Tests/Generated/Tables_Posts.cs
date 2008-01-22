namespace Castle.Tools.SQLQueryGenerator.Tests.Generated
{
	public class Tables_Posts : Runtime.Model.Table
	{
		public Tables_Posts()
			: base("Posts")
		{
			id = new Tables_Posts_Id(this);
			blogId = new Tables_Posts_BlogId(this);
			title = new Tables_Posts_Title(this);
		}

		readonly Tables_Posts_Id id;
		readonly Tables_Posts_BlogId blogId;
		readonly Tables_Posts_Title title;

		public Tables_Posts_Id Id { get { return id; } }
		public Tables_Posts_BlogId BlogId { get { return blogId; } }
		public Tables_Posts_Title Title { get { return title; } }
	}
}
