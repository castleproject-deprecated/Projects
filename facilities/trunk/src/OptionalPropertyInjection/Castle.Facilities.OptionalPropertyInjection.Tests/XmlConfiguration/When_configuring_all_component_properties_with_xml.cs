using System;
using Xunit;

namespace Castle.Facilities.OptionalPropertyInjection.Tests.XmlConfiguration {
    public class When_configuring_all_component_properties_with_xml : WhenConfiguringWithXml {
        public static string questionXmlConfig = @"
        <component id=""question-to-life"" 
            type=""Castle.Facilities.OptionalPropertyInjection.Tests.QuestionOfLifeUniverseAndEverything, Castle.Facilities.OptionalPropertyInjection.Tests"">
            <wire-properties value=""{0}"" />
        </component>";
        public When_configuring_all_component_properties_with_xml() {
            register_question = delegate { };
        }
        [Fact]
        public void can_wire_all_properties() {
            create_container = () => create_container_configured_with_xml(
                xmlConfig.Use(facilityXml.Use(false), questionXmlConfig.Use(true)));

            check = all_properties_were_injected;
        }
        [Fact] public void can_wire_no_properties() {
            create_container = () => create_container_configured_with_xml(
                xmlConfig.Use(facilityXml.Use(true), questionXmlConfig.Use(false)));

            check = no_properties_were_injected;
        }
    }
}
