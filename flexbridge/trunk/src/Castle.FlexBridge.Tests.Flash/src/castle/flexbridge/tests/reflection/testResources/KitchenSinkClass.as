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

package castle.flexbridge.tests.reflection.testResources
{
	public class KitchenSinkClass extends EmptyClass implements EmptyInterface
	{
		public function KitchenSinkClass(a:int, b:*, c:String = null)
		{
			throw new Error("Exception thrown to ensure that the robustDescribeType() hack "
				+ "can handle the case properly when it applies the workaround for missing "
				+ "constructor parameter types.");
		}
		
		public const instanceConstant:int = 0;
		public static const staticConstant:int = 0;
		
		public var instanceField:int = 0;		
		public static var staticField:int = 0;

		public function simpleInstanceMethod():void { }
		public function complexInstanceMethod(a:int, b:*, c:String = null):Object { return null; }		
		public static function simpleStaticMethod():void { }
		public static function complexStaticMethod(a:int, b:*, c:String = null):Object { return null; }
		
		public function get readOnlyInstanceProperty():int { return 0; }		
		public function get readWriteInstanceProperty():int { return 0; }
		public function set readWriteInstanceProperty(value:int):void { }
		public function set writeOnlyInstanceProperty(value:int):void { }
		public static function get readOnlyStaticProperty():int { return 0; }		
		public static function get readWriteStaticProperty():int { return 0; }
		public static function set readWriteStaticProperty(value:int):void { }
		public static function set writeOnlyStaticProperty(value:int):void { }
	}
}