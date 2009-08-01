using System;
using Xunit;

namespace Castle.Facilities.OptionalPropertyInjection.Tests.XmlConfiguration {
    public class When_configuring_select_component_properties_with_xml : WhenConfiguringWithXml {

        public readonly static string questionXml =
@"
<component id=""question-to-life"" 
    type=""Castle.Facilities.OptionalPropertyInjection.Tests.QuestionOfLifeUniverseAndEverything, Castle.Facilities.OptionalPropertyInjection.Tests"">
        <wire-properties value=""{0}"">
            <except propertyName=""TheAnswer"" />
        </wire-properties>
</component>";
		        public When_configuring_select_component_properties_with_xml() {
		            register_question = delegate { };
		        }
		        [Fact]
		        public void can_wire_select_property_but_not_others() {
		            create_container = () => create_container_configured_with_xml(
                        xmlConfig.Use(facilityXml.Use(true), questionXml.Use(true))); 

		            check = q => {
		                Assert.Null(q.TheAnswer);
		                Assert.NotNull(q.OtherAnswer);
		            };
		        }
		        [Fact]
		        public void can_wire_no_properties() {
                    create_container = () => create_container_configured_with_xml(
                        xmlConfig.Use(facilityXml.Use(true), questionXml.Use(false))); 

		            check = q => {
		                Assert.NotNull(q.TheAnswer);
		                Assert.Null(q.OtherAnswer);
		            };
		        }
		    }
}
