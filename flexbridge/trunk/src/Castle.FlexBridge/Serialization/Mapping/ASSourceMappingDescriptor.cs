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
    /// An ActionScript target mapping descriptor specifies constraints on a mapping
    /// from <see cref="IASValue" /> values to their corresponding
    /// native .Net representations.  Used by <see cref="IASMapperFactory" /> to
    /// obtain suitable <see cref="IASSourceMapper" /> instances on demand.
    /// </summary>
    public struct ASSourceMappingDescriptor : IEquatable<ASSourceMappingDescriptor>
    {
        private ASTypeKind sourceKind;
        private string sourceClassAlias;
        private ASValueContentFlags sourceContentFlags;
        private Type targetNativeType;

        /// <summary>
        /// Initializes an ActionScript source mapping descriptor.
        /// </summary>
        /// <param name="sourceKind">The source value kind</param>
        /// <param name="sourceClassAlias">The source class alias, may be empty but never null</param>
        /// <param name="targetNativeType">The type of native objects to which values are mapped, never null</param>
        /// <param name="sourceContentFlags">Flags that describe abstract properties of the contents of the source value</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="targetNativeType"/> or
        /// <see cref="sourceClassAlias" /> is null</exception>
        public ASSourceMappingDescriptor(ASTypeKind sourceKind, string sourceClassAlias, ASValueContentFlags sourceContentFlags,
            Type targetNativeType)
        {
            if (targetNativeType == null)
                throw new ArgumentNullException("sourceNativeType");
            if (sourceClassAlias == null)
                throw new ArgumentNullException("sourceClassAlias");

            this.sourceKind = sourceKind;
            this.sourceClassAlias = sourceClassAlias;
            this.sourceContentFlags = sourceContentFlags;
            this.targetNativeType = targetNativeType;
        }

        /// <summary>
        /// Gets the source value kind.
        /// </summary>
        public ASTypeKind SourceKind
        {
            get { return sourceKind; }
        }

        /// <summary>
        /// Gets the source class alias.
        /// </summary>
        public string SourceClassAlias
        {
            get { return sourceClassAlias; }
        }

        /// <summary>
        /// Gets flags that describe abstract properties of the contents of the source value
        /// </summary>
        public ASValueContentFlags SourceContentFlags
        {
            get { return sourceContentFlags; }
        }

        /// <summary>
        /// Gets the type of native objects to which values are mapped.
        /// </summary>
        public Type TargetNativeType
        {
            get { return targetNativeType; }
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return sourceKind.GetHashCode() ^ sourceClassAlias.GetHashCode() ^ sourceContentFlags.GetHashCode() ^ targetNativeType.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is ASSourceMappingDescriptor && Equals((ASSourceMappingDescriptor)obj);
        }

        /// <inheritdoc />
        public bool Equals(ASSourceMappingDescriptor other)
        {
            return sourceKind == other.sourceKind
                && sourceClassAlias == other.sourceClassAlias
                && sourceContentFlags == other.sourceContentFlags
                && targetNativeType == other.targetNativeType;
        }
    }
}
