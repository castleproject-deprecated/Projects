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
using Castle.FlexBridge.Collections;
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.ActionScript
{
    /// <summary>
    /// Adapts an <see cref="IExternalizable" /> object for ActionScript serialization.
    /// </summary>
    public sealed class ASExternalizableObject : BaseASValue
    {
        private ASClass @class;
        private IExternalizable externalizableValue;

        /// <summary>
        /// Creates an uninitialized object.
        /// </summary>
        /// <param name="class">The ActionScript class</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="class"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown when the class layout of <paramref name="class"/> is
        /// not <see cref="ASClassLayout.Externalizable" /></exception>
        private ASExternalizableObject(ASClass @class)
        {
            if (@class == null)
                throw new ArgumentNullException("class");
            if (@class.Layout != ASClassLayout.Externalizable)
                throw new ArgumentException("The class layout must be Externalizable.", "class");

            this.@class = @class;
        }

        /// <summary>
        /// Creates an object with the specified class and externalizable value.
        /// </summary>
        /// <param name="class">The ActionScript class</param>
        /// <param name="externalizableValue">The externalizable value</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="class"/> or <paramref name="externalizableValue"/>
        /// is null</exception>
        /// <exception cref="ArgumentException">Thrown when the class layout of <paramref name="class"/> is
        /// not <see cref="ASClassLayout.Externalizable" /></exception>
        public ASExternalizableObject(ASClass @class, IExternalizable externalizableValue)
        {
            if (@class == null)
                throw new ArgumentNullException("class");
            if (externalizableValue == null)
                throw new ArgumentNullException("externalizableValue");
            if (@class.Layout != ASClassLayout.Externalizable)
                throw new ArgumentException("The class layout must be Externalizable.", "class");

            this.@class = @class;
            this.externalizableValue = externalizableValue;
        }

        /// <summary>
        /// Gets the ActionScript class, never null.
        /// </summary>
        public override ASClass Class
        {
            get { return @class; }
        }

        /// <summary>
        /// Gets the externalizable value, never null.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the object has not been initialized</exception>
        public IExternalizable ExternalizableValue
        {
            get
            {
                ThrowIfNotInitialized();
                return externalizableValue;
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
        /// <exception cref="ArgumentException">Thrown when the class layout of <paramref name="class"/> is
        /// not <see cref="ASClassLayout.Externalizable" /></exception>
        /// <returns>The uninitialized object</returns>
        public static ASExternalizableObject CreateUninitializedInstance(ASClass @class)
        {
            return new ASExternalizableObject(@class);
        }

        /// <summary>
        /// Sets the properties of an instance created by <see cref="CreateUninitializedInstance" />.
        /// </summary>
        /// <remarks>
        /// This special case is used to resolve circular references during the construction of
        /// object graphs.  The object should not be used until its properties have been initialized.
        /// </remarks>
        /// <param name="externalizableValue">The externalizable value</param>
        /// <exception cref="InvalidOperationException">Thrown if the object has already been initialized</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="externalizableValue"/> is null</exception>
        public void SetProperties(IExternalizable externalizableValue)
        {
            if (IsInitialized)
                throw new InvalidOperationException("The object's properties may not be set once initialized.");

            if (externalizableValue == null)
                throw new ArgumentNullException("externalizableValue");

            this.externalizableValue = externalizableValue;
        }

        /// <inheritdoc />
        public override ASTypeKind Kind
        {
            get { return ASTypeKind.Object; }
        }

        /// <inheritdoc />
        public override bool IsInitialized
        {
            get { return externalizableValue != null; }
        }

        /// <inheritdoc />
        public override void AcceptVisitor(IActionScriptSerializer serializer, IASValueVisitor visitor)
        {
            ThrowIfNotInitialized();

            visitor.VisitObject(serializer, @class,
                EmptyArray<IASValue>.Instance, EmptyDictionary<string, IASValue>.Instance,
                externalizableValue);
        }

        /// <inheritdoc />
        public override object GetNativeValue(Type nativeType)
        {
            if (IsInitialized)
            {
                if (nativeType.IsInstanceOfType(externalizableValue))
                    return externalizableValue;
            }

            return base.GetNativeValue(nativeType);
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (! IsInitialized)
                return this == obj;

            IASValue other = obj as IASValue;
            return other != null && externalizableValue.Equals(other.GetNativeValue(null));
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            if (!IsInitialized)
                return 0;

            return externalizableValue.GetHashCode();
        }
    }
}
