using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;

using Castle.Tools.CodeGenerator.Model;

namespace Castle.Tools.CodeGenerator.Services
{
  public class ControllerPartialsGenerator : AbstractGenerator
  {
    #region ControllerPartialsGenerator()
    public ControllerPartialsGenerator(ILogger logger, ISourceGenerator source, INamingService naming, string targetNamespace, string serviceType)
     : base(logger, source, naming, targetNamespace, serviceType)
    {
    }
    #endregion

    #region TreeWalker Members
    public override void Visit(ControllerTreeNode node)
    {
      CodeTypeDeclaration type = _source.GenerateTypeDeclaration(node.Namespace, node.Name);

      string actionWrapperName = _namespace + "." + node.PathNoSlashes + _naming.ToControllerWrapperName(node.Name);
      type.Members.Add(
        _source.CreateReadOnlyProperty("MyActions", _source[actionWrapperName],
                               new CodeObjectCreateExpression(_source[actionWrapperName],
                                 new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "CodeGeneratorServices"))));

      string viewWrapperName = _namespace + "." + node.PathNoSlashes + _naming.ToViewWrapperName(node.Name);
      type.Members.Add(
        _source.CreateReadOnlyProperty("MyViews", _source[viewWrapperName],
                               new CodeObjectCreateExpression(_source[viewWrapperName],
                                 new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "CodeGeneratorServices"))));

      CodeMemberMethod initialize = new CodeMemberMethod();
      initialize.Attributes = MemberAttributes.Override | MemberAttributes.Family;
      initialize.Name = "PerformGeneratedInitialize";
      initialize.Statements.Add(
        new CodeMethodInvokeExpression(new CodeBaseReferenceExpression(), "PerformGeneratedInitialize"));

      initialize.Statements.Add(AddPropertyToPropertyBag("MyViews"));
      initialize.Statements.Add(AddPropertyToPropertyBag("MyActions"));

      type.Members.Add(initialize);

      base.Visit(node);
    }
    #endregion

    #region Methods
    protected virtual CodeStatement AddPropertyToPropertyBag(string property)
    {
      CodeStatement assign = new CodeAssignStatement(
        new CodeIndexerExpression(
          new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), "PropertyBag"),
          new CodePrimitiveExpression(property)
          ),
        new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), property)
        );
      return assign;
    }
    #endregion
  }
}
