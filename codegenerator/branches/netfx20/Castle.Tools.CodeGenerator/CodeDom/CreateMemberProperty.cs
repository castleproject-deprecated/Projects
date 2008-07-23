using System;
using System.CodeDom;

namespace Castle.Tools.CodeGenerator.CodeDom
{
	public class CreateMemberProperty
	{
		private readonly CodeMemberProperty codeMemberProperty;

		private CreateMemberProperty(Type type)
		{
			codeMemberProperty = new CodeMemberProperty();
			codeMemberProperty.Type = new CodeTypeReference(type);
		}
		
		private CreateMemberProperty(CodeTypeReference type)
		{
			codeMemberProperty = new CodeMemberProperty();
			codeMemberProperty.Type = type;
		}

		public static CreateMemberProperty OfType<T>()
		{
			return new CreateMemberProperty(typeof(T));
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
			codeMemberProperty.Name = name;
			return this;
		}

		public CreateMemberProperty WithAttributes(MemberAttributes attributes)
		{
			codeMemberProperty.Attributes = attributes;
			return this;
		}

		public CreateMemberProperty WithSummaryComment(string comment)
		{	
			CodeCommentStatement summaryStart = new CodeCommentStatement("<summary>");
			CodeCommentStatement summary = new CodeCommentStatement(comment);
			CodeCommentStatement summaryEnd = new CodeCommentStatement("</summary>");
			
			summaryStart.Comment.DocComment = true;
			summary.Comment.DocComment = true;
			summaryEnd.Comment.DocComment = true;

			codeMemberProperty.Comments.Add(summaryStart);
			codeMemberProperty.Comments.Add(summary);
			codeMemberProperty.Comments.Add(summaryEnd);

			return this;
		}

		public CreateMemberProperty WithCustomAttributes(params CodeAttributeDeclaration[] attributes)
		{
			codeMemberProperty.CustomAttributes.AddRange(attributes);
			return this;
		}

		public CreateMemberProperty WithGetter(params CodeStatement[] statements)
		{
			codeMemberProperty.HasGet = true;
			codeMemberProperty.GetStatements.AddRange(statements);
			return this;			
		}

		public CreateMemberProperty WithSetter(params CodeStatement[] statements)
		{
			codeMemberProperty.HasSet = true;
			codeMemberProperty.SetStatements.AddRange(statements);

			return this;
		}

		public CodeMemberProperty Property
		{
			get { return codeMemberProperty; }
		}
	}
}