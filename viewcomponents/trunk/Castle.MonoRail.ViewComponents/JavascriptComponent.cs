
#region License
// Copyright (c) 2007, James M. Curran
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
#endregion

using System.Web.Caching;

namespace Castle.MonoRail.ViewComponents
{
    #region Reference
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Web;
    using System.Collections;
    using System.Xml;
    using System.Drawing;

    using Castle.MonoRail.Framework;
    using Castle.MonoRail.Framework.Helpers;
    #endregion

    using StringSet = System.Collections.Generic.List<string>;
    // When ported to .Net v 3.5, use the following line instead.
//    using StringSet = System.Collections.Generic.HashSet<string>;

    /// <summary>
    /// Used internally at run-time by the JavascriptComponent system to hold details of 
    /// scripts to include.
    /// </summary>
    internal class JSsegments
    {
        /// <summary>
        /// Initializes a new instance of the JSsegments class.
        /// </summary>
        public JSsegments()
        {
            segments = new Dictionary<string, string>();
            files = new StringSet();
            stdFiles = new List<LibraryDetail>();
			wasRendered = false;
        }

        internal bool wasRendered;

        internal Dictionary<string, string> segments;
        internal StringSet files;
        internal List<LibraryDetail> stdFiles;

    }

    internal class LibraryDetail
    {
        public string Name;
        public bool AutoLoad;
        public bool UseGoogle;
        public string PathName;
        public string[] Alias;
        public string[] DependsOn;
        public string Version;
        public LibraryDetail(string name, bool autoLoad, bool useGoogle, string version, string pathName, string alias, string dependsOn)
        {
            this.Name = name;
            this.Alias = alias.Split(' ', ',');
            this.AutoLoad = autoLoad;
            this.DependsOn = dependsOn.Split(' ', ',');
            this.PathName = pathName;
            this.UseGoogle = useGoogle;
            this.Version = version;
        }
        public LibraryDetail(string name, bool autoLoad) : this(name, autoLoad, false, "1", null, "", "") { }
        public LibraryDetail(string name, string dependsOn) : this(name, true, false, "1", null, "", dependsOn) { }
        public LibraryDetail(string name, string dependsOn, string alias) : this(name, true, false, "1", null, alias, dependsOn) { }
        public LibraryDetail(XmlElement node)
        {
            this.Name = node.Attributes["name"].Value;
            this.Alias = (Attribute(node, "alias") ?? "").Split(' ', ',');
            this.AutoLoad = AttributeBool(node, "autoLoad");
            this.DependsOn = (Attribute(node, "dependsOn") ?? "").Split(' ', ',');
            this.PathName = Attribute(node, "pathname");
            this.UseGoogle = AttributeBool(node, "useGoogle");
            this.Version = Attribute(node, "version");
        }

        private static bool AttributeBool(XmlElement node, string p)
        {
            string val = Attribute(node, p);
            return val == null ? false : XmlConvert.ToBoolean(val);
        }
        private static string Attribute(XmlElement ele, string key)
        {
            XmlAttribute attr = ele.Attributes[key];
            return attr == null ? null : attr.Value;
        }

        internal  static Predicate<LibraryDetail> ByNameOrAlias(string name)
        {
            name = name.Trim().ToLowerInvariant();
            return delegate(LibraryDetail ld) { return ld.Name == name || Array.IndexOf(ld.Alias, name) !=-1; };
        }
    }
    /// <summary>
    /// Speicifies how the version field of a <see cref="BrowserSpec"/> object is interpreted.
    /// </summary>
    public enum versionDirection 
    {
        /// <summary>
        /// Only the exact browser version matches.
        /// </summary>
        Exact,
        /// <summary>
        /// The browser version cited and all earlier versions of that browser.
        /// </summary>
        andLower,
        /// <summary>
        /// The browser version cited and all newer versions of that browser.
        /// </summary>
        andHigher 
    };
    internal class BrowserSpec
    {
       static string[] browsers = new string[] 
       { "ie", "netscape", "firefox", "opera", "safari", "wince" };

        public string full;
        public string browser;
        public int version;
        public versionDirection direction;

        /// <summary>
        /// Initializes a new instance of the BrowserSpec class.
        /// </summary>
        public BrowserSpec()
        {
        }
        /// <summary>
        /// Initializes a new instance of the BrowserSpec class.
        /// </summary>
        /// <param name="browser"></param>
        /// <param name="version"></param>
        /// <param name="direction"></param>
        public BrowserSpec(string browser, int version, versionDirection direction)
        {
            this.browser = browser;
            this.version = version;
            this.direction = direction;
        }

        public bool MatchesBrowser(HttpBrowserCapabilities browCaps)
        {
            bool segmentChoosen = false;
            if (browCaps.IsBrowser(this.browser) || browCaps.Browser.Equals(this.browser, StringComparison.InvariantCultureIgnoreCase))
            {
                if (this.version == 0 ||
                    (this.direction == versionDirection.andLower && browCaps.MajorVersion <= this.version) ||
                    (this.direction == versionDirection.andHigher && browCaps.MajorVersion >= this.version))
                {
                    segmentChoosen = true;
                }
            }
            return segmentChoosen;
        }


        public static BrowserSpec ParseBrowser(string tag)
        {
            BrowserSpec retn = null;
            BrowserSpec spec = new BrowserSpec();
            spec.full = tag;
            string name = tag.ToLowerInvariant();
            System.Web.Caching.Cache cache = HttpRuntime.Cache;

            foreach (string browser in browsers)
            {
                if (name.StartsWith(browser))
                {
                    spec.browser = browser;
                    if (name == browser)
                    {
                        retn = spec;
                    }
                    else
                    {
                        int len = name.Length - browser.Length;
                        spec.direction = versionDirection.Exact;
                        if (name.EndsWith("u") || name.EndsWith("h"))
                        {
                            len--;
                            spec.direction = versionDirection.andHigher;
                        }

                        if (name.EndsWith("d") || name.EndsWith("l"))
                        {
                            len--;
                            spec.direction = versionDirection.andLower;
                        }

                        string version = name.Substring(browser.Length, len);
                        if (int.TryParse(version, out spec.version))
                            retn = spec;
                    }
                    break;
                }
            }
            return retn;
        }

    }

    /// <summary>
    /// ViewComponent for inserting Javascript.
    /// </summary>
    /// <remarks>A block component for including Javascript files and script.  Code included using this component 
    /// is gathered together, and inserted at the location of the <see cref="InsertJavascriptComponent"/>.  Blocks can 
    /// be conditionally included based on the browser being used.
    /// <para/>
    /// JavascriptComponent can be used as either a block or line component.  It takes two parameters, either of 
    /// which may be optional depending on the context.
    /// <list type="table">
    /// <listheader>
    /// <term>Parameter</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>       id        </term>
    /// <description>The string that uniquely identifies the script block. Blocks with 
    /// the same id as rendered only once. <para/>
    /// <b>Required</b> when used as a block component. 
    /// (Pointless, when used as a line component)</description>
    /// </item>
	/// <item>
	/// <term>       UseGoogle         </term>
	/// <description><c>True</c> or <c>False</c>, specifying if Google AJAX Libraries API should be used to load 
	/// the standard script files given using the <c>Std</c> parameter, or 
	/// if they should be read directly from the website.</description>
	/// </item>
	/// <item>
    /// <term>       Std      </term>
    /// <description>A comma or space seaparated list of the standard Javascript files to include. 
    /// Will automatically include needed requestisits. 
    /// It can include any of the following.
    /// </description>
    /// </item>
    /// </list>
    /// <list type="table">
    /// <listheader>
    /// <term>Keyword</term>
    /// <description>Script</description>
    /// </listheader>
    /// <item>
    /// <term>           ajax              </term>
    /// <description>    prototype.js      </description>
    /// </item>
    /// <item>
    /// <term>           behavior           <br/>
    ///    or <br/>
    ///                  behaviour          <br/>
    /// </term>
    /// <description>    behaviour.js       </description>
    /// </item>
    /// <item>
    /// <term>           scriptaculous      <br/> 
    /// or <br/> 
    ///                  effects2           </term>
    /// <description>    Script.aculo.us    </description>
    /// </item>
    /// <item>
    /// <term>           effectsfat</term>
    /// <description>effectsfat.js</description>
    /// <term>Keyword</term>
    /// </item>
    /// <item>
    /// <term>validate</term>
    /// <description>Validate.Config <br/>
    /// Validate.Core <br/>
    /// Validate.Validators <br/>
    /// Validate.Lang <br/>
    /// </description>
    /// </item>
	/// <item>
	/// <term>                     moo                    </term>
	/// <description>MooTools</description>
	/// </item>
	/// <item>
	/// <term>                     jquery                    </term>
	/// <description>jQuery</description>
	/// </item>
	/// <item>
	/// <term>                     dojo                    </term>
	/// <description>DoJo</description>
	/// </item>
	/// </list>
	/// <para>Note that MooTools, jQuery, and Dojo are not part of the standard Monorail installation, and will not be  available, if UseGoogle is false, 
	/// unless the needed files are manually copied to the site. </para>
	/// <para>Note that behavior, effectfat and validate are presently not part of the Google AJAX Libraries API, and will always be loaded from the website.</para>
    /// <para/>
    /// JavascriptComponent allows a number of subsections, to allow
    /// customizing the included Javascript to the requesting browser.
    /// <para/>
    /// <list type="table">
    /// <listheader>
    /// <term>Section</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>            all                </term>
    /// <description>Block is always rendered. 
    /// Rendered at the site of InsertJavascriptComponent.</description></item>
    /// <item>
    /// <term>            other             </term>
    /// <description>Block is rendered if no browser specific block is rendered.  
    /// Rendered at the site of InsertJavascriptComponent.</description></item>
    /// <item>
    /// <term>            noscript             </term>
    /// <description>Always rendered, within &lt;noscript&gt; tags.  
    /// Rendered at the site of JavascriptComponent.</description></item>
    /// <item>
    /// <term>            inline             </term>
    /// <description>Always rendered.
    /// Rendered at the site of JavascriptComponent.</description></item>
    /// <item>
    /// <term>            <i>Browser Spec</i>  </term>
    /// <description>Rendered if the requesting browser matches the given 
    /// browsers specification. Rendered at the site of InsertJavascriptComponent.</description></item>
    /// </list>
    /// <para/>
    /// <i>Browser Spec</i> is in the following form:<para/>
    /// <c>    #{Browser name}[{version #}[U|D]]       </c><para/>
    /// where: <para/>
    /// <i>browser</i> is  <c>"ie", "netscape", "firefox", "opera", "safari" or "wince"</c> <para/>
    /// <i>Version </i> is an optional integer version number. <para/>
    /// and <i><c>D or U</c></i> indicates that version and lower, or that version and higher. <para/>
    /// For example:  <para/>
    /// <list type="table">
    /// <listheader>
    /// <term>Example</term>
    /// <description>Meaning</description>
    /// </listheader>
    /// <item>
    /// <term>               #safari                 </term>
    /// <description>Any version of Safari.</description>
    /// </item>
    /// <item>
    /// <term>               #firefox2           </term>
    /// <description>Version 2 of FireFox.</description>
    /// </item>
    /// <item>
    /// <term>               #netscape5d         </term>
    /// <description>Version 5 or eariler of Netscape.</description>
    /// </item>
    /// <item>
    /// <term>                #ie6u               </term>
    /// <description>Version 6 or later of Internet Explorer.</description>
    /// </item>
    /// </list>
    /// <para/>
    /// Note: It is assumed that the InsertJavascriptComponent will appear once in the layout template,
    /// while one or more JavascriptComponents will appear in the view.  This will cause all blocks 
    /// included in the view to be inserted in the layout.  However, assuming the InsertJavascriptComponent is 
    /// in the HTML &lt;HEAD&gt; section, any JavascriptComponent used in the layout will be rendered after
    /// the InsertJavascriptComponent.  In this case, the new javascript code will be rendered in-place.
    /// 
    /// </remarks>
    /// <example>
    /// Given the following code:
    /// <code> <![CDATA[
    /// <html><head>
    /// #component (InsertJavascript with "Std=Behavior, Scriptolous")
    /// </head>
    /// <body>
    /// #blockcomponent (Javascript with "id=MyCode" "Std=Effects2, EffectsFat")
    /// #ie6D
    /// function Showit()   { alert("This is IE 6 or lower");
    /// #end
    /// #ie7
    /// function Showit()   { alert("This is IE 7");
    /// #end
    /// #foxfire
    /// function Showit()   { alert("This is Foxfire");
    /// #end
    /// #inline
    /// ShowIt();
    /// #end
    /// #noscript
    /// <DIV id="IfNoScript">Javascript not enabled.</DIV>
    /// #end
    /// </body></html>
    /// ]]></code>
    /// Would generate the following code when requested by an IE5 browser
    /// <code><![CDATA[
    /// <html><head>
    /// <script type="text/javascript" src="/web/MonoRail/Files/AjaxScripts.rails?RC3_0006"></script>
    /// <script type="text/javascript" src="/web/MonoRail/Files/BehaviourScripts.rails?RC3_0006"></script>
    /// <script type="text/javascript" src="/web/MonoRail/Files/Effects2.rails?RC3_0006"></script>
    /// <script type="text/javascript" src="/web/MonoRail/Files/EffectsFat.rails?RC3_0006"></script>
    /// <script type="text/javascript">
    /// function Showit()   { alert("This is IE 6 or lower");
    /// </script>
    /// </head>
    /// <body>
    /// <script type="text/javascript">
    /// ShowIt();
    /// </script>
    /// <noscript>
    /// <DIV id="IfNoScript">Javascript not enabled.</DIV>
    /// </noscript>
    /// </body></html>
    ///  
    /// ]]></code>
    /// </example>
    /// <seealso cref="JavascriptHelper"/> <seealso cref="InsertJavascriptComponent"/>
	[ViewComponentDetails("Javascript")]
    public class JavascriptComponent : ViewComponentEx
    {
        List<BrowserSpec> sectionsUsed;
        private string id;
        private JavascriptHelper helper;

//        private JSsegments js;

        /// <summary>
        /// Initializes a new instance of the JavascriptComponent class.
        /// </summary>
        public JavascriptComponent()
        {
            this.sectionsUsed = new List<BrowserSpec>();
        }

        /// <summary>
        /// Called by the framework once the component instance
        /// is initialized
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
            id = Context.ComponentParameters["id"] as string;

            if (id == null)
            {
                if (sectionsUsed.Count > 0)
                {
                    throw new ViewComponentException("JavascriptComponent: 'id' parameter must be specified if subsections are used.");
                }
                id = GetHashCode().ToString("x");
            }
			helper = new JavascriptHelper(this.Context, this.EngineContext, id);
            string std = Context.ComponentParameters["std"] as string ?? string.Empty;
            helper.IncludeStandardScripts(std);

            string file = Context.ComponentParameters["file"] as string;
            if (!string.IsNullOrEmpty(file))
                helper.IncludeScriptFile(file);
        }

        /// <summary>
        /// Called by the framework so the component can
        /// render its content
        /// </summary>
        public override void Render()
        {
            base.Render();
            StringWriter script = new StringWriter();

            JSsegments js = helper.Segments;

			// if a script has already been provided with the same id, then we use that one
            // and ignore this one.  This way, multiple components can provide the same script, and
            // it will be included only once.

            if (!((ICollection<string>)js.segments.Keys).Contains(id))
            {
                Context.RenderSection("all", script);

                HttpBrowserCapabilities browCaps = this.HttpContext.Request.Browser;
                bool segmentChoosen = false;
                foreach (BrowserSpec spec in sectionsUsed)
                {
                    if (spec.MatchesBrowser(browCaps))
                    {
                        segmentChoosen = true;
                        Context.RenderSection(spec.full, script);
                    }
                }

                if (!segmentChoosen)
                {
                    Context.RenderSection("other", script);
                }
                js.segments[id] = script.ToString();
            }

            if (Context.HasSection("noscript"))
            {
                RenderText("\n\r<noscript>");
                RenderSection("noscript");
                RenderText("</noscript>\n\r");
            }

            if (Context.HasSection("inline"))
            {
                RenderText("\n\r<script>");
                RenderText(@"//<![CDATA[");
                RenderSection("inline");
                RenderText(@"//]]>");
                RenderText("</script>\n\r");
            }

            string inlinefile = Context.ComponentParameters["inlinefile"] as string;
            if (!string.IsNullOrEmpty(inlinefile))
                RenderText(helper.RenderJavascriptFile(inlinefile));

            CancelView();
        }

        /// <summary>
        /// Implementor should return true only if the
        /// <c>name</c> is a known section the view component
        /// supports.
        /// </summary>
        /// <remarks>This is a hack.  Beside assessing if a section label is valid, 
        /// it also makes a list of the section labels used.</remarks>
        /// <param name="tag">section being added</param>
        /// <returns>
        /// 	<see langword="true"/> if section is supported
        /// </returns>
        public override bool SupportsSection(string tag)
        {
            string name = tag.ToLowerInvariant();

            if (name == "inline" || name == "all" || name == "noscript" || name == "other")
                return true;

            BrowserSpec spec = BrowserSpec.ParseBrowser(name);
            bool browserOK = base.SupportsSection(name);
            if (spec != null)
            {
                sectionsUsed.Add(spec);
                browserOK = true;
            }

            return browserOK;
        }


    }

    /// <summary>
    /// ViewComponent to insert the Javascript segments that 
    /// were build using <see cref="JavascriptComponent"/> and
    /// <see cref="JavascriptHelper"/>.  The set is intended to be an
    /// enhanced version of "capturefor(javascript)
    /// <remarks>
    /// </remarks>InsertJavascriptComponent is a companion component to JavascriptHelper 
    /// and JavascriptComponent, and is of little use unless used with one or both of those. 
    /// <para/>
    /// 
    /// It is a line component which can take one optional parameter. 
    /// <list type="table">
    /// <listheader>
    /// <term>Parameter</term>
    /// <description>Description</description>
    /// </listheader>
    /// <item>
    /// <term>       Std      </term>
    /// <description>A comma or space seaparated list of the standard Javascript files to include. 
    /// Will automatically include needed requestisits. 
    /// It can include any of the following.
    /// </description>
    /// </item>
    /// </list>
    /// <list type="table">
    /// <listheader>
    /// <term>Keyword</term>
    /// <description>Script</description>
    /// </listheader>
    /// <item>
    /// <term>           ajax              </term>
    /// <description>    prototype.js      </description>
    /// </item>
    /// <item>
    /// <term>           behavior           <br/>
    ///    or <br/>
    ///                  behaviour          <br/>
    /// </term>
    /// <description>    behaviour.js       </description>
    /// </item>
    /// <item>
    /// <term>           scriptaculous      <br/> 
    /// or <br/> 
    ///                  effects2           </term>
    /// <description>    Script.aculo.us    </description>
    /// </item>
    /// <item>
    /// <term>           effectsfat</term>
    /// <description>effectsfat.js</description>
    /// <term>Keyword</term>
    /// </item>
    /// <item>
    /// <term>validate</term>
    /// <description>Validate.Config <br/>
    /// Validate.Core <br/>
    /// Validate.Validators <br/>
    /// Validate.Lang <br/>
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    /// <example>
    /// See <see cref="JavascriptComponent"/> for an example of usage.
    /// </example>
    /// <seealso cref="JavascriptHelper"/> <seealso cref="JavascriptComponent"/>
//	[ViewComponentDetails("InsertJavascriptComponent")]
    public class InsertJavascriptComponent : ViewComponentEx
    {
        JavascriptHelper helper;
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertJavascriptComponent"/> class.
        /// </summary>
        public InsertJavascriptComponent()
        {

        }
        /// <summary>
        /// Called by the framework once the component instance
        /// is initialized
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

			helper = new JavascriptHelper(this.Context, this.EngineContext, "DummyInsertJavascript");
            string std = Context.ComponentParameters["Std"] as string ?? string.Empty;
            helper.IncludeStandardScripts(std);
        }

        /// <summary>
        /// Implementor should return true only if the
        /// <c>name</c> is a known section the view component
        /// supports.
        /// </summary>
        /// <param name="name">section being added</param>
        /// <returns>
        /// 	<see langword="true"/> if section is supported
        /// </returns>
        public override bool SupportsSection(string name)
        {
            return false;
        }

        /// <summary>
        /// Called by the framework so the component can
        /// render its contents.
        /// </summary>
        /// 
        public override void Render()
        {
            base.Render();
            JSsegments js = helper.Segments;

            CancelView();
            if (js == null)
                return;

            foreach (LibraryDetail lib in js.stdFiles)
            {
                if (lib.AutoLoad )
                {
                    string name = lib.Name;
                    if (lib.UseGoogle)
                    {
                        RenderText(this.helper.RenderJavascriptFile(string.Format("http://ajax.googleapis.com/ajax/libs/{0}/{1}/{0}.js", name, lib.Version)));
                        continue;
                    }

                    string filePath = null;
                    switch (name)
                    {
                        case "prototype":
                            filePath = (Context.ContextVars["AjaxHelper"] as AjaxHelper).InstallScripts();
                            break;

                        case "behavior":
                            filePath = (Context.ContextVars["BehaviourHelper"] as BehaviourHelper).InstallScripts();
                            break;

                        case "scriptaculous":
                            filePath = (Context.ContextVars["ScriptaculousHelper"] as ScriptaculousHelper).InstallScripts();
                            break;

                        case "effectsfat":
                            filePath = (Context.ContextVars["EffectsFatHelper"] as EffectsFatHelper).InstallScripts();
                            break;

                        case "validate":
                            filePath = (Context.ContextVars["ValidationHelper"] as ValidationHelper).InstallScripts();
                            break;

                        //case "jquery":
                        //   filePath = (Context.ContextVars["jQueryHelper"] as jQueryHelper).InstallScripts();
                        //    break;

                        default:
                            if (lib.PathName != null)
                                filePath=this.helper.RenderJavascriptFile(lib.PathName);
                            break;
                    }
                    if (filePath !=null)
                    {
                        RenderText(filePath);
                        RenderText(Environment.NewLine);
                    }
                }
            }
            foreach (string file in js.files)
            {
                RenderText(this.helper.RenderJavascriptFile(file));
            }

            if (js.segments.Count > 0)
            {
                RenderText(this.helper.RenderJavascriptBlocks());
            }
            js.wasRendered = true;
        }
    }

    /// <summary>
    /// Helper class for building Javascript blocks that will be 
    /// inserted by <see cref="InsertJavascriptComponent"/>.
    /// </summary>
    /// <seealso cref="InsertJavascriptComponent"/> <seealso cref="JavascriptComponent"/>

    public class JavascriptHelper
    {
		private const string STR_JSsegments = "JSsegments";
        #region Private fields
        private IViewComponentContext context;
		private IDictionary contextVars;
        private HttpBrowserCapabilities browCaps;
        private JSsegments js;
        private string id;
        private StringBuilder scriptBuilder;
        private bool skipRender;
        private bool segmentChoosen;
        private IEngineContext engine; 

        private LibraryDetail[] libraryDefaults = new LibraryDetail[]
            {
                new LibraryDetail("prototype", true),
                new LibraryDetail("scriptaculous", "prototype", "effects2"),
                new LibraryDetail("effectsfat", "prototype"),
                new LibraryDetail("validate", "prototype"),
                new LibraryDetail("behavior", "prototype", "behaviour")
            };
        #endregion

        /// <summary>
        /// Initializes a new instance of the JavascriptHelper class.
        /// </summary>
        /// <remarks>Different instances of JavascriptHelper created with the same key are render only once.
        /// </remarks>
        /// <example><code>
        /// JavascriptHelper helper = new JavascriptHelper(viewcomp.Context, viewcomp.HttpContext, "MyScript");
        /// </code></example>
        /// <param name="context">The context.</param>
        /// <param name="httpcontext">The httpcontext.</param>
        /// <param name="key">The string that uniquely identifies the script block.</param>
		[Obsolete("Please use 'JavascriptHelper(IViewComponentContext, IEngineContext, string)' ctor instead." )]
        public JavascriptHelper(IViewComponentContext context,HttpContext httpcontext, string key)
        {
            this.context = context;
			this.contextVars = context.ContextVars;
			this.id = key;
			this.browCaps = httpcontext.Request.Browser;
//            this.engine = engine;
            Init();
		}

		/// <summary>
		/// Initializes a new instance of the JavascriptHelper class.
		/// </summary>
		/// <remarks>Different instances of JavascriptHelper created with the same key are render only once.
		/// </remarks>
		/// <example><code>
		/// JavascriptHelper helper = new JavascriptHelper(viewcomp.Context, viewcomp.EngineContext, "MyScript");
		/// </code></example>
		/// <param name="context">The context.</param>
		/// <param name="engine">The engine.</param>
		/// <param name="key">The key.</param>
		public JavascriptHelper(IViewComponentContext context, IEngineContext engine, string key)
		{
			this.context = context;
			this.contextVars = context.ContextVars;
			this.id = key;
			this.browCaps = engine.UnderlyingContext.Request.Browser;
            this.cacheProvider = engine.Services.CacheProvider;
            this.engine = engine;
            GetJSSegments(engine);
            Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JavascriptHelper"/> class.
		/// </summary>
		/// <param name="controller">The controller.</param>
		/// <param name="key">The key.</param>
		public JavascriptHelper(Controller controller, string key)
		{
			this.context = null;
			this.contextVars = controller.PropertyBag;
			this.id = key;
			this.browCaps = controller.Context.UnderlyingContext.Request.Browser;
            cacheProvider = controller.Context.Services.CacheProvider;
            GetJSSegments(controller.Context);
			Init();

		}
		private void Init()
		{

			if (((ICollection<string>)js.segments.Keys).Contains(id))
				this.skipRender = true;

			this.scriptBuilder = new StringBuilder(1024);
		}

        private void GetJSSegments(IEngineContext engine)
        {
            // nVeloecity preserves controller.PropertyBag between ViewComponents and the layout.
            // Brail preserves engine.Flash between ViewComponents and the layout.
            // so, one way or another, this gets saved.
            this.js = (contextVars[STR_JSsegments] ?? engine.Flash[STR_JSsegments] ?? new JSsegments()) as JSsegments;
            contextVars[STR_JSsegments]  = js;
            engine.Flash[STR_JSsegments] = js;
            contextVars[STR_JSsegments + ".@bubbleUp"] = true;
        }
        /// <summary>
        /// Indicates whether or not the user's browser mets the given specification.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="baseversion">The baseversion.</param>
        /// <param name="direction">The direction.</param>
        /// <returns></returns>
		public bool BrowserIs(string browser, int baseversion, versionDirection direction)
		{
			BrowserSpec spec = new BrowserSpec(browser, baseversion, direction);
			return spec.MatchesBrowser(browCaps);
		}

        /// <summary>
        /// Indicates whether or not the user's browser mets the given specification.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <returns></returns>
		public bool BrowserIs(string browser)
		{
			return BrowserIs(browser, 0, versionDirection.Exact);
		}

        /// <summary>
        /// Indicates whether or not the user's browser mets the given specification.
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="baseversion">The baseversion.</param>
        /// <returns></returns>
		public bool BrowserIs(string browser, int baseversion)
		{
			return BrowserIs(browser, baseversion, versionDirection.Exact);
		}

		/// <summary>
        /// Includes the script text for the given browser in the given version range.
        /// </summary>
        /// <remarks>Script blocks are only include in the output is the browser specified matches the
        /// browser making the request.  <paramref name="browser"/> can be the name of a browser defined in the BrowserCaps 
        /// (<c>"ie", "firefox", "netscape"</c>, et al, including all defined in the *.browser files installed), 
        /// <c>"all", "other", "inline" </c>or <c> "noscript"</c>.
        /// "all" block are always included. "other" blocks are include only if a more specific block has
        /// not already included. <para/>
        /// "inline" and "noscript" blocks are rendered in place with the rest of the viewcomponent.
        /// Other blocks are gathered together, and rendered at the location of the <see cref="InsertJavascriptComponent"/>.
        /// </remarks>
        /// <param name="browser">The name of the browser which this script is specific to.</param>
        /// <param name="baseversion">The baseversion.</param>
        /// <param name="direction">The direction versionDirection.andUp or .andDown.</param>
        /// <param name="script">The script.</param>
        public void IncludeScriptText(string browser, int baseversion, versionDirection direction, string script)
        {
            BrowserSpec spec = new BrowserSpec(browser, baseversion, direction);
            if (browser == "all")
            {
                AddScriptBlock(script);
            }

            if ((browser == "inline"))
            {
                RenderScriptImmedaitely(script);
            }

            if (!segmentChoosen)
            {
                if (browser=="other" || spec.MatchesBrowser(browCaps))
                {
                    AddScriptBlock(script);
                    segmentChoosen = true;
                }
            }
        }

        /// <summary>
        /// Includes the script text for a specific browser.  Must match the exact version.
        /// For more details, see <see cref="IncludeScriptText(string, int, versionDirection, string)"/>
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="baseversion">The baseversion.</param>
        /// <param name="script">The script.</param>
        public void IncludeScriptText(string browser, int baseversion, string script)
        {
            IncludeScriptText(browser, baseversion, versionDirection.Exact, script);
        }
        /// <summary>
        /// Includes the script text for any version of the specified browser.
        /// For more details, see <see cref="IncludeScriptText(System.String, System.Int32, Castle.MonoRail.ViewComponents.versionDirection, System.String)"/>
        /// </summary>
        /// <param name="browser">The browser.</param>
        /// <param name="script">The script.</param>
        public void IncludeScriptText(string browser, string script)
        {
            IncludeScriptText(browser, 0, versionDirection.Exact, script);
        }
        /// <summary>
        /// Includes the script text for all browsers.
        /// For more details, see <see cref="IncludeScriptText(string, int, versionDirection, string)"/>
        /// </summary>
        /// <param name="script">The script.</param>
        public void IncludeScriptText(string script)
        {
            IncludeScriptText("all", 0, versionDirection.Exact, script);
        }

        /// <summary>
        /// Includes the standard scripts.
        /// </summary>
        /// <param name="std">Comma or space separated list of scripts to include</param>
        public void IncludeStandardScripts(string std)
        {
            string[] scripts = std.Split(' ', ',');
            foreach (string script in scripts)
            {
                if (script.Length > 0)
                {
                    if (this.LibraryDetails.Exists(LibraryDetail.ByNameOrAlias(script)))
                        InsertDependancy(script, js.stdFiles.Count);
                }
            }
        }

        /// <summary>
        /// Inserts the details of a required javascript file into the list.
        /// </summary>
        /// <remarks>
        /// Places the details of the js script file known by <paramref name="name"/> onto the list
        /// of files that will be included on this page.  Designed to place new files at the end, and
        /// files they depend on just before them.
        /// </remarks>
        /// <param name="name">The name.</param>
        /// <param name="index">The index.</param>
        private void InsertDependancy(string name, int index)
        {
            if (string.IsNullOrEmpty(name))
                return;

            if (js.stdFiles.Find(LibraryDetail.ByNameOrAlias(name)) == null)
            {
                LibraryDetail details = LibraryDetails.Find(LibraryDetail.ByNameOrAlias(name));
                js.stdFiles.Insert(index, details);
                foreach (string fname in details.DependsOn)
                    InsertDependancy(fname, index);
            }
        }

        /// <summary>
        /// Includes the specified Javascript file.
        /// </summary>
        /// <remarks>Includes the specified Javascript file. Only one download command will be render, 
        /// regardless of how many times a file is included.  <br/> <br/>
        /// Script download commands are gathered together
        /// and rendered at the location of the <see cref="InsertJavascriptComponent"/>.
        /// <br/> <br/>
        /// If the filename began with "http://", then it is assumed to be external, 
        /// and rendered as-is.  If not, it is assumed to be in a "\javascript" folder
        /// off of the site root.
        /// </remarks>
        /// <param name="file">The filename.</param>
        public void IncludeScriptFile(string file)
        {
			// ... if it's not on the list of files that we will/have already rendered
			if (!js.files.Contains(file))
            {
				// ... then see if it's a standard library added the wrong way.
				LibraryDetail stdfile = LibraryDetails.Find(ld => MatchLibraryByPath(ld, file));

				// If it is a std file & on the list to be rendered, we're done.
				if (stdfile != null && js.stdFiles.Contains(stdfile))
					return;

				// If it's not on either list, and we've already rendered the lists, render it now.
				if (js.wasRendered)
                {
                    this.context.Writer.Write(RenderJavascriptFile(file));
                }

				// Else add it to the appropriate list.
				if (stdfile != null)
					js.stdFiles.Add(stdfile);
				else
					js.files.Add(file);
            }
        }

		internal bool MatchLibraryByPath(LibraryDetail ld, string path)
		{
			string filename = Path.GetFileName(path);
			string basename = Path.GetFileNameWithoutExtension(filename);
			return ld.PathName == path || ld.Name == basename || ld.PathName == filename || Array.IndexOf(ld.Alias, basename) !=-1;
		}

        internal string RenderJavascriptFile(string file)
        {
            StringBuilder sb = new StringBuilder();
            if (file.StartsWith("http://"))
            {
                sb.AppendFormat(@"<script type=""text/javascript"" src=""{0}""></script>", file );
                sb.Append(Environment.NewLine);
            }
            else
            {
                sb.AppendFormat(@"<script type=""text/javascript"" src=""{1}/javascript/{0}""></script>", file, contextVars["siteRoot"]);
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        private void RenderScriptImmedaitely(string text)
        {
            this.context.Writer.WriteLine(@"<script type=""text/javascript"">");
            this.context.Writer.WriteLine("//<![CDATA[\n\n");
            this.context.Writer.WriteLine(text);
            this.context.Writer.WriteLine(@"//]]>");
            this.context.Writer.WriteLine("</script>");
        }
        internal void AddScriptBlock(string text)
        {
            if (js.wasRendered)
            {
                if (!((ICollection<string>)js.segments.Keys).Contains(id))
                {
                    RenderScriptImmedaitely(text);
                }
            }
            else
            {
                scriptBuilder.Append(text);
                js.segments[id] = scriptBuilder.ToString();
            }
        }


        internal string RenderJavascriptBlocks()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Environment.NewLine);
            sb.AppendLine(@"<script type=""text/javascript"">");
			sb.AppendLine(@"//<![CDATA[");

            foreach (string script in js.segments.Values)
            {
				if (!String.IsNullOrEmpty(script))
					sb.AppendLine(script.Trim());
            }

			sb.AppendLine(@"//]]>");
            sb.AppendLine("</script>");
            return sb.ToString();
        }

        private XmlDocument libraryInfo = null;
        private ICacheProvider cacheProvider = null;
        private const string xmlfilename = @"jslibraries.xml";
        private const string detailskeyword = @"jslibrariesDetails";


        private List<LibraryDetail> libraryDetails = null;
        internal List<LibraryDetail> LibraryDetails
        {
            get
            {
                if (libraryDetails == null)
                {
                    libraryDetails = cacheProvider.Get(detailskeyword) as List<LibraryDetail>;
                }

                if (libraryDetails == null )
                {
                    if (LibraryInfo != null)
                    {
                        List<LibraryDetail> details = new List<LibraryDetail>();
                        foreach (XmlElement lib in LibraryInfo.DocumentElement.ChildNodes)
                        {
                            details.Add(new LibraryDetail(lib));
                        }
                        libraryDetails = details;
                    }
                    else
                        libraryDetails = new List<LibraryDetail>(libraryDefaults);

                    Cache webcache = HttpContext.Current.Cache;
                    if (webcache != null)
                    {
                        CacheDependency depend = new CacheDependency(new string[0], new string[] { xmlfilename });
                        webcache.Add(detailskeyword, libraryDetails, depend, Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                    }
                    else
                    {
                        cacheProvider.Store(detailskeyword, libraryDetails);
                    }
                    
                }
                return libraryDetails;
            }
        }

        internal XmlDocument LibraryInfo
        {
            get
            {
                if (libraryInfo == null)
                {
                    libraryInfo= cacheProvider.Get(xmlfilename) as XmlDocument;
                }

                if (libraryInfo == null)
                {
//                    string xmlpath = Path.Combine(Path.GetDirectoryName(this.GetType().Assembly.CodeBase), xmlfilename);
//                    string webPath = engine.Server.MapPath(null);
//                    string xmlpath = Path.Combine(webPath, "jslibraries.xml");
                    string xmlpath = engine.Server.MapPath(VirtualPathUtility.ToAbsolute("~/jslibraries.xml"));
                    if (File.Exists(xmlpath))
                    {
                        libraryInfo = new XmlDocument();
                        libraryInfo.Load(xmlpath);
                        Cache webcache = HttpContext.Current.Cache;
                        if (webcache != null)
                        {
                            webcache.Add(xmlfilename, libraryInfo, new CacheDependency(xmlpath), Cache.NoAbsoluteExpiration, Cache.NoSlidingExpiration, CacheItemPriority.Default, null);
                        }
                        else
                        {
                            cacheProvider.Store(xmlfilename, libraryInfo);
                        }
                    }
                }
                return libraryInfo;
            }
        }

        public string PreferredLibrary
        {
            get
            {
                if (LibraryInfo != null)
                {
//                    XmlAttribute attr = LibraryInfo.DocumentElement.SelectSingleNode("/libraries/@preferredLibrary") as XmlAttribute;
                    XmlAttribute attr = LibraryInfo.DocumentElement.Attributes["preferredLibrary"];
                    if (attr != null)
                    {
                        return attr.Value;
                    }
                }
                return "prototype";
            }
        }

        internal JSsegments Segments
        {
            get
            {
                return js;
            }
        }

        internal bool SkipRender
        {
            get
            {
                return skipRender;
            }
        }
    }
}
