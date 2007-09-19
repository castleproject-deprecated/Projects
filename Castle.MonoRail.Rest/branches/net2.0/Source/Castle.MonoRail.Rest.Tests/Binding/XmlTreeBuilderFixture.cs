using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using Castle.MonoRail.Rest.Binding;
using System.Xml.Linq;
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
            var doc = new XDocument(
                        new XElement("Customer",
                            new XElement("Name", "Chris")));
            var node = builder.BuildNode(doc);
            
            Assert.IsNotNull(node);
            Assert.AreEqual("root", node.Name);
        }

        [Test]
        public void Element_WithChildElement_IsComplex()
        {
            var el = new XElement("Customer",
                            new XElement("Name", "Chris"));

            
            Assert.IsTrue(builder.IsComplex(el));
            
            
        }

        [Test]
        public void Element_WithMultipleChildElements_IsComplex()
        {
            var el = new XElement("Customer",
                            new XElement("Name", "Chris"),
                            new XElement("Address","USA"));


            Assert.IsTrue(builder.IsComplex(el));
            
        }

        [Test]
        public void Element_WithOneAttribute_IsComplex()
        {
            var el = new XElement("Customer",
                        new XAttribute("Name", "chris"));
            Assert.IsTrue(builder.IsComplex(el));
        }

        [Test]
        public void Element_WithMultipleAttributes_IsComplex()
        {
            var el = new XElement("Customer",
                        new XAttribute("Name", "chris"),
                        new XAttribute("Address","USA"));

            Assert.IsTrue(builder.IsComplex(el));
        }

        [Test]
        public void Element_ThatOnlyContainsText_IsNotComplex()
        {
            var el = new XElement("Name", "Chris");            
            Assert.IsFalse(builder.IsComplex(el));
        }

        [Test]
        public void ProcessElement_CreatesCompositeNode_ForComplexElements()
        {
            var el = new XElement("Customer",
                      new XAttribute("Name", "chris"));

            var node = builder.ProcessElement(el);
            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(typeof(CompositeNode), node);
        }

        [Test]
        public void ComplexNodes_CreatedFromProcessElement_ShouldHaveSameNameAsElement()
        {
            var el = new XElement("Customer",
                      new XAttribute("Name", "chris"));

            var node = builder.ProcessElement(el);
            Assert.AreEqual("Customer", node.Name);
        }

        [Test]
        public void ProcessElement_CreatesLeafNode_ForNonComplexElement()
        {
            var el = new XElement("Name", "Chris");
            var node = builder.ProcessElement(el);

            Assert.IsNotNull(node);
            Assert.IsInstanceOfType(typeof(LeafNode), node);
        }

        [Test]
        public void ProcessElement_CreatesLeafNodesOnCompositeNode_ForElementWithAttributes()
        {
            var el = new XElement("Customer",
                 new XAttribute("Name", "chris"));

            var node = builder.ProcessElement(el);
            Assert.IsInstanceOfType(typeof(CompositeNode), node);

            CompositeNode cnode = (CompositeNode)node;
            Assert.AreEqual(1, cnode.ChildrenCount);

            var lnode = cnode.ChildNodes[0] as LeafNode;
            Assert.IsNotNull(lnode);
            Assert.AreEqual("Name", lnode.Name);
            Assert.AreEqual("chris", lnode.Value);
        }

        [Test]
        public void ProcessElement_CreatesChildCompositeNodes_ForElementWithComplexChildNodes()
        {
            var el = new XElement("Customer",
                        new XElement("Name",
                            new XElement("First", "chris")));

            var node = builder.ProcessElement(el);
            Assert.IsInstanceOfType(typeof(CompositeNode), node);

            var cnode = (CompositeNode)node;
            Assert.AreEqual(1, cnode.ChildrenCount);

            var childNode = cnode.ChildNodes[0];
            Assert.IsInstanceOfType(typeof(CompositeNode), childNode);
            Assert.AreEqual("Name", childNode.Name);
        }

        [Test]
        public void ProcessElement_CreatesLeafNodesOnComposite_ForElementWithNonComplexChildNodes()
        {
            var el = new XElement("Name",
                        new XElement("First", "chris"));

            var node = builder.ProcessElement(el);
            Assert.IsInstanceOfType(typeof(CompositeNode), node);

            var cnode = (CompositeNode)node;
            Assert.AreEqual(1, cnode.ChildrenCount);

            Assert.IsInstanceOfType(typeof(LeafNode), cnode.ChildNodes[0]);
            

        }
    }
}
