using System;
using System.Xml;

namespace SolutionTransform.ProjectFile
{
    public class ChangeOutputPaths : MSBuild2003Transform
    {
        private readonly FilePath path;

        public ChangeOutputPaths(string path) : this(new FilePath(path, true, false))
        {
            
        }

        public ChangeOutputPaths(FilePath path)
        {
            this.path = path;
        }

        public override void DoApplyTransform(XmlDocument document)
        {
            foreach (XmlElement outputPath in document.SelectNodes("/*/x:PropertyGroup/x:OutputPath", namespaces))
            {
                var condition = ((XmlElement) outputPath.ParentNode).GetAttribute("Condition");
                var config = ExtractConfiguration(condition);
                var newOutputPath = path.Directory(config).Path;
                outputPath.InnerText = newOutputPath;
            }
        }

        internal string ExtractConfiguration(string condition)
        {
            var pipeIndex = condition.LastIndexOf('|');
            if (pipeIndex < 0)
            {
                throw new Exception(string.Format("Couldn't find a pipe symbol in {0}", condition));
            }
            var prePipe = condition.Substring(0, pipeIndex);
            var quoteIndex = prePipe.LastIndexOf('\'');
            if (quoteIndex < 0) {
                throw new Exception(string.Format("Couldn't find a quote symbol before a pipe in {0}", condition));
            }
            return prePipe.Substring(quoteIndex + 1);
        }
    }
}
