using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using SolutionTransform.Solutions;

namespace SolutionTransform.ProjectFile
{

    public class RebaseAssemblies : MSBuild2003Transform
    {
        private readonly IEnumerable<FilePath> absolutePaths;

        public RebaseAssemblies(string basePath, params string[] relativePaths)
            : this(basePath, (IEnumerable<string>)relativePaths)
        {
            
        }

        public RebaseAssemblies(string basePath, IEnumerable<string> relativePaths)
        {
            var solutionDirectory = new FilePath(basePath, true);
            this.absolutePaths = relativePaths.Select(p => (new FilePath(p, true)).ToAbsolutePath(solutionDirectory)).ToList();
        }

        public override void DoApplyTransform(XmlFile xmlFile)
        {
            // TODO: Centralize path hacking logic
            foreach (XmlElement hintPath in xmlFile.Document.SelectNodes("//x:HintPath", namespaces))
            {
                var fileName = Path.GetFileName(hintPath.InnerText);
                var directory = absolutePaths.FirstOrDefault(p => File.Exists(p.File(fileName).Path));
                if (directory == null)
                {
                    var error = string.Format("Couldn't rebase {0}.", hintPath.InnerText);
                    var comment = hintPath.OwnerDocument.CreateComment(error);
                    Console.WriteLine(error);
                    hintPath.ParentNode.AppendChild(comment);
                    
                } else
                {
                    var relative = directory.File(fileName).PathFrom(xmlFile.Path).Path;
                    hintPath.InnerText = relative;
                }
            }
        }


        public override void DoApplyTransform(XmlDocument document)
        {
            throw new NotImplementedException();
        }
    }
}