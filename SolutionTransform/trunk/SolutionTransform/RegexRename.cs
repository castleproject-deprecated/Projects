using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SolutionTransform {
    public class RegexRename : IRename {
        private readonly Regex pattern;
        private readonly string replacement;

        public RegexRename(string pattern, string replacement)
            : this (new Regex(pattern, RegexOptions.IgnoreCase), replacement)
        {
            
        }
        public RegexRename(Regex pattern, string replacement)
        {
            this.pattern = pattern;
            this.replacement = replacement;
        }


        public string RenameCsproj(string csproj)
        {
            return pattern.Replace(csproj, replacement);
        }

        public string RenameSln(string solutionPath)
        {
            return RenameCsproj(solutionPath);
        }

        public string RenameSolutionProjectName(string name) {
            return RenameCsproj(name);
        }

        public string RenameProjectName(string name)
        {
            return name + replacement;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SolutionTransform {
    public class RegexRename : IRename {
        private readonly Regex pattern;
        private readonly string replacement;

        public RegexRename(string pattern, string replacement)
            : this (new Regex(pattern, RegexOptions.IgnoreCase), replacement)
        {
            
        }
        public RegexRename(Regex pattern, string replacement)
        {
            this.pattern = pattern;
            this.replacement = replacement;
        }


        public string RenameCsproj(string csproj)
        {
            return pattern.Replace(csproj, replacement);
        }

        public string RenameSln(string solutionPath)
        {
            return RenameCsproj(solutionPath);
        }

        public string RenameSolutionProjectName(string name) {
            return RenameCsproj(name);
        }

        public string RenameProjectName(string name)
        {
            return name + replacement;
        }
    }
}
