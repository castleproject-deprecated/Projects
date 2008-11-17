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
    using System.Collections.Generic;
    using System.Web;

    using Castle.MonoRail.Framework;
    #endregion

    /// <summary>
    /// 
    /// ViewComponent to display a set of text hyperlinks that enable users to more easily navigate a Web site, 
    /// while taking a minimal amount of page space.
    /// 
    /// </summary><remarks><para>
    /// ViewComponent modeled after the standard ASP.NET <see cref="SiteMapPath"/> WebControl.
    /// In fact, much of the documentation below has been adapted from the MSDN entry on <see cref="SiteMapPath"/>
    /// 
    /// </para><para>
    /// 
    /// The BreadcrumbViewComponent is a site navigation control that reflects data provided by the <see cref="SiteMap"/> object. 
    /// It provides a space-saving way to easily navigate a site and serves as a point of reference for where the currently displayed 
    /// page is within a site. This type of control is commonly called a breadcrumb, or eyebrow, because it displays a hierarchical path 
    /// of hyperlinked page names that provides an escape up the hierarchy of pages from the current location.
    /// 
    /// </para><para>
    /// 
    /// The SiteMapPath control works directly with your Web site's site map data. If you use it on a page that is not represented 
    /// in your site map, it will not be displayed (it will generate an empty "&lt;div>&lt;/div>", however)
    /// 
    /// </para>
    ///  <para>
    /// 
    /// BreadcrumbViewComponent is a line component with 11 optional parameters.
    ///  
    /// </para><para><list type="table"><listheader>
    /// 
    /// <term>                                Section                                                             </term>
    /// <description>      Description                                                                       </description>
    /// </listheader>
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
    /// <term>                                 cssClassNode                                                                 </term>
    /// <description>
    /// 
    /// The name of the CSS class to be used for the titles &amp; links for nodes, besides the root &amp; current.
    /// string, defaults to "breadcrumbNode"
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 cssClassCurrent                                                                 </term>
    /// <description>
    /// 
    /// The name of the CSS class to be used for the titles &amp; links for the current node.
    /// String, defaults to the value of cssClassNode.
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 cssClassRoot                                                               </term>
    /// <description>
    /// 
    /// The name of the CSS class to be used for the titles &amp; links for the root node.
    /// string, defaults to the value of cssClassNode.
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 cssClassSeparator                                                                 </term>
    /// <description>
    /// 
    /// The name of the CSS class to be used for the separator between nodes
    /// string, defaults to the value of cssClassNode.
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 PathSeparator                                                                 </term>
    /// <description>
    /// 
    /// Text be used for the separator between nodes.  MUST be Html Encoded
    /// string, defaults to "&amp;nbsp;&amp;gt;&amp;nbsp;"   (" > ")
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 RenderCurrentNodeAsLink                                                                 </term>
    /// <description>
    /// 
    /// Indicates whether the site navigation node that represents the currently displayed page is
    /// rendered as a hyperlink. (Boolean, default to false)
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 ShowToolTips                                                                 </term>
    /// <description>
    /// 
    /// Gets or sets a value indicating whether an additional hyperlink attribute for navigation 
    /// nodes is written. Depending on client support, when a mouse hovers over a node that has 
    /// the additional attribute set, a ToolTip is displayed.  (Boolean, default to true)
    /// 
    /// </description></item>
    /// <item>
    /// <term>                                 ShowRoot                                                                 </term>
    /// <description>
    /// 
    /// Indicates whether the top-most site navigation node is displayed. (Boolean, default to true)
    /// 
    /// </description></item>
    ///  
    /// </list>
    /// </para>
    ///   TODO:  Future development will include implementing the PathDirection property as it is in SiteMapPath,
    ///   and adding section support for templates for #Node, #CurrentNode, #RootNode, and #PathSeparator
    /// </remarks>
    public class BreadcrumbViewComponent : ViewComponentUsingSiteMap
    {
        /// <summary>
        /// Gets or sets the name of the CSS class to be used for the titles &amp; links for nodes, besides the root and  current.
        /// String, defaults to "breadcrumbNode"
        /// </summary>
        /// <value>The CSS class.</value>
        [ViewComponentParam(Default = "breadcrumbNode")]
        public string cssClassNode { get; set; }

        /// <summary>
        /// Gets or sets the name of the CSS class to be used for the titles &amp; links for the current node.
        /// String, defaults to the value of <see cref="cssClassNode"/>.
        /// </summary>
        /// <value>The CSS class.</value>
        [ViewComponentParam]
        public string cssClassCurrent { get; set; }

        /// <summary>
        /// Gets or sets the name of the CSS class to be used for the titles and links for the root node.
        /// String, defaults to the value of cssClassNode.
        /// </summary>
        /// <value>The CSS class.</value>
        [ViewComponentParam]
        public string cssClassRoot { get; set; }

        /// <summary>
        /// Gets or sets the name of the CSS class to be used for the separator between nodes.
        /// String, defaults to the value of cssClassNode.
        /// </summary>
        /// <value>The CSS class.</value>
        [ViewComponentParam]
        public string cssClassSeparator { get; set; }

        /// <summary>
        /// Gets or sets the name of the CSS class to be used for the separator between nodes
        /// string, defaults to the value of cssClassNode.
        /// </summary>
        /// <value>The path separator.</value>
        [ViewComponentParam(Default = "&nbsp;&gt;&nbsp;")]
        public string PathSeparator { get; set; }

        [ViewComponentParam(Default = "false")]
        public bool RenderCurrentNodeAsLink { get; set; }

        [ViewComponentParam(Default="true")]
        public bool ShowToolTips { get; set; }

        [ViewComponentParam(Default = "true")]
        public bool ShowRoot { get; set; }

        /// <summary>
        /// Called by the framework once the component instance
        /// is initialized
        /// </summary>
        public override void Initialize()
        {
            cssClassCurrent = cssClassCurrent ?? cssClassNode;
            cssClassRoot = cssClassRoot ?? cssClassNode;
            cssClassSeparator = cssClassSeparator ?? cssClassNode;
            base.Initialize();
        }

        /// <summary>
        /// Called by the framework so the component can
        /// render its content
        /// </summary>
        public override void Render()
        {
            base.Render();

            // Find current page in SiteMap
            string page = this.HttpContext.Request.AppRelativeCurrentExecutionFilePath;
            SiteMapNode node = this.Provider.FindSiteMapNode(this.HttpContext);

            Stack<SiteMapNode> nodeStack = new Stack<SiteMapNode>();
            while (node != null)
            {
                nodeStack.Push(node);
                node = node.ParentNode;
            }

            RenderText("<div>");
            // the "real" work (aka the standard case) is handled by the while() loop below.
            // But we must also treat the first (root) and last (current) as special cases.
            // 
            if (nodeStack.Count > 1)
            {
                node = nodeStack.Pop();
                if (this.ShowRoot)
                {
                    RenderNode(node, cssClassRoot, false);
                    RenderSeparator();
                }

                while (nodeStack.Count > 1)
                {
                    node = nodeStack.Pop();

                    RenderNode(node, cssClassNode, false);
                    RenderSeparator();
                }
            }

            if (nodeStack.Count > 0)
            {
                node = nodeStack.Pop();
                RenderNode(node, cssClassCurrent, !this.RenderCurrentNodeAsLink);
            }
            RenderText("</div>");
            CancelView();
        }

        private static readonly string[] nodeFormats = 
        {
            @"<span class=""{0}"" title=""{3}""><a href=""{1}"">{2}</a></span>",       // link + title
            @"<span class=""{0}""><a href=""{1}"">{2}</a></span>",                           // link, no title
            @"<span class=""{0}"" title=""{3}"">{2}</span>",                                     // no link, title
            @"<span class=""{0}"">{2}</span>"                                                         // no link, no title
        };

        void RenderNode(SiteMapNode node, string cssClass, bool noLink)
        {
            int inx=0;
          
            if (noLink || string.IsNullOrEmpty(node.Url))
                inx+=2;

            if (!this.ShowToolTips || String.IsNullOrEmpty(node.Description))
                inx+=1;

            RenderTextFormat(nodeFormats[inx], cssClass, node.Url, node.Title, node.Description);
        }

        void RenderSeparator()
        {
            RenderTextFormat(@"<span class=""{0}"">{1}</span>", cssClassSeparator, this.PathSeparator);
        }
    }
}
