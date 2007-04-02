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
	public class ViewComponentsTestFixture : AbstractMRTestCase
    {
        [Test]
        public void SimpleViewComponentWorks()
        {
            #region expected
			string expected = @"A simple viewcomponent, without a body and sections
Hello from SimpleViewComponentI was supposed to be rendered after the viewcomponent";
            #endregion
            DoGet("UsingViewComponents/Simple.rails");
            AssertReplyEqualTo(expected);
        }
		[Test]
		public void ViewComponentWithBodyWorks()
		{
			#region expected
			string expected = @"A simple viewcomponent, without a body and sections
<b>I was supposed to be rendered in the viewcomponent</b>I was supposed to be rendered after the viewcomponent";
			#endregion
			DoGet("UsingViewComponents/WithBody.rails");
			AssertReplyEqualTo(expected);
		}
		[Test]
		public void ViewComponentWithSectionsWorks()
		{
			#region expected
			string expected = @"A simple viewcomponent, without a body and sections
<table id='grid'>	<table>
		<thead>
			<th>Id</th>
			<th>Word</th>
		</thead>
			<tr>
			<td>1</td>
			<td>AspView</td>
		</tr>
			<tr>
			<td>1</td>
			<td>Can</td>
		</tr>
			<tr>
			<td>1</td>
			<td>Now</td>
		</tr>
			<tr>
			<td>1</td>
			<td>Handle</td>
		</tr>
			<tr>
			<td>1</td>
			<td>ViewComponents</td>
		</tr>
		</table>
	</table><div class='pagination'><table><tr><td>
Showing 1 - 5 of 5
</td><td align='right'>first | prev | next | last</td></tr></table></div>I was supposed to be rendered after the viewcomponent";
			#endregion
			DoGet("UsingViewComponents/WithSections.rails");
			AssertReplyEqualTo(expected);
		}
		[Test]
		public void CaptureForWorks()
		{
			#region expected
			string expected = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">

<html xmlns=""http://www.w3.org/1999/xhtml"" >
<head>
    <title>AspView layout test</title>
</head>
<body>
    <div>
        hello from UsingCaptureFor layout
    </div>
    <div>
		<h1>Under me should appear the regular content of the view</h1>
        a. Some text, located before the capturedContent component
b. Some text, located after the capturedContent component
This text should be rendered right after text a.
    </div>
    <div>
		<h1>Under me should appear the contents of a CaptureFor component, with id=""capturedContent""</h1>
		This content should be rendered in the captured-for place holder
    </div>
</body>
</html>";
			#endregion
			DoGet("UsingViewComponents/UsingCaptureFor.rails");
			AssertReplyEqualTo(expected);
		}
		[Test]
		public void MultipleViewComponentsWorksTogether()
		{
			#region expected
			string expected = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">

<html xmlns=""http://www.w3.org/1999/xhtml"" >
<head>
    <title>AspView layout test</title>
</head>
<body>
    <div>
        hello from UsingMultipleViewComponents layout
    </div>
    <div>
		<h1>Under me should appear the regular content of the view</h1>
        Some view text
Some view text
The next text should be bolded:
<b>I should be bold, some variable textand within a BoldViewComponent</b>Some view text - not bolded    </div>
    <div>
		<h1>Under me should appear the contents of a CaptureFor component, with id=""capturedContent1""</h1>
		This content should be rendered in the captured-for place holder no. 1
    </div>
    <div>
		<h1>Under me should appear the contents of a CaptureFor component, with id=""capturedContent2""</h1>
		This content should be rendered in the captured-for place holder no. 2
    </div>
</body>
</html>";
			#endregion
			DoGet("UsingViewComponents/UsingMultipleViewComponents.rails");
			AssertReplyEqualTo(expected);
		}
	}
}
