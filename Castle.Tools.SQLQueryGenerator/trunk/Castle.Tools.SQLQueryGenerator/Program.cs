using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Castle.Tools.SQLQueryGenerator.DatabaseMetadataProviders;
using Castle.Tools.SQLQueryGenerator.Descriptors;

namespace Castle.Tools.SQLQueryGenerator
{
	class Program
	{
		static void Main(string[] args)
		{
			string ns = "Generated.SQLQuery";
			string server = "(local)";
			string db = "MusicGlue";
			string connectionString = null;

			foreach (string arg in args)
			{
				string[] parts = arg.Split(':');
				switch (parts[0].ToLowerInvariant())
				{
					case "/server":
						server = parts[1];
						break;

					case "/ns":
						ns = parts[1];
						break;

					case "/db":
						db = parts[1];
						break;

					case "/connectionstring":
						connectionString = parts[1];
						break;
				}
			}

			if (db == null)
				Error("db parameter must be specified");

			if (connectionString == null)
				connectionString = GetConnectionString(server, db);

			IDatabaseMetadataProvider metadataProvider = new SQL2005MetadataProvider(db);


			StringBuilder SQLQueryFile = new StringBuilder();
			
			IEnumerable<DbPropertyMetadata> metadata = metadataProvider.ExtractMetadata();

			MetadataProcessor processor = new MetadataProcessor();

			ICollection<TableDescriptor> tables = processor.GetTableDescriptorsFrom(metadata).Values;

			foreach (TableDescriptor table in tables)
			{
				SQLQueryFile.AppendLine(GetClassesFrom(table));
			}

			System.Console.WriteLine(SQLQueryFile.ToString());
		}

		static string GetClassesFrom(TableDescriptor table)
		{
			StringBuilder tableClasses = new StringBuilder()
				.AppendFormat(@"	public class Tables_{0} : Runtime.Model.Table
	{{
		public Tables_Blogs() : base(""{1}"")
		{{
{2}
		}}
", NameToClassName(table.Name), table.Name, GetFieldInitializersFor(table));	

			return tableClasses.ToString();
		}

		static string GetFieldInitializersFor(TableDescriptor table)
		{
			StringBuilder initializers = new StringBuilder();
			foreach (DbPropertyMetadata property in table.Properties)
			{
				initializers.AppendFormat(
@"			{0} = new {1}(this);", property.Column, property.GeneratedClassName);
			}

			return initializers.ToString();
		}

		static string NameToClassName(string name)
		{
			name = name
				.Replace("[","")
				.Replace("]","")
				.Replace('.','_');
			
			return name;
		}

		static void Error(string message)
		{
			Console.WriteLine(message);
			Environment.Exit(1);
		}

		static  string GetConnectionString(string server, string db)
		{
			return string.Format("Server:{0}; Initial Catalog:{1}; Integrated Security=SSPI;",
				server, db);
		}

		static string GetRuntimeFile(string ns)
		{
			string runtimeFilePath = "Castle.Tools.SQLQueryGenerator.Runtime.cs";
			string runtime;
			using (Stream runtimeFile = Assembly.GetExecutingAssembly().GetManifestResourceStream(runtimeFilePath))
			using (StreamReader reader = new StreamReader(runtimeFile))
			{
				runtime = reader.ReadToEnd();
			}

			return runtime.Replace("namespace Castle.Tools.SQLQueryGenerator", "namespace " + ns);

		}
	}
}
