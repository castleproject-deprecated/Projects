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

package castle.flexbridge.tests.common
{
	import flexunit.framework.TestCase;
	import flexunit.framework.Assert;
	import castle.flexbridge.common.*;
	import flash.utils.ByteArray;
	
	public class Base64Test extends TestCase
	{
		public function testEncodeEmptyArray():void
		{
			var bytes:ByteArray = new ByteArray();
			var base64:String = Base64.encode(bytes);
			
			Assert.assertEquals("", base64);
		}

		public function testEncodeOneByteArray():void
		{
			var bytes:ByteArray = new ByteArray();
			bytes.writeByte(42);
			
			var base64:String = Base64.encode(bytes);
			
			Assert.assertEquals("Kg==", base64);
		}

		public function testEncodeTwoByteArray():void
		{
			var bytes:ByteArray = new ByteArray();
			bytes.writeByte(42);
			bytes.writeByte(0);
			
			var base64:String = Base64.encode(bytes);
			
			Assert.assertEquals("KgA=", base64);
		}
		

		public function testEncodeThreeByteArray():void
		{
			var bytes:ByteArray = new ByteArray();
			bytes.writeByte(42);
			bytes.writeByte(0);
			bytes.writeByte(255);
			
			var base64:String = Base64.encode(bytes);
			
			Assert.assertEquals("KgD/", base64);
		}

		public function testEncodeFourByteArray():void
		{
			var bytes:ByteArray = new ByteArray();
			bytes.writeByte(42);
			bytes.writeByte(0);
			bytes.writeByte(255);
			bytes.writeByte(117);
			
			var base64:String = Base64.encode(bytes);
			
			Assert.assertEquals("KgD/dQ==", base64);
		}
		
		public function testDecodeEmptyArray():void
		{
			var base64:String = "";
			var bytes:ByteArray = Base64.decode(base64);
			
			assertByteArrayEquals([ ], bytes);
		}

		public function testDecodeOneByteArray():void
		{
			var base64:String = "Kg==";
			var bytes:ByteArray = Base64.decode(base64);
			
			assertByteArrayEquals([ 42 ], bytes);
		}

		public function testDecodeTwoByteArray():void
		{
			var base64:String = "KgA=";
			var bytes:ByteArray = Base64.decode(base64);
			
			assertByteArrayEquals([ 42, 0 ], bytes);
		}

		public function testDecodeThreeByteArray():void
		{
			var base64:String = "KgD/";
			var bytes:ByteArray = Base64.decode(base64);
			
			assertByteArrayEquals([ 42, 0, 255 ], bytes);
		}

		public function testDecodeFourByteArray():void
		{
			var base64:String = "KgD/dQ==";
			var bytes:ByteArray = Base64.decode(base64);
			
			assertByteArrayEquals([ 42, 0, 255, 117 ], bytes);
		}
		
		private function assertByteArrayEquals(expectedValue:Array, actualValue:ByteArray):void
		{
			Assert.assertEquals(expectedValue.length, actualValue.length);
			
			actualValue.position = 0;
			for (var i:int = 0; i < expectedValue.length; i++)
				Assert.assertEquals(expectedValue[i], actualValue.readByte() & 0xff);
		}
	}
}