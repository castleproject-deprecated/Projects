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

    public class HomeController : SmartDispatcherController
    {
        [Layout("default")]
        public void Index()
        {
            string[] strings = new string[3];
            strings[0] = "string no 1";
            strings[1] = "string no 2";
            strings[2] = "string no 3";
            PropertyBag["strings"] = strings;
        }
		public void NullableValueTypesWithDefaultValue()
		{
		}
        public void Print(string theText)
        {
            CancelView();
            RenderText("hello from print(). theText='{0}'", theText);
        }
        public void SiteRoot()
        {
        }
        public void DefaultValues()
        {
        }
    }
}
