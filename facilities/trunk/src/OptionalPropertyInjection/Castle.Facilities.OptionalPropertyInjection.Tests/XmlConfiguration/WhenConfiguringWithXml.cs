using System;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace Castle.Facilities.OptionalPropertyInjection.Tests.XmlConfiguration {
    public class WhenConfiguringWithXml : WhenTestingConfigurationOptions {
        protected const string xmlConfig = @"
		    <configuration>
                <facilities>
                    {0}
                </facilities>
		        <components>
                    {1}
		        </components>
		    </configuration>";
        protected const string facilityXml = @"
            <facility id=""optional-property-injection"" 
                type=""Castle.Facilities.OptionalPropertyInjection.OptionalPropertyInjectionFacility, Castle.Facilities.OptionalPropertyInjection""
                provideByDefault=""{0}""/>";

        public WhenConfiguringWithXml() {
            register_answer = c => c.AddComponent<AnswerToLifeUniverseAndEverything>();
            add_facility = delegate { };
        }
        protected IWindsorContainer create_container_configured_with_xml(string xmlString) {
            return new WindsorContainer(new XmlInterpreter(new XmlStringResource(xmlString)));
        }
        protected void add_facility_with_injection_default_of(bool wireByDefault) {
            add_facility = c => c.AddFacility("facility", new OptionalPropertyInjectionFacility(wireByDefault));
        }
    }
}
