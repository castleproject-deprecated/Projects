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
using Castle.FlexBridge.ActionScript;

namespace Castle.FlexBridge.Serialization.Mapping
{
    /// <summary>
    /// An ActionScript target mapping descriptor specifies constraints on a mapping from native
    /// .Net values to their corresponding
    /// <see cref="IASValue" /> representations.  Used by <see cref="IASMapperFactory" /> to
    /// obtain suitable <see cref="IASTargetMapper" /> instances on demand.
    /// </summary>
    public struct ASTargetMappingDescriptor : IEquatable<ASTargetMappingDescriptor>
    {
        private Type sourceNativeType;

        /// <summary>
        /// Initializes an ActionScript target mapping descriptor.
        /// </summary>
        /// <param name="sourceNativeType">The type of native objects from which values are mapped</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="sourceNativeType"/> is null</exception>
        public ASTargetMappingDescriptor(Type sourceNativeType)
        {
            if (sourceNativeType == null)
                throw new ArgumentNullException("sourceNativeType");

            this.sourceNativeType = sourceNativeType;
        }

        /// <summary>
        /// Gets the type of native objects from which values are mapped.
        /// </summary>
        public Type SourceNativeType
        {
            get { return sourceNativeType; }
        }

        public override int GetHashCode()
        {
            return sourceNativeType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is ASTargetMappingDescriptor && Equals((ASTargetMappingDescriptor)obj);
        }

        public bool Equals(ASTargetMappingDescriptor other)
        {
            return sourceNativeType == other.sourceNativeType;
        }
    }
}