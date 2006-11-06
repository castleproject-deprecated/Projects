// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Castle.ActiveRecord.Framework.Validators;
using Castle.ActiveRecord.Validation.Attributes;

namespace Castle.ActiveRecord.Validation.Validators.CultureIndependant
{
    public class AtLeastOneIsNotEmptyValidator : AbstractValidator
    {
        private string group;
        
        public AtLeastOneIsNotEmptyValidator(string group)
        {
            this.group = group;
        }

        public override bool Perform(object instance, object fieldValue)
        {
            PropertyInfo[] propertyInfos = instance.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            ICollection<PropertyInfo> propertiesInGroup = new Collection<PropertyInfo>();
                
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                object[] attributes = propertyInfo.GetCustomAttributes(false);

                foreach (object attribute in attributes)
                {
                    ValidateAtLeastOneIsNotEmptyAttribute validator = attribute as ValidateAtLeastOneIsNotEmptyAttribute;

                    if ((validator != null) && (validator.Group == group))
                        propertiesInGroup.Add(propertyInfo);
                }
            }

            foreach (PropertyInfo propertyInfo in propertiesInGroup)
            {
                if (propertyInfo.PropertyType == typeof(string))
                {
                    if (!string.IsNullOrEmpty((string) propertyInfo.GetValue(instance, null)))
                        return true;
                }
                else if (propertyInfo.GetValue(instance, null) != null)                
                    return true;                
            }
            
            return false;
        }

        /// <summary>
        /// Builds the default error message.
        /// </summary>
        /// <returns></returns>
        protected override string BuildErrorMessage()
        {
            return String.Format("Field {0} belongs to a group of fields of which at least one should not be empty.", Property.Name);
        }
    }
}