using System;

namespace Castle.Facilities.OptionalPropertyInjection.RegistrationOptions {
    public class SelectAllOrNoneOptions<RETURN_TYPE> {
        private Action<RETURN_TYPE, bool> _setAll;
        private RETURN_TYPE _returnInstance;
        public SelectAllOrNoneOptions(RETURN_TYPE returnInstance, Action<RETURN_TYPE, bool> setPolicy) {
            _setAll = setPolicy;
            _returnInstance = returnInstance;
        }
        public RETURN_TYPE All() {
            _setAll(_returnInstance, true);
            return _returnInstance;
        }
        public RETURN_TYPE None() {
            _setAll(_returnInstance, false);
            return _returnInstance;
        }
    }
}
