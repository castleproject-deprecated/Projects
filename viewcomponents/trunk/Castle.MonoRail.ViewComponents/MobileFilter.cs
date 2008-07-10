
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

namespace Castle.MonoRail.Framework.Filters
{
    using System;
    using Castle.MonoRail.Framework;

    /// <summary>
    /// Detects if controller is being accessed via a mobile device. <para/>
    /// </summary>
    /// <remarks>
    /// Detects if controller is being accessed via a mobile device.  
    /// If so, it will change the layout name by prepending <c>"mobile-"</c> to the name, and by defining
    /// a PropertyBag value <c>"_IsMobileDevice"</c>.<para/>
    /// 
    /// This assumes that for the most part, the layout contain a lot of extreneous 
    /// elements (fly-out menus, images) as well as the CSS include.  By changing the 
    /// layout (and including a different CSS) the same view template can be reformated
    /// to be presentable on a mobile device.<para/>
    /// 
    /// This will detect any mobile device that has a *.browser file defined either for the system 
    /// (in C:\Windows\Microsoft.NET\Framework\v2.0.50727\CONFIG\Browsers) or locally (in ~\App_Browsers).
    /// Since these tend to be out of date, it also triggers on "Windows CE" or "Smartphone" in the 
    /// user-agent.<para/>
    /// 
    /// TODO: An explicit test for iPhones should probably be added.<para/>
    /// </remarks>
    /// <example><code>
    ///  [Layout("default"), Rescue("generalerror")]
    ///  [Filter(ExecuteWhen.BeforeAction,typeof(MobileFilter))]
    ///  public class ShowController : SmartDispatcherController
    /// </code>
    /// If viewed from a mobile device, the layout will be changed to "mobile-default".
    /// </example>
    public class MobileFilter : IFilter
    {
        #region IFilter Members

		/// <summary>
		/// Called by Framework.
		/// </summary>
		/// <param name="exec">When this filter is being invoked. Must be BeforeAction</param>
		/// <param name="context">Current context</param>
		/// <param name="controller">The controller instance</param>
		/// <param name="controllerContext">The controller context.</param>
		/// <returns><c>true</c> Always</returns>

		public bool Perform(ExecuteWhen exec, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			if (exec != ExecuteWhen.BeforeAction)
			{
				throw new ControllerException("MobileFilter can only be performed as a ExecuteWhen.BeforeAction");
			}

			if (context.UnderlyingContext.Request.Browser.IsMobileDevice ||
				 context.UnderlyingContext.Request.UserAgent.Contains("Windows CE") ||
				 context.UnderlyingContext.Request.UserAgent.Contains("Smartphone"))
			{
				controllerContext.LayoutNames[0] = "mobile-" + controllerContext.LayoutNames[0];
				controllerContext.PropertyBag["_IsMobileDevice"] = "True";
			}
			return true;
		}

		#endregion
	}
}
