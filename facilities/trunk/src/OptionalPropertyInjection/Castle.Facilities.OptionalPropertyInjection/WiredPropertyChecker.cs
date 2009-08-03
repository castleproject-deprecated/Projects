using System;
using System.Linq;
using Castle.Core;
using Castle.Core.Configuration;
using System.Reflection;
using Castle.Facilities.OptionalPropertyInjection.RegistrationOptions;

namespace Castle.Facilities.OptionalPropertyInjection {
    public class WiredPropertyChecker {
        private bool _wirePropertiesInContainerByDefault;
        public bool IsWired(PropertyInfo pi, ComponentModel model) {
            if (CurrentRegistrationOptions.IsNotNull()) {
                var res = CurrentRegistrationOptions.ShouldWire(pi);
                if (res.IsNotNull())
                    return res??false;
            }
            if (model.Configuration.IsNull())
                return _wirePropertiesInContainerByDefault;
            return GetChildNode("wire-properties", model.Configuration.Children)
                .IfNotNull(n => {
                    var wireAllOnComponent = n.Attributes["value"].ToBool() ?? true;
                    if (n.Children.Any(c => Eq("except", c.Name) && Eq(pi.Name, c.Attributes["propertyName"])))
                        return (!wireAllOnComponent) as bool?;
                    return wireAllOnComponent as bool?;
                }) ?? _wirePropertiesInContainerByDefault;
        }
        public WiredPropertyChecker(bool wirePropertiesInContainerByDefault) {
            _wirePropertiesInContainerByDefault = wirePropertiesInContainerByDefault;
        }

        static bool Eq(string s1, string s2) {
            return string.Equals(s1, s2, StringComparison.InvariantCultureIgnoreCase);
        }
        private IConfiguration GetChildNode(string nodeName, ConfigurationCollection configCollection) {
            return configCollection.FirstOrDefault(c => Eq(c.Name, nodeName));
        }
        public FinalRegistrationOptions CurrentRegistrationOptions { get; set; }
    }
}
