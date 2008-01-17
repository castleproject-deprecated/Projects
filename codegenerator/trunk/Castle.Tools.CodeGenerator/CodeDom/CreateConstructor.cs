using System.CodeDom;

namespace Castle.Tools.CodeGenerator.CodeDom
{
	public class CreateConstructor
	{
		private readonly CodeConstructor codeConstructor;

		private CreateConstructor(CodeParameterDeclarationExpression[] parameters)
		{
			codeConstructor = new CodeConstructor();
			codeConstructor.Parameters.AddRange(parameters);
		}

		public static CreateConstructor WithParameters(params CodeParameterDeclarationExpression[] parameters)
		{
			CreateConstructor createConstructor = new CreateConstructor(parameters);

			return createConstructor;
		}

		public CreateConstructor WithAttributes(MemberAttributes attributes)
		{
			codeConstructor.Attributes = attributes;
			return this;
		}

		public CreateConstructor WithBaseConstructorArgs(params CodeExpression[] parameters)
		{
			codeConstructor.BaseConstructorArgs.AddRange(parameters);
			return this;
		}

		public CreateConstructor WithBody(params CodeStatement[] statements)
		{
			codeConstructor.Statements.AddRange(statements);
			return this;
		}

		public CodeConstructor Constructor
		{
			get { return codeConstructor; }
		}
	}
}
