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
	 * Provides reflection information about a method.
	 */
	public class MethodInfo extends MemberInfo
	{
		/**
		 * Creates a reflection wrapper.
		 * @param methodElement The &lt;method&gt; element of the
		 * XML returned by describeType().
		 * @param isStatic True if the method is static.
		 */
		public function MethodInfo(methodElement:XML, isStatic:Boolean)
		{
			super(methodElement, isStatic);
		}
		
		/**
		 * Gets the name of the method.
		 */
		public function get name():String
		{
			return xmlElement.@name;
		}
		
		/**
		 * Gets the declaring type of the method.
		 */
		public function get declaringType():Class
		{
			return ReflectionUtils.getClassByName(xmlElement.@declaredBy);
		}
		
		/**
		 * Gets the return type of the method.
		 */
		public function get returnType():Class
		{
			return ReflectionUtils.getClassByName(xmlElement.@returnType);
		}

		/**
		 * Gets the array of method parameters.
		 */
		[ArrayElementType("castle.flexbridge.reflection.ParameterInfo")]
		public function get parameters():Array
		{
			return ReflectionUtils.convertXMLListToArray(
				xmlElement.elements("parameter"),
				function(parameterElement:XML):ParameterInfo { return new ParameterInfo(parameterElement) });
		}
	}
}