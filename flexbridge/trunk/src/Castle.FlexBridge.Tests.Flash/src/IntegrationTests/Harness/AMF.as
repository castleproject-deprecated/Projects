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

package IntegrationTests.Harness
{
	import flash.utils.ByteArray;
	import flash.net.ObjectEncoding;
	
	/**
	 * Encodes objects using AMF.
	 */
	public class AMF
	{
		private static const AMF3_TAG:int = 0x11;
		
		public static function encode(object:Object):ByteArray
		{
			var byteArray:ByteArray = new ByteArray();
			byteArray.writeByte(AMF3_TAG); // switch from AMF0 to AMF3
			byteArray.objectEncoding = ObjectEncoding.AMF3;
			byteArray.writeObject(object);
			byteArray.position = 0;
			
			return byteArray;
		}
		
		public static function decode(byteArray:ByteArray):Object
		{
			if (byteArray.readByte() == AMF3_TAG)
			{
				byteArray.objectEncoding = ObjectEncoding.AMF3;
			}
			else
			{
				byteArray.position -= 1;
				byteArray.objectEncoding = ObjectEncoding.AMF0;
			}
			
			return byteArray.readObject();
		}
	}
}