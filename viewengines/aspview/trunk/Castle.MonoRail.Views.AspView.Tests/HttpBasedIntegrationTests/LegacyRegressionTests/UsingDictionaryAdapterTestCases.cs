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

namespace Castle.MonoRail.Views.AspView.Tests.HttpBasedIntegrationTests.LegacyRegressionTests
{
    using TestSupport;
    using NUnit.Framework;

    [TestFixture]
	public class UsingDictionaryAdapterTestCases : BaseHttpBasedIntegrationTestFixture
    {
        [Test]
		public void WithTypedPropertiesWorks()
        {
            string expected = @"<p id=""No_33"">Hello Bird, Larry</p>
<form action=""Save.rails"">
    <input type=""text"" name=""post.PublishDate"" value='16/10/2007 00:00:00' />
    <input type=""text"" name=""post.Content"" value='Some content' />
    <input type=""submit"" value=""Save"" />
</form>";
			DoGet("UsingDictionaryAdapter/WithTypedProperties.rails");
            AssertReplyEqualTo(expected);
        }
		[Test]
		public void WithInterfaceHierarchyWorks()
		{
			string expected = @"<p id=""No_32"">
<strong>
Hello McHale, Kevin
</strong>
</p>
<form action=""Save.rails"">
    <input type=""text"" name=""post.PublishDate"" value='16/10/2007 00:00:00' />
    <input type=""text"" name=""post.Content"" value='Some content' />
    <input type=""submit"" value=""Save"" />
</form>";
			DoGet("UsingDictionaryAdapter/Nested/AlsoWithTypedProperties.rails");
			AssertReplyEqualTo(expected);
		}
	}
}