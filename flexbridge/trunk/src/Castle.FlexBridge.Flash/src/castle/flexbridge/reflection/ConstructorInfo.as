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
	 * Provides reflection information about a constructor.
	 */
	public class ConstructorInfo
	{
		private var _constructorElement:XML;
		
		/**
		 * Creates a reflection wrapper.
		 * @param constructorElement The &lt;constructor&gt; element of the
		 * XML returned by describeType().
		 */
		public function ConstructorInfo(constructorElement:XML)
		{
			_constructorElement = constructorElement;
		}
		
		/**
		 * Gets the array of constructor parameters.
		 */
		[ArrayElementType("castle.flexbridge.reflection.ParameterInfo")]
		public function get parameters():Array
		{
			return ReflectionUtils.convertXMLListToArray(
				_constructorElement.elements("parameter"),
				function(parameterElement:XML):ParameterInfo { return new ParameterInfo(parameterElement) });
		}
	}
}