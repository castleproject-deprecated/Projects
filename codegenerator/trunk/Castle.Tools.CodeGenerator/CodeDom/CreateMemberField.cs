// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.Tools.CodeGenerator.CodeDom
{
	using System;
	using System.CodeDom;

	public class CreateMemberField
	{
		private CreateMemberField(Type type, string name)
		{
			Field = new CodeMemberField(type, name);
		}

		private CreateMemberField(string type, string name)
		{
			Field = new CodeMemberField(type, name);
		}

		public static CreateMemberField WithNameAndType<T>(string name)
		{
			return new CreateMemberField(typeof (T), name);
		}

		public static CreateMemberField WithNameAndType(string name, string type)
		{
			return new CreateMemberField(type, name);
		}

		public CreateMemberField WithAttributes(MemberAttributes attributes)
		{
			Field.Attributes = attributes;
			return this;
		}

		public CreateMemberField WithInitialValue(object value)
		{
			Field.InitExpression = new CodePrimitiveExpression(value);
			return this;
		}

		public CodeMemberField Field { get; private set; }
	}
}