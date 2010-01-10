using System.Xml;

namespace SolutionTransform.ProjectFile
{
// ReSharper disable InconsistentNaming
    public class LocalizeGACedReferences : MSBuild2003Transform
// ReSharper restore InconsistentNaming
    {
        public override void DoApplyTransform(XmlDocument document)
        {
            var references = document.SelectNodes("/*/x:ItemGroup/x:Reference", namespaces);
            foreach (XmlElement reference in references)
            {
                if (reference.GetAttribute("Include") == "mscorlib")
                {
                    continue; // We need to process everything except mscorlib
                }
                var copyLocal = GetCopyLocal(reference);
                copyLocal.InnerText = "True";
            }
        }

        private XmlElement GetCopyLocal(XmlNode reference)
        {
            var result = reference.SelectSingleNode("x:Private", namespaces);
            if (result == null)
            {
                return AddElement(reference, "Private");
            }
            return (XmlElement) result;
        }
    }
}
