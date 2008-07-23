using System;
using System.CodeDom;
using Castle.Tools.CodeGenerator.Model;
using Castle.Tools.CodeGenerator.Model.TreeNodes;
using Castle.Tools.CodeGenerator.Services.Generators;

namespace Castle.Tools.CodeGenerator.Services.Generators
{
	public class WizardStepMapGenerator : AbstractGenerator
	{
		public WizardStepMapGenerator(ILogger logger, ISourceGenerator source, INamingService naming, string targetNamespace,
		                              string serviceType) : base(logger, source, naming, targetNamespace, serviceType)
		{
		}

		public override void Visit(WizardControllerTreeNode node)
		{
			CodeTypeDeclaration type = GenerateTypeDeclaration(_namespace, node.PathNoSlashes + _naming.ToWizardStepWrapperName(node.Name));

			foreach (string wizardStepPage in node.WizardStepPages)
			{
				CodeMemberMethod method = new CodeMemberMethod();
				method.Name = wizardStepPage;
				method.ReturnType = _source[typeof(IControllerActionReference)];
				method.Attributes = MemberAttributes.Public;
				method.CustomAttributes.Add(_source.DebuggerAttribute);
				method.Statements.Add(new CodeMethodReturnStatement(CreateNewWizardStepReference(node, wizardStepPage)));
				type.Members.Add(method);
			}

			base.Visit(node);
		}

		protected CodeExpression CreateNewWizardStepReference(WizardControllerTreeNode node, string wizardStepPage)
		{
			CodeExpression createMethodSignature = new CodeObjectCreateExpression(
				_source[typeof (MethodSignature)],
				new CodeExpression[]
					{
						new CodeTypeOfExpression(node.FullName),
						new CodePrimitiveExpression(node.Name),
						new CodeArrayCreateExpression(_source[typeof (Type)], 0)
					}
				);

			CodeExpression[] constructionArguments = new CodeExpression[]
				{
					new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
					                                 _naming.ToMemberVariableName(_serviceIdentifier)),
					new CodeTypeOfExpression(node.FullName),
					new CodePrimitiveExpression(node.Area),
					new CodePrimitiveExpression(_naming.ToControllerName(node.Name)),
					new CodePrimitiveExpression(wizardStepPage),
					createMethodSignature,
					new CodeArrayCreateExpression(_source[typeof (ActionArgument)], 0)
				};

			return new CodeMethodInvokeExpression(
				new CodeMethodReferenceExpression(
					new CodePropertyReferenceExpression(
						new CodeFieldReferenceExpression(new CodeThisReferenceExpression(),
						                                 _naming.ToMemberVariableName(_serviceIdentifier)),
						"ControllerReferenceFactory"),
					"CreateActionReference"),
				constructionArguments
				);
		}
	}
}