using System;

namespace Castle.Facilities.OptionalPropertyInjection.RegistrationOptions {
    public class InitialRegistrationOptions : SelectAllOrNoneOptions<UnChainableFinalRegistrationOptions> {
        public InitialRegistrationOptions() :
            base(new UnChainableFinalRegistrationOptions(), (opt, b) => opt.SetWireProperties(b)) { }
        public SelectAllOrNoneOptions<ChainableFinalRegistrationOptions<APPLY_TO_TYPE>> OnType<APPLY_TO_TYPE>() {
            return new SelectAllOrNoneOptions<ChainableFinalRegistrationOptions<APPLY_TO_TYPE>>(
                new ChainableFinalRegistrationOptions<APPLY_TO_TYPE>(null),
                (opt, b) => opt.SetWireProperties(b));
        }
    }
}
