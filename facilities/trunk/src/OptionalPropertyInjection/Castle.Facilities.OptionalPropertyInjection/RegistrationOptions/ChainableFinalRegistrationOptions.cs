using System;
using System.Reflection;
using System.Linq.Expressions;
using System.Linq;

namespace Castle.Facilities.OptionalPropertyInjection.RegistrationOptions {
    public class ChainableFinalRegistrationOptions<APPLY_TO_TYPE> : ChainableFinalRegistrationOptions, FinalRegistrationOptions {
        public ChainableFinalRegistrationOptions(ChainableFinalRegistrationOptions next)
            : base(typeof(APPLY_TO_TYPE), next) {
        }
        public SelectAllOrNoneOptions<ChainableFinalRegistrationOptions<NEW_TYPE>> AndOnType<NEW_TYPE>() {
            var link = new ChainableFinalRegistrationOptions<NEW_TYPE>(this);
            return new SelectAllOrNoneOptions<ChainableFinalRegistrationOptions<NEW_TYPE>>(
                link, (opt, b) => opt.SetWireProperties(b));
        }
        public ChainableFinalRegistrationOptions<APPLY_TO_TYPE> Except(params string[] propertyNames) {
            SetExceptions(propertyNames);
            return this;
        }
        public ChainableFinalRegistrationOptions<APPLY_TO_TYPE> Except<PROPERTY_TYPE>(
            params Expression<Func<APPLY_TO_TYPE, PROPERTY_TYPE>>[] expressions) {
            var parameterNames = expressions.Select(expr => {
                if (expr.Body is MemberExpression)
                    return (expr.Body as MemberExpression).Member.Name;
                else if (expr.Body is MethodCallExpression)
                    return (expr.Body as MethodCallExpression).Method.Name;
                else
                    throw new InvalidOperationException("Cannot derive member name. Proper use: .WireProperties(o=>o.None().Except(x=>x.SomeProperty)");
            });
            return Except(parameterNames.ToArray());
        }
    }

    public class ChainableFinalRegistrationOptions : AbstractFinalRegistrationOptions, FinalRegistrationOptions {
        private ChainableFinalRegistrationOptions _next;
        public ChainableFinalRegistrationOptions(Type applyToType, ChainableFinalRegistrationOptions next)
            : base(applyToType) {
            _next = next;
        }
        public override bool? ShouldWire(PropertyInfo pi) {
            var res = base.ShouldWire(pi);
            if (res.IsNull() && _next.IsNotNull())
                res = _next.ShouldWire(pi);
            return res;
        }
    }
}
