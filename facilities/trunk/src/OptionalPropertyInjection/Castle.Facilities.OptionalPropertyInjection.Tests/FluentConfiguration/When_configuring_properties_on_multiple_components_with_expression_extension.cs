using System;
using Castle.MicroKernel.Registration;
using Xunit;

namespace Castle.Facilities.OptionalPropertyInjection.Tests.FluentConfiguration {
    public class When_configuring_properties_on_multiple_components_with_expression_extension : WhenConfiguringWithDSL {
        public When_configuring_properties_on_multiple_components_with_expression_extension() {
            register_answer = delegate { };
            add_facility = c => c.AddFacility("property.injection.facility", new OptionalPropertyInjectionFacility(true));
        }
        [Fact]
        public void will_inject_all_properties_on_all_components() {
            register_question = c => c.Register(
                AllTypes.FromAssemblyContaining<QuestionOfLifeUniverseAndEverything>().Where(x => true)
                .WireProperties(o => o.None())
                );
            check_container = c => {
                var q1 = c.Resolve<QuestionOfLifeUniverseAndEverything>();
                Assert.Null(q1.TheAnswer);
                var q2 = c.Resolve<QuestionToBeOrNotToBe>();
                Assert.Null(q2.TheAnswer);
            };
        }
        [Fact]
        public void will_inject_all_properties_on_multiple_select_components() {
            register_question = c => c.Register(
                AllTypes.FromAssemblyContaining<QuestionOfLifeUniverseAndEverything>().Where(x => true)
                .WireProperties(o => o.OnType<QuestionOfLifeUniverseAndEverything>().None()
                    .AndOnType<QuestionToBeOrNotToBe>().None())
                );
            check_container = c => {
                var q1 = c.Resolve<QuestionOfLifeUniverseAndEverything>();
                Assert.Null(q1.TheAnswer);
                var q2 = c.Resolve<QuestionToBeOrNotToBe>();
                Assert.Null(q2.TheAnswer);
            };
        }
        [Fact] 
        public void will_inject_all_properties_on_select_components() {
            register_question = c => c.Register(
                AllTypes.FromAssemblyContaining<QuestionOfLifeUniverseAndEverything>().Where(x=>true)
                .WireProperties(o => o.OnType<QuestionOfLifeUniverseAndEverything>().None())
                );
            check_container = c => {
                var q1 = c.Resolve<QuestionOfLifeUniverseAndEverything>();
                Assert.Null(q1.TheAnswer);
                var q2 = c.Resolve<QuestionToBeOrNotToBe>();
                Assert.NotNull(q2.TheAnswer);
            };
        }
        [Fact]
        public void can_inject_select_properties_on_select_components() {
            register_question = c => c.Register(
                AllTypes.FromAssemblyContaining<QuestionOfLifeUniverseAndEverything>().Where(x => true)
                .WireProperties(o => o
                    .OnType<QuestionOfLifeUniverseAndEverything>().None().Except(x=>x.TheAnswer)
                    .AndOnType<QuestionToBeOrNotToBe>().All().Except(x=>x.TheAnswer))
                );
            check_container = c => {
                var q1 = c.Resolve<QuestionOfLifeUniverseAndEverything>();
                Assert.NotNull(q1.TheAnswer);
                Assert.Null(q1.OtherAnswer);
                var q2 = c.Resolve<QuestionToBeOrNotToBe>();
                Assert.Null(q2.TheAnswer);
            };
        }
    }
}
