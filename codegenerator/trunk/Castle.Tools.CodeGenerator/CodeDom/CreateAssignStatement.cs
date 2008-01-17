using System.CodeDom;

namespace Castle.Tools.CodeGenerator.CodeDom
{
	public class CreateAssignStatement
	{
		private readonly CodeAssignStatement codeAssignStatement;

		private CreateAssignStatement()
		{
			codeAssignStatement = new CodeAssignStatement();
		}

		public static CreateAssignStatement This(string fieldName)
		{
			CreateAssignStatement createAssignStatement = new CreateAssignStatement();
			createAssignStatement.Statement.Left = new CodeFieldReferenceExpression(
				new CodeThisReferenceExpression(), fieldName);
			return createAssignStatement;
		}

		public CreateAssignStatement EqualsArgument(string argumentName)
		{
			codeAssignStatement.Right = new CodeArgumentReferenceExpression(argumentName);

			return this;
		}

		public CreateAssignStatement EqualsExpression(CodeExpression expression)
		{
			codeAssignStatement.Right = expression;

			return this;
		}

		public CodeAssignStatement Statement
		{
			get { return codeAssignStatement; }
		}
	}
}
