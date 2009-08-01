using System;
using Castle.MicroKernel.Registration;
using Xunit;

namespace Castle.Facilities.OptionalPropertyInjection.Tests.FluentConfiguration {
    public class When_configuring_select_component_properties_with_string_extension : WhenConfiguringWithDSL {
        [Fact]
        public void will_override_the_component_default_and_inject_selected() {
            additonal_registration = reg => reg.WireProperties(o => o.None().Except("TheAnswer"));
            check = q => {
                Assert.NotNull(q.TheAnswer);
                Assert.Null(q.OtherAnswer);
            };
        }
        [Fact]
        public void will_override_the_component_default_and_not_inject_selected() {
            additonal_registration = reg => reg.WireProperties(o => o.All().Except("OtherAnswer"));
            check = q => {
                Assert.NotNull(q.TheAnswer);
                Assert.Null(q.OtherAnswer);
            };
        }
        [Fact]
        public void can_select_multiple_properties() {
            additonal_registration = reg => reg.WireProperties(o => o.None().Except("TheAnswer", "OtherAnswer"));
            check = all_properties_were_injected;
        }
    }
}
