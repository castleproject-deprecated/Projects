
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

namespace Castle.Components.Localization.Monorail.Helpers
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Text;
	using System.Reflection;

	#endregion Using Directives

	public static class ResourcesUtils
	{

		#region Public Methods (2) 

		public static Assembly FindAssemblyFromResources( string resourceName )
		{
			bool continueSearch = true;
			Assembly result = null;
			string assemblyNameFromResource = GetAssemblyNameFromResourceName( resourceName );

			while ( continueSearch )
			{
				try
				{
					Assembly assembly = Assembly.Load( assemblyNameFromResource );

					string[] resources = assembly.GetManifestResourceNames();

					foreach ( string resource in resources )
					{
						if ( resource == resourceName )
						{
							result = assembly;
							continueSearch = false;
							break;
						}
					}
				}
				catch 
				{
					// Try to remove last part of the current name 
					assemblyNameFromResource = assemblyNameFromResource.Substring( 0, assemblyNameFromResource.LastIndexOf( '.' ) );
				}
			}


			return result;
		}

		public static string GetAssemblyNameFromResourceName( string resourceName )
		{
			// First, get the file name without the extension
			string result = resourceName.Substring( 0, resourceName.LastIndexOf( '.' ) );

			// Now, remove the file name
			result = result.Substring( 0, result.LastIndexOf( '.' ) );

			// If the resourceName contains the ".resources" word, removes it
			result = result.Replace( ".resources", "" );

			// We should have the assembly's name
			return result;
		}

		#endregion Public Methods 

	}
}
