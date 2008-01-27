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

namespace Castle.Tools.SQLQueryGenerator.Tests.GeneratedClasses
{
	public class Tables_ForumMessages : Runtime.Model.Table.AbstractTable
	{
		public Tables_ForumMessages(string alias) : base("dbo", "ForumMessages", alias)
		{
			Id = new Tables_ForumMessages_Id(this);
			ParentId = new Tables_ForumMessages_ParentId(this);
			Content = new Tables_ForumMessages_Content(this);
		}

		public Tables_ForumMessages() : this(null)
		{
		}

		public readonly Tables_ForumMessages_Id Id;
		public readonly Tables_ForumMessages_ParentId ParentId;
		public readonly Tables_ForumMessages_Content Content;

		public Tables_ForumMessages As(string alias)
		{
			return new Tables_ForumMessages(alias);
		}
	}
}
