
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
	using System.Globalization;
	using System.IO;

	using Castle.Core.Resource;
	using Castle.MonoRail.Framework.Services;

	using Castle.Components.Localization.MonoRail.Framework.Resource;
	using System.Reflection;

	#endregion Using Directives

	public class DefaultStaticResourceRegistryEx : IStaticResourceRegistryEx
	{

		#region Static Variables 

		private static readonly Dictionary<ResourceKey, ResourceHolder> _KeysToResources = new Dictionary<ResourceKey, ResourceHolder>();

		#endregion Static Variables 

		#region Constructors 

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultStaticResourceRegistryEx"/> class.
		/// </summary>
		public DefaultStaticResourceRegistryEx()
		{
			RegisterAssemblyResource( "BehaviourScripts", null, null, "Castle.MonoRail.Framework", "Castle.MonoRail.Framework.JSResources.Behaviour", "jsfunctions", MimeTypes.JavaScript );
			RegisterAssemblyResource( "AjaxScripts", null, null, "Castle.MonoRail.Framework", "Castle.MonoRail.Framework.JSResources.Ajax", "jsfunctions", MimeTypes.JavaScript );
			RegisterAssemblyResource( "FormHelperScript", null, null, "Castle.MonoRail.Framework", "Castle.MonoRail.Framework.JSResources.FormHelper", "jsfunctions", MimeTypes.JavaScript );
			RegisterAssemblyResource( "ZebdaScripts", null, null, "Castle.MonoRail.Framework", "Castle.MonoRail.Framework.JSResources.ZebdaValidation", "jsfunctions", MimeTypes.JavaScript );
			RegisterAssemblyResource( "ValidateCore", null, null, "Castle.MonoRail.Framework", "Castle.MonoRail.Framework.JSResources.Validation", "fValidateCore", MimeTypes.JavaScript );
			RegisterAssemblyResource( "ValidateLang", null, null, "Castle.MonoRail.Framework", "Castle.MonoRail.Framework.JSResources.ValidationLang", "fValidateLang", MimeTypes.JavaScript );
			RegisterAssemblyResource( "ValidateValidators", null, null, "Castle.MonoRail.Framework", "Castle.MonoRail.Framework.JSResources.Validation", "fValidateValidators", MimeTypes.JavaScript );
			RegisterAssemblyResource( "ValidateConfig", null, null, "Castle.MonoRail.Framework", "Castle.MonoRail.Framework.JSResources.Validation", "fValidateConfig", MimeTypes.JavaScript );
			RegisterAssemblyResource( "Effects2", null, null, "Castle.MonoRail.Framework", "Castle.MonoRail.Framework.JSResources.Effects2", "functions", MimeTypes.JavaScript );
			RegisterAssemblyResource( "EffectsFatScripts", null, null, "Castle.MonoRail.Framework", "Castle.MonoRail.Framework.JSResources.EffectsFat", "fatfunctions", MimeTypes.JavaScript );

			RegisterFlagsResources();
		}

		#endregion Constructors 

		#region Private Methods 

		private void AssertParams( string name, IResource resource, string mimeType )
		{
			if ( string.IsNullOrEmpty( name ) )
			{
				throw new ArgumentNullException( "name" );
			}
			if ( string.IsNullOrEmpty( mimeType ) )
			{
				throw new ArgumentNullException( "mimeType" );
			}
			if ( resource == null )
			{
				throw new ArgumentNullException( "resource" );
			}
		}

		private void AssertParams( string name, string assemblyName, string resourceName, string resourceEntry, string mimeType )
		{
			if ( string.IsNullOrEmpty( name ) )
			{
				throw new ArgumentNullException( "name" );
			}
			if ( string.IsNullOrEmpty( mimeType ) )
			{
				throw new ArgumentNullException( "mimeType" );
			}
			if ( string.IsNullOrEmpty( assemblyName ) )
			{
				throw new ArgumentNullException( "assemblyName" );
			}
			if ( string.IsNullOrEmpty( resourceName ) )
			{
				throw new ArgumentNullException( "resourceName" );
			}
			if ( string.IsNullOrEmpty( resourceEntry ) )
			{
				throw new ArgumentNullException( "resourceEntry" );
			}
		}

		private void AssertParams( string name, string assemblyName, string resourceName, string mimeType )
		{
			if ( string.IsNullOrEmpty( name ) )
			{
				throw new ArgumentNullException( "name" );
			}
			if ( string.IsNullOrEmpty( mimeType ) )
			{
				throw new ArgumentNullException( "mimeType" );
			}
			if ( string.IsNullOrEmpty( assemblyName ) )
			{
				throw new ArgumentNullException( "assemblyName" );
			}
			if ( string.IsNullOrEmpty( resourceName ) )
			{
				throw new ArgumentNullException( "resourceName" );
			}
		}

		void RegisterFlagsResources()
		{
			Assembly assembly = Assembly.GetExecutingAssembly();

			string[] resourceNames = assembly.GetManifestResourceNames();

			foreach ( string resourceName in resourceNames )
			{
				if ( resourceName.Contains( "Resources.Flags." ) )
				{
					string cultureCode = resourceName.Substring( 0, resourceName.LastIndexOf( '.' ) );
					cultureCode = cultureCode.Substring( cultureCode.LastIndexOf( '.' ) + 1 );

					RegisterAssemblyResource( 
						"Flags_" + cultureCode, 
						null, 
						null, 
						"Castle.Components.Localization.MonoRail", 
						string.Format( 
							"Castle.Components.Localization.MonoRail.Resources.Flags.{0}.gif", 
							cultureCode ), 
						MimeTypes.Gif );
				}
			}
		}

		#endregion Private Methods

		#region IStaticResourceRegistryEx Members

		public Stream GetResourceStream( string name, string location, string version, out string mimeType )
		{
			ResourceHolder holder;

			if ( !_KeysToResources.TryGetValue( new ResourceKey( name, location, version ), out holder ) )
			{
				throw new ResourceException( "Could not load resource: " + name + " location: " + location + " version: " + version );
			}

			mimeType = holder.MimeType;

			return holder.Stream;
		}

		public void RegisterAssemblyResource( string name, string location, string version, string assemblyName, string resourceName, string mimeType )
		{
			AssertParams( name, assemblyName, resourceName, mimeType );

			CultureInfo invariantCulture = CultureInfo.InvariantCulture;

			if ( ( location != null ) && ( location != "neutral" ) )
			{
				invariantCulture = CultureInfo.CreateSpecificCulture( location );
			}

			IResource resource = new AssemblyResourceEx( new CustomUri( "assembly://" + assemblyName + "/" + resourceName ), invariantCulture );

			_KeysToResources[ new ResourceKey( name, location, version ) ] = new ResourceHolder( resource, mimeType );
		}

		#endregion IStaticResourceRegistryEx Members

		#region IStaticResourceRegistry Members 

		public bool Exists( string name, string location, string version )
		{
			return _KeysToResources.ContainsKey( new ResourceKey( name, location, version ) );
		}

		public string GetResource( string name, string location, string version, out string mimeType )
		{
			ResourceHolder holder;

			if ( !_KeysToResources.TryGetValue( new ResourceKey( name, location, version ), out holder ) )
			{
				throw new ResourceException( "Could not load resource: " + name + " location: " + location + " version: " + version );
			}

			mimeType = holder.MimeType;

			return holder.Content;
		}

		public void RegisterAssemblyResource( string name, string location, string version, string assemblyName, string resourceName, string resourceEntry, string mimeType )
		{
			AssertParams( name, assemblyName, resourceName, resourceEntry, mimeType );
			
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;

			if ( ( location != null ) && ( location != "neutral" ) )
			{
				invariantCulture = CultureInfo.CreateSpecificCulture( location );
			}

			IResource resource = new AssemblyBundleResource( new CustomUri( "assembly://" + assemblyName + "/" + resourceName + "/" + resourceEntry ), invariantCulture );
			
			_KeysToResources[ new ResourceKey( name, location, version ) ] = new ResourceHolder( resource, mimeType );
		}

		public void RegisterCustomResource( string name, string location, string version, IResource resource, string mimeType )
		{
			AssertParams( name, resource, mimeType );

			_KeysToResources[ new ResourceKey( name, location, version ) ] = new ResourceHolder( resource, mimeType );
		}

		#endregion IStaticResourceRegistry Members
 
	}
}
