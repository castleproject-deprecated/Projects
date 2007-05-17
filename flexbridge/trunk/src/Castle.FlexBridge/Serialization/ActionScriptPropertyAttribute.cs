// Copyright 2007 Castle Project - http://www.castleproject.org/
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
using System.Text;

namespace Castle.FlexBridge.Serialization
{
    /// <summary>
    /// Maps a .Net property or field to an ActionScript property.
    /// When omitted, the property or field will be automatically mapped to the
    /// non-dynamic ActionScript property of the same name.
    /// <seealso cref="ActionScriptClassAttribute"/>
    /// <seealso cref="ActionScriptIgnoreAttribute"/>
    /// </summary>
    /// <remarks>
    /// This attribute cannot be used in a class that implements <see cref="IExternalizable" />
    /// because the properties and fields of externalizable classes are not mapped directly.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ActionScriptPropertyAttribute : Attribute
    {
        private string propertyName;
        private bool isDynamic;

        /// <summary>
        /// Associates the decorated property or field with the ActionScript property
        /// of the same name.
        /// </summary>
        public ActionScriptPropertyAttribute()
        {
        }

        /// <summary>
        /// Associates the decorated property or field with the ActionScript property
        /// with the specified name.
        /// </summary>
        /// <param name="propertyName">The ActionScript property name that corresponds
        /// to the decorated property or field, or null if the ActionScript property
        /// has the name same as the decorated property or field.</param>
        public ActionScriptPropertyAttribute(string propertyName)
        {
            this.propertyName = propertyName;
        }

        /// <summary>
        /// Gets or sets the ActionScript property name that corresponds
        /// to the decorated property or field, or null if the ActionScript property
        /// has the name same as the decorated property or field.
        /// </summary>
        public string PropertyName
        {
            get { return propertyName; }
            set { propertyName = value; }
        }

        /// <summary>
        /// Gets or sets whether the property should be serialized as a dynamic class member.
        /// This value only makes sense if the corresponding ActionScript class is
        /// declared to be "dynamic".  When set to true, the value of the decorated property
        /// or field will be assigned to a dynamic property of the receiving ActionScript
        /// class upon deserialization.
        /// </summary>
        /// <remarks>
        /// This option can be used together with <see cref="IDynamic" /> or as an alternative
        /// for cases where the set of "dynamic" properties is statically known to the .Net code
        /// (but perhaps not to ActionScript) such as when mapping a pre-existing .Net class.
        /// Usually it is better to implement <see cref="IDynamic" /> instead.
        /// </remarks>
        /// <value>The default value is false.</value>
        public bool IsDynamic
        {
            get { return isDynamic; }
            set { isDynamic = value; }
        }
    }
}
