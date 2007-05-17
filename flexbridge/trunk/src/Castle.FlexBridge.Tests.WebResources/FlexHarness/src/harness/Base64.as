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

package harness
{
	import flash.utils.ByteArray;
	import flash.errors.EOFError;
	
	/**
	 * Performs Base64 encoding and decoding of ByteArrays.
	 */
	public class Base64
	{
		private static const base64PaddingByte:int = '='.charCodeAt(0) as int;
		
		internal static const base64Bytes:Array = new Array();
		internal static const base64Values:Array = new Array();
		
		initializeLookupTables();
		
		public static function encode(byteArray:ByteArray):String
		{
			var resultBuffer:ByteArray = new ByteArray();
			
			for (;;)
			{
				var b1:int = readNextByte(byteArray);
				if (b1 < 0)
					break;
					
				resultBuffer.writeByte(base64Bytes[b1 >> 2]);
				
				var b2:int = readNextByte(byteArray);
				if (b2 < 0)
				{
					resultBuffer.writeByte(base64Bytes[(b1 << 4) & 0x30]);
					resultBuffer.writeByte(base64PaddingByte);
					resultBuffer.writeByte(base64PaddingByte);
					break;
				}

				resultBuffer.writeByte(base64Bytes[(b1 << 4) & 0x30 | (b2 >> 4)]);
				
				var b3:int = readNextByte(byteArray);
				if (b3 < 0)
				{
					resultBuffer.writeByte(base64Bytes[(b2 << 2) & 0x3c]);
					resultBuffer.writeByte(base64PaddingByte);
					break;
				}
				
				resultBuffer.writeByte(base64Bytes[(b2 << 2) & 0x3c | (b3 >> 6)]);
				resultBuffer.writeByte(base64Bytes[b3 & 0x3f]);
			}
			
			return resultBuffer.toString();
		}
		
		public static function decode(string:String):ByteArray
		{
			var stringBuffer:StringBuffer = new StringBuffer(string, base64Values);
			var resultBuffer:ByteArray = new ByteArray();
			
			for (;;)
			{
				var v1:int = stringBuffer.nextBase64Value();
				if (v1 < 0)
					break;
					
				var v2:int = stringBuffer.nextBase64Value();
				if (v2 < 0)
					break;
					
				resultBuffer.writeByte((v1 << 2) | (v2 >> 4));
				
				var v3:int = stringBuffer.nextBase64Value();
				if (v3 < 0)
					break;

				resultBuffer.writeByte((v2 << 4) | (v3 >> 2));
				
				var v4:int = stringBuffer.nextBase64Value();
				if (v4 < 0)
					break;

				resultBuffer.writeByte((v3 << 6) | v4);					
			}
			
			resultBuffer.position = 0;
			return resultBuffer;
		}
		
		private static function readNextByte(byteArray:ByteArray):int
		{
			try
			{
				return byteArray.readByte();
			}
			catch (ex:EOFError)
			{
			}
			return -1;
		}
		
		private static function initializeLookupTables():void
		{
			var chars:Array = [
				'A','B','C','D','E','F','G','H',
				'I','J','K','L','M','N','O','P',
				'Q','R','S','T','U','V','W','X',
				'Y','Z','a','b','c','d','e','f',
				'g','h','i','j','k','l','m','n',
				'o','p','q','r','s','t','u','v',
				'w','x','y','z','0','1','2','3',
				'4','5','6','7','8','9','+','/'
			];

			for (var i:int = 0; i < chars.length; i++)
			{
				var char:String = chars[i];
				var charCode:int = int(char.charCodeAt(0));
				
				base64Bytes[i] = charCode;
				base64Values[charCode] = i;
			}
		}		
	}
}

import harness.Base64;

internal class StringBuffer
{
	private var base64Values:Array;
	private var string:String;
	private var index:int;
	
	public function StringBuffer(string:String, base64Values:Array)
	{
		this.string = string;
		this.base64Values = base64Values;
		this.index = 0;
	}
	
	public function nextBase64Value():int
	{
		while (index < string.length)
		{
			var char:int = int(string.charCodeAt(index++));
			var value:* = base64Values[char];
			if (value != undefined)
				return int(value);
		}
		
		return -1;
	}
}
