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
using System.Reflection;
using System.Text;

namespace Castle.FlexBridge.Serialization.Mapping
{
    /// <summary>
    /// Describes how properties of an ActionScript object as mapped
    /// to native properties.
    /// </summary>
    public sealed class ActionScriptPropertyMapping
    {
        private MemberInfo nativePropertyOrField;
        private string asPropertyName;
        private bool isDynamic;

        /// <summary>
        /// Creates an ActionScript property mapping.
        /// </summary>
        /// <param name="nativePropertyOrField">The native property or field</param>
        /// <param name="asPropertyName">The ActionScript property name</param>
        /// <param name="isDynamic">Whether the property should be serialized as a dynamic class member</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="nativePropertyOrField"/>
        /// or <paramref name="asPropertyName"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="nativePropertyOrField"/>
        /// is not a <see cref="PropertyInfo" /> or <see cref="FieldInfo" /></exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="asPropertyName"/>
        /// is empty</exception>
        public ActionScriptPropertyMapping(MemberInfo nativePropertyOrField,
            string asPropertyName, bool isDynamic)
        {
            if (nativePropertyOrField == null)
                throw new ArgumentNullException("nativePropertyOrField");
            if (asPropertyName == null)
                throw new ArgumentNullException("asPropertyName");
            if (! (nativePropertyOrField is FieldInfo) && ! (nativePropertyOrField is PropertyInfo))
                throw new ArgumentException("Must be a property or field reference.", "nativePropertyOrField");
            if (asPropertyName.Length == 0)
                throw new ArgumentException("ActionScript property name must not be empty.", "actionScriptPropertyName");

            this.nativePropertyOrField = nativePropertyOrField;
            this.asPropertyName = asPropertyName;
            this.isDynamic = isDynamic;
        }

        /// <summary>
        /// Gets the native property or field, never null.
        /// Always an instance of <see cref="PropertyInfo" /> or <see cref="FieldInfo" />.
        /// </summary>
        public MemberInfo NativePropertyOrField
        {
            get { return nativePropertyOrField; }
        }

        /// <summary>
        /// Gets the ActionScript property name, never null or empty.
        /// </summary>
        public string ASPropertyName
        {
            get { return asPropertyName; }
        }

        /// <summary>
        /// Gets whether the property should be serialized as a dynamic class member.
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
        public bool IsDynamic
        {
            get { return isDynamic; }
        }
    }
}
