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
	using System.CodeDom;

	public class CreateAssignStatement
	{
		private CreateAssignStatement()
		{
			Statement = new CodeAssignStatement();
		}

		public CodeAssignStatement Statement { get; private set; }
		
		public static CreateAssignStatement This(string fieldName)
		{
			return new CreateAssignStatement
			{
				Statement =
					{
						Left = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), fieldName)
					}
			};
		}

		public CreateAssignStatement EqualsArgument(string argumentName)
		{
			Statement.Right = new CodeArgumentReferenceExpression(argumentName);

			return this;
		}

		public CreateAssignStatement EqualsExpression(CodeExpression expression)
		{
			Statement.Right = expression;

			return this;
		}
	}
}