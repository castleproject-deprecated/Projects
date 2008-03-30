
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

namespace Castle.Components.Localization
{
	#region Using Directives

	using System;

	#endregion Using Directives

	/// <summary>
	/// An enumeration of the supported naming pattern for finding the resources targeting an enumeration.
	/// </summary>
	public enum LocalizedEnumNamingPattern
	{
		/// <summary>
		/// The resource's key for the enumeration value must be the enumeration value's name
		/// </summary>
		EnumerationValueName,

		/// <summary>
		/// <para>
		/// The resource's key for the enumeration value must follow the pattern <c>EnumerationName</c>_<c>EnumerationValueName</c>.
		/// </para>
		/// <para>
		/// For example : <c>MyEnumeration_Value1</c>.
		/// </para>
		/// </summary>
		EnumerationName_EnumerationValueName
	}
}
