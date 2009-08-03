using System.Linq;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;
using Castle.Core.Configuration;

namespace Castle.MicroKernel.Registration {
    public class ExceptionWirePropertiesOptions<COMPONENT_TYPE> {
        WirePropertiesOptions<COMPONENT_TYPE> _parentOptions;
        public ExceptionWirePropertiesOptions(WirePropertiesOptions<COMPONENT_TYPE> parentOptions) {
            if (parentOptions.IsNull())
                throw new ArgumentNullException("parentOptions", "parentOptions is null.");
            _parentOptions = parentOptions;
        }
        public ExceptionWirePropertiesOptions<COMPONENT_TYPE> Except(params string[] propertyNames) {
            _parentOptions.ExceptedPropertyNames.AddRange(propertyNames); return this;
        }
        public ExceptionWirePropertiesOptions<COMPONENT_TYPE> Except<PROPERTY_TYPE>(
            Expression<Func<COMPONENT_TYPE, PROPERTY_TYPE>> expresion) {
            string name;

            if (expresion.Body is MemberExpression)
                name = (expresion.Body as MemberExpression).Member.Name;
            else if (expresion.Body is MethodCallExpression)
                name = (expresion.Body as MethodCallExpression).Method.Name;
            else
                throw new InvalidOperationException("Cannot derive member name. Proper use: .WireProperties(o=>o.None().Except(x=>x.SomeProperty)");

            return Except(name);
        }
    }
    public class WirePropertiesOptions<COMPONENT_TYPE> {
        private List<string> _exceptedPropertyNames = new List<string>();
        public List<string> ExceptedPropertyNames { get { return _exceptedPropertyNames; } }
        public bool? WireComponentProperties { get; private set; }
        public ExceptionWirePropertiesOptions<COMPONENT_TYPE> All() {
            WireComponentProperties = true;
            return new ExceptionWirePropertiesOptions<COMPONENT_TYPE>(this);
        }
        public ExceptionWirePropertiesOptions<COMPONENT_TYPE> None() {
            WireComponentProperties = false;
            return new ExceptionWirePropertiesOptions<COMPONENT_TYPE>(this);
        }
    }

    public static partial class OptionalPropertyInjectionFacility_ComponentRegistrationExtensions {
        public static ComponentRegistration<S> WireProperties<S>(this ComponentRegistration<S> reg, Action<WirePropertiesOptions<S>> optionCreation) {
            var opt = new WirePropertiesOptions<S>();
            optionCreation(opt);
            if (opt.WireComponentProperties.IsNull())
                return reg;
            var c = new MutableConfiguration("wire-properties")
                .Attribute("value", opt.WireComponentProperties.Value.ToString());
            opt.ExceptedPropertyNames.ForEach(p =>
                c.CreateChild("except").Attribute("propertyName", p));
            return reg.Configuration(c);
        }
    }
}
