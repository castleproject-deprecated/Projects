namespace Castle.MonoRail.Framework.View.Xslt
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	/// <summary>
	/// Class responsible for always providing 
	/// the most recent version of a certain stylesheet.
	/// </summary>
	public class XsltTemplateStore
	{
		private List<FileSystemWatcher> _genericWatchers = new List<FileSystemWatcher>();
		private IXsltEngine _xsltEngine;

		public XsltTemplateStore(IViewSourceLoader viewSourceLoader, IXsltEngine xsltEngine)
		{
			_viewSourceLoader = viewSourceLoader;
			_viewSourceLoader.ViewChanged += new FileSystemEventHandler(_viewSourceLoader_ViewChanged);
			_xsltEngine = xsltEngine;
		}

		void _viewSourceLoader_ViewChanged(object sender, FileSystemEventArgs e)
		{
			string templateKey = e.Name.Replace(".xslt", string.Empty).ToLower();
			lock (_lock)
			{
				if (_templateCache.ContainsKey(templateKey))
				{
					_templateCache.Remove(templateKey);
				}
			}
		}

		private Dictionary<string, IXsltTransform> _templateCache = new Dictionary<string, IXsltTransform>();
		private object _lock = new object();
		private IViewSourceLoader _viewSourceLoader;

		private Stream GetTemplateStream(String templateName)
		{
			return _viewSourceLoader.GetViewSource(ResolveTemplateName(templateName)).OpenViewStream();

		}

		private IXsltTransform CompileTransform(string templateName, XsltTransformationArguments arguments)
		{
			using (Stream templateStream = GetTemplateStream(templateName))
			{
				return CompileTransform(templateStream, arguments);
			}
		}

		private IXsltTransform CompileTransform(Stream transformStream, XsltTransformationArguments arguments)
		{
			return _xsltEngine.LoadTransform(transformStream, arguments);
		}

		public IXsltTransform LoadTemplate(string templateName, XsltTransformationArguments arguments)
		{
			lock (_lock)
			{
				string key = templateName.ToLower();
				if (_templateCache.ContainsKey(key))
					return _templateCache[key];

				IXsltTransform transform = CompileTransform(key, arguments);

				_templateCache[key] = transform;

				return transform;
			}
		}

		void watcher_Changed(object sender, FileSystemEventArgs e)
		{
			lock (_lock)
			{
				if (_templateCache.ContainsKey(e.FullPath))
				{
					_templateCache.Remove(e.FullPath);
				}
			}
		}

		protected virtual String ResolveTemplateName(string templateName)
		{
			return templateName + ".xslt";
		}

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <returns><c>true</c> if it exists</returns>
		public bool HasTemplate(string templateName)
		{
			return _viewSourceLoader.HasTemplate(ResolveTemplateName(templateName));
		}
	}
}
