using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SolutionTransform
{
    [DebuggerDisplay("{path}")]
    public class FilePath
    {
        private readonly string path;
        private readonly bool isDirectory;
        private readonly bool isAbsolute;

        public FilePath(string path, bool isDirectory) : this(path, isDirectory, path.Contains(":") || path.StartsWith("\\"))
        {
            
        }
        public FilePath(string path, bool isDirectory, bool isAbsolute)
        {
            this.path = path;
            this.isDirectory = isDirectory;
            this.isAbsolute = isAbsolute;
            if (path.EndsWith(".sln") && isDirectory)
            {
                int x = 0;
            }
        }


        public bool IsDirectory
        {
            get { return isDirectory; }
        }

        public string Path {
            get {
                return path;
            }
        }

        public FilePath File(string fileName)
        {
            if (!isDirectory)
            {
                return Parent.File(fileName);
            }
            return new FilePath(System.IO.Path.Combine(path, fileName), false, isAbsolute);
        }

        public FilePath Directory(string directoryName) {
            if (!isDirectory) {
                return Parent.Directory(directoryName);
            }
            return new FilePath(System.IO.Path.Combine(path, directoryName), true, isAbsolute);
        }

        public FilePath ToAbsolutePath(FilePath from) {
            if (isAbsolute)
            {
                return this;
            }
            if (!from.isDirectory)
            {
                return ToAbsolutePath(from.Parent);
            }
            return new FilePath(System.IO.Path.Combine(from.Path, this.Path), this.IsDirectory, true);
        }

        public FilePath PathFrom(FilePath from) {
            if (!from.isDirectory)
            {
                return PathFrom(from.Parent);
            }
            if (isAbsolute)
            {
                return new FilePath(WorkOutRelativePath(from.Path, this.Path, 0), this.IsDirectory, false);
            }
            return this;
        }

        public FilePath Parent
        {
            get
            {
                var parentPath = System.IO.Path.GetDirectoryName(Path);
                if (parentPath == null)
                {
                    return null;
                }
                return new FilePath(parentPath, true, isAbsolute);
            }
        }

        static string WorkOutRelativePath(string from, string to, int directoriesUp) {
            var match = Regex.Match(to, Regex.Escape(from));
            if (match == Match.Empty) {
                return WorkOutRelativePath(System.IO.Path.GetDirectoryName(from), to, directoriesUp + 1);
            }
            var result = new string[directoriesUp + 1];
            for (int index = 0; index < directoriesUp; index++) {
                result[index] = "..";
            }
            result[directoriesUp] = to.Substring(match.Length + 1);
            return string.Join("\\", result);
        }
    }
}
