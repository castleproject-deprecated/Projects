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

	public class CreateMemberMethod
	{
		public CreateMemberMethod(string name)
		{
			Method = new CodeMemberMethod {Name = name};
		}

		public static CreateMemberMethod Called(string name)
		{
			return new CreateMemberMethod(name);
		}

		public CreateMemberMethod Returning(CodeTypeReference type)
		{
			Method.ReturnType = type;
			return this;
		}

		public CreateMemberMethod Returning<T>()
		{
			return Returning(new CodeTypeReference(typeof (T)));
		}

		public CreateMemberMethod WithAttributes(MemberAttributes attributes)
		{
			Method.Attributes = attributes;
			return this;
		}

		public CreateMemberMethod WithCustomAttributes(params CodeAttributeDeclaration[] attributes)
		{
			Method.CustomAttributes.AddRange(attributes);
			return this;
		}

		public CreateMemberMethod WithBody(params CodeStatement[] statements)
		{
			Method.Statements.AddRange(statements);
			return this;
		}

		public CreateMemberMethod WithParameters(params CodeParameterDeclarationExpression[] parameters)
		{
			Method.Parameters.AddRange(parameters);
			return this;
		}

		public CreateMemberMethod WithSummaryComment(string comment)
		{
			var summaryStart = new CodeCommentStatement("<summary>");
			var summary = new CodeCommentStatement(comment);
			var summaryEnd = new CodeCommentStatement("</summary>");

			summaryStart.Comment.DocComment = true;
			summary.Comment.DocComment = true;
			summaryEnd.Comment.DocComment = true;

			Method.Comments.Add(summaryStart);
			Method.Comments.Add(summary);
			Method.Comments.Add(summaryEnd);

			return this;
		}

		public CodeMemberMethod Method { get; private set; }
	}
}