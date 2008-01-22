namespace Castle.Tools.SQLQueryGenerator.Runtime
{
	public class SQLQuery
	{
		public static Queries.SQLSelectQuery Select(params Model.Field[] fields)
		{
			Clauses.SelectClause selectClause = new Clauses.SelectClause(fields);
			Queries.SQLSelectQuery q = new Queries.SQLSelectQuery(selectClause);
			return q;
		}

		public static implicit operator string(SQLQuery q)
		{
			return q.ToString();
		}
	}
}
namespace Castle.Tools.SQLQueryGenerator.Runtime
{
	public class StringLike
	{
		public static implicit operator string(StringLike stringLike)
		{
			return stringLike.ToString();
		}
	}
}
namespace Castle.Tools.SQLQueryGenerator.Runtime.Clauses
{
	public abstract class AbstractSqlClause : StringLike
	{
		
	}
}
namespace Castle.Tools.SQLQueryGenerator.Runtime.Clauses
{
	public class FromClause : AbstractSqlClause
	{
		readonly Model.Table table;
		readonly System.Collections.Generic.List<Expressions.JoinExpression> joins = new System.Collections.Generic.List<Expressions.JoinExpression>();
		public FromClause(Model.Table table)
		{
			this.table = table;
		}

		public void Join(Expressions.JoinExpression join)
		{
			joins.Add(join);
		}

		public override string ToString()
		{
			System.Text.StringBuilder select = new System.Text.StringBuilder()
				.Append(table);
			foreach (Expressions.JoinExpression join in joins)
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
}
namespace Castle.Tools.SQLQueryGenerator.Runtime.Clauses
{
	public class SelectClause : AbstractSqlClause
	{
		readonly System.Collections.Generic.IEnumerable<Model.Field> fields;

		public SelectClause(System.Collections.Generic.IEnumerable<Model.Field> fields)
		{
			this.fields = fields;
		}

		System.Collections.Generic.IEnumerable<Model.Field> Fields { get { return fields; } }
		public override string ToString()
		{
			System.Text.StringBuilder select = new System.Text.StringBuilder()
				.Append("SELECT ");
			System.Collections.Generic.List<string> fieldNames = new System.Collections.Generic.List<string>();

			foreach (Model.Field field in Fields)
				fieldNames.Add(field.ToString());
			select.Append(string.Join(", ", fieldNames.ToArray()));
			return select.ToString();
		}
	}
}
namespace Castle.Tools.SQLQueryGenerator.Runtime.Clauses
{
	public class WhereClause : AbstractSqlClause
	{
		readonly Expressions.WhereExpression whereExpression;

		public WhereClause(Expressions.WhereExpression whereExpression)
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
namespace Castle.Tools.SQLQueryGenerator.Runtime.Expressions
{
	public class JoinExpression
	{
		readonly Model.Table table;
		readonly WhereExpression on;

		public JoinExpression(Model.Table table, WhereExpression on)
		{
			this.table = table;
			this.on = on;
		}

		public override string ToString()
		{
			return " JOIN " + table + " ON " + on;
		}

	}
}
namespace Castle.Tools.SQLQueryGenerator.Runtime.Expressions
{
	public class WhereExpression
	{
		public WhereExpression(Model.Field field, string operatorSign, object other)
		{
			string otherExpression = other.ToString();
			if (other is string)
				otherExpression = "'" + otherExpression.Replace("'", "''") + "'";
			Expression = "(" + field + operatorSign + otherExpression + ")";
		}

		public readonly string Expression;

		public override string ToString()
		{
			return Expression;
		}
	}
}
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
namespace Castle.Tools.SQLQueryGenerator.Runtime.Queries
{
	public class SQLSelectQuery : SQLQuery
	{
		readonly Clauses.SelectClause selectClause;
		Clauses.FromClause fromClause;
		Clauses.WhereClause whereClause;
		public SQLSelectQuery(Clauses.SelectClause selectClause)
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

		public SQLSelectQuery From(Model.Table table)
		{
			fromClause = new Clauses.FromClause(table);
			return this;
		}

		public SQLSelectQuery Join(Model.Table table, Expressions.WhereExpression on)
		{
			fromClause.Join(new Expressions.JoinExpression(table, on));
			return this;
		}

		public SQLSelectQuery Where(Expressions.WhereExpression where)
		{
			whereClause = new Clauses.WhereClause(where);
			return this;
		}

		public static implicit operator string(SQLSelectQuery q)
		{
			return q.ToString();
		}
	}
}
