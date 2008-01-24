namespace Castle.Tools.SQLQueryGenerator.Runtime.Model.Field
{
	public abstract class AbstractField : IAliasedField
	{
		public AbstractField(Table.AbstractTable table, string name, string alias) : this(table, name)
		{
			this.alias = alias;
		}

		public AbstractField(Table.AbstractTable table, string name)
		{
			this.table = table;
			this.name = name;
		}

		readonly Table.AbstractTable table;
		readonly string name;
		string alias = null;

		Table.AbstractTable IFormatableField.Table { get { return table; } }

		string IFormatableField.Name { get { return name; } }

		string IFormatableField.Alias { get { return alias; } }

		public override string ToString()
		{
			return Format.Formatting.Format(this);
		}

		public abstract IField As(string alias);

		public static Expressions.WhereExpression operator ==(AbstractField field, AbstractField other)
		{
			return new Expressions.WhereExpression(field, " = ", other);
		}
		public static Expressions.WhereExpression operator !=(AbstractField field, AbstractField other)
		{
			return new Expressions.WhereExpression(field, " <> ", other);
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

	public class AbstractField<T> : AbstractField
	{
		public AbstractField(Table.AbstractTable table, string name, string alias) : base(table, name, alias)
		{
		}
		public AbstractField(Table.AbstractTable table, string name) : base(table, name)
		{
		}
		public static Expressions.WhereExpression operator ==(AbstractField<T> field, T other)
		{
			return new Expressions.WhereExpression(field, " = ", other);
		}
		public static Expressions.WhereExpression operator !=(AbstractField<T> field, T other)
		{
			return new Expressions.WhereExpression(field, " <> ", other);
		}

		public override IField As(string alias)
		{
			IFormatableField thisField = this;
			return new AbstractField<T>(thisField.Table, thisField.Name, alias);
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

class B : System.ICloneable
{

	#region ICloneable Members

	public object Clone()
	{
		throw new System.Exception("The method or operation is not implemented.");
	}

	#endregion
}