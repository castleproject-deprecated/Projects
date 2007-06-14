// Copyright 2007 Castle Project - http://www.castleproject.org/
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

package castle.flexbridge.tests
{
	import flexunit.framework.TestSuite;
	import castle.flexbridge.tests.reflection.ReflectionUtilsTest;
	import castle.flexbridge.tests.kernel.DefaultKernelTest;
	import castle.flexbridge.tests.common.Base64Test;
	
	public class UnitTestSuite extends TestSuite
	{
		public function UnitTestSuite()
		{
 			addTestSuite(ReflectionUtilsTest);
 			addTestSuite(DefaultKernelTest);
 			addTestSuite(Base64Test);
		}
	}
}