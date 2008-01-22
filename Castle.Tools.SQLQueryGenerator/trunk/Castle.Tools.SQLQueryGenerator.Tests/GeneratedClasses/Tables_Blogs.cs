namespace Castle.Tools.SQLQueryGenerator.Tests.GeneratedClasses
{
	public class Tables_Blogs : Runtime.Model.Table.AbstractTable
	{
		public Tables_Blogs()
			: base("dbo", "Blogs")
		{
			Id = new Tables_Blogs_Id(this);
			name = new Tables_Blogs_Name(this);
		}

		public readonly Tables_Blogs_Id Id;
		readonly Tables_Blogs_Name name;

		//public Tables_Blogs_Id Id { get { return id; } }
		public Tables_Blogs_Name Name { get { return name; } }

	}
}