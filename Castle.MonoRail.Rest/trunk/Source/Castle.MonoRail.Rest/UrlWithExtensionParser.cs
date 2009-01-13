// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Rest
{
	using System;
	using System.Collections.Generic;

	public class UrlWithExtensionParser
	{
		public string[] GetParts(string url)
		{
			var urlSplits = new List<string>(url.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries));
			var last = urlSplits[urlSplits.Count - 1];
			var lastSplits = last.Split('.');
			var extension = lastSplits.Length > 1 ? lastSplits[lastSplits.Length - 1] : null;
			last = lastSplits.Length == 1 ? last : string.Join(".", lastSplits, 0, lastSplits.Length - 1);

			urlSplits.RemoveAt(urlSplits.Count - 1);
			urlSplits.Add(last);

			if (extension != null)
				urlSplits.Add(extension);

			return urlSplits.ToArray();
		}
	}
}