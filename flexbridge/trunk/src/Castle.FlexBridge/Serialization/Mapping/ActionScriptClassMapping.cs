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
using Castle.FlexBridge.ActionScript;

namespace Castle.FlexBridge.Serialization.Mapping
{
    /// <summary>
    /// Describes how instances of a <see cref="Type" /> should be mapped
    /// to and from ActionScript classes.
    /// </summary>
    public sealed class ActionScriptClassMapping
    {
        private Type nativeType;
        private ASClass asClass;
        private ICollection<ActionScriptPropertyMapping> properties;

        /// <summary>
        /// Creates an ActionScript class mapping.
        /// </summary>
        /// <param name="nativeType">The mapped native type</param>
        /// <param name="asClass">The mapped ActionScript class</param>
        /// <param name="properties">The property mappings</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="nativeType"/>,
        /// <paramref name="asClass"/> or <paramref name="properties"/> is null</exception>
        public ActionScriptClassMapping(Type nativeType, ASClass asClass, ICollection<ActionScriptPropertyMapping> properties)
        {
            if (nativeType == null)
                throw new ArgumentNullException("nativeType");
            if (asClass == null)
                throw new ArgumentNullException("asClass");
            if (properties == null)
                throw new ArgumentNullException("properties");

            this.nativeType = nativeType;
            this.asClass = asClass;
            this.properties = properties;
        }

        /// <summary>
        /// Gets the mapped native type.
        /// </summary>
        public Type NativeType
        {
            get { return nativeType; }
        }

        /// <summary>
        /// Gets the mapped ActionScript class.
        /// </summary>
        public ASClass ASClass
        {
            get { return asClass; }
        }

        /// <summary>
        /// Gets the property mappings.
        /// </summary>
        public ICollection<ActionScriptPropertyMapping> Properties
        {
            get { return properties; }
        }
    }
}
