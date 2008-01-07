
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


namespace Castle.MonoRail.ViewComponents
{
    #region Reference
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web;

    using Castle.MonoRail.Framework;
    using Castle.MonoRail.Framework.Helpers;
    using System.IO;
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
        }
        internal bool incAjax;
        internal bool incBehavior;
        internal bool incScriptaculous;
        internal bool incEffectsFat;
        internal bool incValidate;

        internal bool wasRendered;

        internal Dictionary<string, string> segments;
        internal StringSet files;

        internal void Preserve(Flash flash)
        {
            JSsegments js = flash["JSsegments"] as JSsegments;
            if (js == null)
                flash["JSsegments"] = this;
            else if (js != this)
                throw new ViewComponentException("JSsegment mismatch");
        }
    }
    /// <summary>
    /// Speicifies how the version field of a <see cref="BrowserSpec"/> object is interpreted.
    /// </summary>
    public enum versionDirection 
    {
        /// <summary>
        /// Only the exact browswr version matches.
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

    public class JavascriptComponent : ViewComponentEx
    {
        List<BrowserSpec> sectionsUsed;
        private string id;
        private JavascriptHelper helper;

        private JSsegments js;

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
            helper = new JavascriptHelper(this.Context, this.HttpContext, Flash, id);
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

            js = Flash["JSsegments"] as JSsegments ?? new JSsegments();
            Flash["JSsegments"] = js;

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
                RenderSection("inline");
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

            helper = new JavascriptHelper(this.Context, this.HttpContext, Flash, "DummyInsertJavascript");
            string std = Context.ComponentParameters["std"] as string ?? string.Empty;
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
        public override void Render()
        {
            base.Render();
            JSsegments js = Flash["JSsegments"] as JSsegments;
            CancelView();
            if (js == null)
                return;

            if (js.incAjax)
            {
                AjaxHelper helper = Context.ContextVars["AjaxHelper"] as AjaxHelper;
                RenderText(helper.InstallScripts());
                RenderText(Environment.NewLine);
            }

            if (js.incBehavior)
            {
                BehaviourHelper helper = Context.ContextVars["BehaviourHelper"] as BehaviourHelper;
                RenderText(helper.InstallScripts());
                RenderText(Environment.NewLine);
            }

            if (js.incScriptaculous)
            {
                ScriptaculousHelper helper = Context.ContextVars["ScriptaculousHelper"] as ScriptaculousHelper;
                RenderText(helper.InstallScripts());
                RenderText(Environment.NewLine);
            }

            if (js.incEffectsFat)
            {
                EffectsFatHelper helper = Context.ContextVars["EffectsFatHelper"] as EffectsFatHelper;
                RenderText(helper.InstallScripts());
                RenderText(Environment.NewLine);
            }

            if (js.incValidate)
            {
                ValidationHelper helper = Context.ContextVars["ValidationHelper"] as ValidationHelper;
                RenderText(helper.InstallScripts());
                RenderText(Environment.NewLine);
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
        #region Private fields
        private IViewComponentContext context;
        private HttpBrowserCapabilities browCaps;
        private JSsegments js;
        private string id;
        private StringBuilder scriptBuilder;
        private bool skipRender;
        private bool segmentChoosen;
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
        /// <param name="flash">The viewComponent's Flash collection.</param>
        /// <param name="key">The string that uniquely identifies the script block.</param>
        public JavascriptHelper(IViewComponentContext context,HttpContext httpcontext, Flash flash, string key)
        {
            this.context = context;
            js = flash["JSsegments"] as JSsegments ?? new JSsegments();
            flash["JSsegments"] = js;

            this.id = key;
            if (((ICollection<string>)js.segments.Keys).Contains(key))
                skipRender = true;

            this.browCaps = httpcontext.Request.Browser;
            this.scriptBuilder = new StringBuilder(1024);
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
                    switch (script.Trim().ToLowerInvariant())
                    {
                        case "ajax":
                            js.incAjax = true;
                            break;

                        case "behavior":
                        case "behaviour":
                            js.incAjax = true;
                            js.incBehavior = true;
                            break;

                        case "effects2":
                        case "scriptaculous":
                            js.incAjax = true;
                            js.incScriptaculous = true;
                            break;

                        case "effectsfat":
                            js.incAjax = true;
                            js.incEffectsFat = true;
                            break;

                        case "validate":
                            js.incAjax = true;
                            js.incValidate = true;
                            break;

                        default:
                            throw new ViewComponentException("JavascriptHelper: '" + script + "' is not a valid standard script");
                    }
                }
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
            if (!js.files.Contains(file))
            {
                if (js.wasRendered)
                {
                    this.context.Writer.Write(RenderJavascriptFile(file));
                }
                js.files.Add(file);
            }

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
                sb.AppendFormat(@"<script type=""text/javascript"" src=""{1}/javascript/{0}""></script>", file, context.ContextVars["siteRoot"]);
                sb.Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        private void RenderScriptImmedaitely(string text)
        {
            this.context.Writer.WriteLine(@"<script type=""text/javascript"">");
            this.context.Writer.WriteLine(text);
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
            }
            js.segments[id]= text;
        }


        internal string RenderJavascriptBlocks()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Environment.NewLine);
            sb.Append(@"<script type=""text/javascript"">");
            sb.Append(Environment.NewLine);

            foreach (string script in js.segments.Values)
            {
                sb.Append(script);
                sb.Append(Environment.NewLine);
            }

            sb.Append(Environment.NewLine);
            sb.Append("</script>");
            sb.Append(Environment.NewLine);
            return sb.ToString();
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