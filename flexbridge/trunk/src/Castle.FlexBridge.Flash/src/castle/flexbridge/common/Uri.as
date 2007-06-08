package castle.flexbridge.common
{
	import mx.utils.URLUtil;
	
	/**
	 * A typed wrapper for a Uri string with additional helpers
	 * for manipulating its contents.
	 */
	public class Uri
	{
		private var _uri:String;
		private var _isUnrooted:Boolean;
		
		private static const UriRegExp:RegExp = /(?P<scheme>\w+):(?:(?:\/\/(?P<host>[^:\/]+)(?::(?P<port>\d+))?(?P<path1>\/[^:]*)?)|(?P<path2>\/?[^:]*))/;
		
		/**
		 * Constructs a Uri from its string representation.
		 * 
		 * @param uri The Uri
		 */
		public function Uri(uri:String)
		{
			_uri = uri;			
			_isUnrooted = URLUtil.getProtocol(uri) == "";
		}
		
		/**
		 * Gets the scheme part of the Uri.
		 */
		public function get scheme() : String
		{
			return _isUnrooted ? "" : URLUtil.getProtocol(_uri);
		}
		
		/**
		 * Gets the host part of the Uri or "" if none.
		 */
		public function get host() : String
		{
			return _isUnrooted ? "" : URLUtil.getServerName(_uri);
		}
		
		/**
		 * Gets the port part of the Uri or 0 if none.
		 */
		public function get port() : uint
		{
			return _isUnrooted ? 0 : URLUtil.getPort(_uri);
		}
		
		/**
		 * Gets the path part of the Uri or "" if none.
		 */
		public function get path() : String
		{
			if (_isUnrooted)
				return _uri;
			
			var result:Object = UriRegExp.exec(_uri);
			if (result)
			{
				if (result.path1)
					return result.path1;
				if (result.path2)
					return result.path2;
			}
				
			return "";
		}
		
		/**
		 * Gets the path segments of the Uri including trailing slashes.
		 * The uri "http://localhost/Folder/File" has the following segments:
		 *    - "/"
		 *    - "Folder/"
		 *    - "File"
		 */
		[ArrayElementType("String")]
		public function get segments() : Array /*of String*/
		{
			var segments:Array = new Array();
			
			var _path:String = path;
			var start:int = 0;
			
			for (;;)
			{
				if (start == _path.length)
					break;
				
				var pos:int = path.indexOf("/", start);
				
				if (pos < 0)
				{
					segments.push(_path.substring(start));
					break;
				}
				else
				{
					segments.push(_path.substring(start, pos + 1));
					start = pos + 1;
				}
			}
			
			return segments;
		}
		
		/**
		 * Gets an array of path segments without '/' delimiters and
		 * excluding any blank segment.
		 */
		public function get trimmedSegments() : Array /*of String*/
		{
			return path.split("/").filter(function(segment:String, index:int, array:Array):Boolean
			{
				return segment.length != 0;
			});
		}
		
		/**
		 * Returns a new Uri whose path has been combined with the specified
		 * path.  If the path begins with "/" it is considered absolute and
		 * becomes the full path of the resulting Uri, otherwise it is considered
		 * relative and is appended to the path of this Uri to form the path of
		 * the resulting Uri.
		 * 
		 * @param pathSegment The relative or absolute path segment to combine.
		 * @return The new Uri with the combined path.
		 */
		public function combinePath(pathSegment:String):Uri
		{
			var uriBuilder:UriBuilder = new UriBuilder(this);
			uriBuilder.combinePath(pathSegment);
			
			return uriBuilder.toUri();
		}
		
		/**
		 * Returns the Uri as a string.
		 */
		public function toString():String
		{
			return _uri;
		}
		
		/**
		 * Returns true if the Uri lacks a scheme and therefore can only
		 * be interpreted relative to some other rooted Uri.
		 */
		public function isUnrooted():Boolean
		{
			return _isUnrooted;
		}
		
		/**
		 * Returns true if the path begins with a "/".
		 */
		public static function isAbsolutePath(path:String):Boolean
		{
			return path && path.length > 0 && path.charAt(0) == "/";
		}
		
		/**
		 * Returns true if the path ends with a "/".
		 */
		public static function isFolderPath(path:String):Boolean
		{
			return path && path.length > 0 && path.charAt(path.length - 1) == "/";			
		}
	}
}