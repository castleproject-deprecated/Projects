using System;
using Castle.MicroKernel.Registration;
using Xunit;

namespace Castle.Facilities.OptionalPropertyInjection.Tests.FluentConfiguration {
    public class When_configuring_select_component_properties_with_expression_extension : WhenConfiguringWithDSL {
        [Fact]
        public void will_override_the_component_default_and_inject_selected() {
            register_question = c =>
                c.Register(Component.For<QuestionOfLifeUniverseAndEverything>()
                    .WireProperties(o => o.None().Except(x => x.TheAnswer)));

            check = q => {
                Assert.NotNull(q.TheAnswer);
                Assert.Null(q.OtherAnswer);
            };
        }
        [Fact]
        public void will_override_the_component_default_and_not_inject_selected() {
            register_question = c =>
                c.Register(Component.For<QuestionOfLifeUniverseAndEverything>()
                    .WireProperties(o => o.All().Except(x => x.OtherAnswer)));

            check = q => {
                Assert.NotNull(q.TheAnswer);
                Assert.Null(q.OtherAnswer);
            };
        }
        [Fact]
        public void can_select_multiple_properties() {
            register_question = c =>
                c.Register(Component.For<QuestionOfLifeUniverseAndEverything>()
                    .WireProperties(o => o.None().Except(x => x.TheAnswer).Except(x=>x.OtherAnswer)));

            check = all_properties_were_injected;
        }
    }
}
