
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
	using System.Reflection;
	using System.Globalization;
	using System.Text;
	using System.Resources;

	#endregion Using Directives

	public class AssemblyResourceEx : AbstractStreamResource, IResourceEx
	{

		#region Instance Variables

		string _AssemblyName;
		string _BasePath;
		readonly CultureInfo _CultureInfo;
		readonly string _Resource;
		string _ResourcePath;
		readonly CustomUri _ResourceUri;
		StringReader _StringReader;

		#endregion Instance Variables

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyResourceEx"/> class.
		/// </summary>
		/// <param name="resource">The resource.</param>
		public AssemblyResourceEx( CustomUri resource )
			: this( resource, CultureInfo.InvariantCulture )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyResourceEx"/> class.
		/// </summary>
		/// <param name="resource">The resource.</param>
		public AssemblyResourceEx( string resource )
			: this( resource, CultureInfo.InvariantCulture )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyResourceEx"/> class.
		/// </summary>
		/// <param name="resource">The resource.</param>
		/// <param name="cultureInfo">The culture info.</param>
		public AssemblyResourceEx( CustomUri resource, CultureInfo cultureInfo )
		{
			_ResourceUri = resource;
			_CultureInfo = cultureInfo;

			AbstractStreamResource.StreamFactory factory = null;

			if ( factory == null )
			{
				factory = delegate
				{
					return CreateResourceFromUri( resource, null );
				};
			}

			base.CreateStream = factory;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyResourceEx"/> class.
		/// </summary>
		/// <param name="resource">The resource.</param>
		/// <param name="cultureInfo">The culture info.</param>
		public AssemblyResourceEx( string resource, CultureInfo cultureInfo )
		{
			_Resource = resource;
			_CultureInfo = cultureInfo;

			AbstractStreamResource.StreamFactory factory = null;

			if ( factory == null )
			{
				factory = delegate
				{
					return CreateResourceFromPath( resource, _BasePath );
				};
			}

			base.CreateStream = factory;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AssemblyResourceEx"/> class.
		/// </summary>
		/// <param name="resource">The resource.</param>
		/// <param name="basePath">The base path.</param>
		public AssemblyResourceEx( CustomUri resource, string basePath )
		{
			AbstractStreamResource.StreamFactory factory = null;

			if ( factory == null )
			{
				factory = delegate
				{
					return CreateResourceFromUri( resource, basePath );
				};
			}

			base.CreateStream = factory;
		}

		#endregion Constructors

		#region Public Methods

		public override IResource CreateRelative( string relativePath )
		{
			throw new NotImplementedException();
		}

		public override void Dispose()
		{
			if ( _StringReader != null )
			{
				_StringReader.Dispose();
			}
		}

		public override TextReader GetStreamReader()
		{
			Assembly assembly = ObtainAssembly( _ResourceUri.Host );

			string[] resourcePathParts = _ResourceUri.Path.Split( new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries );

			if ( resourcePathParts.Length != 2 )
			{
				throw new ResourceException( "AssemblyBundleResource does not support paths with more than 2 levels in depth. See " + _ResourceUri.Path );
			}

			ResourceManager manager = new ResourceManager( resourcePathParts[ 0 ], assembly );

			_StringReader = new StringReader( manager.GetString( resourcePathParts[ 1 ] ) );

			return _StringReader;
		}

		public override TextReader GetStreamReader( Encoding encoding )
		{
			return GetStreamReader();
		}

		#endregion Public Methods

		#region Protected Methods

		protected virtual string ConvertToPath( string resource )
		{
			string result = resource.Replace( '.', '/' );
			if ( result[ 0 ] != '/' )
			{
				result = string.Format( "/{0}", result );
			}
			return result;
		}

		protected virtual string ConvertToResourceName( string assemblyName, string resource )
		{
			return string.Format(
				"{0}{1}",
				assemblyName,
				resource.Replace( '/', '.' ) );
		}

		protected virtual Stream CreateResourceFromPath( string resource, string path )
		{
			if ( !resource.StartsWith( "assembly" + CustomUri.SchemeDelimiter ) )
			{
				resource = "assembly" + CustomUri.SchemeDelimiter + resource;
			}
			return CreateResourceFromUri( new CustomUri( resource ), path );
		}

		protected virtual Stream CreateResourceFromUri( CustomUri resourcUri, string basePath )
		{
			if ( resourcUri == null )
			{
				throw new ArgumentNullException( "resourcex" );
			}

			_AssemblyName = resourcUri.Host;
			_ResourcePath = ConvertToResourceName(
								_AssemblyName,
								resourcUri.Path );

			Assembly assembly = ObtainAssembly( _AssemblyName );
			string[] manifestResourceNames = assembly.GetManifestResourceNames();
			string nameFound = GetNameFound( manifestResourceNames );

			if ( nameFound == null )
			{
				_ResourcePath = resourcUri.Path.Replace( '/', '.' ).Substring( 1 );
				nameFound = GetNameFound( manifestResourceNames );
			}

			if ( nameFound == null )
			{
				throw new ResourceException( string.Format( "The assembly resource {0} could not be located", _ResourcePath ) );
			}

			_BasePath = ConvertToPath( _ResourcePath );

			return assembly.GetManifestResourceStream( nameFound );
		}

		protected virtual string GetNameFound( string[] names )
		{
			foreach ( string name in names )
			{
				if ( string.Compare( _ResourcePath, name, true ) == 0 )
				{
					return name;
				}
			}
			return null;
		}

		protected virtual Assembly ObtainAssembly( string assemblyName )
		{
			Assembly assembly;
			try
			{
				assembly = Assembly.Load( assemblyName );
			}
			catch ( Exception exception )
			{
				throw new ResourceException( string.Format( "The assembly {0} could not be loaded", assemblyName ), exception );
			}
			return assembly;
		}

		#endregion Protected Methods

		#region IResourceEx Members

		public virtual Stream GetStream()
		{
			return base.CreateStream();
		}

		#endregion IResourceEx Members

	}
}
