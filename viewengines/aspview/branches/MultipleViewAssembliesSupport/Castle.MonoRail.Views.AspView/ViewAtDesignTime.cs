// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.AspView
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;
    using Castle.MonoRail.Framework;
    using Castle.MonoRail.Framework.Helpers;

    public class ViewAtDesignTime : System.Web.UI.Page
    {
        #region mocking AspViewBase
        public Dictionary<string, object> Properties
        { get { throw new ShouldNotBeImplemented(); } }
        public void OutputSubView(string subViewName)
        { throw new ShouldNotBeImplemented(); }
        public void OutputSubView(string subViewName, IDictionary<string, object> parameters)
        { throw new ShouldNotBeImplemented(); }
        public string ViewContents
        { get { throw new ShouldNotBeImplemented(); } }
        public void Output(string message) { }
        public void Output(string message, params object[] arguments) { }

        public AjaxHelper AjaxHelper
        { get { throw new ShouldNotBeImplemented(); } }
		protected BehaviourHelper BehaviourHelper
		{ get { throw new ShouldNotBeImplemented(); } }
		protected DateFormatHelper DateFormatHelper
		{ get { throw new ShouldNotBeImplemented(); } }
		protected DictHelper DictHelper
        { get { throw new ShouldNotBeImplemented(); } }
        protected EffectsFatHelper EffectsFatHelper
        { get { throw new ShouldNotBeImplemented(); } }
        protected FormHelper FormHelper
        { get { throw new ShouldNotBeImplemented(); } }
        protected HtmlHelper HtmlHelper
        { get { throw new ShouldNotBeImplemented(); } }
        protected PaginationHelper PaginationHelper
        { get { throw new ShouldNotBeImplemented(); } }
		protected PrototypeHelper PrototypeHelper
		{ get { throw new ShouldNotBeImplemented(); } }
		protected ScriptaculousHelper ScriptaculousHelper
        { get { throw new ShouldNotBeImplemented(); } }
		protected TextHelper TextHelper
		{ get { throw new ShouldNotBeImplemented(); } }
		protected UrlHelper UrlHelper
		{ get { throw new ShouldNotBeImplemented(); } }
        protected ValidationHelper ValidationHelper
        { get { throw new ShouldNotBeImplemented(); } }
        protected WizardHelper WizardHelper
        { get { throw new ShouldNotBeImplemented(); } }
		protected ZebdaHelper ZebdaHelper
		{ get { throw new ShouldNotBeImplemented(); } }
		
        #endregion

        public class ShouldNotBeImplemented : NotImplementedException
        {
            public ShouldNotBeImplemented() :
                base("This method is a mock for intellisense purposes only. It should not be called in runtime through this class or any of it's successors")
            {}
        }
    }
}
