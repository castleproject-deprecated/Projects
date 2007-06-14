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

package castle.flexbridge.controls.factories
{
	import flash.display.DisplayObject;
	
	/**
	 * A component factory is used to construct components based on a collection
	 * of attribute key/value pairs that may be embedded in some other content.
	 * This is used when components are embedded in places where MXML cannot be
	 * used such as in the enhanced HTML of ActiveText controls.
	 * 
	 * @see castle.flexbridge.controls.ActiveText
	 * @see castle.flexbridge.controls.ActiveTextArea
	 * @see castle.flexbridge.controls.ComponentFactoryRegistry 
	 */
	public interface IComponentFactory
	{
		/**
		 * Creates a new instance of the component.
		 * 
		 * @param attributes The name/value String pairs that describe the object
		 *   to be constructed and its properties.
		 * @return The new component instance.
		 */
		function newInstance(attributes:Object):DisplayObject;
	}
}