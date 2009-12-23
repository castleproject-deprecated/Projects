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

namespace SolutionTransform.Solutions
{
	using System;

	public class SolutionProject {
		internal int lineIndex;
		private readonly SolutionFile file;
		private Guid type;
		private string name;
        private string path;
        private XmlFile xmlFile;
		private Guid id;

		public SolutionProject(string line, int lineIndex, SolutionFile file) {
			this.lineIndex = lineIndex;
			this.file = file;
			var components = line.Split('"');
			type = new Guid(components[1]);
			name = components[3];
            path = components[5];
            xmlFile = new XmlFile(System.IO.Path.Combine(file.BasePath, path));
			id = new Guid(components[7]);
		}

	    public XmlFile XmlFile
	    {
	        get { return xmlFile; }
	    }

	    public bool IsFolder { get
		{
				return type == FolderProject;
		}}

		public readonly static Guid FolderProject = new Guid("{2150E333-8FDC-42A3-9474-1A3956D46DE8}");

		public Guid Id {
			get { return id; }
		}

		public string Path {
			get { return path; }
			set { path = value; WriteLineBack(); }
		}

		public string Name {
			get { return name; }
			set { name = value; WriteLineBack(); }
		}

		public Guid Type {
			get { return type; }
		}

		void WriteLineBack() {
			var line = string.Format(@"Project(""{{{0}}}"") = ""{1}"", ""{2}"", ""{{{3}}}""",
									 Type, Name, Path, Id
				);
			file.lines[lineIndex] = line;
		}

        

	    
	}
}
