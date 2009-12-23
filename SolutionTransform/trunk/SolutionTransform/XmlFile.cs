using System.Xml;

namespace SolutionTransform
{
    public class XmlFile
    {
        private readonly string filePath;
        XmlDocument document;

        public XmlFile(string filePath)
        {
            this.filePath = filePath;
        }

        internal XmlDocument Document {
            get {
                if (document == null) {
                    document = new XmlDocument();
                    document.Load(filePath);
                }
                return document;
            }
        }

        public string Path
        {
            get { return filePath; }
        }
    }
}
