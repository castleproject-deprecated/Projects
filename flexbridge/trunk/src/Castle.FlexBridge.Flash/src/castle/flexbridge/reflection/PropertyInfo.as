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
	 * Provides reflection information about a property.
	 */
	public class PropertyInfo extends MemberInfo
	{
		/**
		 * Creates a reflection wrapper.
		 * @param accessorElement The &lt;accessor&gt; element of the
		 * XML returned by describeType().
		 * @param isStatic True if the property is static.
		 */
		public function PropertyInfo(accessorElement:XML, isStatic:Boolean)
		{
			super(accessorElement, isStatic);
		}
		
		/**
		 * Gets the name of the property.
		 */
		public function get name():String
		{
			return xmlElement.@name;
		}
		
		/**
		 * Gets the type of the property.
		 */
		public function get type():Class
		{
			return ReflectionUtils.getClassByName(xmlElement.@type);
		}
		
		/**
		 * Gets the declaring type of the property.
		 */
		public function get declaringType():Class
		{
			return ReflectionUtils.getClassByName(xmlElement.@declaredBy);
		}
		
		/**
		 * Returns true if the property is readable.
		 */
		public function get isReadable():Boolean
		{
			return xmlElement.@access != "writeonly";
		}
		
		/**
		 * Returns true if the property is writeable.
		 */
		public function get isWriteable():Boolean
		{
			return xmlElement.@access != "readonly";
		}
	}
}