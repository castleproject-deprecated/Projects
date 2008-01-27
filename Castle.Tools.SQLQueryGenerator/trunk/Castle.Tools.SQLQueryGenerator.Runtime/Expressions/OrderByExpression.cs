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
	public class OrderByExpression
	{
		readonly Model.Field.AbstractField field;
		bool desc;

		public OrderByExpression(Model.Field.AbstractField field) 
		{
			this.field = field;
		}

		public override string ToString()
		{
			return "\t\t\t\t" + field + (desc ? " DESC" : string.Empty);
		}

		public static explicit operator string (OrderByExpression order)
		{
			return order.ToString();
		}

		public OrderByExpression Desc
		{
			get
			{
				desc = true;
				return this;
			}
		}
	}

	public static class Order
	{
		public static OrderByExpression By(Model.Field.AbstractField field)
		{
			return new OrderByExpression(field);
		}
	}
}
