using System;
using System.Collections.Generic;
using System.Text;
using MbUnit.Framework;
using System.IO;
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
        	XmlDocument doc = new XmlDocument();
			doc.Load(new StringReader("<Customer><Name>Chris</Name></Customer>"));

            XmlWriter writer = XmlWriter.Create(ms);
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
        [Row(typeof(XmlDocument))]
        public void CreateValuFromInputStream_CanConvertTo_PredefinedTypes(Type predefinedType)
        {
            object converted = attr.CreateValueFromInputStream(predefinedType, inputStream);
            Assert.IsNotNull(converted, "Converted value was null for type " + predefinedType.ToString());
            Assert.IsInstanceOfType(predefinedType, converted, "Converted type should have been " + predefinedType.ToString() + " but was " + converted.GetType().ToString());
        }

        [Test]
        public void IfNoFactoryDefinedForParameterType_CreateValueFromInputStream_ShouldDeserializeInput()
        {
            Customer converted = attr.CreateValueFromInputStream(typeof(Customer), inputStream) as Customer;
            Assert.IsNotNull(converted);
            Assert.IsInstanceOfType(typeof(Customer), converted);
            Assert.AreEqual("Chris", converted.Name);
        }
    }

    [Serializable]
    public class Customer
    {
    	private string _name;


    	public string Name
    	{
    		get { return _name; }
    		set { _name = value; }
    	}
    }
}
