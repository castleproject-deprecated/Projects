
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

namespace Castle.Components.Localization.MonoRail.Helpers
{
	#region Using Directives

	using System;
	using System.Reflection;
	using System.Text.RegularExpressions;

	using Castle.Core;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Services;

	#endregion Using Directives

	public class ResourceHelper : AbstractHelper, IServiceEnabledComponent
	{

		#region Constant Variables 

		const string BaseResourceUrl = "/MonoRail/Files/{0}.{1}";

		#endregion Constant Variables 

		#region Instance Variables 

		ILogger _Logger;

		#endregion Instance Variables 

		#region Properties 

		public ILogger Logger
		{
			get { return _Logger; }
			set { _Logger = value; }
		}

		#endregion Properties 

		#region Constructors 

		public ResourceHelper()
		{
		}

		public ResourceHelper( IEngineContext engineContext )
			: base( engineContext )
		{
		}

		#endregion Constructors 

		#region Public Methods 

		public string GetImageResourceUrl( string resourceName )
		{
			if ( resourceName != null )
			{
				return string.Format( BaseResourceUrl, resourceName, Context.UrlInfo.Extension );
			}
			else
			{
				Logger.WarnFormat( "ResourceHelper: The view for the action \"{0}\" of the controller \"{1}\" has requested a link to an image resource but did not specified the resource name.", Context.CurrentControllerContext.Action, Context.CurrentControllerContext.Name );
				return string.Empty;
			}
		}

		#endregion Public Methods 

		#region Private Methods 

		bool HasResource( Assembly assembly, string resourceName )
		{
			string[] resources = assembly.GetManifestResourceNames();

			foreach( string resource in resources )
			{
				if( string.Compare( resource, resourceName, StringComparison.InvariantCultureIgnoreCase ) == 0 )
					return true;
			}

			return false;
		}

		#endregion Private Methods 


		#region IServiceEnabledComponent Members 

		public void Service( IServiceProvider provider )
		{
			_Logger = provider.GetService( typeof( ILogger ) ) as ILogger;
		}

		#endregion IServiceEnabledComponent Members

	}
}
