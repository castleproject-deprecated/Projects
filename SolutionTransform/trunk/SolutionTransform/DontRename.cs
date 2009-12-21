namespace SolutionTransform
{
    public class DontRename : IRename
    {
        public string RenameCsproj(string csproj)
        {
            return csproj;
        }

        public string RenameSln(string solutionPath)
        {
            return solutionPath;
        }

        public string RenameSolutionProjectName(string name)
        {
            return name;
        }

        public string RenameProjectName(string name)
        {
            return name;
        }
    }
}