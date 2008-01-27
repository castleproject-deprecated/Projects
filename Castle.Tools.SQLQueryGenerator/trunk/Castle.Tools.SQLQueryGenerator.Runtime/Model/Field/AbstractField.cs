#region license
// Copyright 2008 Ken Egozi http://www.kenegozi.com/blog
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

namespace Castle.Tools.SQLQueryGenerator.Runtime.Model.Field
{
	using Expressions;

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
		readonly string alias;

		Table.AbstractTable IFormatableField.Table { get { return table; } }

		string IFormatableField.Name { get { return name; } }

		string IFormatableField.Alias { get { return alias; } }

		public override string ToString()
		{
			return Format.Formatting.Format(this);
		}

		public abstract IField As(string alias);

	}

	public class AbstractField<T> : AbstractField, IOperateable<T>
	{
		public AbstractField(Table.AbstractTable table, string name, string alias) : base(table, name, alias)
		{
		}
		public AbstractField(Table.AbstractTable table, string name) : base(table, name)
		{
		}

		public static WhereExpression operator ==(AbstractField<T> field, T other)
		{
			return new WhereExpression(field, " = ", other);
		}
		public static WhereExpression operator ==(AbstractField<T> field, IOperateable<T> other)
		{
			return new WhereExpression(field, " = ", other);
		}
		public static WhereExpression operator !=(AbstractField<T> field, T other)
		{
			return new WhereExpression(field, " <> ", other);
		}
		public static WhereExpression operator !=(AbstractField<T> field, IOperateable<T> other)
		{
			return new WhereExpression(field, " <> ", other);
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