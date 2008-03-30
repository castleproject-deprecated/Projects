
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
	using System.IO;
	using System.Web;

	using Castle.Core;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Services;

	#endregion Using Directives

	public class ResourceFileHandlerEx : IHttpHandler
	{

		#region Instance Variables 

		readonly IStaticResourceRegistryEx _StaticResourceRegistryEx;
		readonly UrlInfo _UrlInfo;
		readonly IServiceProviderEx _ServiceProvider;

		#endregion Instance Variables 

		#region Properties 

		/// <summary>
		/// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
		/// </summary>
		/// <value></value>
		/// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}

		#endregion Properties 

		#region Constructors 

		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceFileHandlerEx"/> class.
		/// </summary>
		/// <param name="urlInfo">The current URL info.</param>
		/// <param name="staticResourceRegistryEx">The <see cref="IStaticResourceRegistryEx"/> service.</param>
		public ResourceFileHandlerEx( UrlInfo urlInfo, IStaticResourceRegistryEx staticResourceRegistryEx )
		{
			_UrlInfo = urlInfo;
			_StaticResourceRegistryEx = staticResourceRegistryEx;
		}

		#endregion Constructors 

		#region Public Methods 

		public void ProcessRequest( HttpContext context )
		{
			string name = context.Request.QueryString[ "name" ];
			string location = context.Request.QueryString[ "location" ];
			string version = context.Request.QueryString[ "version" ];

			if ( name == null )
			{
				name = _UrlInfo.Action;
			}
			try
			{
				if ( !_StaticResourceRegistryEx.Exists( name, location, version ) )
				{
					context.Response.StatusCode = 0x194;
				}
				else
				{
					string mimeType;
					Stream resourceStream = null;

					try
					{
						resourceStream = _StaticResourceRegistryEx.GetResourceStream( name, location, version, out mimeType );

						if ( resourceStream != null )
						{
							byte[] bytes = new byte[ resourceStream.Length ];

							resourceStream.Read( bytes, 0, Convert.ToInt32( resourceStream.Length ) );

							context.Response.Clear();
							context.Response.AppendHeader( "content-disposition", "filename=" + name );
							context.Response.AppendHeader( "content-Length", bytes.Length.ToString() );
							context.Response.ContentType = mimeType;

							context.Response.OutputStream.Write( bytes, 0, bytes.Length );
						}
						else
						{
							string resourceContent = _StaticResourceRegistryEx.GetResource( name, location, version, out mimeType );

							context.Response.ContentType = mimeType;
							context.Response.Write( resourceContent );
						}
					}
					finally
					{
						if ( resourceStream != null )
							resourceStream.Close();
					}
				}
			}
			catch ( Exception )
			{
				context.Response.StatusCode = 500;
				throw;
			}
		}

		#endregion Public Methods 

	}
}
