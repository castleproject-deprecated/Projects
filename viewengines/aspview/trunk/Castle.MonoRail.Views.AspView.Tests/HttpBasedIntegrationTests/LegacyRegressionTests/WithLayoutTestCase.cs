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

namespace Castle.MonoRail.Views.AspView.Tests
{
	using NUnit.Framework;
    using TestSupport;

    [TestFixture]
    public class WithLayoutTestCase : AbstractMRTestCase
    {
        [Test]
        public void WithoutProperties()
        {
            string expected = DecorateWithDefaultLayout("A View without any properties");
            DoGet("Withlayout/WithoutProperties.rails");
            AssertReplyEqualTo(expected);
        }
        [Test]
        public void WithPropertiesUsingFlash()
        {
            string expected = DecorateWithDefaultLayout("A View with properties: Flashed data");
            DoGet("Withlayout/UsingFlash.rails");
            AssertReplyEqualTo(expected);
        }
        [Test]
        public void WithPropertiesUsingPropertyBag()
        {
            string expected = DecorateWithDefaultLayout("A View with properties: PropertyBaged data");
            DoGet("Withlayout/UsingPropertyBag.rails");
            AssertReplyEqualTo(expected);
        }

        private string DecorateWithDefaultLayout(string html)
        {
            return string.Format(
                @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">

<html xmlns=""http://www.w3.org/1999/xhtml"" >
<head>
    <title>AspView layout test</title>
</head>
<body>
    <div>
        hello from default layout
    </div>
    <div>
        {0}
    </div>
</body>
</html>
",html);
        }

    }
}
