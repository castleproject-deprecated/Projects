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


namespace Castle.MonoRail.Views.AspView.Tests
{
    using System;
    using Castle.MonoRail.TestSupport;
    using NUnit.Framework;
    using System.Diagnostics;

    [TestFixture]
    public class SanityTestCase : AbstractMRTestCase
    {
        [Test]
        public void SanityTest()
        {
            #region expected
            string expected = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">

<html xmlns=""http://www.w3.org/1999/xhtml"" >
<head>
    <title>AspView layout test</title>
</head>
<body>
    <div>
        hello from default layout
    </div>
    <div>
        hello from index<br />
This are the strings:<br />
    string no 1<br />
    string no 2<br />
    string no 3<br />
        
<br />
End of normal view
<br />
<div>Hello</div>
<div>1</div><form action=""Print.rails"">
<input type=""text"" name=""theText"" />
<input type=""submit"" value=""send"" />
</form>    </div>
</body>
</html>
";
            #endregion
            DoGet("home/index.rails");
            AssertReplyEqualTo(expected);
        }
    }
}
