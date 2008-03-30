
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

namespace Castle.Components.Localization.MonoRail.Configuration
{
	#region Using Directives

	using System;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Configuration;
	using System.Globalization;
	using System.IO;
	using System.Xml;
	using System.Web;

	using Castle.Core.Configuration;
	using Castle.Core.Configuration.Xml;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.Core;
	using Castle.MonoRail.Framework;


	#endregion Using Directives

	/// <summary>
	/// A Castle like configuration object which can obtain the supported cultures.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The supported cultures are determined either by reading the configuration 
	/// section named <c>localization</c> in the web.config or by simply looping 
	/// over the culture folders inside the application bin folder.
	/// </para>
	/// <para>
	/// If you use <c>Castle.Components.Validators</c>, be sure to remove the cultures 
	/// folder for the cultures you don't want to support or to specify them through the 
	/// configuration file.
	/// </para>
	/// </remarks>
	/// <example>
	/// <para>
	/// This example shows how to specify the support of the cultures <c>fr-FR</c> and <c>en-US</c> :
	/// </para>
	/// <code>
	/// &lt;configuration&gt;
	/// 
	///		&lt;configSections&gt;
	/// 	&#09;&lt;section 
	/// 	&#09;&#09;name="localization" 
	/// 	&#09;&#09;type="Castle.Components.Localization.MonoRail.Configuration.LocalizationConfigurationSectionHandler, Castle.Components.Localization.MonoRail"/&gt;
	///		&lt;/configSections&gt;
	/// 
	///		&lt;localization&gt;
	///		
	/// 	&#09;&lt;cultures&gt;
	/// 	&#09;&#09;&lt;culture&gt;fr-FR&lt;/culture&gt;
	/// 	&#09;&#09;&lt;culture&gt;en-US&lt;/culture&gt;
	/// 	&#09;&lt;/cultures&gt;
	/// 	
	///		&lt;/localization&gt;
	/// 
	/// &lt;/configuration&gt;
	/// </code>
	/// </example>
	public class LocalizationConfiguration : ISerializedConfig
	{

		#region Constant Variables 

		/// <summary>
		/// The name of the configuration section in the web.config file.
		/// </summary>
		public const string LocalizationConfigurationSectionName = "localization";

		#endregion Constant Variables 

		#region Instance Variables 

		IConfiguration _ConfigurationSection;
		ILogger _Logger;
		List<CultureInfo> _SupportedCultures;

		#endregion Instance Variables 

		#region Properties 

		/// <summary>
		/// Gets or sets the supported cultures.
		/// </summary>
		/// <value>A <see cref="List&lt;CultureInfo&gt;"/> of the supported <see cref="CultureInfo"/>.</value>
		public List<CultureInfo> SupportedCultures
		{
			get { return _SupportedCultures; }
			set { _SupportedCultures = value; }
		}

		#endregion Properties 

		#region Constructors 

		/// <summary>
		/// Initializes a new instance of the <see cref="LocalizationConfiguration"/> class.
		/// </summary>
		public LocalizationConfiguration()
		{
			IServiceProviderEx serviceProvider = ServiceProviderLocator.Instance.LocateProvider();

			_SupportedCultures = new List<CultureInfo>();
			_Logger = serviceProvider.GetService<ILogger>();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LocalizationConfiguration"/> class.
		/// </summary>
		/// <param name="section">The section.</param>
		public LocalizationConfiguration( XmlNode section )
			: this()
		{
			_ConfigurationSection = XmlConfigurationDeserializer.GetDeserializedNode( section );
		}

		#endregion Constructors 

		#region Static Methods 

		/// <summary>
		/// Gets the <see cref="LocalizationConfiguration"/>.
		/// </summary>
		/// <returns>A <see cref="LocalizationConfiguration"/> object.</returns>
		public static LocalizationConfiguration GetConfig()
		{
			LocalizationConfiguration config = ConfigurationManager.GetSection( LocalizationConfigurationSectionName ) as LocalizationConfiguration;

			if( config == null )
			{
				config = new LocalizationConfiguration();
				config.Deserialize( null );
			}
			return config;
		}

		#endregion Static Methods 

		#region Private Methods 

		void GetSupportedCulturesFromConfig( XmlNodeList cultureNodes )
		{
			foreach ( XmlNode node in cultureNodes )
			{
				CultureInfo culture = CultureInfo.GetCultureInfo( node.ChildNodes[ 0 ].Value.Trim() );

				if ( culture == null )
					_Logger.WarnFormat( "Error in the Localization configuration secion (web.config). An invalid supported culture is specified : {0}. This culture has not been registered.", node.ChildNodes[ 0 ].Value );
				else
					_SupportedCultures.Add( culture );
			}
		}

		void GetSupportedCulturesFromCultureFiles()
		{
			// 1. Adds the default english culture.
			_SupportedCultures.Add( CultureInfo.GetCultureInfo( "en" ) );

			DirectoryInfo directoryInfo = new DirectoryInfo( HttpRuntime.BinDirectory );

			DirectoryInfo[] cultureFolders = directoryInfo.GetDirectories();

			for ( int i = 0; i < cultureFolders.Length; i++ )
			{
				CultureInfo culture = CultureInfo.GetCultureInfo( cultureFolders[ i ].Name );

				if ( culture != null )
					_SupportedCultures.Add( culture );
			}
		}

		#endregion Private Methods

		#region ISerializedConfig Members

		public void Deserialize( XmlNode section )
		{
			if( section != null )
			{
				XmlNodeList cultureNodes = section.SelectNodes( "cultures/culture" );

				if( cultureNodes.Count > 0 )
					GetSupportedCulturesFromConfig( cultureNodes );
				else
					GetSupportedCulturesFromCultureFiles();
			}
			else
				GetSupportedCulturesFromCultureFiles();
		}

		#endregion ISerializedConfig Members

	}
}
