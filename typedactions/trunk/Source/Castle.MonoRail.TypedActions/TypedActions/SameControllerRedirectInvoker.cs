#region Licence

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

#endregion

namespace Castle.MonoRail.TypedActions
{
	using System.Collections;
	using Framework;

	/// <summary>
	/// Redirect invoker for redirection to actions in the current <see cref="Controller"/>.
	/// <para>
	/// If no routing is used, it redirects using 
	/// <see cref="Controller.RedirectToAction(string,IDictionary)"/>.
	/// If <see cref="ResponseRedirectInvoker.UseRouting"/> is true, the standard redirect 
	/// from <see cref="ResponseRedirectInvoker"/> is used.
	/// </para>
	/// </summary>
	public class SameControllerRedirectInvoker : ResponseRedirectInvoker
	{
		private readonly Controller _controller;

		public SameControllerRedirectInvoker(Controller controller)
		{
			_controller = controller;
		}

		public SameControllerRedirectInvoker(Controller controller, bool useRouting)
			: base(useRouting)
		{
			_controller = controller;
		}

		protected override void Redirect(IRedirectSupport redirecter, string area, string controller, string action,
		                                 IDictionary parameters)
		{
			if (UseRouting)
			{
				base.Redirect(redirecter, area, controller, action, parameters);
			}
			else
			{
				if (redirecter != _controller)
				{
					throw new TypedActionException(
						"The controller and the redirecter must be the same instance, but they are not. " +
						"Controller = {0} and redirecter = {1}.", _controller, redirecter);
				}
				_controller.RedirectToAction(action, parameters);
			}
		}
	}
}