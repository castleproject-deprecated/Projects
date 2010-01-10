using System.Xml;

namespace SolutionTransform
{
    public class XmlFile
    {
        private readonly FilePath filePath;
        XmlDocument document;

        public XmlFile(FilePath filePath)
        {
            this.filePath = filePath;
        }

        internal XmlFile(XmlDocument document)
        {
            this.document = document;
        }

        internal XmlDocument Document {
            get {
                if (document == null) {
                    document = new XmlDocument();
                    document.Load(filePath.Path);
                }
                return document;
            }
        }

        public FilePath Path
        {
            get { return filePath; }
        }
    }
}
