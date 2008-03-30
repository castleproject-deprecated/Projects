
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

namespace Castle.Components.Localization.MonoRail.Framework
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.IO;

	using Castle.MonoRail.Framework.Services;

	#endregion Using Directives

	/// <summary>
	/// An extended version of the <see cref="IStaticResourceRegistry"/> service 
	/// which handles resource files such as images.
	/// </summary>
	public interface IStaticResourceRegistryEx : IStaticResourceRegistry
	{
		/// <summary>
		/// Gets the resource <see cref="Stream"/>.
		/// </summary>
		/// <param name="name">The resource's name.</param>
		/// <param name="location">The resource's location.</param>
		/// <param name="version">The resource's version.</param>
		/// <param name="mimeType">The resource's MIME TYPE.</param>
		/// <returns>The <see cref="Stream"/> for the resource.</returns>
		Stream GetResourceStream( string name, string location, string version, out string mimeType );

		/// <summary>
		/// Registers an assembly resource.
		/// </summary>
		/// <param name="name">The resource's name.</param>
		/// <param name="location">The resource's location.</param>
		/// <param name="version">The resource's version.</param>
		/// <param name="assemblyName">The name of the resource's assembly.</param>
		/// <param name="resourceName">The name of the resource.</param>
		/// <param name="mimeType">The resource's MIME TYPE.</param>
		void RegisterAssemblyResource( string name, string location, string version, string assemblyName, string resourceName, string mimeType );
	}
}
