using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.XPath;
using Castle.MonoRail.Framework;
using System.Xml.Linq;
using System.Xml.Serialization;
namespace Castle.MonoRail.Rest.Binding
{
    public class XmlBindAttribute : System.Attribute, IParameterBinder
    {
        private Dictionary<Type,Func<Stream,Object>> _factories;

        public XmlBindAttribute() {

            _factories = new Dictionary<Type, Func<Stream, object>>();

            _factories[typeof(XmlReader)] = inputStream => XmlReader.Create(inputStream);
            _factories[typeof(String)] = inputStream => new StreamReader(inputStream).ReadToEnd();
            _factories[typeof(XPathNavigator)] = inputStream => { var doc = new XPathDocument(inputStream); return doc.CreateNavigator(); };
            _factories[typeof(XDocument)] = inputStream => XDocument.Load(XmlReader.Create(inputStream)); 
        }

        public Object CreateValueFromInputStream(Type valueType, Stream inputStream)
        {
            if (_factories.ContainsKey(valueType))
            {
                return _factories[valueType](inputStream);
            }
            else
            {
                XmlSerializer serial = new XmlSerializer(valueType);
                return serial.Deserialize(inputStream);
            }
            
        }

        #region IParameterBinder Members

        public object Bind(SmartDispatcherController controller, System.Reflection.ParameterInfo parameterInfo)
        {
            var inputStream = controller.Context.UnderlyingContext.Request.InputStream;
            return CreateValueFromInputStream(parameterInfo.ParameterType, inputStream);
        }

        public int CalculateParamPoints(SmartDispatcherController controller, System.Reflection.ParameterInfo parameterInfo)
        {
            return 10; 
        }

        #endregion
    }
}
