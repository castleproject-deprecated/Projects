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
    using TestModel;
    using System;

    public class UsingReferencesController : SmartDispatcherController
	{
        public void Show()
        {
            Post post = new Post();
            post.PublishDate = new DateTime(2001, 01, 01);
            post.Content = "AspView rock";
            PropertyBag["post"] = post;
        }

        public void Save([DataBind("post")]Post post)
        {
            CancelView();
            RenderText("Post saved: date:{0}, content:{1}", post.PublishDate.ToString("dd/MM/yyyy"), post.Content);
        }
	}
}

