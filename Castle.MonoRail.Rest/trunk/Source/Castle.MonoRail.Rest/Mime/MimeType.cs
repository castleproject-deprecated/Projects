// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Rest.Mime
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	public class MimeType
	{
		
		public string MimeString { get; set; }
		public string Symbol {get; set;}
		public List<String> Synonyms { get; set; }
		public List<String> ExtensionSynonyms { get; set; }

		
	}

	public class MimeTypes : System.Collections.Generic.List<MimeType>
	{
		

		public MimeTypes(): base(15)
		{
		
			
		}

		public void RegisterBuiltinTypes()
		{
			//copied from rails mime_types

			Register("*/*", "all");
			Register("text/plain", "text", null, new[] { "txt" });
			Register("text/html", "html", new[] { "application/xhtml+xml" }, new[] { "xhtml" });
			Register("text/javascript", "js", new[] { "application/javascript", "application/x-javascript" });
			Register("text/css", "css");
			Register("text/calendar", "ics");
			Register("text/csv", "csv");
			Register("application/xml", "xml", new[] { "text/xml application/x-xml" });
			Register("application/rss+xml", "rss");
			Register("application/atom+xml", "atom");
			Register("application/x-yaml", "yaml", new[] { "text/yaml" });
			Register("multipart/form-data", "multipart_form");
			Register("application/x-www-form-urlencoded", "url_encoded_form");

			//http://www.ietf.org/rfc/rfc4627.txt
			Register("application/json", "json", new[] { "text/x-json" });         

		}

		public void Register(string mimeString, string symbol)
		{
			Register(mimeString, symbol, null, null);
		}

		public void Register(string mimeString, string symbol, IEnumerable<string> synonyms)
		{
			Register(mimeString, symbol, synonyms,null);
		}

		public void Register(string mimeString, string symbol, IEnumerable<string> synonyms, IEnumerable<string> extensionSynonyms)
		{
			List<string> synList = new List<string>(), extentionList = new List<string>();

			if (synonyms != null) synList.AddRange(synonyms);
			if (extensionSynonyms != null) extentionList.AddRange(extensionSynonyms);

			this.Add(
				new MimeType() { 
					MimeString = mimeString, 
					Symbol = symbol, 
					Synonyms = synList, 
					ExtensionSynonyms = extentionList }
				);                           
		}
	}
}
