namespace SolutionTransform
{
    public interface IRename
    {
        string RenameCsproj(string csproj);
        string RenameSln(string solutionPath);
        string RenameSolutionProjectName(string name);

        string RenameProjectName(string name);
    }
}
