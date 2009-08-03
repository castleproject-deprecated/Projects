using System;
using Castle.Facilities.OptionalPropertyInjection;
using System.Linq;
using Castle.Facilities.OptionalPropertyInjection.RegistrationOptions;

namespace Castle.MicroKernel.Registration {
    public class PropertyFacilityRegistration : IRegistration {
        private FinalRegistrationOptions _options;
        private IRegistration _inner;
        public PropertyFacilityRegistration(IRegistration inner, FinalRegistrationOptions options) {
            if (inner == null)
                throw new ArgumentNullException("inner", "inner is null.");
            if (options == null)
                throw new ArgumentNullException("options", "options is null.");
            _options = options;
            _inner = inner;
        }

        public void Register(IKernel kernel) {
            var fclty = kernel.GetFacilities().FirstOrDefault(f => f is OptionalPropertyInjectionFacility) as OptionalPropertyInjectionFacility;
            if (fclty.IsNotNull()) {
                try {
                    fclty.SetRegistrationOptions(_options);
                    _inner.Register(kernel);
                }
                finally {
                    fclty.ClearRegistrationOptions();
                }
            }
            else
                _inner.Register(kernel);
        }

    }
}
