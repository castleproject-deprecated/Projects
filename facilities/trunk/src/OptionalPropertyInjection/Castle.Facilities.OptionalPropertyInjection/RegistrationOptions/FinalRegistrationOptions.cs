using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;

namespace Castle.Facilities.OptionalPropertyInjection.RegistrationOptions {
    public abstract class AbstractFinalRegistrationOptions : FinalRegistrationOptions {
        private Type _applyToType;
        private bool _wireProperties;
        private string[] _exceptions = new string[0];
        public void SetWireProperties(bool wireProperties) { _wireProperties = wireProperties; }
        public void SetExceptions(string[] exceptions) { _exceptions = exceptions; }
        public virtual bool? ShouldWire(PropertyInfo pi) {
            if (_applyToType.IsNull() || pi.DeclaringType == _applyToType)
                return _exceptions.Contains(pi.Name) ? !_wireProperties : _wireProperties;                
            return null;
        }
        public AbstractFinalRegistrationOptions(Type applyToType) {
            _applyToType = applyToType;
        }
        public AbstractFinalRegistrationOptions() : this(null) { }
    }
    public class UnChainableFinalRegistrationOptions : AbstractFinalRegistrationOptions, FinalRegistrationOptions {
    }
    public interface FinalRegistrationOptions {
        bool? ShouldWire(PropertyInfo property);

    }
}
