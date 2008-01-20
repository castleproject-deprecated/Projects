using System;
using System.Collections.Generic;
using System.Text;

namespace SQLQueryGenerator
{
	public class Class1
	{
		public void a()
		{
			SQLQuery q = SQLQuery
				.Select(SQL.Blogs.Id, SQL.Blogs.Name)
				.From(SQL.Blogs)
					.Join(SQL.Posts, Join.On(SQL.Blogs.Id == SQL.Posts.BlogId))
				.Where(SQL.Blogs.Name != "Ken's blog");

			Console.WriteLine(q);
		}
	}

	public static class Join
	{
		public static WhereExpression On(WhereExpression whereExpression)
		{
			return whereExpression;
		}
	}

	public class Tables_Blogs : TableSpecification
	{
		public Tables_Blogs()
			: base("Blogs")
		{
			id = new Tables_Blogs_Id(this);
			name = new Tables_Blogs_Name(this);
		}

		readonly Tables_Blogs_Id id;
		readonly Tables_Blogs_Name name;

		public Tables_Blogs_Id Id { get { return id; } }
		public Tables_Blogs_Name Name { get { return name; } }
	}
	public class Tables_Posts : TableSpecification
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

	public class Tables_Blogs_Id : FieldSpecification<int>
	{
		public Tables_Blogs_Id(TableSpecification table)
			: base(table, "Id")
		{
		}
	}
	public class Tables_Blogs_Name : FieldSpecification<string>
	{
		public Tables_Blogs_Name(TableSpecification table)
			: base(table, "Name")
		{
		}
	}

	public class Tables_Posts_Id : FieldSpecification<int>
	{
		public Tables_Posts_Id(TableSpecification table)
			: base(table, "Id")
		{
		}
	}
	public class Tables_Posts_BlogId : FieldSpecification<int>
	{
		public Tables_Posts_BlogId(TableSpecification table)
			: base(table, "BlogId")
		{
		}
	}
	public class Tables_Posts_Title : FieldSpecification<string>
	{
		public Tables_Posts_Title(TableSpecification table)
			: base(table, "Title")
		{
		}
	}


	public static class SQL
	{
		public static Tables_Blogs Blogs = new Tables_Blogs();
		public static Tables_Posts Posts = new Tables_Posts();
	}


}
