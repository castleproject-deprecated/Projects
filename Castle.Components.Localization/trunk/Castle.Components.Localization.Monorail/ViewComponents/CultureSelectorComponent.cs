
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

namespace Castle.Components.Localization.MonoRail.ViewComponents
{
	#region Using Directives

	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using System.Globalization;
	using System.Text;

	using Castle.MonoRail.Framework;
	using Castle.Core;
	using Castle.MonoRail.Framework.Services;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Providers;
	using System.Reflection;
	using Castle.MonoRail.Framework.Descriptors;
	using System.Text.RegularExpressions;


	#endregion Using Directives

	/// <summary>
	/// This component generates a list of links for each supported cultures.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The HTML generated can be customized by using the three supported 
	/// sections named <c>start</c>, <c>culture</c> and <c>end</c>. The default 
	/// generated HTML looks like this :
	/// <code>
	///		&lt;ul&gt;
	///			&lt;li&gt;&lt;a href="/Culture/SetCulture.rails?cultureCode=fr-FR&backUrl=/Home/Default.rails" title="Français"&gt;Français&lt;/a&gt;&lt;/li&gt;
	///			&lt;li&gt;&lt;a href="/Culture/SetCulture.rails?cultureCode=en-US&backUrl=/Home/Default.rails" title="English (United-States)"&gt;English (United-States)&lt;/a&gt;&lt;/li&gt;
	///		&lt;/ul&gt;
	/// </code>
	/// </para>
	/// <para>
	/// The supported cultures can be automatically detected by the component (by 
	/// looping over the culture directories in the application bin directory), 
	/// or can be configured by using the <see cref="LocalizationConfiguration"/> 
	/// and adding a <c>LocalizationConfiguration</c> section in your web.config.
	/// Lastly, you can specify the supported cultures as a comma separated 
	/// list of cultures codes (en-US, fr-FR, fr, etc.) directly as a 
	/// component's parameter named <c>Cultures</c>.
	/// </para>
	/// </remarks>
	/// <example>
	/// <para>
	/// This example shows how to configure the supported cultures by using 
	/// the web.config :
	/// </para>
	/// <code>
	/// &lt;configuration&gt;
	/// 
	///		&lt;configSections&gt;
	/// 		&lt;section 
	/// 			name="localization" 
	/// 			type="Castle.Components.Localization.MonoRail.Configuration.LocalizationConfigurationSectionHandler, Castle.Components.Localization.MonoRail"/&gt;
	///		&lt;/configSections&gt;
	/// 
	///		&lt;localization&gt;
	/// 		&lt;cultures&gt;
	/// 			&lt;culture&gt;fr-FR&lt;/culture&gt;
	/// 			&lt;culture&gt;en-US&lt;/culture&gt;
	/// 		&lt;/cultures&gt;
	///		&lt;/localization&gt;
	/// 
	/// &lt;/configuration&gt;
	/// </code>
	/// <para>
	/// This example shows how to configure the supported cultures as a 
	/// component's parameter in a view file with brail:
	/// </para>
	/// <code>
	/// &lt;?brail component CultureSelector, { @Cultures:'fr-FR, en-US' } ?&gt;
	/// </code>
	/// <para>
	/// This example shows how to customize the generated HTML in a view file 
	/// with brail (we generates the same HTML as the default here):
	/// </para>
	/// <code>
	/// &lt;?brail component CultureSelector, { @Cultures:'fr-FR, en-US' } : ?&gt;
	/// &lt;?brail		section start: ?&gt;
	///						&lt;ul&gt;
	/// &lt;?brail		end ?&gt;
	/// &lt;?brail		section culture: ?&gt;
	///						&lt;li&gt;&lt;a href="${ SelectionUrl }" title="${ CultureCulture.NativeName }"&gt;${ CultureCulture.NativeName }&lt;/a&gt;&lt;/li&gt;
	/// &lt;?brail		end ?&gt;
	/// &lt;?brail		section end: ?&gt;
	///						&lt;/ul&gt;
	/// &lt;?brail		end ?&gt;
	/// &lt;?brail end ?&gt;
	/// </code>
	/// </example>
	[ViewComponentDetails( "CultureSelector", Sections = "startblock,culture,endblock" )]
	public class CultureSelectorComponent : ViewComponent
	{

		#region Enums

		/// <summary>
		/// An enumeration of the different rendering modes.
		/// </summary>
		public enum RenderingMode
		{
			/// <summary>
			/// Output a list of cultures links which use the culture's native name as text.
			/// </summary>
			NativeNames,

			/// <summary>
			/// Output a list of cultures links which use the culture's most common flag as an image.
			/// </summary>
			Flags
		}

		#endregion Enums

		#region Constant Variables

		const string SectionCulture = "culture";
		const string SectionEnd = "endblock";
		const string SectionStart = "startblock";

		#endregion Constant Variables

		#region Instance Variables

		StringCollection _Cultures;
		RenderingMode _Mode;
		Helpers.ResourceHelper _ResourceHelper;

		#endregion Instance Variables

		#region Properties

		/// <summary>
		/// Gets or sets the cultures as a comma separated list of cultures'codes.
		/// </summary>
		/// <example>
		/// <code>FR-fr, EN-us</code>
		/// </example>
		/// <value>The cultures.</value>
		[ViewComponentParam( Required = false )]
		public string Cultures
		{
			get
			{
				StringBuilder builder = new StringBuilder();

				for ( int i = 0; i < _Cultures.Count; i++ )
				{

					builder.Append( _Cultures[ i ] );

					if ( i < _Cultures.Count - 1 )
						builder.Append( ", " );
				}

				return builder.ToString();
			}
			set
			{
				if ( _Cultures != null )
					_Cultures.Clear();

				string[] cultures = value.Split( ',' );

				foreach ( string culture in cultures )
				{
					_Cultures.Add( culture.Trim() );
				}
			}
		}

		/// <summary>
		/// Gets or sets the rendering mode.
		/// </summary>
		/// <value>The mode.</value>
		[ViewComponentParam( Required = false )]
		public RenderingMode Mode
		{
			get { return _Mode; }
			set { _Mode = value; }
		}

		#endregion Properties

		#region Constructors

		public CultureSelectorComponent()
		{
			// IResourceDescriptorProvider resourceDescriptorProvider
			_Cultures = new StringCollection();
		}

		#endregion Constructors

		#region Public Methods

		public override void Initialize()
		{
			_ResourceHelper = new Helpers.ResourceHelper( EngineContext );

			base.Initialize();
		}

		public override void Render()
		{
			RenderStartBlock();

			if ( _Cultures != null && _Cultures.Count > 0 )
			{
				RenderCulturesFromParams();
			}
			else
			{
				RenderCulturesFromConfig();
			}

			RenderEndBlock();
		}

		#endregion Public Methods

		#region Protected Methods

		protected override void RegisterComponentResources()
		{
			base.RegisterComponentResources( GetAssemblyResources() );
		}

		#endregion Protected Methods

		#region Private Methods

		static string Capitalize( string original )
		{
			if ( original.Length == 0 )
			{
				return original;
			}
			char originalFirst = original[ 0 ];
			char upperFirst = char.ToUpper( originalFirst );

			if ( originalFirst == upperFirst )
			{
				return original;
			}

			return upperFirst + original.Substring( 1 );
		}

		static string GetFlagResourceName( CultureInfo cultureCulture )
		{
			string flagResource = string.Concat(
				"Flags_",
				cultureCulture.IsNeutralCulture ? cultureCulture.Name : cultureCulture.Parent.Name );
			return flagResource;
		}

		void DefaultRenderCulture( CultureInfo cultureCulture )
		{
			StringBuilder builder = new StringBuilder();

			// Start list item
			builder.Append( "<li>" );

			// The link
			builder.Append( "<a href=\"" );
			builder.Append( PropertyBag[ "SelectionUrl" ] );
			builder.Append( "\" title=\"" );
			builder.Append( Capitalize( cultureCulture.NativeName ) );
			builder.Append( "\">" );

			if ( _Mode == RenderingMode.NativeNames )
			{
				builder.Append( Capitalize( cultureCulture.NativeName ) );
			}
			else
			{
				builder.AppendFormat(
					"<img src=\"{0}\" />",
					_ResourceHelper.GetImageResourceUrl( GetFlagResourceName( cultureCulture ) ) );
			}

			builder.Append( "</a>" );

			// End list item
			builder.Append( "</li>" );

			RenderText( builder.ToString() );
		}

		ResourceDescriptor[] GetAssemblyResources()
		{
			List<ResourceDescriptor> descriptors = new List<ResourceDescriptor>();

			Assembly typeAssembly = GetType().Assembly;

			IDictionaryEnumerator enumerator = Resources.Flags.ResourceManager.GetResourceSet( CultureInfo.InvariantCulture, true, true ).GetEnumerator();
			string resourceName = "Castle.Components.Localization.MonoRail.Resources.Flags";

			while ( enumerator.MoveNext() )
			{
				string name = "Flags_" + enumerator.Key.ToString();
				string assemblyName = typeAssembly.GetName().Name;

				descriptors.Add( new ResourceDescriptor(
					null,
					name,
					resourceName,
					enumerator.Key.ToString(),
					null,
					assemblyName,
					"image/png",
					true ) );
			}

			return descriptors.ToArray();
		}

		protected virtual void InitializePropertyBagForCulture( CultureInfo culture )
		{
			UrlBuilderParameters parameters = new UrlBuilderParameters( "Culture", "SetCulture" );
			parameters.QueryString = DictHelper.CreateN( "cultureCode", culture.Name ).N( "backUrl", EngineContext.Request.Url );

			bool isCurrent = CultureInfo.CurrentCulture.Name == culture.Name ||
								( !CultureInfo.CurrentCulture.IsNeutralCulture &&
									CultureInfo.CurrentCulture.Parent.Name.Equals( culture.Name ) );

			PropertyBag[ "CultureCulture" ] = culture;
			PropertyBag[ "IsCurrent" ] = isCurrent;
			PropertyBag[ "SelectionUrl" ] = EngineContext.Services.UrlBuilder.BuildUrl( EngineContext.UrlInfo, parameters );
			PropertyBag[ "FlagImageUrl" ] = _ResourceHelper.GetImageResourceUrl( GetFlagResourceName( culture ) );
		}

		protected virtual void RenderCulture( CultureInfo cultureCulture )
		{
			InitializePropertyBagForCulture( cultureCulture );

			if ( HasSection( SectionCulture ) )
				RenderSection( SectionCulture );
			else
				DefaultRenderCulture( cultureCulture );
		}

		void RenderCulturesFromConfig()
		{
			Configuration.LocalizationConfiguration configuration = Configuration.LocalizationConfiguration.GetConfig();

			foreach ( CultureInfo cultureCulture in configuration.SupportedCultures )
				RenderCulture( cultureCulture );
		}

		void RenderCulturesFromParams()
		{
			foreach ( string culture in _Cultures )
			{
				CultureInfo cultureCulture = CultureInfo.GetCultureInfo( culture );

				RenderCulture( cultureCulture );
			}
		}

		protected virtual void RenderEndBlock()
		{
			if ( HasSection( SectionEnd ) )
				RenderSection( SectionEnd );
			else
				RenderText( "</ul>" );
		}

		protected virtual void RenderStartBlock()
		{
			if ( HasSection( SectionStart ) )
				RenderSection( SectionStart );
			else
				RenderText( "<ul>" );
		}

		#endregion Private Methods

	}
}
