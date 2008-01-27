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

namespace Castle.Tools.SQLQueryGenerator.Runtime.Format
{
	public static class Formatting
	{
		static IFormatter formatter = new SQLServerFormatter();

		public static void SetFormatterTo(IFormatter newFormatter)
		{
			formatter = newFormatter;
		}

		public static string Format(Model.Table.AbstractTable table)
		{
			return formatter.Format(table);
		}

		public static string FormatForFromClause(Model.Table.IFormatableTable table)
		{
			return formatter.FormatForFromClause(table);
		}

		public static string Format(Model.Field.AbstractField field)
		{
			return formatter.Format(field);
		}

		public static string FormatForSelectClause(Model.Field.IFormatableField field)
		{
			return formatter.FormatForSelectClause(field);
		}
	}
}