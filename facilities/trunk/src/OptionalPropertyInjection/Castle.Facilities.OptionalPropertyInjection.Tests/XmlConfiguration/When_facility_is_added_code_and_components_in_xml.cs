using System;
using Xunit;

namespace Castle.Facilities.OptionalPropertyInjection.Tests.XmlConfiguration {
    public class When_facility_is_added_code_and_components_in_xml : WhenConfiguringWithXml {
        public static string questionXmlConfig =
        @"
        <component id=""question-to-life"" 
		    type=""Castle.Facilities.OptionalPropertyInjection.Tests.QuestionOfLifeUniverseAndEverything, Castle.Facilities.OptionalPropertyInjection.Tests"">
		    <wire-properties value=""true"" />
        </component>";
        public When_facility_is_added_code_and_components_in_xml() {
            register_question = delegate { };
        }
        [Fact(Skip="TODO: make this pass")]
        public void can_control_property_injection_through_xml() {
            create_container = () => create_container_configured_with_xml(xmlConfig.Use("", questionXmlConfig));
            add_facility_with_injection_default_of(true);

            check = no_properties_were_injected;
        }
    }
}
