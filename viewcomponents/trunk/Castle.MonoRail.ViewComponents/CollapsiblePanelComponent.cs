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
    using System;
    using Framework;
    using Framework.Helpers;
    using ViewComponents;

    /// <summary>
    /// A ViewComponent that renders a collapsible panel.
    /// </summary>
    [ViewComponentDetails("CollapsiblePanel",Sections="body,caption")]
    public class CollapsiblePanelComponent : ViewComponentEx
    {
        private string javascriptFunctionName;
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
        private double effectDuration;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public override void Initialize()
        {
            GetParameters();

            initialCommand = collapsed ? expandLinkText : collapseLinkText;
            bodyId = string.Format("{0}Body", id);
            toggleId = string.Format("{0}Toggle", id);
            javascriptCall = string.Format("javascript:{0}(\"{1}\", \"{2}\")",
                javascriptFunctionName, bodyId, toggleId);
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
            expandLinkText = ComponentParams["expandLinkText"] as string ?? "Show";
            collapseLinkText = ComponentParams["collapseLinkText"] as string ?? "Hide";
            caption = ComponentParams["caption"] as string;
            cssClass = ComponentParams["cssClass"] as string;
            style = ComponentParams["style"] as string;
            collapsed = GetBoolParamValue("collapsed", false);
            effect = ComponentParams["effect"] as string ?? "blind";
            effectDuration = ComponentParams["effectDuration"] as double? ?? 0.3;
            toggleOnClickHeader = GetBoolParamValue("toggleOnClickHeader", false);
            javascriptFunctionName = string.Format("expandCollapse_{0}", id);
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
            RenderTextFormat("<div id='{0}' class='{1}'{2}>", id,
                !string.IsNullOrEmpty(cssClass) 
                    ? cssClass 
                    : "collapsiblePanel",
                !string.IsNullOrEmpty(style) 
                    ? string.Format("style='{0}'", style) 
                    : null);

            RenderHeader();

            RenderBodySection();

            RenderText("</div>");
        }

        private void RenderJavascript()
        {
            if (Context.ContextVars[wasScriptaculousInstalledKey] as bool? != true)
            {
				AjaxHelper ajaxHelper = new AjaxHelper(EngineContext);
				RenderTextFormat("\r\n{0}\r\n", ajaxHelper.InstallScripts());
				ScriptaculousHelper helper = new ScriptaculousHelper(EngineContext);
                RenderTextFormat("\r\n{0}\r\n", helper.InstallScripts());
                Context.ContextVars[wasScriptaculousInstalledKey] = true;
            }
            RenderText(AjaxHelper.ScriptBlock(ToggleJsFunction));
        }

        private void RenderHeader()
        {
            string toolTipAttribute = "title='Click to expand/collapse'";

            RenderTextFormat("<div class='header'{0}><table><tr>", 
                toggleOnClickHeader 
                    ? string.Format(" onclick='{0}' style='cursor:pointer;' {1}",
                        javascriptCall, toolTipAttribute) 
                    : null);

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

        private string ToggleJsFunction
        {
            get
            {
				if (!toggleOnClickHeader)
				{
					return string.Format(
                        CultureInfo.InvariantCulture,
	@"
function {0}(controlName, togglerName)
{{
    new Effect.toggle(controlName, '{1}', {{duration:{2}}});
    var toggler = document.getElementById(togglerName);
    if (toggler.{3} == '{4}')
    {{
        toggler.{3} = '{5}';{6}
    }}
    else if (toggler.{3} == '{5}')
    {{
        toggler.{3} = '{4}';{7}
    }}
}}
",
					javascriptFunctionName,
					effect,
					effectDuration,
					useImage ? "alt" : "innerHTML",
					collapseLinkText,
					expandLinkText,
					useImage ? string.Format(" toggler.src = '{0}';", expandImagePath) : null,
					useImage ? string.Format(" toggler.src = '{0}';", collapseImagePath) : null);
				}
				else
				{
					return string.Format(
	@"
function {0}(controlName, togglerName)
{{
    new Effect.toggle(controlName, '{1}', {{duration:{2}}});
}}
",
					javascriptFunctionName,
					effect,
					effectDuration);
				}
            }
        }
    }
}
