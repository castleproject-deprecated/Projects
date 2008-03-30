
#region License

// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

#endregion License

namespace Castle.Components.Localization.MonoRail.Framework.Resource
{
	#region Using Directives

	using System;
	using System.IO;
	using Castle.Core.Resource;

	#endregion Using Directives

	public class ResourceHolder
	{

		#region Instance Variables 

		private string _MimeType;
		private IResource _Resource;

		#endregion Instance Variables 

		#region Properties

		/// <summary>
		/// Gets the resource's MIME TYPE.
		/// </summary>
		/// <value>The resource's MIME TYPE.</value>
		public string MimeType
		{
			get
			{
				return _MimeType;
			}
		}

		/// <summary>
		/// Gets the resource's content.
		/// </summary>
		/// <value>The content as a <see cref="string"/> object.</value>
		public string Content
		{
			get
			{
				return _Resource.GetStreamReader().ReadToEnd();
			}
		}

		/// <summary>
		/// Gets the resource's stream.
		/// </summary>
		/// <value>The resource's stream.</value>
		public Stream Stream
		{
			get
			{
				IResourceEx resourceEx = _Resource as IResourceEx;

				if ( resourceEx != null )
					return resourceEx.GetStream();
				else
					return null;
			}
		}

		#endregion Properties 

		#region Constructors 

		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceHolder"/> class.
		/// </summary>
		/// <param name="resource">The resource.</param>
		/// <param name="mimeType">The resource's MIME TYPE.</param>
		public ResourceHolder( IResource resource, string mimeType )
		{
			_Resource = resource;
			_MimeType = mimeType;
		}

		#endregion Constructors 

	}
}
