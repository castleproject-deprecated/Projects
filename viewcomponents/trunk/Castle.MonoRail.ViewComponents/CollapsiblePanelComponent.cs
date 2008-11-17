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
    [ViewComponentDetails("CollapsiblePanel",Sections="body,caption")]
    public class CollapsiblePanelComponent : ViewComponentEx
    {
        private const string wasScriptaculousInstalledKey = "wasScriptaculousInstalled";

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
            this.Flash["test"] = (this.Flash["test"] ?? "") + "+";
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
            collapsed = GetBoolParamValue("collapsed", false);
            effect = ComponentParams["effect"] as string;
            effectDuration = ComponentParams["effectDuration"] as double?;
            toggleOnClickHeader = GetBoolParamValue("toggleOnClickHeader", false);

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
            helper.IncludeScriptText("\nvar CollapseConponentDefaults = {effect: 'blind', effectDuration:0.3,expandLinkText: 'Show', collapseLinkText: 'Hide'};");
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
