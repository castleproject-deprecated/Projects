using SolutionTransform.Solutions;

namespace SolutionTransform
{
    public interface IProjectFilter
    {
        bool ShouldApply(SolutionProject project);
    }
}
