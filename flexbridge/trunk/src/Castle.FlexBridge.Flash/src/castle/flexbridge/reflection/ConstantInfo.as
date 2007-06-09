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

package castle.flexbridge.reflection
{
	/**
	 * Provides reflection information about a constant.
	 */
	public class ConstantInfo extends MemberInfo
	{
		/**
		 * Creates a reflection wrapper.
		 * @param constantElement The &lt;constant&gt; element of the
		 * XML returned by describeType().
		 * @param isStatus True if the constant is static.
		 */
		public function ConstantInfo(constantElement:XML, isStatic:Boolean)
		{
			super(constantElement, isStatic);
		}
		
		/**
		 * Gets the name of the constant.
		 */
		public function get name():String
		{
			return xmlElement.@name;
		}
		
		/**
		 * Gets the type of the constant.
		 */
		public function get type():Class
		{
			return ReflectionUtils.getClassByName(xmlElement.@type);
		}
	}
}