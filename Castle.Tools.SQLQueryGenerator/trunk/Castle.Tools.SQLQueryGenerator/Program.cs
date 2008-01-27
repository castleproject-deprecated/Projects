using System;
using System.Collections.Generic;
using System.Text;
using Castle.Tools.SQLQueryGenerator.DatabaseMetadataProviders;
using Castle.Tools.SQLQueryGenerator.Descriptors;

namespace Castle.Tools.SQLQueryGenerator
{
	class SQLQueryGenerator : ApplicationException
	{
		public SQLQueryGenerator(string message) : base(message)
		{
		}
	}
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				string ns = "Generated.SQLQuery";
				string server = null;
				string db = "(local)";
				string connectionString = null;
				string userId = null;
				string password = null;
				bool withSchema = false;

				foreach (string arg in args)
				{
					string[] parts = arg.Split(':');
					switch (parts[0].ToLowerInvariant())
					{
						case "/ns":
							ns = parts[1];
							break;

						case "/server":
							server = parts[1];
							break;

						case "/db":
							db = parts[1];
							break;

						case "/userid":
							userId = parts[1];
							break;

						case "/password":
							password = parts[1];
							break;

						case "/connectionstring":
							connectionString = parts[1];
							break;

						case "/withschema":
							withSchema = bool.Parse(parts[1]);
							break;
					}
				}

				IDatabaseMetadataProvider metadataProvider = null;

				if (connectionString != null)
					metadataProvider = new SQL2005MetadataProvider(connectionString);
				else if (db != null)
				{
					if (userId != null)
						metadataProvider = new SQL2005MetadataProvider(db, server, userId, password);
					else
						metadataProvider = new SQL2005MetadataProvider(db, server);
				}

				if (metadataProvider == null)
						throw new SQLQueryGenerator("You must provide either /db or /connectionstring");

				StringBuilder SQLQueryFile = new StringBuilder();

				IEnumerable<DbPropertyMetadata> metadata = metadataProvider.ExtractMetadata();

				MetadataProcessor processor = new MetadataProcessor();

				ICollection<TableDescriptor> tables = processor.GetTableDescriptorsFrom(metadata, withSchema).Values;

				SQLQueryFile.AppendLine(GetSQLClassesFrom(tables));

				foreach (TableDescriptor table in tables)
				{
					SQLQueryFile.AppendLine(GetClassesFrom(table));
				}

				string sqlQueryFile = WrapInNamespace(ns, SQLQueryFile);

				Console.WriteLine(sqlQueryFile);
			}
			catch (SQLQueryGenerator ex)
			{
				Error(ex.Message);
			}
		}

		static string WrapInNamespace(string ns, StringBuilder content)
		{
			content.Insert(0, string.Format(@"namespace {0}
{{
", ns));
			content.AppendLine("}");
			return content.ToString();
		}

		static string GetSQLClassesFrom(IEnumerable<TableDescriptor> tables)
		{
			StringBuilder sqlClass = new StringBuilder().Append(
@"	public static class SQL
	{{
");
			foreach (TableDescriptor table in tables)
			{
				sqlClass.AppendFormat(
@"		public static {1} {0} = new {1}();
"
					, table.Name, table.ClassName);
			}
			sqlClass.Append(@"	}}");

			return sqlClass.ToString();
		}

		static string GetClassesFrom(TableDescriptor table)
		{
			StringBuilder classes = new StringBuilder()
				.AppendLine(GetTableClassFrom(table));
			foreach (DbPropertyMetadata property in table.Properties)
			{
				classes.AppendLine(GetPropertyClassFrom(table, property));
			}

			return classes.ToString();
		}

		static string GetPropertyClassFrom(TableDescriptor table, DbPropertyMetadata property)
		{
			return string.Format(
@"	public class {0}_{1} : Castle.Tools.SQLQueryGenerator.Runtime.Model.Field.AbstractField<{2}>
	{{
		public {0}_{1}(Castle.Tools.SQLQueryGenerator.Runtime.Model.Table.AbstractTable table)
			: base(table, ""{1}"")
		{{
		}}
	}}",
			table.ClassName, property.Column, property.Type.FullName);
		}

		static string GetTableClassFrom(TableDescriptor table)
		{
			return string.Format(
@"	public class {2} : Castle.Tools.SQLQueryGenerator.Runtime.Model.Table.AbstractTable
	{{
		public {2}(string alias) : base(""{0}"", ""{1}"", alias)
		{{
{3}
		}}

		public {2}() : this(null)
		{{
		}}

{4}

		public {2} As(string alias)
		{{
			return new {2}(alias);
		}}
	}}",
				table.Schema, table.Name, table.ClassName, GetFieldInitializersFor(table), GetFieldDeclerationsFor(table));
		}

		static string GetFieldInitializersFor(TableDescriptor table)
		{
			List<string> initializers = new List<string>(table.Properties.Count);
			foreach (DbPropertyMetadata property in table.Properties)
			{
				initializers.Add(string.Format(
					@"			{0} = new {1}_{2}(this);",
					property.Column, table.ClassName, property.Column));
			}

			return string.Join(Environment.NewLine, initializers.ToArray());
		}

		static string GetFieldDeclerationsFor(TableDescriptor table)
		{
			List<string> declerations = new List<string>(table.Properties.Count);
			foreach (DbPropertyMetadata property in table.Properties)
			{
				declerations.Add(string.Format(
					@"		public readonly {1}_{2} {0};",
					property.Column, table.ClassName, property.Column));
			}

			return string.Join(Environment.NewLine, declerations.ToArray());
		}




		static void Error(string message)
		{
			Console.WriteLine(message);
			Environment.Exit(1);
		}

	}
}

