namespace Castle.MonoRail.Rest.Binding
{
    using System;
    using System.Xml;
    using Castle.Components.Binder;

    public class XmlTreeBuilder
    {
        public CompositeNode BuildNode(XDocument doc)
        {
            CompositeNode rootNode = new CompositeNode("root");
            rootNode.AddChildNode(ProcessElement(doc.Root));
            return rootNode;
        }

        public void AddToRoot(CompositeNode rootNode, XDocument doc)
        {
            Node top = ProcessElement(doc.Root);
            rootNode.AddChildNode(top);
        }

        public Node ProcessElement(XElement startEl)
        {
            if (IsComplex(startEl))
            {
                CompositeNode top = new CompositeNode(startEl.Name.LocalName);
                foreach(var attr in startEl.Attributes())
                {
                    LeafNode leaf = new LeafNode(typeof(String), attr.Name.LocalName, attr.Value);
                    top.AddChildNode(leaf);
                }
                foreach(var childEl in startEl.Elements())
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

        public bool IsComplex(XElement element)
        {
            if (element.HasElements || element.Attributes().Count() > 0)
            {
                if (element.Elements().Count() == 1 && element.FirstNode.NodeType == XmlNodeType.Text)
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