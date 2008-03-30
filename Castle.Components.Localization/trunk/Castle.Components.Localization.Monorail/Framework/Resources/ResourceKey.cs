
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

	#endregion Using Directives

	public class ResourceKey : IEquatable<ResourceKey>
	{

		#region Instance Variables 

		string _Location;
		string _Name;
		string _Version;

		#endregion Instance Variables 

		#region Constructors 

		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceKey"/> class.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="version">The version.</param>
		public ResourceKey( string name, string location, string version )
		{
			_Name = name;
			_Location = location;
			_Version = version;
		}

		#endregion Constructors 

		#region Public Methods 

		public bool Equals( ResourceKey resourceKey )
		{
			if ( resourceKey == null )
			{
				return false;
			}
			if ( string.Compare( _Name, resourceKey._Name, StringComparison.InvariantCultureIgnoreCase ) != 0 )
			{
				return false;
			}
			if ( string.Compare( _Location, resourceKey._Location, StringComparison.InvariantCultureIgnoreCase ) != 0 )
			{
				return false;
			}
			if ( string.Compare( _Version, resourceKey._Version, StringComparison.InvariantCultureIgnoreCase ) != 0 )
			{
				return false;
			}
			return true;
		}

		public override int GetHashCode()
		{
			int hashCode = _Name.ToLowerInvariant().GetHashCode();
			hashCode = ( 0x1d * hashCode ) + ( ( _Location != null ) ? _Location.ToLowerInvariant().GetHashCode() : 0 );
			return ( ( 0x1d * hashCode ) + ( ( _Version != null ) ? _Version.ToLowerInvariant().GetHashCode() : 0 ) );
		}

		#endregion Public Methods 

	}
}
