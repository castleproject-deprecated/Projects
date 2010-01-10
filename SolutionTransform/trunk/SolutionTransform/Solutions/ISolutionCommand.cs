namespace SolutionTransform.Solutions
{
    public interface ISolutionCommand
    {
        void Process(SolutionFile solutionFile);
    }
}
