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
using Castle.FlexBridge.Collections;
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.ActionScript
{
    /// <summary>
    /// Adapts a list and dictionary for ActionScript serializable, including
    /// support for arrays with dynamic properties with non-integer keys.
    /// </summary>
    public sealed class ASArray : BaseASValue, IMixedArray<IASValue>
    {
        private IList<IASValue> indexedValues;
        private IDictionary<string, IASValue> dynamicProperties;

        /// <summary>
        /// Gets a singleton read-only empty array instance.
        /// </summary>
        public static readonly ASArray Empty = new ASArray(EmptyArray<IASValue>.Instance, EmptyDictionary<string, IASValue>.Instance);

        /// <summary>
        /// Creates an uninitialized array.
        /// </summary>
        private ASArray()
        {
        }

        /// <summary>
        /// Creates an array with the specified indexed length and no dynamic properties initially
        /// where the length of the indexed values list is fixed but the dynamic properties collection is mutable.
        /// </summary>
        /// <param name="indexedValuesLength">The length of the indexed values list to create</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="indexedValuesLength"/>
        /// is negative</exception>
        public ASArray(int indexedValuesLength)
        {
            if (indexedValuesLength < 0)
                throw new ArgumentOutOfRangeException("indexedValuesLength", "Indexed values length must be non-negative.");

            indexedValues = new IASValue[indexedValuesLength];
            dynamicProperties = new Dictionary<string, IASValue>();
        }

        /// <summary>
        /// Creates an array with the specified indexed values and no dynamic properties initially.
        /// </summary>
        /// <param name="indexedValues">The indexed values list</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="indexedValues"/> is null</exception>
        public ASArray(IList<IASValue> indexedValues)
            : this(indexedValues, new Dictionary<string, IASValue>())
        {
        }

        /// <summary>
        /// Creates an array with the specified indexed values and dynamic properties.
        /// </summary>
        /// <param name="indexedValues">The indexed values list</param>
        /// <param name="dynamicProperties">The dynamic properties dictionary</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="indexedValues"/> or
        /// <paramref name="dynamicProperties"/> is null</exception>
        public ASArray(IList<IASValue> indexedValues, IDictionary<string, IASValue> dynamicProperties)
        {
            if (indexedValues == null)
                throw new ArgumentNullException("indexedValues");
            if (dynamicProperties == null)
                throw new ArgumentNullException("dynamicProperties");

            this.indexedValues = indexedValues;
            this.dynamicProperties = dynamicProperties;
        }

        /// <summary>
        /// Gets the zero-based indexed values of the elements in the array.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the object has not been initialized</exception>
        public IList<IASValue> IndexedValues
        {
            get
            {
                ThrowIfNotInitialized();
                return indexedValues;
            }
        }

        /// <summary>
        /// Gets the non-numerically indexed key/value pairs of the ActionScript array.
        /// </summary>
        /// <returns>The properties of the ActionScript object</returns>
        /// <exception cref="InvalidOperationException">Thrown if the object has not been initialized</exception>
        public IDictionary<string, IASValue> DynamicProperties
        {
            get
            {
                ThrowIfNotInitialized();
                return dynamicProperties;
            }
        }

        /// <summary>
        /// Gets or sets an indexed value.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The value</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of bounds</exception>
        /// <exception cref="InvalidOperationException">Thrown if the object has not been initialized</exception>
        public IASValue this[int index]
        {
            get
            {
                ThrowIfNotInitialized();
                return indexedValues[index];
            }
            set
            {
                ThrowIfNotInitialized();
                indexedValues[index] = value;
            }
        }

        /// <summary>
        /// Gets or sets a dynamic property value.
        /// </summary>
        /// <param name="key">The dynamic property key</param>
        /// <returns>The value</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the dynamic property key was not found during a lookup</exception>
        /// <exception cref="NotSupportedException">Thrown if the value could not be set because the dynamic property dictionary is readonly</exception>
        /// <exception cref="InvalidOperationException">Thrown if the object has not been initialized</exception>
        public IASValue this[string key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                ThrowIfNotInitialized();
                return dynamicProperties[key];
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                ThrowIfNotInitialized();
                dynamicProperties[key] = value;
            }
        }

        /// <summary>
        /// Creates an instance whose initialization is to be deferred.
        /// </summary>
        /// <remarks>
        /// This special case is used to resolve circular references during the construction of
        /// object graphs.  The object should not be used until its properties have been initialized.
        /// </remarks>
        /// <returns>The uninitialized array</returns>
        public static ASArray CreateUninitializedInstance()
        {
            return new ASArray();
        }

        /// <summary>
        /// Sets the properties of an instance created by <see cref="CreateUninitializedInstance" />.
        /// </summary>
        /// <remarks>
        /// This special case is used to resolve circular references during the construction of
        /// object graphs.  The object should not be used until its properties have been initialized.
        /// </remarks>
        /// <param name="indexedValues">The indexed values list</param>
        /// <param name="dynamicProperties">The mixed values dictionary</param>
        /// <exception cref="InvalidOperationException">Thrown if the object has already been initialized</exception>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="indexedValues"/> or
        /// <paramref name="dynamicProperties"/> is null</exception>
        public void SetProperties(IList<IASValue> indexedValues, IDictionary<string, IASValue> dynamicProperties)
        {
            if (IsInitialized)
                throw new InvalidOperationException("The object's properties may not be set once initialized.");

            if (indexedValues == null)
                throw new ArgumentNullException("indexedValues");
            if (dynamicProperties == null)
                throw new ArgumentNullException("dynamicProperties");

            this.indexedValues = indexedValues;
            this.dynamicProperties = dynamicProperties;
        }

        public override bool IsInitialized
        {
            get { return indexedValues != null; }
        }

        public override ASTypeKind Kind
        {
            get { return ASTypeKind.Array; }
        }

        public override ASValueContentFlags ContentFlags
        {
            get
            {
                ThrowIfNotInitialized();

                ASValueContentFlags flags = ASValueContentFlags.None;
                if (indexedValues.Count != 0)
                    flags |= ASValueContentFlags.HasIndexedValues;
                if (dynamicProperties.Count != 0)
                    flags |= ASValueContentFlags.HasDynamicProperties;

                return flags;
            }
        }

        public override void AcceptVisitor(IActionScriptSerializer serializer, IASValueVisitor visitor)
        {
            ThrowIfNotInitialized();
            visitor.VisitArray(serializer, indexedValues.Count, indexedValues, dynamicProperties);
        }

        public override object GetNativeValue(Type nativeType)
        {
            if (IsInitialized)
            {
                // Note: We can only return the inner contents of this object when we're
                //       certain we won't want to apply any mapping to its elements at all.
                //       It's a pretty safe bet that this is the case if the native type
                //       is an array or list type directly involving IASValue but otherwise
                //       we can't really be sure.  So we check to make sure the native type
                //       really involves IASValue -- it can't be just "object" or something.
                if (dynamicProperties.Count == 0)
                {
                    if (nativeType.IsInstanceOfType(indexedValues)
                        && typeof(IList<IASValue>).IsAssignableFrom(nativeType))
                        return indexedValues;
                }
                else if (indexedValues.Count == 0)
                {
                    if (nativeType.IsInstanceOfType(dynamicProperties)
                        && typeof(IDictionary<string, IASValue>).IsAssignableFrom(nativeType))
                        return dynamicProperties;
                }
            }

            return base.GetNativeValue(nativeType);
        }
    }
}
