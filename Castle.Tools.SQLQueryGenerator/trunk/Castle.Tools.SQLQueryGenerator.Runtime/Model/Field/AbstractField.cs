namespace Castle.Tools.SQLQueryGenerator.Runtime.Model.Field
{
	public abstract class AbstractField : IFormatableField
	{
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

		public INonAliasedField As(string alias)
		{
			this.alias = alias;
			return this;
		}

		public override string ToString()
		{
			return Format.Formatting.Format(this);
		}

		public static Expressions.WhereExpression operator ==(AbstractField field, AbstractField other)
		{
			return new Expressions.WhereExpression(field, "=", other);
		}
		public static Expressions.WhereExpression operator !=(AbstractField field, AbstractField other)
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

	public abstract class AbstractField<T> : AbstractField
	{
		public AbstractField(Table.AbstractTable table, string name)
			: base(table, name)
		{
		}
		public static Expressions.WhereExpression operator ==(AbstractField<T> field, T other)
		{
			return new Expressions.WhereExpression(field, "=", other);
		}
		public static Expressions.WhereExpression operator !=(AbstractField<T> field, T other)
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
