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

package castle.flexbridge.common
{
	/**
	 * Provides utilities for working with HTML.
	 */
	public final class HtmlUtils
	{
		/**
		 * Encodes the specified string as HTML.
		 * Converts sensitive characters to HTML entities as appropriate.
		 */
		public static function htmlEncode(s:String):String
		{
			// TODO: Optimize.
			s = s.replace(/&/g, "&amp;");
			s = s.replace(/</g, "&lt;");
			s = s.replace(/>/g, "&gt;");
			s = s.replace(/\"/g, "&quot;");
			
			return s;
		}
		
		/**
		 * Decodes the specified string from HTML.
		 * Converts HTML entities to their corresponding simple characters.
		 */
		public static function htmlDecode(s:String):String
		{
			// TODO: Optimize and support more entities as needed.
			s = s.replace(/&amp;/gi, "&");
			s = s.replace(/&lt;/gi, "<");
			s = s.replace(/&gt;/gi, ">");
			s = s.replace(/&quot;/gi, "\"");
			
			return s;
		}
		
		/**
		 * Formats plain text for display.
		 * This does a bit more than htmlEncode in that it translates control
		 * characters and may modify the content to make it more amenable to
		 * presentation.
		 */
		public static function htmlFormat(s:String):String
		{
			s = htmlEncode(s);
			s = s.replace(/\r/g, "");
			s = s.replace(/\n{3,}/g, "\n\n"); // allow no more than 2 consecutive breaks
			s = s.replace(/\n/g, "<br/>");
			s = s.replace(/\t/g, " &nbsp;&nbsp; ");	
			s = s.replace(/[\u0000-\u001f]+/g, " ");
			
			return s;
		}
	}
}