using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SolutionTransform.ProjectFile;

namespace SolutionTransform.Solutions
{
    public class ExternalSolutionApi
    {
        private readonly SolutionFile2 solutionFile;

        public ExternalSolutionApi(SolutionFile2 solutionFile)
        {
            this.solutionFile = solutionFile;
        }

        public void Transform(IRename rename, ISolutionCommand solutionCommand) {
            solutionCommand.Process(solutionFile);
            var renameCommand = new TransformCommand(new DontFilter(), new NameTransform(rename));
            renameCommand.Process(solutionFile);

            foreach (var project in solutionFile.Projects.Where(p => !p.IsFolder)) {
                project.Name = rename.RenameSolutionProjectName(project.Name);
                project.Path = rename.RenameCsproj(project.Path);
                project.XmlFile.Document.Save(rename.RenameCsproj(project.XmlFile.Path));
                // Note that project.Path and project.XmlFile.Path have different values....
            }
            solutionFile.Save(rename.RenameSln(solutionFile.FullPath));
        }

        public void Transform(IRename rename, IProjectFilter filter, ITransform transform) {
            Transform(rename, new TransformCommand(filter, transform));
        }

        public void Transform(IRename rename, IProjectFilter filter, params ITransform[] transforms) {
            Transform(rename, filter, new CompositeTransform(transforms));
        }

        public string BasePath
        {
            get
            {
                return Path.GetDirectoryName(solutionFile.FullPath);
            }
        }
    } 

    public class SolutionFile2
    {
        private readonly string solutionPath;
        List<string> preamble;
        List<SolutionChapter> chapters;

        public SolutionFile2(string solutionPath, IEnumerable<string> preamble, IEnumerable<SolutionChapter> chapters)
        {
            this.solutionPath = solutionPath;
            this.preamble = preamble.ToList();
            this.chapters = chapters.ToList();
        }

        public IEnumerable<SolutionProject> Projects
        {
            get { return chapters.OfType<SolutionProject>(); }
        }

        public IEnumerable<string> Lines()
        {
            var result = new List<string>();
            result.AddRange(preamble);
            result.AddRange(chapters.SelectMany(c => c.Lines()));
            return result;
        }

        internal GlobalChapter Globals
        {
            get
            {
                return chapters.OfType<GlobalChapter>().Single();
            }
        }

        public string FullPath
        {
            get { return solutionPath; }
        }

        public void Remove(SolutionProject project)
        {
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
                foreach (var line in Lines())
				{
                    writer.WriteLine(line);
                }
                writer.Flush();
            }
        }
    }
}
