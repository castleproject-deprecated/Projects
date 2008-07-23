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

namespace Castle.Tools.CodeGenerator.Services
{
	using System;
	using System.Collections.Generic;
	using ICSharpCode.NRefactory;

	public class DefaultSourceStorageService : IParsedSourceStorageService
	{
		private readonly Dictionary<string, IParser> cache = new Dictionary<string, IParser>(StringComparer.CurrentCultureIgnoreCase);

		public void Add(string path, IParser parser)
		{
			if (cache.ContainsKey(path))
				throw new ArgumentException(String.Format("Source for {0} already cached!", path));
		
			cache[path] = parser;
		}

		public IParser GetParsedSource(string path)
		{
			if (!cache.ContainsKey(path))
				throw new KeyNotFoundException(path);
			
			return cache[path];
		}
	}
}