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

namespace Castle.Tools.SQLQueryGenerator.Runtime
{
	public class SQLQuery
	{
		public static Queries.SQLSelectQuery Select(params Model.Field.IFormatableField[] fields)
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