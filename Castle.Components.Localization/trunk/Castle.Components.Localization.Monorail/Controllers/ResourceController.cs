
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
// See the License for the specific culture governing permissions and
// limitations under the License.

#endregion License

namespace Castle.Components.Localization.Monorail.Controllers
{
	#region Using Directives

	using System;
	using System.Collections;
	using System.Globalization;
	using System.IO;
	using System.Reflection;

	using Castle.MonoRail.Framework;

	#endregion Using Directives

	public partial class ResourceController : Controller
	{

		#region Constant Variables 

		const string DefaultMIME_TYPE = "image/jpeg";

		#endregion Constant Variables 

		#region Instance Variables 

		Helpers.ResourceHelper _ResourceHelper;

		#endregion Instance Variables 

		#region Constructors 

		public ResourceController()
		{
			_ResourceHelper = new Helpers.ResourceHelper();
		}

		#endregion Constructors 

		#region Public Methods 

		/// <summary>
		/// Ouput the image resource specified by its assembly's name and resource's name 
		/// to the current <see cref="IResponse"/>.<see cref="IResponse.OutputStream"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The following HTTP Headers are set :
		/// </para>
		/// <list type="table">
		///		<item>
		///			<term>content-disposition</term>
		///			<description>filename=[resourceName]</description>
		///		</item>
		///		<item>
		///			<term>content-length</term>
		///			<description>The resource's bytes length.</description>
		///		</item>
		///		<item>
		///			<term>content-type</term>
		///			<description><c>image/jpeg</c> (this works for gif and png files too)</description>
		///		</item>
		/// </list>
		/// </remarks>
		/// <param name="assemblyName">The name of the image's resource assembly.</param>
		/// <param name="resourceName">The name of the image's resource.</param>
		public void GetImage( string assemblyName, string resourceName )
		{
			GetImageWithMIME( assemblyName, resourceName, DefaultMIME_TYPE );
		}

		/// <summary>
		/// Ouput the image resource specified by resource's name 
		/// to the current <see cref="IResponse"/>.<see cref="IResponse.OutputStream"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The following HTTP Headers are set :
		/// </para>
		/// <list type="table">
		///		<item>
		///			<term>content-disposition</term>
		///			<description>filename=[resourceName]</description>
		///		</item>
		///		<item>
		///			<term>content-length</term>
		///			<description>The resource's bytes length.</description>
		///		</item>
		///		<item>
		///			<term>content-type</term>
		///			<description><c>image/jpeg</c> (this works for gif and png files too)</description>
		///		</item>
		/// </list>
		/// </remarks>
		/// <param name="resourceName">The name of the image's resource.</param>
		public void GetImage( string resourceName )
		{
			GetImageWithMIME( null, resourceName, DefaultMIME_TYPE );
		}

		/// <summary>
		/// Ouput the image resource specified by its assembly's name and resource's name 
		/// to the current <see cref="IResponse"/>.<see cref="IResponse.OutputStream"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The following HTTP Headers are set :
		/// </para>
		/// <list type="table">
		///		<item>
		///			<term>content-disposition</term>
		///			<description>filename=[resourceName]</description>
		///		</item>
		///		<item>
		///			<term>content-length</term>
		///			<description>The resource's bytes length.</description>
		///		</item>
		///		<item>
		///			<term>content-type</term>
		///			<description>The <paramref name="mimeType"/> value</description>
		///		</item>
		/// </list>
		/// </remarks>
		/// <param name="assemblyName">The name of the image's resource assembly.</param>
		/// <param name="resourceName">The name of the image's resource.</param>
		/// <param name="mimeType">The MIME-TYPE to use.</param>
		public void GetImageWithMIME( string assemblyName, string resourceName, string mimeType )
		{
			CancelLayout();
			CancelView();

			Assembly assemblyFound = null;

			if ( assemblyName == null || assemblyName.Length == 0 )
			{
				assemblyName = _ResourceHelper.GetAssemblyNameFromResourceName( resourceName );

				// Try to find the assembly from its name
				try
				{
					assemblyFound = Assembly.Load( assemblyName );
				}
				catch { }

				// If not found, then the resource did not have a recommended name, so we try to find it 
				// by manually searching parsing the resource's name
				if ( assemblyFound == null )
					assemblyFound = _ResourceHelper.FindAssemblyFromResources( resourceName );
			}
			else
			{
				try
				{
					assemblyFound = Assembly.Load( assemblyName );
				}
				catch { }
			}

			if ( assemblyFound != null )
				GetImageFromAssembly( assemblyFound, resourceName, mimeType );
			else
				Logger.WarnFormat( "ResourceController: cannot find the resource \"{0}\" in the modules assemblies.", resourceName );
		}

		/// <summary>
		/// Ouput the image resource specified by its assembly's name and resource's name 
		/// to the current <see cref="IResponse"/>.<see cref="IResponse.OutputStream"/>.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The following HTTP Headers are set :
		/// </para>
		/// <list type="table">
		///		<item>
		///			<term>content-disposition</term>
		///			<description>filename=[resourceName]</description>
		///		</item>
		///		<item>
		///			<term>content-length</term>
		///			<description>The resource's bytes length.</description>
		///		</item>
		///		<item>
		///			<term>content-type</term>
		///			<description>The <paramref name="mimeType"/> value</description>
		///		</item>
		/// </list>
		/// </remarks>
		/// <param name="resourceName">The name of the image's resource.</param>
		/// <param name="mimeType">The MIME-TYPE to use.</param>
		public void GetImageWithMIME( string resourceName, string mimeType )
		{
			GetImageWithMIME( null, resourceName, mimeType );
		}

		public override void SetEngineContext( IEngineContext engineContext )
		{
			_ResourceHelper.SetContext( engineContext );

			base.SetEngineContext( engineContext );
		}

		#endregion Public Methods 

		#region Private Methods 

		bool GetImageFromAssembly( Assembly assembly, string resourceName, string mimeType )
		{
			Stream stream = null;

			try
			{
				stream = assembly.GetManifestResourceStream( resourceName );
				byte[] bytes = new byte[ stream.Length ];

				stream.Read( bytes, 0, Convert.ToInt32( stream.Length ) );

				Context.Response.Clear();
				Context.Response.AppendHeader( "content-disposition", "filename=" + resourceName );
				Context.Response.AppendHeader( "content-Length", bytes.Length.ToString() );
				Context.Response.ContentType = mimeType;

				Context.Response.BinaryWrite( bytes );

				return true;
			}
			catch( Exception ex )
			{
				Logger.WarnFormat( ex, "ResourceController: cannot find the resource \"{0}\" in the assembly \"{1}\".", resourceName, assembly.FullName );
			}
			finally
			{
				if( stream != null )
					stream.Close();
			}

			return false;
		}

		#endregion Private Methods 

	}
}
