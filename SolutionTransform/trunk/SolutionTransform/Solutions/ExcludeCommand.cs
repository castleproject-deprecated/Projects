using System.Linq;

namespace SolutionTransform.Solutions
{
    public class ExcludeCommand : ISolutionCommand {
        private readonly IProjectFilter filter;

        public ExcludeCommand(IProjectFilter filter) {
            this.filter = filter;
        }

        public void Process(SolutionFile solutionFile) {
            foreach (var project in solutionFile.projects.ToList()) {
                if (!filter.ShouldApply(project)) {
                    solutionFile.Remove(project);
                }
            }
        }
    }
}
