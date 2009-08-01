using System;
using Xunit;

namespace Castle.Facilities.OptionalPropertyInjection.Tests.XmlConfiguration {
    public class When_configuring_with_xml_facility_initialization : WhenConfiguringWithXml {

        public When_configuring_with_xml_facility_initialization() {
            add_facility = delegate { };
            register_question = c => c.AddComponent<QuestionOfLifeUniverseAndEverything>();
        }
        [Fact]
        public void can_set_all_properties_to_inject_by_default() {
            create_container = () => create_container_configured_with_xml(xmlConfig.Use(facilityXml.Use(true), ""));
            check = all_properties_were_injected;
        }
        [Fact]
        public void can_set_all_properties_to_not_inject_by_default() {
            create_container = () => create_container_configured_with_xml(xmlConfig.Use(facilityXml.Use(false), ""));
            check = no_properties_were_injected;
        }
    }
}
