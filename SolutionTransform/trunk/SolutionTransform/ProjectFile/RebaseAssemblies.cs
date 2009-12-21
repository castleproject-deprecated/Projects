using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace SolutionTransform.ProjectFile
{
    public class RebaseAssemblies : MSBuild2003Transform
    {
        private readonly IEnumerable<string> relativePaths;

        public RebaseAssemblies(IEnumerable<string> relativePaths)
        {
            this.relativePaths = relativePaths;
        }

        public override void DoApplyTransform(string path, XmlDocument document) {
            // TODO: Centralize path hacking logic
            var projectDirectory = Path.GetDirectoryName(path);
            var paths = relativePaths.Select(
                p => new
                         {
                             relative = p,
                             absolute = p.Contains(":") ? p : Path.Combine(projectDirectory, p)
                         }
                ).ToList();
            foreach (XmlElement hintPath in document.SelectNodes("//x:HintPath", namespaces))
            {
                var fileName = Path.GetFileName(hintPath.InnerText);
                var directory = paths.FirstOrDefault(p => File.Exists(Path.Combine(p.absolute, fileName)));
                if (directory == null)
                {
                    hintPath.InnerText = Path.Combine(directory.relative, fileName);
                } else
                {
                    var comment = hintPath.OwnerDocument.CreateComment(
                        string.Format("Couldn't rebase {0}.", hintPath.InnerText));
                    hintPath.ParentNode.AppendChild(comment);
                }
            }
        }

        public override void DoApplyTransform(XmlDocument document)
        {
            throw new NotImplementedException();
        }
    }
}