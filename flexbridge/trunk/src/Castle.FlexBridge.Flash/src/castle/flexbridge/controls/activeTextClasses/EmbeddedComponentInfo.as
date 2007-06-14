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

package castle.flexbridge.controls.activeTextClasses
{
	import flash.display.DisplayObject;
	
	/**
	 * Contains information to describe an embedded component within
	 * an ActiveTextField.
	 */
	public class EmbeddedComponentInfo
	{
		/**
		 * Gets the external id of the embedded object.
		 */
		public var id:String;
		
		/**
		 * Gets the attributes of the component as key/value pairs as
		 * they were specified in the &lt;EMBED&gt; tag.
		 */
		public var attributes:Object;
	
		/**
		 * Gets the index of the character position within the text that contains
		 * the component, or -1 if the component has not been created yet.
		 */
		public var charIndex:int = -1;
	
		/**
		 * The embedded display object, or null if not created yet.
		 */
		public var component:DisplayObject = null;
	}
}