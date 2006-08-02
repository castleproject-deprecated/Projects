using System;
using QIT.Management.Migrator;

namespace <%= Namespace %>
{
	/// <summary>
	/// Migration <%= ClassName %>.
	/// </summary>
	[Migration(<%= Version %>)]
	public class <%= ClassName %> : Migration
	{
		public override void Up()
		{
			base.Database.AddTable("<%= ClassName %>", new Column[] { new Column("id", typeof(int), ColumnProperties.PrimaryKeyWithIdentity) } );
		}
		
		public override void Down()
		{
			base.Database.RemoveTable("<%= ClassName %>");
		}
		
	}
}
