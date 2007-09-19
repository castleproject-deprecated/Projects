using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MbUnit.Framework;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using Castle.MonoRail.Rest.Binding;
using System.Xml.XPath;
namespace Castle.MonoRail.Rest.Tests.Binding
{
    [TestFixture]
    public class XmlBindAttributeFixture
    {
        private Stream inputStream;
        private XmlBindAttribute attr;

        [SetUp]
        public void Setup()
        {
            MemoryStream ms = new MemoryStream();
            var doc = new XDocument(
                        new XElement("Customer",
                            new XElement("Name", "Chris")));

            var writer = XmlWriter.Create(ms);
            doc.WriteTo(writer);
            writer.Flush();
            ms.Position = 0;

            inputStream = ms;

            attr = new XmlBindAttribute();
        }
        

        [RowTest]
        [Row(typeof(XmlReader))]
        [Row(typeof(XPathNavigator))]
        [Row(typeof(string))]
        [Row(typeof(XDocument))]
        public void CreateValuFromInputStream_CanConvertTo_PredefinedTypes(Type predefinedType)
        {
            var converted = attr.CreateValueFromInputStream(predefinedType, inputStream);
            Assert.IsNotNull(converted, "Converted value was null for type " + predefinedType.ToString());
            Assert.IsInstanceOfType(predefinedType, converted, "Converted type should have been " + predefinedType.ToString() + " but was " + converted.GetType().ToString());
        }

        [Test]
        public void IfNoFactoryDefinedForParameterType_CreateValueFromInputStream_ShouldDeserializeInput()
        {
            var converted = attr.CreateValueFromInputStream(typeof(Customer), inputStream) as Customer;
            Assert.IsNotNull(converted);
            Assert.IsInstanceOfType(typeof(Customer), converted);
            Assert.AreEqual("Chris", converted.Name);
        }
    }

    [Serializable]
    public class Customer
    {
        public string Name { get; set; }
    }
}
