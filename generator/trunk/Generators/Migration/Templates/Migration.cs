using System;
using QIT.Management.Migrator;

namespace <%= Namespace %>
{
	/// <summary>
	/// <%= HumanName %> to the database
	/// </summary>
	[Migration(<%= Version %>)]
	public class <%= ClassName %> : Migration
	{
		public override void Up()
		{
			Database.AddTable("<%= ClassName %>",
							  new Column("id", typeof(int), ColumnProperties.PrimaryKeyWithIdentity)
							  );
		}
		
		public override void Down()
		{
			Database.RemoveTable("<%= ClassName %>");
		}
		
	}
}
