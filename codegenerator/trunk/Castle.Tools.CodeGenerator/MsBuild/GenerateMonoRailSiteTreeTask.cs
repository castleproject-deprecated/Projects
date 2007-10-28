using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Castle.Tools.CodeGenerator.Model;
using ICSharpCode.NRefactory.Parser;

using Microsoft.Build.BuildEngine;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

using Castle.Tools.CodeGenerator.Services;

using ILogger = Castle.Tools.CodeGenerator.Services.ILogger;

namespace Castle.Tools.CodeGenerator.MsBuild
{
    public class GenerateMonoRailSiteTreeTask : AppDomainIsolatedTask
    {
        #region Member Data
        private ISiteTreeGeneratorService _service;
        private List<ITaskItem> _generatedItems = new List<ITaskItem>();
        private ITaskItem[] _controllerSources;
        private ITaskItem[] _viewSources;
        private ITaskItem[] _assemblyReferences = new ITaskItem[0];
        private ITaskItem[] _allSources;
        private ITaskItem[] _viewComponentSources = new ITaskItem[0];
        private string _namespace;
        private string _file;
        private string _serviceTypeName;
        private ILogger _logger;
        private INamingService _naming;
        private IParsedSourceStorageService _sourceStorage;
        private ISourceGenerator _source;
        private ITypeResolver _typeResolver;
        private ITreeCreationService _treeService;
        private IViewSourceMapper _viewSourceMapper;
        private List<IGenerator> _generators = new List<IGenerator>();
		private bool _debug = false;
        #endregion

        #region Properties
        [Required]
        public string Namespace
        {
            get { return _namespace; }
            set { _namespace = value; }
        }

        // [Required]
        public string ServiceTypeName
        {
            get { return _serviceTypeName; }
            set { _serviceTypeName = value; }
        }

        [Required]
        public string OutputFile
        {
            get { return _file; }
            set { _file = value; }
        }

        [Required]
        public ITaskItem[] ControllerSources
        {
            get { return _controllerSources; }
            set { _controllerSources = value; }
        }

        public ITaskItem[] ViewComponentSources
        {
            get { return _viewComponentSources; }
            set { _viewComponentSources = value; }
        }

        [Required]
        public ITaskItem[] ViewSources
        {
            get { return _viewSources; }
            set { _viewSources = value; }
        }

        [Required]
        public ITaskItem[] Sources
        {
            get { return _allSources; }
            set { _allSources = value; }
        }

        public ITaskItem[] AssemblyReferences
        {
            get { return _assemblyReferences; }
            set { _assemblyReferences = value; }
        }

        [Output]
        public ITaskItem[] GeneratedItems
        {
            get { return _generatedItems.ToArray(); }
        }


    	public bool Debug
    	{
    		get { return _debug; }
    		set { _debug = value; }
    	}

    	#endregion

        #region GenerateMonoRailSiteTreeTask()
        public GenerateMonoRailSiteTreeTask()
        {
            _logger = new MsBuildLogger(this.Log);
            _naming = new DefaultNamingService();
            _treeService = new DefaultTreeCreationService();
            _source = new DefaultSourceGenerator();
            _viewSourceMapper = new ViewSourceMapper(_logger, _treeService, _naming);
            _sourceStorage = new DefaultSourceStorageService();
            _typeResolver = new TypeResolver();
            _service = new SiteTreeGeneratorService(_logger, _typeResolver, _sourceStorage, new NRefactoryParserFactory());
            this.ServiceTypeName = typeof(ICodeGeneratorServices).FullName;
        }

        public GenerateMonoRailSiteTreeTask(ILogger logger, ISiteTreeGeneratorService service, INamingService naming, ISourceGenerator source, IParsedSourceStorageService sourceStorage, ITypeResolver typeResolver, ITreeCreationService treeService, IViewSourceMapper viewSourceMapper, IGenerator generator)
        {
            _service = service;
            _logger = logger;
            _naming = naming;
            _source = source;
            _sourceStorage = sourceStorage;
            _typeResolver = typeResolver;
            _treeService = treeService;
            _viewSourceMapper = viewSourceMapper;
            _generators.Add(generator);
            this.ServiceTypeName = typeof(ICodeGeneratorServices).FullName;
        }
        #endregion

        #region Task Members
        public override bool Execute()
        {
            this.Log.LogMessage(MessageImportance.High, "OutputFile: {0}", this.OutputFile);
            this.Log.LogMessage(MessageImportance.High, "Namespace: {0}", this.Namespace);
            this.Log.LogMessage(MessageImportance.High, "ControllerSources: {0}", this.ControllerSources.Length);
            this.Log.LogMessage(MessageImportance.High, "ViewSources: {0}", this.ViewSources.Length);
            this.Log.LogMessage(MessageImportance.High, "ViewComponentSources: {0}", this.ViewComponentSources.Length);
            this.Log.LogMessage(MessageImportance.High, "Loading References...");

			if (Debug) System.Diagnostics.Debugger.Launch();
			
            foreach (ITaskItem reference in _assemblyReferences)
            {
                try
                {
                    Assembly.LoadFrom(reference.ItemSpec);
                    Log.LogMessage(MessageImportance.Low, "Loaded: {0}", reference.ItemSpec);
                }
                catch (Exception ex)
                {
                    this.Log.LogMessage(MessageImportance.High, "Error loading reference: '{0}': {1}", reference.ItemSpec, ex.Message);
                }                
            }

            if (File.Exists(this.OutputFile))
            {
                File.Delete(this.OutputFile);
            }

            this.Log.LogMessage(MessageImportance.High, "Parsing Sources...");
            foreach (ITaskItem ti in this.Sources)
            {
            	string filePath = ti.GetMetadata("FullPath");
            	if (File.Exists(filePath))
                {
                    TypeInspectionVisitor visitor = new TypeInspectionVisitor(_typeResolver);
                    _service.Parse(visitor, filePath);
                }
            }

        	foreach (ITaskItem item in this.ViewComponentSources)
            {
                _typeResolver.Clear();

                ViewComponentVisitor visitor = new ViewComponentVisitor(_logger, _typeResolver, _treeService);
            	string filePath = item.GetMetadata("FullPath");
            	visitor.VisitCompilationUnit(_sourceStorage.GetParsedSource(filePath).CompilationUnit, null);
            }

            foreach (ITaskItem item in this.ControllerSources)
            {
                _typeResolver.Clear();

				ControllerVisitor visitor = new ControllerVisitor(_logger, _typeResolver, _treeService);
            	string filePath = item.GetMetadata("FullPath");
            	visitor.VisitCompilationUnit(_sourceStorage.GetParsedSource(filePath).CompilationUnit, null);
            }

            foreach (ITaskItem bi in this.ViewSources)
            {
            	string filePath = bi.GetMetadata("FullPath");
                _viewSourceMapper.AddViewSource(filePath);
            }

            this.Log.LogMessage(MessageImportance.High, "Generating {0}", this.OutputFile);

            CreateDefaultGenerators();

            foreach (IGenerator generator in _generators)
            {
				generator.Generate(_treeService.Root);
            }

            CreateGeneratedItems();

            return true;
        }
        #endregion

        #region Methods
        protected virtual void CreateDefaultGenerators()
        {
            if (_generators.Count > 0) return;
            string serviceType = this.ServiceTypeName;
            _generators.Add(new ActionMapGenerator(_logger, _source, _naming, this.Namespace, serviceType));
			_generators.Add(new RouteMapGenerator(_logger, _source, _naming, this.Namespace, serviceType));
            _generators.Add(new ViewMapGenerator(_logger, _source, _naming, this.Namespace, serviceType));
            _generators.Add(new ControllerPartialsGenerator(_logger, _source, _naming, this.Namespace, serviceType));
            _generators.Add(new ControllerMapGenerator(_logger, _source, _naming, this.Namespace, serviceType));
			_generators.Add(new WizardStepMapGenerator(_logger, _source, _naming, this.Namespace, serviceType));
        }

        protected virtual void CreateGeneratedItems()
        {
            using (StreamWriter writer = File.CreateText(this.OutputFile))
            {
                System.CodeDom.Compiler.CodeGenerator.ValidateIdentifiers(_source.Ccu);
                CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
                CodeGeneratorOptions options = new CodeGeneratorOptions();
                provider.GenerateCodeFromCompileUnit(_source.Ccu, writer, options);
            }
            _generatedItems.Add(new TaskItem(this.OutputFile));
        }

        /*
        private Project GetCurrentProject()
        {
          Type engineType = this.BuildEngine.GetType();
          FieldInfo field = engineType.GetField("project", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.InvokeMethod);
          if (field == null)
            throw new ArgumentException("Unable to resolve Project!");
          Project project = field.GetValue(this.BuildEngine) as Project;
          if (project == null)
            throw new ArgumentNullException("Project is Null!");
          return project;
        }
        */
        /*
        private void ShowTree(TreeNode node, int depth)
        {
          this.Log.LogMessage("{0}{1}", new String(' ', depth * 2), node);
          foreach (TreeNode child in node.Children)
          {
            ShowTree(child, depth + 1);
          }
        }
        */
        #endregion
    }
}
