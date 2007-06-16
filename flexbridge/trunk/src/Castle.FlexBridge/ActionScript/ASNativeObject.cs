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
    /// Adapts a native object for ActionScript serialization.
    /// The object is mapped to ActionScript members and dynamic properties on demand.
    /// </summary>
    /// <remarks>
    /// The mapping itself is not cached as it would tend to increase memory pressure and
    /// unnecessarily degrade performance in the common case where an instance of
    /// <see cref="ASNativeObject" /> is only used once.
    /// </remarks>
    public sealed class ASNativeObject : BaseASValue
    {
        private ASClass @class;
        private object nativeObject;
        private IASNativeObjectMapper mapper;

        /// <summary>
        /// Creates an uninitialized object.
        /// </summary>
        /// <param name="class">The ActionScript class</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="class"/> is null</exception>
        private ASNativeObject(ASClass @class)
        {
            if (@class == null)
                throw new ArgumentNullException("class");

            this.@class = @class;
        }

        /// <summary>
        /// Creates a wrapper for a native value.
        /// </summary>
        /// <param name="class">The ActionScript class</param>
        /// <param name="nativeObject">The native object</param>
        /// <param name="mapper">The mapper to use for mapping the object to ActionScript on demand</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="class"/>, <paramref name="nativeObject"/>,
        /// or <paramref name="mapper"/> is null</exception>
        public ASNativeObject(ASClass @class, object nativeObject, IASNativeObjectMapper mapper)
        {
            if (@class == null)
                throw new ArgumentNullException("class");
            if (nativeObject == null)
                throw new ArgumentNullException("nativeObject");
            if (mapper == null)
                throw new ArgumentNullException("mapper");

            this.@class = @class;
            this.nativeObject = nativeObject;
            this.mapper = mapper;
        }

        /// <summary>
        /// Gets the ActionScript class, never null.
        /// </summary>
        public override ASClass Class
        {
            get { return @class; }
        }

        /// <summary>
        /// Gets the native object, never null.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the object has not been initialized</exception>
        public object NativeObject
        {
            get
            {
                ThrowIfNotInitialized();
                return nativeObject;
            }
        }

        /// <summary>
        /// Creates an instance whose initialization is to be deferred.
        /// </summary>
        /// <remarks>
        /// This special case is used to resolve circular references during the construction of
        /// object graphs.  The object should not be used until its properties have been initialized.
        /// </remarks>
        /// <param name="class">The ActionScript class</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="class"/> is null</exception>
        /// <returns>The uninitialized object</returns>
        public static ASNativeObject CreateUninitializedInstance(ASClass @class)
        {
            return new ASNativeObject(@class);
        }

        /// <summary>
        /// Sets the properties of an instance created by <see cref="CreateUninitializedInstance" />.
        /// </summary>
        /// <remarks>
        /// This special case is used to resolve circular references during the construction of
        /// object graphs.  The object should not be used until its properties have been initialized.
        /// </remarks>
        /// <param name="nativeObject">The native object</param>
        /// <param name="mapper">The mapper to use for mapping the object to ActionScript on demand</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="nativeObject"/> or <paramref name="mapper"/> is null</exception>
        /// <exception cref="InvalidOperationException">Thrown if the object has already been initialized</exception>
        public void SetProperties(object nativeObject, IASNativeObjectMapper mapper)
        {
            if (IsInitialized)
                throw new InvalidOperationException("The object's properties may not be set once initialized.");

            if (nativeObject == null)
                throw new ArgumentNullException("nativeObject");
            if (mapper == null)
                throw new ArgumentNullException("mapper");

            this.nativeObject = nativeObject;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public override bool IsInitialized
        {
            get { return nativeObject != null; }
        }

        /// <inheritdoc />
        public override ASTypeKind Kind
        {
            get { return ASTypeKind.Object; }
        }

        /// <inheritdoc />
        public override ASValueContentFlags ContentFlags
        {
            get
            {
                ThrowIfNotInitialized();
                return mapper.GetContentFlags(nativeObject);
            }
        }

        /// <inheritdoc />
        public override void AcceptVisitor(IActionScriptSerializer serializer, IASValueVisitor visitor)
        {
            ThrowIfNotInitialized();

            mapper.AcceptVisitor(serializer, @class, nativeObject, visitor);
        }

        /// <inheritdoc />
        public override object GetNativeValue(Type nativeType)
        {
            if (IsInitialized)
            {
                if (nativeType.IsInstanceOfType(nativeObject))
                    return nativeObject;
            }

            return base.GetNativeValue(nativeType);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!IsInitialized)
                return this == obj;

            IASValue other = obj as IASValue;
            return other != null && nativeObject.Equals(other.GetNativeValue(null));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            if (!IsInitialized)
                return 0;

            return nativeObject.GetHashCode();
        }
    }
}