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

using System.Text;

namespace SolutionTransform
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Text.RegularExpressions;
	using System.Xml;
    using SolutionTransform.ProjectFile;

	public class SolutionFile {
		private readonly string path;
		internal List<string> lines = new List<string>();
		internal List<SolutionProject> projects;

		public SolutionFile(string path, string content) {
			this.path = path;
			lines.AddRange(content.AsLines());
			// .SkipWhile(l => l.TrimEnd().Length == 0)
			// The whole skipwhile thing is to deal with the blank lines at the start of the Castle solution file
			projects = GetProjects().ToList();
		}

		

		IEnumerable<SolutionProject> GetProjects() {
			int index = 0;
			foreach (var line in lines)
			{
				if (line.StartsWith("Project"))
				{
					yield return new SolutionProject(line, index, this);
				}
				index++;
			}
		}

		public void Remove(SolutionProject solutionProject) {
			lines.RemoveRange(solutionProject.lineIndex, 2);  // Take out the end project as well
			foreach (var project in projects.Where(p => p.lineIndex > solutionProject.lineIndex)) {
				project.lineIndex -= 2;
			}
			var regex = new Regex(solutionProject.Id.ToString(), RegexOptions.IgnoreCase);
			lines.RemoveAll(regex.IsMatch);
			projects.Remove(solutionProject);
		}

		public void ProcessProjects(IRename rename, ITransform transform)
		{
			var nameTransform = new NameTransform(rename);
			var fullTransform = new CompositeTransform(transform, nameTransform);
			string basePath = Path.GetDirectoryName(path);
			foreach (var project in projects.Where(p => !p.IsFolder))
			{
				var projectPath = Path.Combine(basePath, project.Path);
				var document = new XmlDocument();
				document.Load(projectPath);
				fullTransform.ApplyTransform(projectPath, document);
				project.Name = rename.RenameSolutionProjectName(project.Name);
				project.Path = rename.RenameCsproj(project.Path);
                document.Save(rename.RenameCsproj(projectPath));
				// Note that projectPath and project.Path have different values....
			}
			Save(rename.RenameSln(path));
		}

        public void Transform(IRename rename, IProjectFilter filter, ITransform transform)
		{
			FilterProjects(filter);
			ProcessProjects(rename, transform);
		}

        public void Transform(IRename rename, IProjectFilter filter, params ITransform[] transforms) {
			Transform(rename, filter, new CompositeTransform(transforms));
		}

		private void Save(string destination)
		{
            // NB Solution files will not load unless you save them as Unicode.  In particular, UTF8 doesn't work.
			using (var writer = new StreamWriter(destination, false, Encoding.Unicode))
			{
				foreach (var line in lines)
				{
					writer.WriteLine(line);
				}
				writer.Flush();
			}
		}

        public void FilterProjects(IProjectFilter filter)
		{
			foreach (var project in projects.ToList())
			{
				if (!filter.ShouldIncludeProject(project))
				{
					Remove(project);
				}
			}
		}
	}
}
