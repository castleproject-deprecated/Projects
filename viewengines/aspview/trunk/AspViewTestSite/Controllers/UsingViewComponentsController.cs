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

namespace AspViewTestSite.Controllers
{
	using Castle.MonoRail.Framework;
	using System;

	public class UsingViewComponentsController : Controller
	{
		public void Simple() { }
		public void WithBody() { }
		public void WithSections() 
		{
			PropertyBag["items"] = new string[5] { "AspView", "Can", "Now", "Handle", "ViewComponents" };
		}
		public void UsingCaptureFor()
		{
			LayoutName = "UsingCaptureFor";
		}
		public void Nested()
		{
		}
		public void NestedInCaptureFor()
		{
			LayoutName = "UsingCaptureFor";
		}
		
		public void UsingMultipleViewComponents()
		{
			LayoutName = "UsingMultipleViewComponents";
		}

		public void UsingComponentWithDotInItsName()
		{ 
		}

		public void UsingComponentWithASingleLetterName()
		{
		}

		public void UsingComponentWithDotInAParameterValue()
		{
		}

		public void UsingComponentWithDotInALiteralParameterValue()
		{
		}

		public void PassNonStringValueToViewComponent() 
		{
			PropertyBag["version"] = new Version(1, 2, 3, 4);
		}
		
	}
}
