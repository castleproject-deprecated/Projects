// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

using NUnit.Framework;
using System.Diagnostics;
namespace Castle.MonoRail.Views.AspView.Tests
{
    using System;
    using Castle.MonoRail.TestSupport;
    using NUnit.Framework;

    [TestFixture]
    public class WithLayoutTestCase : AbstractMRTestCase
    {
        [Test]
        public void WithoutProperties()
        {
            string expected = DecorateWithDefaultLayout("\r\nA View without any properties");
            DoGet("Withlayout/WithoutProperties.rails");
            AssertReplyEqualTo(expected);
        }
        [Test]
        public void WithPropertiesUsingFlash()
        {
            string expected = DecorateWithDefaultLayout("\r\nA View with properties: Flashed data");
            DoGet("Withlayout/UsingFlash.rails");
            AssertReplyEqualTo(expected);
        }
        [Test]
        public void WithPropertiesUsingPropertyBag()
        {
            string expected = DecorateWithDefaultLayout("\r\nA View with properties: PropertyBaged data");
            DoGet("Withlayout/UsingPropertyBag.rails");
            AssertReplyEqualTo(expected);
        }

        private string DecorateWithDefaultLayout(string html)
        {
            return string.Format(
                @"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">

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
[TestFixture]
public class ATest
{
	[Test]
	public void Test()
	{
		string fileName = "\\afas\\fasfas\\fasfas.rails.rails";
		string className = fileName.Replace('\\', '_');
		int i = className.LastIndexOf('.');
		if (i > -1)
			className = className.Substring(0, i);
		Trace.WriteLine(className);
	}
}