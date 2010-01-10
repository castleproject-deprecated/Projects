using System.Collections.Generic;

namespace SolutionTransform.Solutions
{
    public class CompositeCommand : ISolutionCommand
    {
        private readonly IEnumerable<ISolutionCommand> underlying;

        public CompositeCommand(IEnumerable<ISolutionCommand> underlying)
        {
            this.underlying = underlying;
        }

        public void Process(SolutionFile solutionFile)
        {
            foreach (var command in underlying)
            {
                command.Process(solutionFile);
            }
        }
    }
}
