using System.CodeDom;
using System.Reflection;

namespace Castle.Tools.CodeGenerator.CodeDom
{
	public class CreateTypeDeclaration
	{
		private readonly CodeTypeDeclaration codeTypeDeclaration;

		public CreateTypeDeclaration(string name)
		{
			codeTypeDeclaration = new CodeTypeDeclaration(name);
		}

		public static CreateTypeDeclaration Called(string name)
		{
			return new CreateTypeDeclaration(name);
		}

		public CreateTypeDeclaration AsPartial
		{
			get
			{
				codeTypeDeclaration.IsPartial = true;
				return this;
			}
		}

		public CreateTypeDeclaration WithAttributes(TypeAttributes attributes)
		{
			codeTypeDeclaration.TypeAttributes = attributes;
			return this;
		}

		public CreateTypeDeclaration WithCustomAttributes(CodeAttributeDeclaration attribute, params CodeAttributeDeclaration[] attributes)
		{
			codeTypeDeclaration.CustomAttributes.Add(attribute);
			
			foreach (CodeAttributeDeclaration a in attributes)
				codeTypeDeclaration.CustomAttributes.Add(a);
			
			return this;
		}

		public CodeTypeDeclaration Type
		{
			get { return codeTypeDeclaration; }
		}
	}
}