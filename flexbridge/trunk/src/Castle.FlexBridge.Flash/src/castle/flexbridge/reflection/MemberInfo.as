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
	 * Abstract base class for class member reflection types.
	 */
	public class MemberInfo
	{
		private var _xmlElement:XML;
		private var _isStatic:Boolean;
		
		/**
		 * Creates a reflection wrapper.
		 * @param xmlElement The XML element of the returned by describeType().
		 * @param isStatic True if the member is static.
		 */
		public function MemberInfo(xmlElement:XML, isStatic:Boolean)
		{
			_xmlElement = xmlElement;
			_isStatic = isStatic;
		}
		
		/**
		 * Returns true if the member is static.
		 */
		public function get isStatic():Boolean
		{
			return _isStatic;
		}
		
		/**
		 * Gets the XML element returned by describeType().
		 */
		protected function get xmlElement():XML
		{
			return _xmlElement;
		}
	}
}