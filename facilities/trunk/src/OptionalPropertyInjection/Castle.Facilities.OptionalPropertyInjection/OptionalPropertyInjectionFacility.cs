using System;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel;
using System.Linq;
using Castle.Core;
using Castle.MicroKernel.Registration;
using System.Collections.Generic;

namespace Castle.Facilities.OptionalPropertyInjection {
    internal static partial class ComponentModelExtensions {
        public static bool? GetBoolAttribute(this ComponentModel model, string attributeName) {
            return model.Configuration.IfNotNull(x=>
                x.Attributes[attributeName].ToBool());
        }
        public static bool? ToBool(this string stringBool) {
            bool res;
            bool success = Boolean.TryParse(stringBool, out res);
            return success == false ? null : res as bool?;
        }
    }
    public class OptionalPropertyInjectionFacility : AbstractFacility, IFacility {
        private List<WireIRegistrationPropertiesOptions> _options = new List<WireIRegistrationPropertiesOptions>();
        public IEnumerable<WireIRegistrationPropertiesOptions> RegistrationOptions {
            get { return _options; }
        }
        public void AddRegistrationOptions(WireIRegistrationPropertiesOptions opt) {
            _options.Add(opt);
        }
        public static OptionalPropertyInjectionFacility GetInstance() {
            if (_instance.IsNull())
                _instance = new OptionalPropertyInjectionFacility();
            return _instance;
        }
        private static OptionalPropertyInjectionFacility _instance;
        private bool _wirePropertiesInContainerByDefault;
        public OptionalPropertyInjectionFacility() : this(true) { }
        public OptionalPropertyInjectionFacility(bool provideByDefault) {
            _wirePropertiesInContainerByDefault = provideByDefault;
        }
        public OptionalPropertyInjectionFacility(string provideByDefault) {
            _wirePropertiesInContainerByDefault = true;
        }
        
        protected override void Init() {
            Kernel.ComponentRegistered += new ComponentDataDelegate(OnComponentRegistered);

            if (FacilityConfig.IsNotNull()) {
                if (FacilityConfig.Attributes["provideByDefault"].ToBool().IsNotNull())
                    _wirePropertiesInContainerByDefault = (bool)FacilityConfig.Attributes["provideByDefault"].ToBool();
            }
        }

        void OnComponentRegistered(string key, IHandler handler) {
            var model = handler.ComponentModel;
            var propertiesToRemove = model.Properties.Where(p => ShouldRemove(p.Property, model)).ToArray();
            propertiesToRemove.ForEach(ps =>
                model.Properties.Remove(ps));
        }

        private bool ShouldRemove(System.Reflection.PropertyInfo pi, ComponentModel model) {
            return ! new WiredPropertyChecker(_wirePropertiesInContainerByDefault).IsWired(pi, model);
        }
    }

}
