using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Castle.Facilities.OptionalPropertyInjection.Tests.FluentConfiguration {
    public class WhenConfiguringWithDSL : WhenTestingConfigurationOptions {
        protected Func<OptionalPropertyInjectionFacility> create_in_code_facility =
            () => new OptionalPropertyInjectionFacility(false);
        protected Func<ComponentRegistration<QuestionOfLifeUniverseAndEverything>, ComponentRegistration<QuestionOfLifeUniverseAndEverything>>
            additonal_registration = x => x;
        public WhenConfiguringWithDSL() {
            create_container = () => new WindsorContainer();
            add_facility = c => c.AddFacility("optional-property-injection", create_in_code_facility());
            register_answer = c => c.Register(Component.For<AnswerToLifeUniverseAndEverything>());
            register_question = c => c.Register(additonal_registration(Component.For<QuestionOfLifeUniverseAndEverything>()));
        }
    }
}
