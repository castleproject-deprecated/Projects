using System;
using System.CodeDom;

namespace Castle.Tools.CodeGenerator.CodeDom
{
	public class CreateMemberField
	{
		private readonly CodeMemberField codeMemberField;

		private CreateMemberField(Type type, string name)
		{
			codeMemberField = new CodeMemberField(type, name);
		}

		private CreateMemberField(string type, string name)
		{
			codeMemberField = new CodeMemberField(type, name);
		}

		public static CreateMemberField WithNameAndType<T>(string name)
		{
			return new CreateMemberField(typeof(T), name);
		}

		public static CreateMemberField WithNameAndType(string name, string type)
		{
			return new CreateMemberField(type, name);
		}

		public CreateMemberField WithAttributes(MemberAttributes attributes)
		{
			codeMemberField.Attributes = attributes;
			return this;
		}

		public CreateMemberField WithInitialValue(object value)
		{
			codeMemberField.InitExpression = new CodePrimitiveExpression(value);
			return this;
		}

		public CodeMemberField Field
		{
			get { return codeMemberField; }
		}
	}
}
