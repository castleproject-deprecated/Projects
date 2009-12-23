using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using SolutionTransform.Solutions;

namespace SolutionTransform.ProjectFile
{
    public class RebaseAssemblies : MSBuild2003Transform
    {
        private readonly IEnumerable<string> absolutePaths;

        public RebaseAssemblies(SolutionFile root, params string[] relativePaths) : this(root.FullPath, (IEnumerable<string>) relativePaths)
        {
            
        }

        public RebaseAssemblies(string solutionPath, IEnumerable<string> relativePaths)
        {
            var solutionDirectory = Path.GetDirectoryName(solutionPath);
            this.absolutePaths = relativePaths.Select(p => p.Contains(":") ? p : Path.Combine(solutionDirectory, p)).ToList();
        }

        public override void DoApplyTransform(XmlFile xmlFile)
        {
            // TODO: Centralize path hacking logic
            foreach (XmlElement hintPath in xmlFile.Document.SelectNodes("//x:HintPath", namespaces))
            {
                var fileName = Path.GetFileName(hintPath.InnerText);
                var directory = absolutePaths.FirstOrDefault(p => File.Exists(Path.Combine(p, fileName)));
                if (directory == null)
                {
                    var error = string.Format("Couldn't rebase {0}.", hintPath.InnerText);
                    var comment = hintPath.OwnerDocument.CreateComment(error);
                    Console.WriteLine(error);
                    hintPath.ParentNode.AppendChild(comment);
                    
                } else
                {
                    var relative = RelativePath(Path.GetDirectoryName(xmlFile.Path), Path.Combine(directory, fileName));
                    hintPath.InnerText = relative;
                }
            }
        }

        static string RelativePath(string from, string to)
        {
            return RelativePath(from, to, 0);
        }

        static string RelativePath(string from, string to, int directoriesUp)
        {
            var match = Regex.Match(to, Regex.Escape(from));
            if (match == Match.Empty)
            {
                return RelativePath(Path.GetDirectoryName(from), to, directoriesUp+1);
            }
            var result = new string[directoriesUp+1];
            for (int index = 0; index < directoriesUp; index++)
            {
                result[index] = "..";
            }
            result[directoriesUp] = to.Substring(match.Length+1);
            return string.Join("\\", result);
        }

        public override void DoApplyTransform(XmlDocument document)
        {
            throw new NotImplementedException();
        }
    }
}