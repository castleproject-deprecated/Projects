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

	public class CreateMemberProperty
	{
		private CreateMemberProperty(Type type)
		{
			Property = new CodeMemberProperty {Type = new CodeTypeReference(type)};
		}

		private CreateMemberProperty(CodeTypeReference type)
		{
			Property = new CodeMemberProperty {Type = type};
		}

		public static CreateMemberProperty OfType<T>()
		{
			return new CreateMemberProperty(typeof (T));
		}

		public static CreateMemberProperty OfType(CodeTypeReference type)
		{
			return new CreateMemberProperty(type);
		}

		public static CreateMemberProperty OfType(string type)
		{
			return OfType(new CodeTypeReference(type));
		}

		public CreateMemberProperty Called(string name)
		{
			Property.Name = name;
			return this;
		}

		public CreateMemberProperty WithAttributes(MemberAttributes attributes)
		{
			Property.Attributes = attributes;
			return this;
		}

		public CreateMemberProperty WithSummaryComment(string comment)
		{
			var summaryStart = new CodeCommentStatement("<summary>");
			var summary = new CodeCommentStatement(comment);
			var summaryEnd = new CodeCommentStatement("</summary>");

			summaryStart.Comment.DocComment = true;
			summary.Comment.DocComment = true;
			summaryEnd.Comment.DocComment = true;

			Property.Comments.Add(summaryStart);
			Property.Comments.Add(summary);
			Property.Comments.Add(summaryEnd);

			return this;
		}

		public CreateMemberProperty WithCustomAttributes(params CodeAttributeDeclaration[] attributes)
		{
			Property.CustomAttributes.AddRange(attributes);
			return this;
		}

		public CreateMemberProperty WithGetter(params CodeStatement[] statements)
		{
			Property.HasGet = true;
			Property.GetStatements.AddRange(statements);
			return this;
		}

		public CreateMemberProperty WithSetter(params CodeStatement[] statements)
		{
			Property.HasSet = true;
			Property.SetStatements.AddRange(statements);

			return this;
		}

		public CodeMemberProperty Property { get; private set; }
	}
}