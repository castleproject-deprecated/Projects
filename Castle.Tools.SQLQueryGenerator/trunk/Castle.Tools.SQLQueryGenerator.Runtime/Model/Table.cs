namespace Castle.Tools.SQLQueryGenerator.Runtime.Model
{
	public class Table
	{
		public Table(string name)
		{
			this.name = name;
		}

		readonly string name;

		public override string ToString()
		{
			return name;
		}

	}
}
