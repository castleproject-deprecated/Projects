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
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.ActionScript
{
    /// <summary>
    /// Adapts a native array-like object for ActionScript serialization.
    /// The object is mapped to ActionScript indexed values and dynamic properties on demand.
    /// </summary>
    /// <remarks>
    /// The mapping itself is not cached as it would tend to increase memory pressure and
    /// unnecessarily degrade performance in the common case where an instance of
    /// <see cref="ASNativeArray" /> is only used once.
    /// </remarks>
    public sealed class ASNativeArray : BaseASValue
    {
        private object nativeArray;
        private IASNativeArrayMapper mapper;
        private bool isByteArray;

        /// <summary>
        /// Creates an uninitialized object.
        /// </summary>
        /// <param name="isByteArray">Set to true if the array should be mapped as a byte array</param>
        private ASNativeArray(bool isByteArray)
        {
            this.isByteArray = isByteArray;
        }

        /// <summary>
        /// Creates a wrapper for a native array-like value.
        /// </summary>
        /// <param name="nativeArray">The native array-like object</param>
        /// <param name="mapper">The mapper to use for mapping the array to ActionScript on demand</param>
        /// <param name="isByteArray">Set to true if the array should be mapped as a byte array</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nativeArray"/>
        /// or <paramref name="mapper"/> is null</exception>
        public ASNativeArray(object nativeArray, IASNativeArrayMapper mapper, bool isByteArray)
        {
            if (nativeArray == null)
                throw new ArgumentNullException("nativeArray");
            if (mapper == null)
                throw new ArgumentNullException("mapper");

            this.nativeArray = nativeArray;
            this.mapper = mapper;
            this.isByteArray = isByteArray;
        }

        /// <summary>
        /// Gets the native array-like object, never null.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the object has not been initialized</exception>
        public object NativeArray
        {
            get
            {
                ThrowIfNotInitialized();
                return nativeArray;
            }
        }

        /// <summary>
        /// Creates an instance whose initialization is to be deferred.
        /// </summary>
        /// <remarks>
        /// This special case is used to resolve circular references during the construction of
        /// object graphs.  The object should not be used until its properties have been initialized.
        /// </remarks>
        /// <param name="isByteArray">Set to true if the array should be mapped as a byte array</param>
        /// <returns>The uninitialized object</returns>
        public static ASNativeArray CreateUninitializedInstance(bool isByteArray)
        {
            return new ASNativeArray(isByteArray);
        }

        /// <summary>
        /// Sets the properties of an instance created by <see cref="CreateUninitializedInstance" />.
        /// </summary>
        /// <remarks>
        /// This special case is used to resolve circular references during the construction of
        /// object graphs.  The object should not be used until its properties have been initialized.
        /// </remarks>
        /// <param name="nativeArray">The native array-like object</param>
        /// <param name="mapper">The mapper to use for mapping the array to ActionScript on demand</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nativeArray"/> or <paramref name="mapper"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the object has already been initialized</exception>
        public void SetProperties(object nativeArray, IASNativeArrayMapper mapper)
        {
            if (IsInitialized)
                throw new InvalidOperationException("The object's properties may not be set once initialized.");

            if (nativeArray == null)
                throw new ArgumentNullException("nativeArray");
            if (mapper == null)
                throw new ArgumentNullException("mapper");

            this.nativeArray = nativeArray;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public override bool IsInitialized
        {
            get { return nativeArray != null; }
        }

        /// <inheritdoc />
        public override ASTypeKind Kind
        {
            get { return isByteArray ? ASTypeKind.ByteArray : ASTypeKind.Array; }
        }

        /// <inheritdoc />
        public override ASValueContentFlags ContentFlags
        {
            get
            {
                ThrowIfNotInitialized();
                return mapper.GetContentFlags(nativeArray);
            }
        }

        /// <inheritdoc />
        public override void AcceptVisitor(IActionScriptSerializer serializer, IASValueVisitor visitor)
        {
            ThrowIfNotInitialized();

            mapper.AcceptVisitor(serializer, nativeArray, visitor);
        }

        /// <inheritdoc />
        public override object GetNativeValue(Type nativeType)
        {
            if (IsInitialized)
            {
                if (nativeType.IsInstanceOfType(nativeArray))
                    return nativeArray;
            }

            return base.GetNativeValue(nativeType);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!IsInitialized)
                return this == obj;

            IASValue other = obj as IASValue;
            return other != null && nativeArray.Equals(other.GetNativeValue(null));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            if (!IsInitialized)
                return 0;

            return nativeArray.GetHashCode();
        }
    }
}