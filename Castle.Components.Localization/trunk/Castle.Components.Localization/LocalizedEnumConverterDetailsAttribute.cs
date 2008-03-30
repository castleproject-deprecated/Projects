
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
	using System.Resources;

	#endregion Using Directives

	/// <summary>
	/// This attribute can be used to specify to the <see cref="LocalizedEnumConverter"/> 
	/// which <see cref="ResourceManager"/> to use and optionnaly, in which assembly to get it.
	/// </summary>
	public class LocalizedEnumConverterDetailsAttribute : Attribute
	{

		#region Instance Variables 

		string _AssemblyName;
		LocalizedEnumNamingPattern _NamingPattern;
		string _ResourceManagerName;

		#endregion Instance Variables 

		#region Properties 

		/// <summary>
		/// <para>
		/// Gets the name of the assembly which contains the resource manager. 
		/// </para>
		/// <para>
		/// This is optional, by default, the enumeration's assembly will be used.
		/// </para>
		/// </summary>
		/// <value>The name of the assembly.</value>
		public string AssemblyName
		{
			get { return _AssemblyName; }
		}

		/// <summary>
		/// Gets or sets the <see cref="LocalizedEnumNamingPattern"/> to use to find the resource for an enumeration value.
		/// </summary>
		/// <value>The naming pattern.</value>
		public LocalizedEnumNamingPattern NamingPattern 
		{
			get { return _NamingPattern; }
			set { _NamingPattern = value; }
		}

		/// <summary>
		/// Gets the name of the <see cref="ResourceManager"/>.
		/// </summary>
		/// <value>The name of the <see cref="ResourceManager"/>.</value>
		public string ResourceManagerName
		{
			get { return _ResourceManagerName; }
		}

		#endregion Properties 

		#region Constructors 

		/// <summary>
		/// Initializes a new instance of the <see cref="T:LocalizedEnumConverterDetailsAttribute"/> class.
		/// </summary>
		/// <param name="resourceManagerName">The name of the <see cref="ResourceManager"/>.</param>
		public LocalizedEnumConverterDetailsAttribute( string resourceManagerName )
			: this( null, resourceManagerName )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:LocalizedEnumConverterDetailsAttribute"/> class.
		/// </summary>
		/// <param name="assemblyName">The name of the assembly which contains the resource manager.</param>
		/// <param name="resourceManagerName">The name of the <see cref="ResourceManager"/>.</param>
		public LocalizedEnumConverterDetailsAttribute( string assemblyName, string resourceManagerName )
			: this( assemblyName, resourceManagerName, LocalizedEnumNamingPattern.EnumerationValueName )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:LocalizedEnumConverterDetailsAttribute"/> class.
		/// </summary>
		/// <param name="assemblyName">The name of the assembly which contains the resource manager.</param>
		/// <param name="resourceManagerName">The name of the <see cref="ResourceManager"/> to use.</param>
		/// <param name="namingPattern">The <see cref="LocalizedEnumNamingPattern"/> to use.</param>
		public LocalizedEnumConverterDetailsAttribute( string assemblyName, string resourceManagerName, LocalizedEnumNamingPattern namingPattern )
		{
			if ( resourceManagerName == null )
				throw new ArgumentNullException( "resourceManagerName" );

			if ( resourceManagerName.Length == 0 )
				throw new ArgumentException( "resourceManagerName", "The resourceManagerName cannot be empty." );

			_AssemblyName = assemblyName;
			_ResourceManagerName = resourceManagerName;
			_NamingPattern = namingPattern;
		}

		#endregion Constructors 

	}
}
