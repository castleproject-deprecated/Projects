
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

namespace Castle.Components.Localization.MonoRail.Helpers
{
	#region Using Directives

	using System;
	using System.Collections;
	using System.Collections.Generic;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;

	using Castle.Components.Localization;
	using Castle.MonoRail.Framework.Internal;

	#endregion Using Directives

	public class LocalizedFormHelper : FormHelper
	{

		#region Constructors 

		public LocalizedFormHelper()
		{
		}

		public LocalizedFormHelper( IEngineContext engineContext )
			: base( engineContext )
		{
		}

		#endregion Constructors 

		#region Dates Format Methods 

		public string AlternativeFriendlyFormatFromNow( DateTime date )
		{
			TimeSpan nowSpan = new TimeSpan( DateTime.Now.Ticks );
			TimeSpan dateSpan = new TimeSpan( date.Ticks );

			TimeSpan negativeDifferenceSpan = nowSpan.Subtract( dateSpan );
			TimeSpan positiveDifferenceSpan = dateSpan.Subtract( nowSpan );

			if ( ( negativeDifferenceSpan.TotalHours >= 0 &&
					negativeDifferenceSpan.TotalHours <= 24.0 ) ||
				 ( positiveDifferenceSpan.TotalHours >= 0 &&
					positiveDifferenceSpan.TotalHours <= 24.0 ) )
				return Resources.Date.Today;

			if ( negativeDifferenceSpan.TotalHours > 0 &&
				negativeDifferenceSpan.TotalHours <= 48.0 )
				return Resources.Date.Yesterday;

			if ( negativeDifferenceSpan.TotalDays > 0 &&
				negativeDifferenceSpan.TotalDays <= 40.0 )
				return string.Format( Resources.Date.XDaysAgo, negativeDifferenceSpan.Days, "s" );

			if ( positiveDifferenceSpan.TotalHours > 0 &&
				positiveDifferenceSpan.TotalHours <= 48.0 )
				return Resources.Date.Tomorrow;

			if ( positiveDifferenceSpan.TotalDays > 0 && 
				positiveDifferenceSpan.TotalDays <= 40.0 )
				return string.Format( Resources.Date.InXDays, positiveDifferenceSpan.Days, "s" );

			if ( negativeDifferenceSpan.TotalDays > 0 && 
				negativeDifferenceSpan.TotalDays > 40.0 )
				return string.Format( Resources.Date.XMonthsAgo, negativeDifferenceSpan.Days / 30, ( negativeDifferenceSpan.Days / 30 ) > 1 ? "s" : "" );
			else
				return string.Format( Resources.Date.InXMonths, positiveDifferenceSpan.Days / 30, ( positiveDifferenceSpan.Days / 30 ) > 1 ? "s" : "" );
		}

		public string FriendlyFormatFromNow( DateTime date )
		{
			TimeSpan nowSpan = new TimeSpan( DateTime.Now.Ticks );
			TimeSpan dateSpan = new TimeSpan( date.Ticks );

			TimeSpan negativeDifferenceSpan = nowSpan.Subtract( dateSpan );
			TimeSpan positiveDifferenceSpan = dateSpan.Subtract( nowSpan );

			if ( negativeDifferenceSpan.TotalSeconds == 0.0 &&
				positiveDifferenceSpan.TotalSeconds == 0.0 )
				return Resources.Date.JustNow;

			if ( negativeDifferenceSpan.Days > 0 )
				return string.Format( Resources.Date.XDaysAgo, negativeDifferenceSpan.Days, ( negativeDifferenceSpan.Days > 1 ) ? "s" : string.Empty );

			if ( negativeDifferenceSpan.Hours > 0 )
				return string.Format( Resources.Date.XHoursAgo, negativeDifferenceSpan.Hours, ( negativeDifferenceSpan.Hours > 1 ) ? "s" : string.Empty );

			if ( negativeDifferenceSpan.Seconds > 0 )
				return string.Format( Resources.Date.XSecondsAgo, negativeDifferenceSpan.Seconds, ( negativeDifferenceSpan.Seconds > 1 ) ? "s" : string.Empty );

			if ( negativeDifferenceSpan.Minutes > 0 )
				return string.Format( Resources.Date.XMinutesAgo, negativeDifferenceSpan.Minutes, ( negativeDifferenceSpan.Minutes > 1 ) ? "s" : string.Empty );

			if ( positiveDifferenceSpan.Days > 0 )
				return string.Format( Resources.Date.InXDays, positiveDifferenceSpan.Days, ( positiveDifferenceSpan.Days > 1 ) ? "s" : string.Empty );

			if ( positiveDifferenceSpan.Hours > 0 )
				return string.Format( Resources.Date.InXHours, positiveDifferenceSpan.Hours, ( positiveDifferenceSpan.Hours > 1 ) ? "s" : string.Empty );

			if ( positiveDifferenceSpan.Seconds > 0 )
				return string.Format( Resources.Date.InXSeconds, positiveDifferenceSpan.Seconds, ( positiveDifferenceSpan.Seconds > 1 ) ? "s" : string.Empty );

			return string.Format( Resources.Date.InXMinutes, positiveDifferenceSpan.Minutes, ( positiveDifferenceSpan.Minutes > 1 ) ? "s" : string.Empty );
		}

		public string ToShortDate( DateTime? date )
		{
			if ( !date.HasValue )
				return string.Empty;

			return date.Value.ToShortDateString();
		}

		public string ToShortDateTime( DateTime? date )
		{
			if ( !date.HasValue )
				return string.Empty;

			return ( date.Value.ToShortDateString() + " " + date.Value.ToShortTimeString() );
		}

		#endregion Dates Format Methods

		#region Select For Enumeration Methods 

		/// <summary>
		/// Generates an HTML SELECT element and populates it with the values of the specified enumeration.
		/// If this enumeration uses the <see cref="LocalizedEnumConverter"/>, the text diplayed for each value will 
		/// be taken from the localized resources; otherwise, the name of enumeration value will be displayed. 
		/// </summary>
		/// <remarks>
		/// Please do not specify the <c>text</c> and <c>value</c> attributes as they will be overriden.
		/// </remarks>
		/// <param name="target">The target.</param>
		/// <param name="enumeration">The enumeration.</param>
		/// <returns></returns>
		public string Select( string target, Type enumeration )
		{
			return this.Select( target, enumeration, null );
		}

		/// <summary>
		/// Generates an HTML SELECT element and populates it with the values of the specified enumeration.
		/// If this enumeration uses the <see cref="LocalizedEnumConverter"/>, the text diplayed for each value will 
		/// be taken from the localized resources; otherwise, the name of enumeration value will be displayed. 
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="enumeration">The enumeration.</param>
		/// <param name="attributes">The attributes.</param>
		/// <returns></returns>
		public string Select( string target, Type enumeration, IDictionary attributes )
		{
			target = this.RewriteTargetIfWithinObjectScope( target );

			object selectedValue = this.ObtainValue( target );

			return this.Select( target, selectedValue, enumeration, attributes );
		}

		/// <summary>
		/// Generates an HTML SELECT element and populates it with the values of the specified enumeration.
		/// If this enumeration uses the <see cref="LocalizedEnumConverter"/>, the text diplayed for each value will 
		/// be taken from the localized resources; otherwise, the name of enumeration value will be displayed. 
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="selectedValue">The enumeration value which will be pre-selected.</param>
		/// <param name="enumeration">The enumeration's type.</param>
		/// <param name="attributes">The attributes.</param>
		/// <returns></returns>
		public string Select( string target, object selectedValue, Type enumeration, IDictionary attributes )
		{
			Hashtable dataSource = new Hashtable();

			List<KeyValuePair<string, Enum>> values = LocalizedEnumConverter.GetValuesWithTextAsKey( enumeration );

			if ( attributes != null )
			{
				// Makes sure the text and value are correct by always 
				// override them
				CommonUtils.ObtainEntryAndRemove( attributes, "text" );
				CommonUtils.ObtainEntryAndRemove( attributes, "value" );

				attributes.Add( "text", "Key" );
				attributes.Add( "value", "Value" );
			}
			else
				attributes = DictHelper.CreateN( "text", "Key" ).N( "value", "Value" );

			return this.GenerateSelect( target, selectedValue, values, attributes );
		}

		/// <summary>
		/// Generates an HTML SELECT element and populates it with the values of the specified enumeration.
		/// If this enumeration uses the <see cref="LocalizedEnumConverter"/>, the text diplayed for each value will 
		/// be taken from the localized resources; otherwise, the name of enumeration value will be displayed. 
		/// </summary>
		/// <remarks>
		/// Please do not specify the <c>text</c> and <c>value</c> attributes as they will be overriden.
		/// </remarks>
		/// <param name="target">The target.</param>
		/// <param name="enumerationTypeName">The enumeration's type's name.</param>
		/// <returns></returns>
		public string SelectForEnumeration( string target, string enumerationTypeName )
		{
			return SelectForEnumeration( target, null, enumerationTypeName, null );
		}

		/// <summary>
		/// Generates an HTML SELECT element and populates it with the values of the specified enumeration.
		/// If this enumeration uses the <see cref="LocalizedEnumConverter"/>, the text diplayed for each value will 
		/// be taken from the localized resources; otherwise, the name of enumeration value will be displayed. 
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="enumerationTypeName">The enumeration's type's name.</param>
		/// <param name="attributes">The attributes.</param>
		/// <returns></returns>
		public string SelectForEnumeration( string target, string enumerationTypeName, IDictionary attributes )
		{
			return SelectForEnumeration( target, null, enumerationTypeName, attributes );
		}

		/// <summary>
		/// Generates an HTML SELECT element and populates it with the values of the specified enumeration.
		/// If this enumeration uses the <see cref="LocalizedEnumConverter"/>, the text diplayed for each value will 
		/// be taken from the localized resources; otherwise, the name of enumeration value will be displayed. 
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="selectedValue">The enumeration value which will be pre-selected.</param>
		/// <param name="enumerationTypeName">The enumeration's type's name.</param>
		/// <param name="attributes">The attributes.</param>
		/// <returns></returns>
		public string SelectForEnumeration( string target, object selectedValue, string enumerationTypeName, IDictionary attributes )
		{
			try
			{
				Type enumerationType = Type.GetType( enumerationTypeName );

				return Select( target, enumerationType, attributes );
			}
			catch ( Exception exception )
			{
				logger.ErrorFormat( "Error in SelectForEnumeration : {0}", exception.Message );
				return string.Empty;
			}
		}
		
		#endregion Select For Enumeration Methods

	}
}
