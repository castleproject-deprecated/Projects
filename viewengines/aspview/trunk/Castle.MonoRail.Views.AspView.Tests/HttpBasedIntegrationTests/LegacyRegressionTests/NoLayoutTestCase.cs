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
    public class NoLayoutTestCase : BaseHttpBasedIntegrationTestFixture
    {
        [Test]
        public void NoLayoutWithoutProperties()
        {
            string expected = "A View without any properties";
            DoGet("nolayout/WithoutProperties.rails");
            AssertReplyEqualTo(expected);
        }
        [Test]
        public void NoLayoutWithPropertiesUsingFlash()
        {
            string expected = "A View with properties: Flashed data";
            DoGet("nolayout/UsingFlash.rails");
            AssertReplyEqualTo(expected);
        }
        [Test]
        public void NoLayoutWithPropertiesUsingPropertyBag()
        {
            string expected = "A View with properties: PropertyBaged data";
            DoGet("nolayout/UsingPropertyBag.rails");
            AssertReplyEqualTo(expected);
        }

    }
}