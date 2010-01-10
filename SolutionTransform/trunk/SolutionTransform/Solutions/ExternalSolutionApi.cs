using System.Linq;
using SolutionTransform.ProjectFile;

namespace SolutionTransform.Solutions
{
    public class ExternalSolutionApi
    {
        private readonly SolutionFile solutionFile;

        public ExternalSolutionApi(SolutionFile solutionFile)
        {
            this.solutionFile = solutionFile;
        }

        public void Transform(IRename rename, ISolutionCommand solutionCommand) {
            solutionCommand.Process(solutionFile);
            var renameCommand = new TransformCommand(new DontFilter(), new NameTransform(rename));
            renameCommand.Process(solutionFile);

            foreach (var project in solutionFile.Projects.Where(p => !p.IsFolder)) {
                project.Name = rename.RenameSolutionProjectName(project.Name);
                project.Path = new FilePath (rename.RenameCsproj(project.Path.Path), false);
                project.XmlFile.Document.Save(rename.RenameCsproj(project.XmlFile.Path.Path));
                // Note that project.Path and project.XmlFile.Path have different values....
            }
            solutionFile.Save(rename.RenameSln(solutionFile.FullPath.Path));
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
                return solutionFile.FullPath.Parent.Path;
            }
        }
    }
}
