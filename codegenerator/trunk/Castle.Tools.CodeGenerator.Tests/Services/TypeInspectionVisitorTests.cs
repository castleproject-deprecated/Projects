using System;
using System.Collections.Generic;

using ICSharpCode.NRefactory.Parser.AST;

using Rhino.Mocks;
using NUnit.Framework;

namespace Castle.Tools.CodeGenerator.Services
{
  [TestFixture]
  public class TypeInspectionVisitorTests
  {
    #region Member Data
  	private MockRepository _mocks;
    private ITypeResolver _typeResolver;
    private TypeInspectionVisitor _visitor;
    private NamespaceDeclaration _namespace;
    private TypeDeclaration _type;
  	#endregion
  	
  	#region Test Setup and Teardown Methods
  	[SetUp]
  	public void Setup()
  	{
  		_mocks = new MockRepository();
      _typeResolver = _mocks.CreateMock<ITypeResolver>();
      _visitor = new TypeInspectionVisitor(_typeResolver);

      _type = new TypeDeclaration(Modifier.Public, new List<AttributeSection>());
      _type.Name = "SomeType";
      _namespace = new NamespaceDeclaration("SomeNamespace");
      _namespace.AddChild(_type);
      _type.Parent = _namespace;
  	}
  	#endregion
  	
  	#region Test Methods
    [Test]
    public void VisitTypeDeclaration_NoNamespace_DoesNothing()
    {
      _type.Parent = null;
      _namespace.Children.Clear();

      _mocks.ReplayAll();
      _visitor.Visit(_type, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitNamespaceDeclaration_Always_AddsToUsing()
    {
      _type.Parent = null;
      _namespace.Children.Clear();

      using (_mocks.Unordered())
      {
        _typeResolver.UseNamespace("SomeNamespace",true);
      }

      _mocks.ReplayAll();
      _visitor.Visit(_namespace, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitTypeDeclaration_WithNamespace_AddsTableEntry()
    {
      using (_mocks.Unordered())
      {
        _typeResolver.AddTableEntry("SomeNamespace.SomeType");
      }

      _mocks.ReplayAll();
      _visitor.Visit(_type, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitTypeDeclaration_NonNamespaceParent_Ignores()
    {
      TypeDeclaration childType = new TypeDeclaration(Modifier.Public, new List<AttributeSection>());
      childType.Parent = _type;

      using (_mocks.Unordered())
      {
      }

      _mocks.ReplayAll();
      _visitor.Visit(childType, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitUsingDeclaration_NonAlias_AddsToUsing()
    {
      UsingDeclaration usings = new UsingDeclaration("System");
      usings.Usings.Add(new Using("System.Collections.Generic"));

      using (_mocks.Unordered())
      {
        _typeResolver.UseNamespace("System");
        _typeResolver.UseNamespace("System.Collections.Generic");
      }

      _mocks.ReplayAll();
      _visitor.Visit(usings, null);
      _mocks.VerifyAll();
    }

    [Test]
    public void VisitUsingDeclaration_Alias_AddsToUsing()
    {
      UsingDeclaration usings = new UsingDeclaration("System");
      usings.Usings.Add(new Using("Bob", new TypeReference("System")));

      using (_mocks.Unordered())
      {
        _typeResolver.UseNamespace("System");
        _typeResolver.AliasNamespace("Bob", "System");
      }

      _mocks.ReplayAll();
      _visitor.Visit(usings, null);
      _mocks.VerifyAll();
    }
  	#endregion	
  }
}
