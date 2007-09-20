using System.IO;
using System.Xml;
using MbUnit.Framework;
using Castle.MonoRail.Rest.Binding;
using Castle.Components.Binder;
namespace Castle.MonoRail.Rest.Tests.Binding
{
    [TestFixture]
    public class XmlTreeBuilderFixture
    {
        private XmlTreeBuilder builder;
        
        [SetUp]
        public void Setup()
        {
            builder = new XmlTreeBuilder();
        }

        [Test]
        public void BuildNode_CreatesRootNode()
        {
			XmlDocument doc = new XmlDocument();
        	doc.Load(new StringReader("<Customer><Name>Chris</Name></Customer>"));
			CompositeNode node = builder.BuildNode(doc);
            
            Assert.IsNotNull(node);
            Assert.AreEqual("root", node.Name);
        }

        [Test]
        public void Element_WithChildElement_IsComplex()
        {
			XmlElement el = CreateXmlElement("<Customer><Name>Chris</Name></Customer>"); 
            
            Assert.IsTrue(builder.IsComplex(el));
            
            
        }

        [Test]
        public void Element_WithMultipleChildElements_IsComplex()
        {
			XmlElement el = CreateXmlElement("<Customer><Name>Chris</Name><Address>USA</Address></Customer>");

            Assert.IsTrue(builder.IsComplex(el));
            
        }

    	[Test]
        public void Element_WithOneAttribute_IsComplex()
        {
			XmlElement el = CreateXmlElement("<Customer Name=\"chris\"/>");
            Assert.IsTrue(builder.IsComplex(el));
        }

        [Test]
        public void Element_WithMultipleAttributes_IsComplex()
        {
			XmlElement el = CreateXmlElement("<Customer Name=\"chris\" Address=\"USA\"/>");

            Assert.IsTrue(builder.IsComplex(el));
        }

        [Test]
        public void Element_ThatOnlyContainsText_IsNotComplex()
        {
			XmlElement el = CreateXmlElement("<Name>Chris</Name>");
            Assert.IsFalse(builder.IsComplex(el));
        }

        [Test]
        public void ProcessElement_CreatesCompositeNode_ForComplexElements()
        {
			XmlElement el = CreateXmlElement("<Customer Name=\"chris\"/>");

            Node node = builder.ProcessElement(el);
            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(typeof(CompositeNode), node);
        }

        [Test]
        public void ComplexNodes_CreatedFromProcessElement_ShouldHaveSameNameAsElement()
        {
			XmlElement el = CreateXmlElement("<Customer Name=\"chris\"/>");

            Node node = builder.ProcessElement(el);
            Assert.AreEqual("Customer", node.Name);
        }

        [Test]
        public void ProcessElement_CreatesLeafNode_ForNonComplexElement()
        {
			XmlElement el = CreateXmlElement("<Name>Chris</Name>");
			Node node = builder.ProcessElement(el);

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(typeof(LeafNode), node);
        }

        [Test]
        public void ProcessElement_CreatesLeafNodesOnCompositeNode_ForElementWithAttributes()
        {
			XmlElement el = CreateXmlElement("<Customer Name=\"chris\"/>");

            Node node = builder.ProcessElement(el);
            Assert.IsInstanceOfType(typeof(CompositeNode), node);

            CompositeNode cnode = (CompositeNode)node;
            Assert.AreEqual(1, cnode.ChildrenCount);

			LeafNode lnode = cnode.ChildNodes[0] as LeafNode;
            Assert.IsNotNull(lnode);
            Assert.AreEqual("Name", lnode.Name);
            Assert.AreEqual("chris", lnode.Value);
        }

        [Test]
        public void ProcessElement_CreatesChildCompositeNodes_ForElementWithComplexChildNodes()
        {
			XmlElement el = CreateXmlElement("<Customer><Name><First>Chris</First></Name></Customer>");

            Node node = builder.ProcessElement(el);
            Assert.IsInstanceOfType(typeof(CompositeNode), node);

			CompositeNode cnode = (CompositeNode)node;
            Assert.AreEqual(1, cnode.ChildrenCount);

            Node childNode = cnode.ChildNodes[0];
            Assert.IsInstanceOfType(typeof(CompositeNode), childNode);
            Assert.AreEqual("Name", childNode.Name);
        }

        [Test]
        public void ProcessElement_CreatesLeafNodesOnComposite_ForElementWithNonComplexChildNodes()
        {
			XmlElement el = CreateXmlElement("<Name><First>Chris</First></Name>");

            Node node = builder.ProcessElement(el);
            Assert.IsInstanceOfType(typeof(CompositeNode), node);

			CompositeNode cnode = (CompositeNode)node;
            Assert.AreEqual(1, cnode.ChildrenCount);

            Assert.IsInstanceOfType(typeof(LeafNode), cnode.ChildNodes[0]);
            

        }

		private XmlElement CreateXmlElement(string xml)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(new StringReader(xml));
			return doc.DocumentElement;
		}

	}
}
