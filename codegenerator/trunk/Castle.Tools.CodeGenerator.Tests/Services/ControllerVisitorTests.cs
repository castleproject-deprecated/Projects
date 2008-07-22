using System;
using System.Collections.Generic;
using System.IO;
using Castle.Tools.CodeGenerator.Model.TreeNodes;
using Castle.Tools.CodeGenerator.Services.Visitors;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Attribute=ICSharpCode.NRefactory.Ast.Attribute;

namespace Castle.Tools.CodeGenerator.Services
{
  [TestFixture]
  public class ControllerVisitorTests
  {
    #region Member Data
    private MockRepository _mocks;
    private ITreeCreationService _treeService;
    private ILogger _logger;
    private ITypeResolver _typeResolver;
    private ControllerVisitor _visitor;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public void Setup()
    {
      _mocks = new MockRepository();
      _logger = new NullLogger();
      _typeResolver = _mocks.DynamicMock<ITypeResolver>();
      _treeService = _mocks.DynamicMock<ITreeCreationService>();
      _visitor = new ControllerVisitor(_logger, _typeResolver, _treeService);
    }
    #endregion

    #region Test Methods
    [Test]
    public void VisitTypeDeclaration_NotController_DoesNothing()
    {
      TypeDeclaration type = new TypeDeclaration(Modifiers.Public, new List<AttributeSection>());
      type.Name = "SomeRandomType";

      _mocks.ReplayAll();
	  _visitor.VisitTypeDeclaration(type, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitTypeDeclaration_AControllerNotPartial_DoesNothing()
    {
      TypeDeclaration type = new TypeDeclaration(Modifiers.Public, new List<AttributeSection>());
      type.Name = "SomeRandomController";

      _mocks.ReplayAll();
	  _visitor.VisitTypeDeclaration(type, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitTypeDeclaration_AControllerNoChildren_PushesAndPops()
    {
      TypeDeclaration type = new TypeDeclaration(Modifiers.Public | Modifiers.Partial, new List<AttributeSection>());
      type.Name = "SomeRandomController";

      using (_mocks.Unordered())
      {
        _treeService.PushNode(new ControllerTreeNode("SomeRandomController", "SomeNamespace"));
        _treeService.PopNode();
      }

      _mocks.ReplayAll();
	  _visitor.VisitTypeDeclaration(type, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitTypeDeclaration_AControllerNoChildrenWithArea_PushesAndPops()
    {
      TypeDeclaration type = new TypeDeclaration(Modifiers.Public | Modifiers.Partial, new List<AttributeSection>());
      type.Name = "SomeRandomController";
      type.Attributes.Add(CreateAreaAttributeCode("ControllerDetails", "Area", new PrimitiveExpression("AnArea", "AnArea")));

      using (_mocks.Unordered())
      {
        Expect.Call(_treeService.FindNode("AnArea")).Return(null);
        _treeService.PushNode(new AreaTreeNode("AnArea"));
        _treeService.PushNode(new ControllerTreeNode("SomeRandomController", "SomeNamespace"));
        _treeService.PopNode();
        _treeService.PopNode();
      }

      _mocks.ReplayAll();
	  _visitor.VisitTypeDeclaration(type, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitTypeDeclaration_AControllerNoChildrenTrickyAreaValue_IgnoresAreaAttribute()
    {
      TypeDeclaration type = new TypeDeclaration(Modifiers.Public | Modifiers.Partial, new List<AttributeSection>());
      type.Name = "SomeRandomController";
      type.Attributes.Add(CreateAreaAttributeCode("ControllerDetails", "Area", new AddressOfExpression(new PrimitiveExpression("ThisNeverHappens", "Ok?"))));

      using (_mocks.Unordered())
      {
        _treeService.PushNode(new ControllerTreeNode("SomeRandomController", "SomeNamespace"));
        _treeService.PopNode();
      }

      _mocks.ReplayAll();
	  _visitor.VisitTypeDeclaration(type, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitTypeDeclaration_AControllerTrickyAttribute_IgnoresAttribute()
    {
      TypeDeclaration type = new TypeDeclaration(Modifiers.Public | Modifiers.Partial, new List<AttributeSection>());
      type.Name = "SomeRandomController";
      type.Attributes.Add(CreateAreaAttributeCode("NotControllerDetails", "NotArea", new PrimitiveExpression("NotAnArea", "NotAnArea")));

      using (_mocks.Unordered())
      {
        _treeService.PushNode(new ControllerTreeNode("SomeRandomController", "SomeNamespace"));
        _treeService.PopNode();
      }

      _mocks.ReplayAll();
      _visitor.VisitTypeDeclaration(type, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitMethodDeclaration_ProtectedMember_DoesNothing()
    {
      MethodDeclaration method = new MethodDeclaration("Action", Modifiers.Protected, null, new List<ParameterDeclarationExpression>(), new List<AttributeSection>());

      using (_mocks.Unordered())
      {
      }

      _mocks.ReplayAll();
	  _visitor.VisitMethodDeclaration(method, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitMethodDeclaration_ActionMemberNoArguments_CreatesEntryInNode()
    {
      MethodDeclaration method = new MethodDeclaration("Action", Modifiers.Public, null, new List<ParameterDeclarationExpression>(), new List<AttributeSection>());
      ControllerTreeNode node = new ControllerTreeNode("SomeController", "SomeNamespace");

      using (_mocks.Unordered())
      {
        Expect.Call(_treeService.Peek).Return(node);
      }

      _mocks.ReplayAll();
      _visitor.VisitMethodDeclaration(method, null);
      _mocks.VerifyAll();
      
      Assert.AreEqual("Action", node.Children[0].Name);
      Assert.AreEqual(0, node.Children[0].Children.Count);
    }

    [Test]
    public void VisitMethodDeclaration_ActionMemberNoArgumentsIsVirtual_CreatesEntryInNode()
    {
      MethodDeclaration method = new MethodDeclaration("Action", Modifiers.Public | Modifiers.Virtual, null, new List<ParameterDeclarationExpression>(), new List<AttributeSection>());
      ControllerTreeNode node = new ControllerTreeNode("SomeController", "SomeNamespace");

      using (_mocks.Unordered())
      {
        Expect.Call(_treeService.Peek).Return(node);
      }

      _mocks.ReplayAll();
      _visitor.VisitMethodDeclaration(method, null);
      _mocks.VerifyAll();
      
      Assert.AreEqual("Action", node.Children[0].Name);
      Assert.AreEqual(0, node.Children[0].Children.Count);
    }

    [Test]
    public void VisitMethodDeclaration_ActionMemberStandardArgument_CreatesEntryInNode()
    {
      MethodDeclaration method = new MethodDeclaration("Action", Modifiers.Public, null, new List<ParameterDeclarationExpression>(), new List<AttributeSection>());
      method.Parameters.Add(new ParameterDeclarationExpression(new TypeReference("bool"), "parameter"));
      ControllerTreeNode node = new ControllerTreeNode("SomeController", "SomeNamespace");

      using (_mocks.Unordered())
      {
        Expect.Call(_treeService.Peek).Return(node);
        Expect.Call(_typeResolver.Resolve(new TypeReference("bool"), true))
			.Constraints(
				Is.Matching(new Predicate<TypeReference>(delegate(TypeReference reference) { return reference.SystemType == "System.Boolean"; })),
				Is.Matching(new Predicate<bool>(delegate(bool throwOnFail) { return throwOnFail; })))
			.Return(typeof(bool));
      }

      _mocks.ReplayAll();
      _visitor.VisitMethodDeclaration(method, null);
      _mocks.VerifyAll();
      
      Assert.AreEqual("Action", node.Children[0].Name);
      Assert.AreEqual("parameter", node.Children[0].Children[0].Name);
    }

    #endregion

    #region Methods
    private static AttributeSection CreateAreaAttributeCode(string attributeName, string argumentName, Expression valueExpression)
    {
      NamedArgumentExpression argument = new NamedArgumentExpression(argumentName, valueExpression);
	  Attribute attribute = new Attribute(attributeName, new List<Expression>(), new List<NamedArgumentExpression>());
      attribute.NamedArguments.Add(argument);
	  AttributeSection attributeSection = new AttributeSection("IDontKnow", new List<Attribute>());
      attributeSection.Attributes.Add(attribute);
      return attributeSection;
    }
    #endregion
  }

  [TestFixture]
  public class ControllerVisitorHighLevelTests
  {
    #region Member Data
    private MockRepository _mocks;
    private ControllerVisitor _visitor;
    private ITreeCreationService _treeService;
    private ITypeResolver _typeResolver;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public void Setup()
    {
      _mocks = new MockRepository();
      _typeResolver = new TypeResolver();
      _typeResolver = _mocks.DynamicMock<ITypeResolver>();
      _treeService = new DefaultTreeCreationService();
      _visitor = new ControllerVisitor(new NullLogger(), _typeResolver, _treeService);
    }
    #endregion

    #region Test Methods
    [Test]
    public void Parsing_MethodWithSystemTypeArrayParameter_YieldsCorrectType()
    {
      using (_mocks.Unordered())
      {
	  _typeResolver.UseNamespace("SomeNamespace", true);
      	_typeResolver.UseNamespace("System");
        Expect.Call(_typeResolver.Resolve(new TypeReference("DateTime"), true))
			.Constraints(
				Is.Matching(new Predicate<TypeReference>(delegate(TypeReference reference) { return reference.SystemType == "DateTime"; })),
				Is.Matching(new Predicate<bool>(delegate(bool throwOnFail) { return throwOnFail; })))
			.Return(typeof(DateTime[]));
      }

      _mocks.ReplayAll();
      Parse(MethodWithSystemTypeArrayParameter);
      _mocks.VerifyAll();
    }

	  [Test]
	  public void Parsing_WizardController_()
	  {
	  	using (_mocks.Unordered())
	  	{
	  		_typeResolver.UseNamespace("SomeNamespace", true);
	  		_typeResolver.UseNamespace("System");
	  	}

	  	_mocks.ReplayAll();
	  	Parse(WizardControllerType, true);
	  	_mocks.VerifyAll();

	  	TreeNode node = _treeService.Peek;

	  	Assert.AreEqual(1, node.Children.Count);

	  	WizardControllerTreeNode wizardControllerTreeNode = (WizardControllerTreeNode) node.Children[0];

	  	Assert.AreEqual(2, wizardControllerTreeNode.WizardStepPages.Length);
	  	Assert.AreEqual("FirstStep", wizardControllerTreeNode.WizardStepPages[0]);
	  	Assert.AreEqual("SecondStep", wizardControllerTreeNode.WizardStepPages[1]);
	  }

  	#endregion

    #region Sources
	protected void Parse(string source)
    {
		Parse(source, false);
	}
    
	protected void Parse(string source, bool parseMethodBodies)
    {
      using (StringReader reader = new StringReader(source))
      {
        IParser parser = new NRefactoryParserFactory().CreateCSharpParser(reader);
		parser.ParseMethodBodies = parseMethodBodies;
        parser.Parse();
        _visitor.VisitCompilationUnit(parser.CompilationUnit, null);
      }
    }

    public static string MethodWithSystemTypeArrayParameter = @"
using System;
namespace SomeNamespace
{
  public partial class SomeController
  {
    public void SomeMethod(DateTime[] values) { }
  }
}
";

    public static string MethodWithSystemTypeListParameter = @"
using System;
using System.Collections.Generic;
namespace SomeNamespace
{
  public partial class SomeController
  {
    public void SomeMethod(List<DateTime> values) { }
  }
}
";

	public static string WizardControllerType = @"
using System;
namespace SomeNamespace
{
  public partial class SomeController : IWizardController
  {
		public void OnWizardStart()
		{
			// Some comments
			int i = 10;
			int j = i + 10;
		}

		public bool OnBeforeStep(string wizardName, string stepName, WizardStepPage step)
		{
			return true;
		}

		public void OnAfterStep(string wizardName, string stepName, WizardStepPage step)
		{
		}

		public WizardStepPage[] GetSteps(IRailsEngineContext context)
		{
			return new WizardStepPage[]
			{
				new FirstStep(),
				IoC.Resolve<SecondStep>()
			};
		}
  }
}
";

    #endregion
  }
}
