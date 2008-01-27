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

namespace Castle.Tools.SQLQueryGenerator.Runtime.Expressions
{
	public class WhereExpression
	{
		public WhereExpression(string expression)
		{
			Expression = "(" + expression + ")";
		}

		public WhereExpression(Model.Field.AbstractField field, string operatorSign, object other)
		{
			string otherExpression = other.ToString();
			if (other is string)
				otherExpression = "N'" + otherExpression.Replace("'", "''") + "'";
			Expression = "(" + field + operatorSign + otherExpression + ")";
		}

		public readonly string Expression;

		public override string ToString()
		{
			return Expression;
		}

		public virtual string ToOnString()
		{
			return "\t\t\t\t\t" + Expression;
		}

		public virtual string ToWhereString()
		{
			return "\t\t\t\t" + Expression;
		}

		public static WhereExpression operator |(WhereExpression where, WhereExpression other)
		{
			return new ComplexWhereExpression(where, " OR ", other);
		}

		public static WhereExpression operator &(WhereExpression where, WhereExpression other)
		{
			return new ComplexWhereExpression(where, " AND ", other);
		}
	
		public static WhereExpression operator !(WhereExpression where)
		{
			return new WhereExpression("NOT " + where);
		}
	}

	public class ComplexWhereExpression : WhereExpression
	{
		public ComplexWhereExpression(WhereExpression where, string operatorSign, WhereExpression other) : base(where + operatorSign + other) 
		{
		}
	}
}
