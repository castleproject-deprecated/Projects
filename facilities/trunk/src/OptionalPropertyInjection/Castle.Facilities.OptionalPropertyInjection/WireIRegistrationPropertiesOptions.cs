using System;
using Castle.Facilities.OptionalPropertyInjection.RegistrationOptions;

namespace Castle.MicroKernel.Registration {
    public static partial class OptionalPropertyInjectionFacility_ComponentRegistrationExtensions {
        public static IRegistration WireProperties(this IRegistration reg, Func<InitialRegistrationOptions, FinalRegistrationOptions> optionCreation) {
            var opt = optionCreation(new InitialRegistrationOptions());
            return new PropertyFacilityRegistration(reg, opt);
        }
    }

}
