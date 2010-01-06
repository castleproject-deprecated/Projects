using System.Linq;

namespace SolutionTransform.Solutions
{
    public class ExcludeCommand : ISolutionCommand {
        private readonly IProjectFilter filter;

        public ExcludeCommand(IProjectFilter filter) {
            this.filter = filter;
        }

        public void Process(SolutionFile2 solutionFile) {
            foreach (var project in solutionFile.Projects.ToList()) {
                if (!filter.ShouldApply(project)) {
                    solutionFile.Remove(project);
                }
            }
        }
    }
}
