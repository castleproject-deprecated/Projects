package Castle.FlexBridge.Flash.Common
{
	/**
	 * Constructs Uris based on properties.
	 */
	public class UriBuilder
	{
		/**
		 * Creates a Uri builder and initializes it with the contents of the
		 * specified Uri, or if null, with scheme 'file', port 0 (none),
		 * an empty hostname (none) and and empty path (none).
		 */
		public function UriBuilder(uri:Uri = null)
		{
			scheme = uri.scheme;
			host = uri.host;
			port = uri.port;
			path = uri.path;
		}
		
		/**
		 * The Uri scheme.
		 */
		public var scheme:String = "file";
		
		/**
		 * The hostname or "" if none.
		 */
		public var host:String = "";
		
		/**
		 * The port number or 0 if none.
		 * Ignored unless the hostname is non-empty.
		 */
		public var port:uint = 0;
		
		/**
		 * The path or "" if none.
		 */
		public var path:String = "";
		
		/**
		 * Combines the path of the builder with the specified path segment.
		 * If the path begins with "/" it is considered absolute and becomes the
		 * full path of the UriBuilder, otherwise it is considered relative and is
		 * appended to the path of this Uri to form the path of the resulting Uri
		 * along with an intervening "/" if the current path lacks one.
		 * 
		 * @param pathSegment The relative or absolute path segment to combine.
		 */
		public function combinePath(pathSegment:String):void
		{
			if (Uri.isAbsolutePath(pathSegment))
			{
				path = pathSegment;
			}
			else
			{
				if (! Uri.isFolderPath(path))
					path += "/";
					
				path += pathSegment;
			}
		}		
		
		/**
		 * Converts the specification to a Uri.
		 */
		public function toUri():Uri
		{
			var uriString:String;
			
			uriString = scheme + ":";
			
			if (host.length != 0)
			{
				uriString += "//" + host;
				
				if (port != 0)
					uriString += ":" + port.toString();
					
				if (path.length != 0 && path.charAt(0) != "/")
					uriString += "/";
			}
			
			uriString += path;
			
			return new Uri(uriString);
		}
	}
}