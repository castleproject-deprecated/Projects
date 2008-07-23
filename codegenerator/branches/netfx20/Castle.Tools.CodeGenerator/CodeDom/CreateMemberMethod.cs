using System.CodeDom;

namespace Castle.Tools.CodeGenerator.CodeDom
{
	public class CreateMemberMethod
	{
		private readonly CodeMemberMethod codeMemberMethod;

		public CreateMemberMethod(string name)
		{
			codeMemberMethod = new CodeMemberMethod();
			codeMemberMethod.Name = name;
		}

		public static CreateMemberMethod Called(string name)
		{
			return new CreateMemberMethod(name);
		}

		public CreateMemberMethod Returning(CodeTypeReference type)
		{
			codeMemberMethod.ReturnType = type;
			return this;
		}

		public CreateMemberMethod Returning<T>()
		{
			return Returning(new CodeTypeReference(typeof (T)));
		}

		public CreateMemberMethod WithAttributes(MemberAttributes attributes)
		{
			codeMemberMethod.Attributes = attributes;
			return this;
		}

		public CreateMemberMethod WithCustomAttributes(params CodeAttributeDeclaration[] attributes)
		{
			codeMemberMethod.CustomAttributes.AddRange(attributes);
			return this;
		}

		public CreateMemberMethod WithBody(params CodeStatement[] statements)
		{
			codeMemberMethod.Statements.AddRange(statements);
			return this;
		}

		public CreateMemberMethod WithParameters(params CodeParameterDeclarationExpression[] parameters)
		{
			codeMemberMethod.Parameters.AddRange(parameters);
			return this;
		}

		public CreateMemberMethod WithSummaryComment(string comment)
		{
			CodeCommentStatement summaryStart = new CodeCommentStatement("<summary>");
			CodeCommentStatement summary = new CodeCommentStatement(comment);
			CodeCommentStatement summaryEnd = new CodeCommentStatement("</summary>");
			
			summaryStart.Comment.DocComment = true;
			summary.Comment.DocComment = true;
			summaryEnd.Comment.DocComment = true;

			codeMemberMethod.Comments.Add(summaryStart);
			codeMemberMethod.Comments.Add(summary);
			codeMemberMethod.Comments.Add(summaryEnd);

			return this;
		}

		public CodeMemberMethod Method
		{
			get { return codeMemberMethod; }
		}
	}
}