// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace SolutionTransform.CodingStandards
{
	using System;
	using System.IO;
	using System.Text.RegularExpressions;
	using System.Xml;

	public class StandardizeTransform : MSBuild2003Transform
	{
		private readonly IStandardizer standardizer;

		static readonly Regex CSharp = new Regex("[.]cs$", RegexOptions.IgnoreCase);
		static readonly Regex AssemblyInfo = new Regex("AssemblyInfo[.]cs$", RegexOptions.IgnoreCase);

		public StandardizeTransform(IStandardizer standardizer)
		{
			this.standardizer = standardizer;
		}

		public override void DoApplyTransform(string path, XmlDocument document)
		{
			string root = Path.GetDirectoryName(path);
			foreach (XmlElement compile in document.SelectNodes("//x:Compile[@Include]", namespaces))
			{
				string include = compile.GetAttribute("Include");
				if (CSharp.IsMatch(include) && !AssemblyInfo.IsMatch(include))
				{
					var filePath = Path.Combine(root, include);
					if (File.Exists(filePath))
					{
						var content = filePath.FileContent();
						var transformed = standardizer.Standardize(filePath, content);
						if (transformed == null || content == transformed)
						{
							continue;
						}
						using (var writer = new StreamWriter(filePath, false))
						{
							writer.Write(transformed);
							writer.Flush();
						}
					}
				}
			}
		}

		public override void DoApplyTransform(XmlDocument document)
		{
			throw new NotImplementedException();
		}
	}
}
