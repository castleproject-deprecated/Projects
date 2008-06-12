using System;
using System.CodeDom;
using System.Collections;
using System.IO;
using System.Reflection;
using Castle.Tools.CodeGenerator.Model.TreeNodes;
using Castle.Tools.CodeGenerator.Services;
using Castle.Tools.CodeGenerator.Services.Generators;
using ICSharpCode.NRefactory;
using ICSharpCode.NRefactory.Ast;
using Microsoft.Build.BuildEngine;
using Microsoft.Build.Framework;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using ILogger=Castle.Tools.CodeGenerator.Services.ILogger;

namespace Castle.Tools.CodeGenerator.MsBuild
{
  [TestFixture]
  public class GenerateMonoRailSiteTreeTaskIntegrationTests
  {
    #region Member Data
    private MockRepository _mocks;
    private Engine _engine;
    private IBuildEngine _buildEngine;
    private GenerateMonoRailSiteTreeTask _task;
    private Project _project;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public void Setup()
    {
      _mocks = new MockRepository();
      _engine = Engine.GlobalEngine;
      if (Directory.Exists(@"C:\Program Files (x86)\MSBuild"))
      {
        _engine.BinPath = @"C:\Program Files (x86)\MSBuild";
      }
      else
      {
        _engine.BinPath = @"C:\Program Files\MSBuild";
      }
      _project = new Project();
      _buildEngine = _mocks.DynamicMock<MockBuildEngine>(_project);
      _task = new GenerateMonoRailSiteTreeTask();
    }
    #endregion

    #region Test Methods
    [Test]
    public void Execute_NoFiles_ReturnsTrue()
    {
      using (_mocks.Unordered())
      {
        _buildEngine.LogMessageEvent(null);
        LastCall.IgnoreArguments().Repeat.Any();
      }

      _task.BuildEngine = _buildEngine;
      _task.Sources = new ITaskItem[0];
      _task.ControllerSources = new ITaskItem[0];
      _task.ViewSources = new ITaskItem[0];
      _task.Namespace = "Eleutian.Empty";
      _task.OutputFile = "SiteMap.generated.cs";

      _mocks.ReplayAll();
      Assert.IsTrue(_task.Execute());
      Assert.AreEqual("SiteMap.generated.cs", _task.GeneratedItems[0].ItemSpec);

      File.Delete(_task.OutputFile);
    }
    #endregion
  }

  [TestFixture]
  public class GenerateMonoRailSiteTreeTaskTests
  {
    #region Member Data
    private MockRepository _mocks;
    private Engine _engine;
    private IBuildEngine _buildEngine;
    private GenerateMonoRailSiteTreeTask _task;
    private Project _project;
    private ILogger _logger;
    private INamingService _naming;
    private ISiteTreeGeneratorService _parserService;
    private IParsedSourceStorageService _sourceStorage;
    private ISourceGenerator _source;
    private ITypeResolver _typeResolver;
    private ITreeCreationService _treeService;
    private IViewSourceMapper _viewSourceMapper;
    private IGenerator _generator;
    private ITaskItem _item;
    private IParser _parsedSource;
    private CodeCompileUnit _ccu;
    #endregion

    #region Test Setup and Teardown Methods
    [SetUp]
    public void Setup()
    {
      _ccu = new CodeCompileUnit();
      _mocks = new MockRepository();
      _engine = Engine.GlobalEngine;
      _engine.BinPath = @"C:\Program Files (x86)\MSBuild";
      _project = new Project();
      _buildEngine = _mocks.DynamicMock<MockBuildEngine>(_project);

      _logger = new NullLogger();
      _parserService = _mocks.DynamicMock<ISiteTreeGeneratorService>();
      _naming = _mocks.DynamicMock<INamingService>();
      _sourceStorage = _mocks.DynamicMock<IParsedSourceStorageService>();
      _source = _mocks.DynamicMock<ISourceGenerator>();
      _typeResolver = _mocks.DynamicMock<ITypeResolver>();
      _treeService = _mocks.DynamicMock<ITreeCreationService>();
      _viewSourceMapper = _mocks.DynamicMock<IViewSourceMapper>();
      _generator = _mocks.DynamicMock<IGenerator>();

      _task = new GenerateMonoRailSiteTreeTask(_logger, _parserService, _naming, _source, _sourceStorage, _typeResolver, _treeService, _viewSourceMapper, _generator);

      _item = _mocks.DynamicMock<ITaskItem>();
      _parsedSource = _mocks.DynamicMock<IParser>();
    }
    #endregion

    #region Test Methods
    [Test]
    public void Execute_NoFiles_ReturnsTrue()
    {
      TreeNode root = new TreeNode("Root");
      using (_mocks.Unordered())
      {
        _buildEngine.LogMessageEvent(null);
        LastCall.IgnoreArguments().Repeat.Any();
        Expect.Call(_treeService.Root).Return(root).Repeat.Any();
        _generator.Generate(root);
        Expect.Call(_source.Ccu).Return(_ccu).Repeat.Any();
      }

      _task.BuildEngine = _buildEngine;
      _task.Sources = new ITaskItem[0];
      _task.ControllerSources = new ITaskItem[0];
      _task.ViewSources = new ITaskItem[0];
      _task.Namespace = "Eleutian.Empty";
      _task.OutputFile = "SiteMap.generated.cs";

      _mocks.ReplayAll();
      Assert.IsTrue(_task.Execute());
      Assert.AreEqual("SiteMap.generated.cs", _task.GeneratedItems[0].ItemSpec);

      File.Delete(_task.OutputFile);
    }

    [Test]
    public void Execute_OneControllerSource_ReturnsTrue()
    {
      TreeNode root = new TreeNode("Root");
      using (_mocks.Unordered())
      {
        _buildEngine.LogMessageEvent(null);
        LastCall.IgnoreArguments().Repeat.Any();
      	Expect.Call(_item.GetMetadata("FullPath")).Return("HomeController.cs").Repeat.Any();
        Expect.Call(_sourceStorage.GetParsedSource("HomeController.cs")).Return(_parsedSource);
        Expect.Call(_parsedSource.CompilationUnit).Return(new CompilationUnit());
        Expect.Call(_treeService.Root).Return(root).Repeat.Any();
        _generator.Generate(root);
        Expect.Call(_source.Ccu).Return(_ccu).Repeat.Any();
        _typeResolver.Clear();
      }

      _task.BuildEngine = _buildEngine;
      _task.Sources = new ITaskItem[0];
      _task.ControllerSources = new ITaskItem[] { _item };
      _task.ViewSources = new ITaskItem[0];
      _task.Namespace = "Eleutian.Empty";
      _task.OutputFile = "SiteMap.generated.cs";

      _mocks.ReplayAll();
      Assert.IsTrue(_task.Execute());
      Assert.AreEqual("SiteMap.generated.cs", _task.GeneratedItems[0].ItemSpec);

      File.Delete(_task.OutputFile);
    }

    [Test]
    public void Execute_OneViewSource_ReturnsTrue()
    {
      TreeNode root = new TreeNode("Root");
      using (_mocks.Unordered())
      {
        _buildEngine.LogMessageEvent(null);
        LastCall.IgnoreArguments().Repeat.Any();       
      	Expect.Call(_item.GetMetadata("FullPath")).Return("Index.brail").Repeat.Any();
        _viewSourceMapper.AddViewSource("Index.brail");
        Expect.Call(_treeService.Root).Return(root).Repeat.Any();
        _generator.Generate(root);
        Expect.Call(_source.Ccu).Return(_ccu).Repeat.Any();
      }

      _task.BuildEngine = _buildEngine;
      _task.Sources = new ITaskItem[0];
      _task.ControllerSources = new ITaskItem[0];
      _task.ViewSources = new ITaskItem[] { _item };
      _task.Namespace = "Eleutian.Empty";
      _task.OutputFile = "SiteMap.generated.cs";

      _mocks.ReplayAll();
      Assert.IsTrue(_task.Execute());
      Assert.AreEqual("SiteMap.generated.cs", _task.GeneratedItems[0].ItemSpec);

      File.Delete(_task.OutputFile);
    }

    [Test]
    public void Execute_WithEvaluatedItems_Works()
    {
      TreeNode root = new TreeNode("Root");

      File.CreateText(@"CoolSuperSourceCode.cs").Close();

      using (_mocks.Unordered())
      {
        _buildEngine.LogMessageEvent(null);
        LastCall.IgnoreArguments().Repeat.Any();        
      	Expect.Call(_item.GetMetadata("FullPath")).Return("CoolSuperSourceCode.cs").Repeat.Any();
        _parserService.Parse(null, null);
        LastCall.Constraints(Is.NotNull(), Is.Equal("CoolSuperSourceCode.cs"));
        Expect.Call(_treeService.Root).Return(root).Repeat.Any();
        _generator.Generate(root);
        Expect.Call(_source.Ccu).Return(_ccu).Repeat.Any();
      }

      _task.BuildEngine = _buildEngine;
      _task.Sources = new ITaskItem[] { _item };
      _task.ControllerSources = new ITaskItem[0];
      _task.ViewSources = new ITaskItem[0];
      _task.Namespace = "Eleutian.Empty";
      _task.OutputFile = "SiteMap.generated.cs";

      _mocks.ReplayAll();
      Assert.IsTrue(_task.Execute());
      Assert.AreEqual("SiteMap.generated.cs", _task.GeneratedItems[0].ItemSpec);

      File.Delete(_task.OutputFile);
    }
    #endregion

    #region Methods
    private void SetEvaluatedItems(BuildItemGroup group)
    {
      FieldInfo field1 =_project.GetType().GetField("dirtyNeedToReprocessXml", BindingFlags.NonPublic | BindingFlags.Instance);
      field1.SetValue(_project, false);
      FieldInfo field2 =_project.GetType().GetField("dirtyNeedToReevaluate", BindingFlags.NonPublic | BindingFlags.Instance);
      field2.SetValue(_project, false);

      FieldInfo field =_project.GetType().GetField("evaluatedItems", BindingFlags.NonPublic | BindingFlags.Instance);
      field.SetValue(_project, group);
    }
    #endregion
  }

  public class MockBuildEngine : IBuildEngine
  {
    #region Member Data
    protected Project project;
    #endregion

    #region MockBuildEngine()
    public MockBuildEngine(Project project)
    {
      this.project = project;
    }
    #endregion

    #region IBuildEngine Members
    public virtual bool BuildProjectFile(string projectFileName, string[] targetNames, IDictionary globalProperties, IDictionary targetOutputs)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public virtual int ColumnNumberOfTaskNode
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    public virtual bool ContinueOnError
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    public virtual int LineNumberOfTaskNode
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }

    public virtual void LogCustomEvent(CustomBuildEventArgs e)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public virtual void LogErrorEvent(BuildErrorEventArgs e)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public virtual void LogMessageEvent(BuildMessageEventArgs e)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public virtual void LogWarningEvent(BuildWarningEventArgs e)
    {
      throw new Exception("The method or operation is not implemented.");
    }

    public virtual string ProjectFileOfTaskNode
    {
      get { throw new Exception("The method or operation is not implemented."); }
    }
    #endregion
  }
}
