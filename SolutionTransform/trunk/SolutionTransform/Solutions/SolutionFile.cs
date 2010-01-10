using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SolutionTransform.Solutions {
    public class SolutionFile {
        private readonly FilePath solutionPath;
        List<string> preamble;
        List<SolutionChapter> chapters;

        public SolutionFile(FilePath solutionPath, IEnumerable<string> preamble, IEnumerable<SolutionChapter> chapters) {
            this.solutionPath = solutionPath;
            this.preamble = preamble.ToList();
            this.chapters = chapters.ToList();
        }

        public IEnumerable<SolutionProject> Projects {
            get { return chapters.OfType<SolutionProject>(); }
        }

        public IEnumerable<string> Lines() {
            var result = new List<string>();
            result.AddRange(preamble);
            result.AddRange(chapters.SelectMany(c => c.Lines()));
            return result;
        }

        internal GlobalChapter Globals {
            get {
                return chapters.OfType<GlobalChapter>().Single();
            }
        }

        public FilePath FullPath {
            get { return solutionPath; }
        }

        public void Remove(SolutionProject project) {
            chapters.Remove(project);
            Globals.ProjectConfigurationPlatforms.Remove(project);
        }

        public void Add(SolutionProject project) {
            chapters.Add(project);
            Globals.ProjectConfigurationPlatforms.Add(project);
        }

        internal void Save(string destination) {
            // NB Solution files will not load unless you save them as Unicode.  In particular, UTF8 doesn't work.
            using (var writer = new StreamWriter(destination, false, Encoding.Unicode)) {
                writer.WriteLine();
                foreach (var line in Lines()) {
                    writer.WriteLine(line);
                }
                writer.Flush();
            }
        }
    }
}
