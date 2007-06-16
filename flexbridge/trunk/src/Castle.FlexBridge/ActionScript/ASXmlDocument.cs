// Copyright 2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.ActionScript
{
    /// <summary>
    /// Adapts an <see cref="XmlDocument" /> for ActionScript serialization.
    /// </summary>
    public sealed class ASXmlDocument : BaseASValue
    {
        private XmlDocument xmlDocument;
        private string xmlString;

        /// <summary>
        /// Creates an empty XML document.
        /// </summary>
        public ASXmlDocument()
        {
            xmlDocument = new XmlDocument();
        }

        /// <summary>
        /// Creates a wrapper for an existing XML document.
        /// </summary>
        /// <param name="xmlDocument">The XML document</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="xmlDocument"/> is null</exception>
        public ASXmlDocument(XmlDocument xmlDocument)
        {
            if (xmlDocument == null)
                throw new ArgumentNullException("xmlDocument");

            this.xmlDocument = xmlDocument;
        }

        /// <summary>
        /// Creates a wrapper for an XML document represented as a string.
        /// The string will be parsed into XML lazily on demand.
        /// </summary>
        /// <param name="xmlString">The XML document</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="xmlString"/> is null</exception>
        public ASXmlDocument(string xmlString)
        {
            if (xmlString == null)
                throw new ArgumentNullException("xmlString");

            this.xmlString = xmlString;
        }

        /// <summary>
        /// Gets or sets the XML document.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null</exception>
        public XmlDocument XmlDocument
        {
            get
            {
                if (xmlDocument == null)
                {
                    // Note: using a temporary variable so that if an exception occurs, the
                    //       state of the object is not clobbered.
                    XmlDocument xml = new XmlDocument();
                    xml.LoadXml(xmlString);

                    xmlDocument = xml;
                    xmlString = null; // clear it in case the contents of the document are modified using the XmlDocument instance
                }

                return xmlDocument;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                xmlDocument = value;
                xmlString = null;
            }
        }

        /// <summary>
        /// Gets or sets the XML document represented as a string.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is null</exception>
        public string XmlString
        {
            get
            {
                return xmlString != null ? xmlString : xmlDocument.OuterXml;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                xmlString = value;
                xmlDocument = null;
            }
        }

        /// <inheritdoc />
        public override ASTypeKind Kind
        {
            get { return ASTypeKind.Xml; }
        }

        /// <inheritdoc />
        public override void AcceptVisitor(IActionScriptSerializer serializer, IASValueVisitor visitor)
        {
            visitor.VisitXml(serializer, XmlString);
        }

        /// <inheritdoc />
        public override object GetNativeValue(Type nativeType)
        {
            return nativeType == typeof(XmlDocument) ? XmlDocument : null;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            ASXmlDocument other = obj as ASXmlDocument;
            return other != null && XmlString == other.XmlString;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return XmlString.GetHashCode();
        }
    }
}
