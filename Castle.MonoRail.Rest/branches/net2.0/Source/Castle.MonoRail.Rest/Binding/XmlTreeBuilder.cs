namespace Castle.MonoRail.Rest.Binding
{
    using System;
    using System.Xml;
    using Components.Binder;

    public class XmlTreeBuilder
    {
        public CompositeNode BuildNode(XmlDocument doc)
        {
            CompositeNode rootNode = new CompositeNode("root");
			rootNode.AddChildNode(ProcessElement(doc.DocumentElement));
            return rootNode;
        }

        public void AddToRoot(CompositeNode rootNode, XmlDocument doc)
        {
            Node top = ProcessElement(doc.DocumentElement);
            rootNode.AddChildNode(top);
        }

        public Node ProcessElement(XmlElement startEl)
        {
            if (IsComplex(startEl))
            {
                CompositeNode top = new CompositeNode(startEl.LocalName);
                foreach(XmlAttribute attr in startEl.Attributes)
                {
                    LeafNode leaf = new LeafNode(typeof(String), attr.LocalName, attr.Value);
                    top.AddChildNode(leaf);
                }
                foreach(XmlElement childEl in startEl.ChildNodes)
                {
                    Node childNode = ProcessElement(childEl);
                    top.AddChildNode(childNode);
                }

                return top;
            }
            else
            {
                LeafNode top = new LeafNode(typeof(String), "", "");
                return top;
            }
        }

        public bool IsComplex(XmlElement element)
        {
            if (element.HasChildNodes || element.Attributes.Count > 0)
            {
                if (element.ChildNodes.Count == 1 && element.ChildNodes[0].NodeType == XmlNodeType.Text)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }
}