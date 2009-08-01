using System;
using Castle.Core.Resource;
using System.IO;

namespace Castle.Facilities.OptionalPropertyInjection.Tests.XmlConfiguration {
    public class XmlStringResource : AbstractResource, IResource {
        private string _xmlConfiguration;
        public XmlStringResource(string xmlConfiguration) {
            _xmlConfiguration = xmlConfiguration;
        }
        public override IResource CreateRelative(string relativePath) {
            throw new NotImplementedException();
        }

        public override TextReader GetStreamReader(System.Text.Encoding encoding) {
            return new StringReader(_xmlConfiguration);
        }

        public override TextReader GetStreamReader() {
            return new StringReader(_xmlConfiguration);
        }
    }
}
