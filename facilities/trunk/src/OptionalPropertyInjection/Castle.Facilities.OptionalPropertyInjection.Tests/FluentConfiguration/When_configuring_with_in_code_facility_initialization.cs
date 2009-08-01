using System;
using Xunit;

namespace Castle.Facilities.OptionalPropertyInjection.Tests.FluentConfiguration {
    public class When_configuring_with_in_code_facility_initialization : WhenConfiguringWithDSL {

        [Fact]
        public void can_set_all_properties_to_inject_by_default() {
            create_in_code_facility = () => new OptionalPropertyInjectionFacility(true);
            check = all_properties_were_injected;
        }
        [Fact]
        public void can_set_all_properties_to_not_inject_by_default() {
            create_in_code_facility = () => new OptionalPropertyInjectionFacility(false);
            check = no_properties_were_injected;
        }
    }
}
