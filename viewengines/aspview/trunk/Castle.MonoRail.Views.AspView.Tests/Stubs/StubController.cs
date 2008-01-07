#region license
// Copyright 2006-2007 Ken Egozi http://www.kenegozi.com/
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

namespace Castle.MonoRail.Views.AspView.Tests.Stubs
{
	using System;
	using Framework;

	public class ControllerStub : IController
	{
		#region IController Members

		public event ControllerHandler AfterAction;

		public event ControllerHandler BeforeAction;

		public void PostSendView(object view)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void PreSendView(object view)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		public void Process(IEngineContext engineContext, IControllerContext context)
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}
}
