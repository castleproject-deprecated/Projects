#region License
// Copyright (c) 2008, James M. Curran
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
#region References
using System;
using System.Web;
using System.Web.Security;
using Castle.MonoRail.Framework;
using System.Resources;
using System.Reflection;
#endregion

    /// <summary>
    /// MenuViewComponent - ViewComponent to generate a multi-level "drop-down" menu, from a SiteMap.
    /// </summary>
    /// <remarks><para>
    /// ViewComponent to generate a multi-level "drop-down" menu, from a SiteMap.
    /// </para>
    /// <b>Features:</b>
    /// <list type="bullet">
    /// <item><description>               Works with jQuery or prototype/scriptaculous.
    /// </description></item>
    /// <item><description>               Degrades gracefully in absence of javascript.
    /// </description></item>
    /// <item><description>               Uses simple Html (UL/LI) for easy styling with CSS.
    /// </description></item>
    /// <item><description>               Degrades gracefully in absence of CSS.
    /// </description></item>
    /// <item><description>               Works using ASP.NET standard web.sitemap provider.
    /// </description></item>
    /// <item><description>               Allows hiding menu items based on ASP.NET membership roles providers..
    /// </description></item>
    /// </list>
    /// <para>
    /// Menu VC is a line component which has eight optional parameters.
    /// </para>
    /// <para><list type="table"><listheader>
    /// 
    /// <term>                                Section                                                             </term>
    /// <description>      Description                                                                       </description>
    /// </listheader>
    /// <item>
    /// <term>                                 Id                                                                 </term>
    /// <description>
    /// 
    /// string, optional.  Unique identifer for this control. Defaults to a random string.
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 SiteMapFile                                                                 </term>
    /// <description>
    /// 
    /// The site map file  String, Optional, defaults to "~/web.sitemap"
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 Provider                                                                 </term>
    /// <description>
    /// 
    /// A <see cref="SiteMapProvider"/> that is associated with the ViewComponent. You will generally not need
    /// to set this.  It is normally set by setting the <see cref="SiteMapProvider"/> property; or, if that's not set,
    /// to the default specified in the web.config; or, to the file specified by the <see cref="SiteMapFile"/> property;
    /// or, to the web.sitemap file.
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 SiteMapProvider                                                                 </term>
    /// <description>
    /// 
    /// The name of the <see cref="SiteMapProvider"/> used to render the site navigation control.
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 cssClass                                                                 </term>
    /// <description>
    /// 
    /// string, optional.  CSS Class assigned to menu.  Defaults to "Menu".
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 JSLibrary                                                                 </term>
    /// <description>
    /// 
    /// string, optional.  Specifes which Javascript library to use.  Presently only "<c>prototype</c>" 
    /// and "<c>jquery</c>" are implemented.
    /// Defaults to value in <c>jslibraries.xml</c>, if present, or "<c>prototype</c>".   <br/> 
    /// May also be set to "<c>none</c>" in which case, a static fully-expanded menu is created.
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 Direction                                                                 </term>
    /// <description>
    /// 
    /// string, optional.  Specifes how the menu should appear, either "horizontal" or "vertical"
    /// Defaults "vertical".   Setting to "horizontal" will have no effect unless InlineStyles is <c>true</c>.
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 InlineStyles                                                                 </term>
    /// <description>
    /// 
    /// boolean, optional.  Specifes whether (if <c>true</c>) required CSS style 
    /// attrbutes should be added to the Html elements., or (if <c>false</c>), they are to be added manually 
    /// via a stylesheet.  Defaults to true.  Has no effect when Direction is vertical.  Unless properly styled, 
    /// menus will appear vertically (regardless of Direction parameter)  See below for needed CSS attributes.
    /// 
    /// </description></item>
    /// </list></para>
    /// <b>CSS Styling</b>
    /// <para>Several CSS classes are added to various elements to aid in CS styling of the menu.
    /// </para>
    /// <list type="bullet">
    /// <item><term>                                  MenuFrame                                                     </term>
    /// <description>
    ///     On the topmost UL element.
    /// </description></item>
    /// <item><term>                                  SubMenu                                                     </term>
    /// <description>
    ///     On each nested UL element.
    /// </description></item>
    /// <item><term>                                  MenuItem                                                     </term>
    /// <description>
    ///     On each LI element on all levels.
    /// </description></item>
    /// <item><term>                                  collapsed                                                     </term>
    /// <description>
    ///     On initially collapsed elements.
    /// </description></item>
    /// </list>
    /// <para>For a horizontal menu, the VC will automatically insert the needed CSS styles, unless InlineStyles is set to false,
    /// in which case, you will have to add the styling manually:
    /// <code><![CDATA[
    /// .MenuFrame li
    /// {
	///   float:left;
    /// }
    /// .SubMenu li
    /// {
    /// 	float: none;
    /// }
    /// ]]></code>
    /// 
    /// You will also probably want to use CSS to add a minimum width.  Set in on the .MenuItem class.
    /// </para>
    /// 
    /// <b>Known Issues:</b>
    /// <para>
    /// 1. jQuery method fails in mixed jQuery/Prototype environments.</para>
    /// <para>
    /// 2. prototype method has an odd visual effect. </para>
    /// <para>It's possibly that both of these are the result of using scriptocolous v1.7.1 (MR standard) which is said to 
    /// be rather buggy, however, little research into this has been done.</para>
    /// <b>TODO</b>
    /// <para>
    /// 1. Expand to a block component, to allow templates for menu items.</para>
    /// <para>
    /// 2. Add methods for other JS frameworks.</para>
    /// </remarks>
    public class MenuComponent : ViewComponentUsingSiteMap
    {
        [ViewComponentParam]
        public string Id { get; set; }

        [ViewComponentParam]
        public string Direction { get; set; }

        [ViewComponentParam(Default="Menu")]
        public string cssClass { get; set; }

        [ViewComponentParam(Default=true)]
        public bool InlineStyles { get; set; }

        [ViewComponentParam(Default = "jquery")]
        public string JSLibrary { get; set; }

        bool isHoriz;



        public override void Initialize()
        {
            base.Initialize();
            if (Id == null)
                Id = MakeUniqueId("Menu");

            isHoriz = ((Direction == null) ? false : (Char.ToLower(Direction[0]) == 'h'));
        }
        
        public override void Render()
        {
            base.Render();
            RenderMenuLevel(Provider.RootNode, 0, Id);
            RenderJavascript();
            CancelView();
        }

        private void RenderJavascript()
        {
            JavascriptHelper helper = new JavascriptHelper(Context, EngineContext, "MenuComponent");
            string jslibrary = this.ComponentParams["JSLibrary"] as string ?? helper.PreferredLibrary ?? "prototype";
            ResourceManager rm = new ResourceManager("Castle.MonoRail.ViewComponents.Menu", 
                            Assembly.GetExecutingAssembly());

            switch (jslibrary.ToLowerInvariant())
            {
                case "jquery":
                    helper.IncludeScriptText(rm.GetString(jslibrary));
                    helper.IncludeStandardScripts("jquery");
                    break;

                case "prototype":
                    helper.IncludeScriptText(rm.GetString(jslibrary));
                    helper.IncludeStandardScripts("effects2");
                    break;

                case "none":
                    break;

                default:
                throw new ViewComponentException("Unsupported JavaScript library requested.  Must be 'prototype' or 'jquery'");
            }
        }


        private void RenderMenuLevel(SiteMapNode root, int level, string id)
        {
            string ulCss = "MenuFrame";
            string inlineStyle = "left";
            string LIformatC = "<li class=\"{0} collapsed MenuItem\">{1}\r\n";
            string LIformatNC = "<li class=\"{0} MenuItem\">";

            if (level > 0)
            {
                ulCss = "SubMenu";
                inlineStyle = "none";
            }
            if (isHoriz && InlineStyles)
            {
                LIformatC = "<li class=\"{0} collapsed MenuItem\" style=\"float:{2}\">{1}\r\n";
                LIformatNC = "<li class=\"{0} MenuItem\" style=\"float:{1}\">";
            }

			Logger.Info(@"<ul id=""{0}"" class=""{1} {2}"">", id, cssClass, ulCss);
            RenderTextFormat(@"<ul id=""{0}"" class=""{1} {2}"">", id, cssClass, ulCss);
            foreach (SiteMapNode item in root.ChildNodes)
            {
                bool allowed = true;
                if (item.Roles != null && item.Roles.Count > 0 && item.Roles[0].ToString() != "*")
                {
                    allowed = false;
                    foreach (string role in item.Roles)
                    {
					if (EngineContext.CurrentUser.IsInRole(role) || Roles.IsUserInRole(role))
                        {
                            allowed = true;
                            break;
                        }
                    }
                }
                if (!allowed)
                    continue;

                string title = item.Title;
                if (string.IsNullOrEmpty(title))
                {
                    if (string.IsNullOrEmpty(item.Url))
                    {
                        RenderText("<hr/>");
                    }
                    else
                    {
                        throw new ArgumentException("title element is required");
                    }
                }
                else
                {
                    if (item.HasChildNodes)
                    {
                        string menuID = MakeUniqueId("Menu:");
                        RenderTextFormat(LIformatC, cssClass, title, inlineStyle);
						RenderMenuLevel(item, level + 1, menuID);
                    }
                    else
                    {
                        RenderTextFormat(LIformatNC, cssClass, inlineStyle);

                        string description = item.Description ?? string.Empty;
                        string url = item.Url;
                        if (url[0] == '~')
                            url = VirtualPathUtility.ToAbsolute(url);

                        RenderTextFormat("<a href=\"{0}\" title=\"{1}\">{2}</a>", url, description, title);
                    }
                    RenderText("</li>\r\n");
                }
            }
            RenderText("</ul>");
        }
    }

#if false
    [Resource("Menu", "StateTheater.Component.Menu")]
    public class MenuFiles : Controller
    {
        public void Collapsed()
        {
            WriteImage("collapsed_png");
        }

        public void Expanded()
        {
            WriteImage("expanded_png");
        }

        private void WriteImage(string name)
        {
            this.Response.ContentType = "image/png";
            byte[] contents = (byte[])StateTheater.Component.Menu.ResourceManager.GetObject(name);

            this.Response.BinaryWrite(contents);
            CancelView();
        }

        void DefaultAction()
        {
        }
    }
#endif
}
