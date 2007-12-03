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

namespace Castle.MonoRail.Views.AspView.Tests.HttpBasedIntegrationTests
{
    using NUnit.Framework;

    [TestFixture]
	public class UsingReferencesTestCase : BaseHttpBasedIntegrationTestFixture
    {
        [Test]
        public void Show()
        {
            string expected = @"<form action=""Save.rails"">
    <input type=""text"" name=""post.PublishDate"" value='01/01/2001 00:00:00' />
    <input type=""text"" name=""post.Content"" value='AspView rock' />
    <input type=""submit"" value=""Save"" />
</form>
";
            DoGet("UsingReferences/Show.rails");
            AssertReplyEqualTo(expected);
        }
        [Test]
        public void Save()
        {
            string expected = "Post saved: date:01/01/2005, content:AspView rock";
            DoPost("UsingReferences/Save.rails","post.PublishDate=01/01/2005 00:00:00","post.Content=AspView rock");
            AssertReplyEqualTo(expected);
        }

    }
}