using System;
using System.CodeDom;

namespace Castle.Tools.CodeGenerator.CodeDom
{
	public class CreateAttributeDeclaration
	{
		private readonly CodeAttributeDeclaration codeAttributeDeclaration;

		private CreateAttributeDeclaration(Type type)
		{
			codeAttributeDeclaration = new CodeAttributeDeclaration(new CodeTypeReference(type));
		}

		public static CreateAttributeDeclaration ForAttributeType<T>()
		{
			return new CreateAttributeDeclaration(typeof(T));
		}

		public CreateAttributeDeclaration WithArgument(object value)
		{
			codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression(value)));

			return this;
		}

		public CodeAttributeDeclaration Attribute
		{
			get { return codeAttributeDeclaration; }
		}
	}
}
