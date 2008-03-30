
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
	using System.ComponentModel;
	using System.Collections.Generic;
	using System.Resources;
	using System.Globalization;
	using System.Reflection;

	#endregion Using Directives

	/// <summary>
	///     The LocalizedEnumConverter can convert enumeration values to localized strings.
	/// </summary>
	/// <remarks>
	///	<para>
	///		The LocalizedEnumConverter can uses the <see cref="LocalizedEnumConverterDetailsAttribute"/> to find 
	///		the correct <see cref="ResourceManager"/>. If the targeted enumeration does not have a <see cref="LocalizedEnumConverterDetailsAttribute"/>, 
	///		it will try to find a <see cref="ResourceManager"/> with the same name as the enumeration's type in the enumeration's assembly. 
	///		If no <see cref="ResourceManager"/> is found, it will try to find one named "Enumerations" and finally one named "Strings". 
	/// </para>
	/// <para>
	///		The path of the resource don't have any importance as only the last part of the resources names is checked. 
	///		This mean you can have an enumeration named <c>MyAssembly.MyNamespace.MyEnumeration</c> and a resource file named 
	///		<c>MyAssembly.MyNamespace.Resources.MyEnumeration</c> for example. You won't have to configure anything for this to work.
	/// </para>
	/// <para>
	///		The <see cref="LocalizedEnumConverterDetailsAttribute"/> allows to defines the naming pattern to use to find the resource for an enumeration value.
	///		The default pattern is <see cref="LocalizedEnumNamingPattern.EnumerationValueName"/> if the resource file has the same name as the enumeration's type; 
	///		otherwise, the pattern used will be <see cref="LocalizedEnumNamingPattern.EnumerationName_EnumerationValueName"/>.
	/// </para>
	/// <para>
	///	If no <see cref="ResourceManager"/> is found, no exception will be thrown but the enumerations will not be translated.
	///	The string representation enumeration's value returned will simply be Enum.ToString(). 	
	/// </para>
	/// </remarks>
	public class LocalizedEnumConverter : EnumConverter
	{

		#region Instance Variables 

		Array _FlagValues;
		bool _IsFlagEnum = false;
		Dictionary<CultureInfo, Dictionary<string, object>> _LookupTables = new Dictionary<CultureInfo, Dictionary<string, object>>();
		LocalizedEnumNamingPattern _NamingPattern = LocalizedEnumNamingPattern.EnumerationValueName;
		ResourceManager _ResourceManager;

		#endregion Instance Variables 

		#region Constructors 

		/// <summary>
		/// Initializes a new instance of the <see cref="LocalizedEnumConverter"/> class.
		/// </summary>
		/// <param name="type">A <see cref="T:System.Type"></see> that represents the type of enumeration to associate with this enumeration converter.</param>
		public LocalizedEnumConverter( Type type )
			: this( type, null )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LocalizedEnumConverter"/> class.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="resourceManager">The resource manager to use to get translated enumeration names.</param>
		public LocalizedEnumConverter( Type type, ResourceManager resourceManager )
			: base( type )
		{
			if( resourceManager == null )
			{
				// 1. Check if the Type has a LocalizedEnumConverterDetailsAttribute
				object[] attributes = type.GetCustomAttributes( typeof( LocalizedEnumConverterDetailsAttribute ), true );

				if( attributes.Length > 0 )
				{
					LocalizedEnumConverterDetailsAttribute detailsAttribute = attributes[ 0 ] as LocalizedEnumConverterDetailsAttribute;

					if( detailsAttribute != null )
					{
						_NamingPattern = detailsAttribute.NamingPattern;

						if( detailsAttribute.AssemblyName == null )
						{
							_ResourceManager = new ResourceManager( detailsAttribute.ResourceManagerName, type.Assembly );
						}
						else
						{
							_ResourceManager = new ResourceManager(
								detailsAttribute.ResourceManagerName,
								Assembly.Load( detailsAttribute.AssemblyName ) );
						}
					}
				}
				else
				{
					// Try to find a ResourceManager for this enumeration, get one named with the enumeration Type's name, 
					// then one named Enumerations and finally one named Strings.
					Assembly assembly = type.Assembly;

					string[] resourceNames = assembly.GetManifestResourceNames();

					foreach( string resourceName in resourceNames )
					{
						string baseName = resourceName.Replace( ".resources", "" );

						if( baseName.EndsWith( "." + type.Name ) )
						{
							_ResourceManager = new ResourceManager( baseName, type.Assembly );
						}
						else if( baseName.EndsWith( ".Enumerations" ) )
						{
							_ResourceManager = new ResourceManager( baseName, type.Assembly );
						}
						else if( baseName.EndsWith( "Strings" ) )
						{
							_ResourceManager = new ResourceManager( baseName, type.Assembly );
						}
					}
				}
			}
			else
			{
				_ResourceManager = resourceManager;
			}

			object[] flagAttributes = type.GetCustomAttributes( typeof( FlagsAttribute ), true );

			_IsFlagEnum = flagAttributes.Length > 0;

			if( _IsFlagEnum )
			{
				_FlagValues = Enum.GetValues( type );
			}
		}

		#endregion Constructors 

		#region Static Methods 

		/// <summary>
		/// Converts the specified enumeration value to its string representation using the 
		/// <see cref="TypeConverter"/> specified for the enumeration's type.
		/// </summary>
		/// <param name="value">The enumeration value.</param>
		/// <returns>The string representation of the enumeration value.</returns>
		public static string ConvertValueToString( Enum value )
		{
			TypeConverter converter = TypeDescriptor.GetConverter( value.GetType() );
			return converter.ConvertToString( value );
		}

		/// <summary>
		/// Converts the specified enumeration value to its string representation using the 
		/// <see cref="TypeConverter"/> specified for the enumeration's type.
		/// </summary>
		/// <param name="enumType">The enumeration's <see cref="Type"/>.</param>
		/// <param name="value">The enumeration value.</param>
		/// <returns>The string representation of the enumeration value.</returns>
		public static string ConvertValueToString( Type enumType, object value )
		{
			TypeConverter converter = TypeDescriptor.GetConverter( enumType );
			object enumValue = Enum.Parse( enumType, value.ToString() );
			return converter.ConvertToString( enumValue );
		}

		/// <summary>
		/// Gets the a <see cref="List"/> containing the values of the specified enumeration as the list's keys and their 
		/// string representation as the list's values for the specified culture.
		/// </summary>
		/// <param name="enumType">The enumeration's <see cref="Type"/>.</param>
		/// <param name="culture">The culture to use.</param>
		/// <returns>A <see cref="List"/> containing the enumeration values as keys and their string representation as values.</returns>
		public static List<KeyValuePair<Enum, string>> GetValues( Type enumType, CultureInfo culture )
		{
			List<KeyValuePair<Enum, string>> result = new List<KeyValuePair<Enum, string>>();
			TypeConverter converter = TypeDescriptor.GetConverter( enumType );

			foreach( Enum value in Enum.GetValues( enumType ) )
			{
				KeyValuePair<Enum, string> pair = new KeyValuePair<Enum, string>( value, converter.ConvertToString( null, culture, value ) );
				result.Add( pair );
			}

			return result;
		}

		/// <summary>
		/// Gets the a <see cref="List"/> containing the values of the specified enumeration as the list's keys and their 
		/// string representation as the list's values for the current UI culture.
		/// </summary>
		/// <param name="enumType">The enumeration's <see cref="Type"/>.</param>
		/// <returns>A <see cref="List"/> containing the enumeration values as keys and their string representation as values.</returns>
		public static List<KeyValuePair<Enum, string>> GetValues( Type enumType )
		{
			return GetValues( enumType, CultureInfo.CurrentUICulture );
		}

		/// <summary>
		/// Gets the a <see cref="List"/> containing the values of the specified enumeration as the list's values and their 
		/// string representation as the list's keys for the specified culture.
		/// </summary>
		/// <param name="enumType">The enumeration's <see cref="Type"/>.</param>
		/// <param name="culture">The culture to use.</param>
		/// <returns>A <see cref="List"/> containing the enumeration values as keys and their string representation as values.</returns>
		public static List<KeyValuePair<string, Enum>> GetValuesWithTextAsKey( Type enumType, CultureInfo culture )
		{
			List<KeyValuePair<string, Enum>> result = new List<KeyValuePair<string, Enum>>();
			TypeConverter converter = TypeDescriptor.GetConverter( enumType );

			foreach( Enum value in Enum.GetValues( enumType ) )
			{
				KeyValuePair<string, Enum> pair = new KeyValuePair<string, Enum>( converter.ConvertToString( null, culture, value ), value );
				result.Add( pair );
			}

			return result;
		}

		/// <summary>
		/// Gets the a <see cref="List"/> containing the values of the specified enumeration as the list's values and their 
		/// string representation as the list's keys for the current UI culture.
		/// </summary>
		/// <param name="enumType">The enumeration's <see cref="Type"/>.</param>
		/// <returns>A <see cref="List"/> containing the enumeration values as keys and their string representation as values.</returns>
		public static List<KeyValuePair<string, Enum>> GetValuesWithTextAsKey( Type enumType )
		{
			return GetValuesWithTextAsKey( enumType, CultureInfo.CurrentUICulture );
		}

		#endregion Static Methods 

		#region Public Methods 

		/// <summary>
		/// Converts the specified value object to an enumeration object.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
		/// <param name="culture">An optional <see cref="T:System.Globalization.CultureInfo"></see>. If not supplied, the current culture is assumed.</param>
		/// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
		/// <returns>
		/// An <see cref="T:System.Object"></see> that represents the converted value.
		/// </returns>
		/// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
		/// <exception cref="T:System.FormatException">value is not a valid value for the target type. </exception>
		public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
		{
			if( value is string )
			{
				object result = null;

				if( _IsFlagEnum )
				{
					result = GetFlagValue( culture, ( string )value );
				}
				else
				{
					result = GetValue( culture, ( string )value );
				}

				if( result == null )
				{
					result = base.ConvertFrom( context, culture, value );
				}

				return result;
			}
			else
			{
				return base.ConvertFrom( context, culture, value );
			}
		}

		/// <summary>
		/// Converts the given value object to the specified destination type.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that provides a format context.</param>
		/// <param name="culture">An optional <see cref="T:System.Globalization.CultureInfo"></see>. If not supplied, the current culture is assumed.</param>
		/// <param name="value">The <see cref="T:System.Object"></see> to convert.</param>
		/// <param name="destinationType">The <see cref="T:System.Type"></see> to convert the value to.</param>
		/// <returns>
		/// An <see cref="T:System.Object"></see> that represents the converted value.
		/// </returns>
		/// <exception cref="T:System.ArgumentException">value is not a valid value for the enumeration. </exception>
		/// <exception cref="T:System.ArgumentNullException">destinationType is null. </exception>
		/// <exception cref="T:System.NotSupportedException">The conversion cannot be performed. </exception>
		public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType )
		{
			if( value != null && destinationType == typeof( string ) )
			{
				object result = null;

				if( _IsFlagEnum )
				{
					result = GetFlagValueText( culture, value ); 
				}
				else
				{
					result = GetValueText( culture, value );
				}

				return result;
			}
			else
			{
				return base.ConvertTo( context, culture, value, destinationType );
			}
		}

		#endregion Public Methods 

		#region Private Methods 

		/// <summary>
		/// Gets the enumeration value matching the specified string for a flagged enumeration.
		/// </summary>
		/// <param name="culture">The culture used for the <paramref name="text"/> parameter.</param>
		/// <param name="text">The enumeration value as a localized string (comma separated list of each enumeration values).</param>
		/// <returns>The enumeration value if found; otherwise <see langword="null"/>.</returns>
		private object GetFlagValue( CultureInfo culture, string text )
		{
			Dictionary<string, object> lookupTable = GetLookupTable( culture );
			string[] textValues = text.Split( ',' );
			ulong result = 0;

			foreach( string textValue in textValues )
			{
				object value = null;

				if( !lookupTable.TryGetValue( textValue.Trim(), out value ) )
				{
					return null;
				}

				result |= Convert.ToUInt32( value );
			}

			return Enum.ToObject( EnumType, result );
		}

		/// <summary>
		/// Gets the string representation of the specified flag value as a comma separated list of the 
		/// string representations of each values which compose it.
		/// </summary>
		/// <param name="culture">The culture to use.</param>
		/// <param name="value">The flag value.</param>
		/// <returns>The string representation of the specified flag value</returns>
		private string GetFlagValueText( CultureInfo culture, object flagValue )
		{
			// if the flag value is a single one, just get its string representation
			if( Enum.IsDefined( flagValue.GetType(), flagValue ) )
			{
				return GetValueText( culture, flagValue );
			}


			// Otherwise, get each values which compose it and create a comma separated list of their string representation
			ulong flagValueAsLong = Convert.ToUInt32( flagValue );
			string result = null;

			foreach( object value in _FlagValues )
			{
				ulong valueAsLong = Convert.ToUInt32( value );

				if( IsSingleBitValue( valueAsLong ) )
				{
					if( ( valueAsLong & flagValueAsLong ) == valueAsLong )
					{
						string valueText = GetValueText( culture, value );

						if( result == null )
						{
							result = valueText;
						}
						else
						{
							result = string.Format( "{0}, {1}", result, valueText );
						}
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Get the lookup table for the given culture (creating it if necessary).
		/// </summary>
		/// <param name="culture">The culture for which to get the lookup table.</param>
		/// <returns>The lookup table for the given culture.</returns>
		private Dictionary<string, object> GetLookupTable( CultureInfo culture )
		{
			Dictionary<string, object> result = null;

			if( culture == null )
			{
				culture = CultureInfo.CurrentCulture;
			}

			if( !_LookupTables.TryGetValue( culture, out result ) )
			{
				result = new Dictionary<string, object>();

				foreach( object value in GetStandardValues() )
				{
					string text = GetValueText( culture, value );

					if( text != null )
					{
						result.Add( text, value );
					}
				}

				_LookupTables.Add( culture, result );
			}

			return result;
		}

		string GetResourceName( object value, Type enumerationType )
		{
			switch ( _NamingPattern )
			{
				case LocalizedEnumNamingPattern.EnumerationValueName:
					return value.ToString();
					break;

				case LocalizedEnumNamingPattern.EnumerationName_EnumerationValueName:
					return string.Format( "{0}_{1}", enumerationType.Name, value );
					break;

				default:
					return value.ToString();
					break;
			}
		}

		/// <summary>
		/// Gets the enumeration value matching the specified string for a simple (non-flagged enumeration).
		/// </summary>
		/// <param name="culture">The culture used for the <paramref name="text"/> parameter.</param>
		/// <param name="text">The enumeration value as a localized string.</param>
		/// <returns>The enumeration value if found; otherwise <see langword="null"/>.</returns>
		private object GetValue( CultureInfo culture, string text )
		{
			Dictionary<string, object> lookupTable = GetLookupTable( culture );
			object result = null;
			lookupTable.TryGetValue( text, out result );

			return result;
		}

		/// <summary>
		/// Gets the string representation of the specified enumeration's value for the specified culture.
		/// </summary>
		/// <param name="culture">The culture to use.</param>
		/// <param name="value">The enumeration's value.</param>
		/// <returns>The string representation for the specified culture.</returns>
		private string GetValueText( CultureInfo culture, object value )
		{
			string result = null;
			Type type = value.GetType();
			string resourceName = GetResourceName( value, type );
			CultureInfo cultureInfo = culture;

			if( !cultureInfo.IsNeutralCulture && cultureInfo.Parent != null )
				cultureInfo = cultureInfo.Parent;
			
			if( _ResourceManager != null )
				result = _ResourceManager.GetString( resourceName, cultureInfo );

			if( result == null )
				result = value.ToString();

			return result;
		}

		/// <summary>
		/// Determines whether the given value can be represented by a single bit.
		/// </summary>
		/// <param name="value">The enumeration value as a long.</param>
		/// <returns><see langword="true"/> if the value can be represented by a single bit; otherwise <see langword="false"/></returns>
		private bool IsSingleBitValue( ulong value )
		{
			bool result = false;

			switch( value )
			{
				case 0:
					result = false;
					break;
				case 1:
					result = true;
					break;
				default:
					result = ( ( value & ( value - 1 ) ) == 0 );
					break;
			}

			return result;
		}

		#endregion Private Methods 

	}

}
