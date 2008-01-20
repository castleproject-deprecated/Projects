namespace GeneratedSQLQuery
{
	public abstract class TableSpecification : StringLike
	{
		public TableSpecification(string name)
		{
			this.name = name;
		}

		readonly string name;

		public override string ToString()
		{
			return name;
		}

	}
	public abstract class FieldSpecification<T> : FieldSpecification
	{
		public FieldSpecification(TableSpecification table, string name)
			: base(table, name)
		{
		}
		public static WhereExpression operator ==(FieldSpecification<T> field, T other)
		{
			return new WhereExpression(field, "=", other);
		}
		public static WhereExpression operator !=(FieldSpecification<T> field, T other)
		{
			return new WhereExpression(field, "<>", other);
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
	public abstract class FieldSpecification : StringLike
	{
		public FieldSpecification(TableSpecification table, string name)
		{
			this.table = table;
			this.name = "[" + name.Trim('[', ']') + "]";
		}

		readonly string name;
		readonly TableSpecification table;

		public TableSpecification Table { get { return table; } }

		public override string ToString()
		{
			return table + "." + name;
		}

		public static WhereExpression operator ==(FieldSpecification field, FieldSpecification other)
		{
			return new WhereExpression(field, "=", other);
		}
		public static WhereExpression operator !=(FieldSpecification field, FieldSpecification other)
		{
			return new WhereExpression(field, "<>", other);
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

	public class WhereExpression
	{
		public WhereExpression(FieldSpecification field, string operatorSign, object other)
		{
			string otherExpression = other.ToString();
			if (other is string)
				otherExpression = "'" + otherExpression.Replace("'", "''") + "'";
			Expression = "(" + field + operatorSign + otherExpression + ")";
		}

		public string Expression;

		public override string ToString()
		{
			return Expression;
		}
	}

	public class SQLSelectQuery : SQLQuery
	{
		readonly SelectClause selectClause;
		FromClause fromClause;
		WhereClause whereClause;
		public SQLSelectQuery(SelectClause selectClause)
		{
			this.selectClause = selectClause;
		}

		public override string ToString()
		{
			System.Text.StringBuilder select = new System.Text.StringBuilder()
				.AppendLine(selectClause);

			if (fromClause != null)
				select.AppendLine(fromClause);

			if (whereClause != null)
				select.AppendLine(whereClause);

			return select.ToString();
		}

		public SQLSelectQuery From(TableSpecification table)
		{
			fromClause = new FromClause(table);
			return this;
		}

		public SQLSelectQuery Join(TableSpecification table, WhereExpression on)
		{
			fromClause.Join(new JoinExpression(table, on));
			return this;
		}

		public SQLSelectQuery Where(WhereExpression where)
		{
			whereClause = new WhereClause(where);
			return this;
		}

		public static implicit operator string(SQLSelectQuery q)
		{
			return q.ToString();
		}
	}

	public class SQLQuery
	{
		public static SQLSelectQuery Select(params FieldSpecification[] fields)
		{
			SelectClause selectClause = new SelectClause(fields);
			SQLSelectQuery q = new SQLSelectQuery(selectClause);
			return q;
		}

		public static implicit operator string(SQLQuery q)
		{
			return q.ToString();
		}
	}

	public abstract class AbstractSqlClause : StringLike
	{
	}


	public class SelectClause : AbstractSqlClause
	{
		readonly System.Collections.Generic.IEnumerable<FieldSpecification> fields;

		public SelectClause(System.Collections.Generic.IEnumerable<FieldSpecification> fields)
		{
			this.fields = fields;
		}

		System.Collections.Generic.IEnumerable<FieldSpecification> Fields { get { return fields; } }
		public override string ToString()
		{
			System.Text.StringBuilder select = new System.Text.StringBuilder()
				.Append("SELECT ");
			System.Collections.Generic.List<string> fieldNames = new System.Collections.Generic.List<string>();

			foreach (FieldSpecification field in Fields)
				fieldNames.Add(field.ToString());
			select.Append(string.Join(", ", fieldNames.ToArray()));
			return select.ToString();
		}
	}

	public class JoinExpression
	{
		readonly TableSpecification table;
		readonly WhereExpression on;
		public JoinExpression(TableSpecification table, WhereExpression on)
		{
			this.table = table;
			this.on = on;
		}

		public override string ToString()
		{
			return " JOIN " + table + " ON " + on;
		}

	}

	public class FromClause : AbstractSqlClause
	{
		readonly TableSpecification table;
		readonly System.Collections.Generic.List<JoinExpression> joins = new System.Collections.Generic.List<JoinExpression>();
		public FromClause(TableSpecification table)
		{
			this.table = table;
		}

		public void Join(JoinExpression join)
		{
			joins.Add(join);
		}

		public override string ToString()
		{
			System.Text.StringBuilder select = new System.Text.StringBuilder()
				.Append(table);
			foreach (JoinExpression join in joins)
			{
				select
					.Insert(0, '(')
					.Append(join)
					.Append(')');
			}

			select.Insert(0, "FROM ");

			return select.ToString();
		}
	}

	public class WhereClause : AbstractSqlClause
	{
		readonly WhereExpression whereExpression;

		public WhereClause(WhereExpression whereExpression)
		{
			this.whereExpression = whereExpression;
		}

		public override string ToString()
		{
			System.Text.StringBuilder where = new System.Text.StringBuilder()
				.Append("WHERE ")
				.Append(whereExpression);

			return where.ToString();
		}
	}
}