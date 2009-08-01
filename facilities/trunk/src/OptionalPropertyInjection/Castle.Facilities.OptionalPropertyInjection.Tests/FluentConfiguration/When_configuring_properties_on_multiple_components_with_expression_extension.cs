using System;
using Castle.MicroKernel.Registration;
using Xunit;

namespace Castle.Facilities.OptionalPropertyInjection.Tests.FluentConfiguration {
    public class When_configuring_properties_on_multiple_components_with_expression_extension : WhenConfiguringWithDSL {
        public When_configuring_properties_on_multiple_components_with_expression_extension() {
            register_answer = delegate { };
            add_facility = c => c.AddFacility("property.injection.facility", new OptionalPropertyInjectionFacility(true));
        }
        [Fact(Skip="TODO: Make it pass")] 
        public void will_inject_all_properties_on_select_components() {
            register_question = c => c.Register(
                AllTypes.FromAssemblyContaining<QuestionOfLifeUniverseAndEverything>().Where(x=>true)
                .InjectProperties(o => o.OnType<QuestionOfLifeUniverseAndEverything>().None())
                );
            check = no_properties_were_injected;
        }
    }
}
