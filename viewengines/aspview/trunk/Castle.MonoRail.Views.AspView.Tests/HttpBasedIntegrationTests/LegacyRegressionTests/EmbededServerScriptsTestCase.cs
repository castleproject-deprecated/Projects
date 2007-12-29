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
	public class EmbededServerScriptsTestCase : BaseHttpBasedIntegrationTestFixture 
	{
		[Test]
		public void TestWithSingleServerScript() 
		{
			string expected = @"

hello 'ViewLogicIsEvil' (1 times)
<hr />

hello 'ViewLogicIsEvil' (2 times)
<hr />

hello 'ViewLogicIsEvil' (3 times)
<hr />

hello 'ViewLogicIsEvil' (4 times)
<hr />

hello 'ViewLogicIsEvil' (5 times)
<hr />

hello 'ViewLogicIsEvil' (6 times)
<hr />

hello 'ViewLogicIsEvil' (7 times)
<hr />

hello 'ViewLogicIsEvil' (8 times)
<hr />

hello 'ViewLogicIsEvil' (9 times)
<hr />

hello 'ViewLogicIsEvil' (10 times)
<hr />


hello 'it works!' (11 times)";
			DoGet("UsingEmbededServerScripts/WithSingleServerScript.rails");
			AssertReplyEqualTo(expected);
		}

		[Test]
		public void TestWithMultipleServerScripts() 
		{
			string expected = @"

hello 'ViewLogicIsEvil' (1 times)
<hr />
evil!!!

hello 'ViewLogicIsEvil' (2 times)
<hr />


hello 'ViewLogicIsEvil' (3 times)
<hr />
evil!!!

hello 'ViewLogicIsEvil' (4 times)
<hr />


hello 'ViewLogicIsEvil' (5 times)
<hr />
evil!!!

hello 'ViewLogicIsEvil' (6 times)
<hr />


hello 'ViewLogicIsEvil' (7 times)
<hr />
evil!!!

hello 'ViewLogicIsEvil' (8 times)
<hr />


hello 'ViewLogicIsEvil' (9 times)
<hr />
evil!!!

hello 'ViewLogicIsEvil' (10 times)
<hr />



hello 'it works!' (11 times)


<hr />
evil!!!
<hr />
&lt;youarehere value=&quot;&amp; encoded or not &amp;&quot;/&gt;
<hr />";
			DoGet("UsingEmbededServerScripts/WithMultipleServerScripts.rails");
			AssertReplyEqualTo(expected);
		}

	}
}
