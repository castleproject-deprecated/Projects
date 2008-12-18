// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

using System.Globalization;

namespace Castle.MonoRail.ViewComponents
{
	using Framework;
    using Framework.Helpers;
    using System.Text;
    using System.Resources;
    using System.Collections;
    using System.Reflection;

	/// <summary>
    /// A ViewComponent that renders a collapsible panel.
    /// </summary>
    /// <remarks>
    /// 
    /// CollapsiblePanelViewComponent is a blockcomponent with two section and 12 parameters.
    ///  
    /// <para><list type="table"><listheader>
    /// 
    /// <term>                                Parameter                                                             </term>
    /// <description>      Description                                                                       </description>
    /// </listheader>
    /// <item>
    /// <term>                                 id                                                                 </term>
    /// <description>
    /// 
    /// string, required.  Unique identifer for this control.
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 expandImagePath                                                                 </term>
    /// <description>
    /// 
    /// string, optional.  Path to image for "Click to Expand" link.  Both expandImagePath &amp; collapseImagePath must be given together.
    /// If either is missing, uses text link instead on image.
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 collapseImagePath                                                                 </term>
    /// <description>
    /// 
    /// string, optional.  Path to image for "Click to Collapse" link.  Both expandImagePath and collapseImagePath must be given together.
    /// If either is missing, uses text link instead on image.
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 expandLinkText                                                                 </term>
    /// <description>
    /// 
    /// string, optional.  Text use for "Click to Expand" link.  Defaults to "Show".
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 collapseLinkText                                                                 </term>
    /// <description>
    /// 
    /// string, optional.  Text use for "Click to Collapse" link.  Defaults to "Hide".
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 caption                                                                 </term>
    /// <description>
    /// 
    /// string, optional.  Text use for in the caption of the panel.  May alsoo 
    /// be specified in the "Caption" section. (section take precedence over parameter)
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 cssClass                                                                 </term>
    /// <description>
    /// 
    /// string, optional.  CSS Class assigned to panel.  Defaults to "collapsiblePanel".
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 style                                                                 </term>
    /// <description>
    /// 
    /// string, optional.  Extra inline stylesheet items that may be added to the panel.
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 collapsed                                                                 </term>
    /// <description>
    /// 
    /// boolean, optional.  Specifies whether the panel should be initially collapsed. Defaults to <c>false</c>.
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 effect                                                                 </term>
    /// <description>
    /// 
    /// string, optional.  The style of effect to use to open and close the panel.  Defaults to "slide".  
    /// Note, only implemented when using prototype library.  under jQuery, "slide" is always iused.
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 effectDuration                                                                 </term>
    /// <description>
    /// 
    /// float, optional.  The length of time used to create the opening effect.  Defaults to .3 seconds.
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 toggleOnClickHeader                                                                 </term>
    /// <description>
    /// 
    /// boolean, optional.  Specifies if the panel header (caption) itself should be the link to open and close the panel, 
    /// rather than a specific link or image (defaults to false).
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 JSLibrary                                                                 </term>
    /// <description>
    /// 
    /// string, optional.  Specifed which Javascript library to use.  Presently only "prototype" and "jquery" are implemented.
    /// Defaults to value in jslibraries.xml, if present, or "prototype".
    /// 
    /// </description></item>
    /// </list>
    /// <list type="table"><listheader>
    /// 
    /// <term>                                Section                                                             </term>
    /// <description>      Description                                                                       </description>
    /// </listheader>
    /// <item>
    /// <term>                                 body                                                                 </term>
    /// <description>
    /// 
    /// Specifies the body of the panel which is to be hidden and revealed.  
    /// Optional, but the control is rather pointless unless specified.
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 caption                                                                 </term>
    /// <description>
    /// 
    /// Specifies the text for the panel header.  May alternately be specifed in the 'caption' parameter.
    /// 
    /// </description></item>
    /// </list></para>
    /// </remarks>
    /// <example><code><![CDATA[
    /// #blockcomponent(CollapsiblePanel with "id=ColumnComponent"  
    ///                 "expandImagePath=/Images/expand.jpg" 
    ///                 "collapseImagePath=/Images/collapse.jpg" 
    ///                 "collapsed=true")
    /// #caption
    /// Columns component
    /// #end
    /// #body
    ///		<a href="/columns/index.rails">Basic Usage</a>
    ///#end
    /// #end
    /// ]]></code>
    /// will generate Htmll like this:
    /// 
    /// <code><![CDATA[
    /// <div id='ColumnComponent' class='collapsiblePanel'>
    /// <div class='header'>
    /// <table><tr><td>
    ///   <img id='ColumnComponentToggle' src='/Images/expand.jpg' class='toggleImage' 
    ///       onclick='javascript:expandCollapse(ColumnComponentOpts)' 
    ///       alt='Show' title='Click to expand/collapse'/>
    ///   </td>
    ///   <td class='caption'><h3>Columns component</h3></td>
    /// </tr></table>
    /// </div>
    /// <div id='ColumnComponentBody' class='body'style='display:none'>
    /// 	<a href="/columns/index.rails">Basic Usage</a>
    /// </div></div>
    /// <script type="text/javascript">
    /// var ColumnComponentOpts = {controlName: 'ColumnComponentBody', togglerName: 'ColumnComponentToggle', collapseImagePath:'/Images/collapse.jpg', expandImagePath:'/Images/expand.jpg'};
    /// </script>
    /// ]]></code></example>

    [ViewComponentDetails("CollapsiblePanel",Sections="body,caption")]
    public class CollapsiblePanelComponent : ViewComponentEx
    {
        private string id;
        private string expandImagePath;
        private string collapseImagePath;
        private string caption;
        private string expandLinkText;
        private string collapseLinkText;
        private bool toggleOnClickHeader;
        private string cssClass;
        private string style;
        private bool collapsed;
        private string initialCommand;
        private string bodyId;
        private string toggleId;
        private string javascriptCall;
        private string initialImage;
        private bool useImage;
        private string effect;
        private double? effectDuration;
        private string jsLibrary;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            GetParameters();

            initialCommand = collapsed ? expandLinkText : collapseLinkText;
            if (initialCommand == null)
                initialCommand = collapsed ? "Show": "Hide";

            bodyId = string.Format("{0}Body", id);
            toggleId = string.Format("{0}Toggle", id);
            javascriptCall = string.Format("javascript:expandCollapse({0}Opts)", id);
            initialImage = collapsed ? expandImagePath : collapseImagePath;
            useImage =
                !string.IsNullOrEmpty(expandImagePath) ||
                !string.IsNullOrEmpty(collapseImagePath);

            base.Initialize();
        }

        private void GetParameters()
        {
            id = ComponentParams["id"] as string;
            if (id == null)
            {
                throw new ViewComponentException("CollapsiblePanelComponent: 'id' parameter is required");
            }
            expandImagePath = ComponentParams["expandImagePath"] as string;
            collapseImagePath = ComponentParams["collapseImagePath"] as string;
            expandLinkText = ComponentParams["expandLinkText"] as string;
            collapseLinkText = ComponentParams["collapseLinkText"] as string;
            caption = ComponentParams["caption"] as string;
            cssClass = ComponentParams["cssClass"] as string ?? "collapsiblePanel";
            style = ComponentParams["style"] as string;
            collapsed = GetParamValue("collapsed", false);
            effect = ComponentParams["effect"] as string;
            effectDuration = ComponentParams["effectDuration"] as double?;
            toggleOnClickHeader = GetParamValue("toggleOnClickHeader", false);

            jsLibrary = ComponentParams["JSLibrary"] as string;
        }

        /// <summary>
        /// Renders this instance.
        /// </summary>
        public override void Render()
        {
            RenderComponent();
            RenderJavascript();
        }

        private void RenderComponent()
        {
            if (string.IsNullOrEmpty(style))
                RenderTextFormat("<div id='{0}' class='{1}'>", id, cssClass);
            else
                RenderTextFormat("<div id='{0}' class='{1}' style='{2}>", id, cssClass, style);

            RenderHeader();

            RenderBodySection();

            RenderText("</div>");
        }

        private void RenderJavascript()
        {
            AddScript();

            JavascriptHelper helper = new JavascriptHelper(this.Context, this.EngineContext, "CollapsiblePanelComponent-"+this.id);

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat(@"var {0}Opts = {{controlName: '{0}Body', togglerName: '{0}Toggle'", this.id);

            if (collapseLinkText != null)
                sb.AppendFormat(@", collapseLinkText:'{0}'", collapseLinkText);

            if (expandLinkText != null)
                sb.AppendFormat(@", expandLinkText: '{0}'", expandLinkText);

            if (this.collapseImagePath != null)
                sb.AppendFormat(@", collapseImagePath:'{0}'", collapseImagePath);

            if (this.expandImagePath != null)
                sb.AppendFormat(@", expandImagePath:'{0}'", expandImagePath);

            if (this.effect != null)
                    sb.AppendFormat(@", effect:'{0}'", effect);

            if (this.effectDuration.HasValue)
                    sb.AppendFormat(@", effectDuration:{0}", effectDuration.Value);

            sb.Append("};");

            helper.IncludeScriptText(sb.ToString());
        }

        void AddScript()
        {
            JavascriptHelper helper = new JavascriptHelper(this.Context, this.EngineContext, "CollapsiblePanelComponent");
            jsLibrary = jsLibrary ?? helper.PreferredLibrary;
            ResourceManager rm = new ResourceManager("Castle.MonoRail.ViewComponents.CollapsiblePanelScripts", Assembly.GetExecutingAssembly());
                string script = rm.GetString(jsLibrary);

            if (script == null)
                throw new ViewComponentException("unsupported JSLibrary option");

            helper.IncludeScriptText(script);
            helper.IncludeStandardScripts(jsLibrary);
            helper.IncludeScriptText("\nvar CollapseComponentDefaults = {effect: 'blind', effectDuration:0.3,expandLinkText: 'Show', collapseLinkText: 'Hide'};");
        }

        private void RenderHeader()
        {
            string toolTipAttribute = "title='Click to expand/collapse'";

            if (toggleOnClickHeader)
                RenderTextFormat("<div class='header' onclick='{0}' style='cursor:pointer;' {1}><table><tr>", javascriptCall, toolTipAttribute);
            else
                RenderText("<div class='header'><table><tr>");

            if (useImage)
            {
                RenderTextFormat("<td><img id='{0}' src='{1}' class='toggleImage' onclick='{2}' alt='{3}' {4}/></td>",
                    toggleId, initialImage, javascriptCall, initialCommand, toolTipAttribute);
            }

            RenderTextFormat("<td class='caption'>");
            if (Context.HasSection("caption"))
            {
                RenderSection("caption");
            }
            else
            {
                RenderText(caption);
            }
            RenderTextFormat("</td>");

            if (!useImage && ! toggleOnClickHeader)
            {
                RenderTextFormat("<td class='toggleLink'><a id='{0}' href='{1}' class='toggleLink'>{2}</a></td>",
                    toggleId, javascriptCall, initialCommand);
            }

            RenderText("</tr></table></div>");
        }

        private void RenderBodySection()
        {
            RenderTextFormat("<div id='{0}' class='body'{1}>", 
                bodyId, collapsed ? "style='display:none'" : null);

            if (Context.HasSection("body"))
            {
                RenderSection("body");
            }

            RenderText("</div>");
        }
    }
}
