// Copyright 2006-2007 Ken Egozi http://www.kenegozi.com/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MonoRail.Views.AspView
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Configuration;
	using System.Reflection;

	using Framework;
	using Core;
	using System.Runtime.Serialization;

	public interface IAspViewEngineTestAccess
	{
		Hashtable Compilations { get; }
	}

	public class AspViewEngine : ViewEngineBase, IInitializable, IAspViewEngineTestAccess
	{
		private bool needsRecompiling = false;
		static AspViewEngineOptions options;

		readonly Hashtable compilations = Hashtable.Synchronized(new Hashtable(CaseInsensitiveStringComparer.Default));
		Hashtable IAspViewEngineTestAccess.Compilations
		{
			get { return compilations; }
		}

		#region IInitializable Members

		public AspViewBase CreateView(Type type, TextWriter output, IRailsEngineContext context, IController controller)
		{
			AspViewBase view = (AspViewBase)FormatterServices.GetUninitializedObject(type);
			view.Initialize(this, output, context, controller);
			return view;
		}

		public void Initialize(AspViewEngineOptions newOptions)
		{
			options = newOptions;
			Initialize();
		}

		public void Initialize()
		{
			LoadPrecompiledViews();
			if (options == null)
				InitializeConfig();
			#region TODO
			//TODO: think about CommonScripts implementation in c#/VB.NET 
			#endregion

			if (options.CompilerOptions.AutoRecompilation)
			{
				// invalidate compiled views cache on any change to the view sources
				ViewSourceLoader.ViewChanged += delegate(object sender, FileSystemEventArgs e)
				{
					if (e.Name.EndsWith("." + ViewFileExtension, StringComparison.InvariantCultureIgnoreCase))
					{
						needsRecompiling = true;
					}
				};
			}
		}
		#endregion

		#region ViewEngineBase implementation
		public override bool HasTemplate(string templateName)
		{
			string className = GetClassName(templateName);
			return compilations.ContainsKey(className);
		}
		public override void Process(IRailsEngineContext context, IController controller, string templateName)
		{
			Process(context.Response.Output, context, controller, templateName);
		}
		public override void Process(TextWriter output, IRailsEngineContext context, IController controller, string templateName)
		{
			string fileName = GetFileName(templateName);
			AspViewBase view;
			TextWriter viewOutput = output;
			AspViewBase layout = null;
			if (controller.LayoutName != null)
			{
				layout = GetLayout(output, context, controller);
				viewOutput = layout.ViewOutput;
			}
			view = GetView(fileName, viewOutput, context, controller);
			if (view == null)
				throw new RailsException(string.Format(
					"Cannot find view '{0}'", fileName));
			controller.PreSendView(view);
			view.Render();
			if (layout != null)
			{
				layout.SetParent(view);
				layout.Render();
			}
			controller.PostSendView(view);
		}
		public override void ProcessContents(IRailsEngineContext context, IController controller, string contents)
		{
			TextWriter viewOutput = controller.Response.Output;
			AspViewBase layout = null;
			if (controller.LayoutName != null)
			{
				layout = GetLayout(viewOutput, context, controller);
				viewOutput = layout.ViewOutput;
			}
			viewOutput.Write(contents);
			if (layout != null)
			{
				layout.Render();
			}
		}
		public override string ViewFileExtension
		{
			get { return "aspx"; }
		}
		#region NJS
		public override object CreateJSGenerator(IRailsEngineContext context)
		{
			throw new RailsException("This version of AspView does not implements NJS.");
		}
		public override string JSGeneratorFileExtension
		{
			get { throw new RailsException("This version of AspView does not implements NJS."); }
		}
		public override void ProcessPartial(TextWriter output, IRailsEngineContext context, IController controller, string partialName)
		{
			throw new RailsException("This version of AspView does not implements NJS.");
		}
		public override bool SupportsJSGeneration
		{
			get { return false; }
		}
		public override void GenerateJS(TextWriter output, IRailsEngineContext context, IController controller, string templateName)
		{
			throw new RailsException("This version of AspView does not implements NJS.");
		}

		#endregion
		#endregion

		public virtual AspViewBase GetView(string fileName, TextWriter output, IRailsEngineContext context, IController controller)
		{
			fileName = NormalizeFileName(fileName);
			string className = GetClassName(fileName);
			if (needsRecompiling)
			{
				needsRecompiling = false;
				RecompileViews();
			}

			Type viewType = compilations[className] as Type;



			if (viewType == null)
				throw new RailsException("Cannot find view type for {0}.",
					fileName);
			// create a view instance
			AspViewBase theView;
			try
			{
				theView = CreateView(viewType, output, context, controller);
			}
			catch (Exception ex)
			{
				throw new RailsException(string.Format(
						"Cannot create view instance from '{0}'.",
						fileName), ex);
			}
			return theView;
		}

		protected virtual AspViewBase GetLayout(TextWriter output, IRailsEngineContext context, IController controller)
		{
			string layoutTemplate = "layouts\\" + controller.LayoutName;
			string layoutFileName = GetFileName(layoutTemplate);
			AspViewBase layout;
			layout = GetView(layoutFileName, output, context, controller);
			layout.ViewOutput = new StringWriter();
			return layout;
		}

		private AspViewCompiler compiler = null;

		protected virtual void RecompileViews()
		{
			if (compiler == null)
				compiler = new AspViewCompiler(options.CompilerOptions);
			compiler.CompileSite();
			LoadPrecompiledViews();
		}

		private string GetFileName(string templateName)
		{
			return templateName + "." + ViewFileExtension;
		}

		private void CacheViewType(Type viewType)
		{
			compilations[viewType.Name] = viewType;
		}

		private void LoadPrecompiledViews()
		{
			compilations.Clear();

			Assembly precompiledViews;
			try
			{
				precompiledViews = Assembly.Load("CompiledViews");
			}
			catch (Exception ex)
			{
				throw new AspViewException(ex, "Couldn't load CompiledViews assembly");
			}
			if (precompiledViews != null)
				foreach (Type type in precompiledViews.GetTypes())
					CacheViewType(type);
		}

		public static string GetClassName(string fileName)
		{
			fileName = fileName.ToLower();
			if (fileName.EndsWith(".aspx"))
				fileName = fileName.Substring(0, fileName.Length - 5);

			string className = fileName
				.Replace('\\', '_')
				.Replace('/', '_')
				.TrimStart('_')
				.Replace('.', '_');

			return className;
		}
		public static string NormalizeFileName(string fileName)
		{
			return fileName.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
		}
		private static void InitializeConfig()
		{
			InitializeConfig("aspView");
			if (options == null)
				InitializeConfig("aspview");
			if (options == null)
				options = new AspViewEngineOptions();
		}
		private static void InitializeConfig(string configName)
		{
			options = (AspViewEngineOptions)ConfigurationManager.GetSection(configName);
		}
	}
}
