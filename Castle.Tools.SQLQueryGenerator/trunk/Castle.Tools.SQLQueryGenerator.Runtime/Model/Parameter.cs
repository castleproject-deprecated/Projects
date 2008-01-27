namespace Castle.Tools.SQLQueryGenerator.Runtime.Model
{
	public class Parameter<T> : IOperateable<T>
	{
		readonly string name;

		public Parameter(string name)
		{
			this.name = name;
		}

		public override string ToString()
		{
			return "@" + name;
		}

		public static implicit operator string(Parameter<T> parameter)
		{
			return parameter.ToString();
		}
	}
}