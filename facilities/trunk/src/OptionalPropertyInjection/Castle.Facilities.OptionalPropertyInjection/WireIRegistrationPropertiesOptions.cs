using System;
using Castle.Facilities.OptionalPropertyInjection;

namespace Castle.MicroKernel.Registration {
    public class PropertyFacilityRegistration : IRegistration {
        private IRegistration _inner;
        public PropertyFacilityRegistration(IRegistration inner) {
            _inner = inner;
        }

        public void Register(IKernel kernel) {
            _inner.Register(kernel);
        }

    }
    public class WireIRegistrationPropertiesOptions {
        public WireIRegistrationPropertiesOptions OnType<APPLY_TO_TYPE>() {
            return this;
        }
        public void All() {

        }
        public void None() {

        }

    }
    public static partial class OptionalPropertyInjectionFacility_ComponentRegistrationExtensions {
        public static IRegistration InjectProperties(this IRegistration reg, Action<WireIRegistrationPropertiesOptions> optionCreation) {
            var opt = new WireIRegistrationPropertiesOptions();
            optionCreation(opt);
            OptionalPropertyInjectionFacility.GetInstance().AddRegistrationOptions(opt);            
            return new PropertyFacilityRegistration(reg);
            return reg;
        }
    }

}
