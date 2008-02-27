// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.View.Xslt
{
	using System;
	using System.IO;

	internal class XsltTemplateStoreBasedXsltTemplateResolver : IXsltTemplateResolver
	{
		private XsltTemplateStore _store;
		private XsltTemplateStore Store
		{
			get { return _store; }
			set { _store = value; }
		}

		private string _referencingTemplateName;

		private string ReferencingTemplateName
		{
			get
			{
				return _referencingTemplateName;
			}
			set
			{
				_referencingTemplateName = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the XsltTemplateStoreBasedXsltTemplateResolver class.
		/// </summary>
		/// <param name="store"></param>
		/// <param name="referencingTemplateName"></param>
		internal XsltTemplateStoreBasedXsltTemplateResolver(XsltTemplateStore store, string referencingTemplateName)
		{
			Store = store;
			ReferencingTemplateName = referencingTemplateName;
		}

		#region IXsltTemplateResolver Members

		public Uri ResolveTemplate(string relativeUri)
		{
			string path = Path.Combine(Store.ViewSourceLoader.ViewRootDir, ReferencingTemplateName);

			
            Uri resolvedUri = new Uri(new Uri(path), relativeUri);

			
			//The referencing template becomes a template of the resolved template			
			Uri basePath = new Uri(Store.ViewSourceLoader.ViewRootDir + Path.DirectorySeparatorChar);
			Uri relative = basePath.MakeRelativeUri(resolvedUri);
			path = relative.OriginalString.ToLowerInvariant().Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			path = path.Replace(Path.GetExtension(path), string.Empty);
			
			Store.AddDependency(path, ReferencingTemplateName);

			return resolvedUri;
		}

		#endregion
	}
}
