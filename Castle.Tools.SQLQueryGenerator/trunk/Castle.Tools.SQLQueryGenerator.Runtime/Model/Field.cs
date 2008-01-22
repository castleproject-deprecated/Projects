namespace Castle.Tools.SQLQueryGenerator.Runtime.Model
{
	public abstract class Field
	{
		public Field(Table table, string name)
		{
			this.table = table;
			this.name = "[" + name.Trim('[', ']') + "]";
		}

		readonly string name;
		readonly Table table;

		public Table Table { get { return table; } }

		public override string ToString()
		{
			return table + "." + name;
		}

		public static Expressions.WhereExpression operator ==(Field field, Field other)
		{
			return new Expressions.WhereExpression(field, "=", other);
		}
		public static Expressions.WhereExpression operator !=(Field field, Field other)
		{
			return new Expressions.WhereExpression(field, "<>", other);
		}

		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.Localizable(false)]
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.Localizable(false)]
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

	}

	public abstract class Field<T> : Field
	{
		public Field(Table table, string name)
			: base(table, name)
		{
		}
		public static Expressions.WhereExpression operator ==(Field<T> field, T other)
		{
			return new Expressions.WhereExpression(field, "=", other);
		}
		public static Expressions.WhereExpression operator !=(Field<T> field, T other)
		{
			return new Expressions.WhereExpression(field, "<>", other);
		}

		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.Localizable(false)]
		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		[System.ComponentModel.Browsable(false)]
		[System.ComponentModel.Localizable(false)]
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}


	}
}
