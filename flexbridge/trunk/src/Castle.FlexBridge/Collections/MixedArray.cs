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

namespace Castle.FlexBridge.Collections
{
    /// <summary>
    /// A mixed array combines a list of indexed values with a dictionary of dynamic properties.
    /// It is used to deserialize ActionScript arrays that contain both indexed and dynamic properties.
    /// </summary>
    /// <remarks>
    /// This implementation of <see cref="IMixedArray{T}" /> uses <see cref="List{T}" /> and
    /// <see cref="Dictionary{TKey, TValue}" /> as its default inner collections but any user-supplied
    /// collections may be used instead.
    /// </remarks>
    /// <typeparam name="T">The type of value to store in the mixed array</typeparam>
    public class MixedArray<T> : IMixedArray<T>
    {
        private IList<T> indexedValues;
        private IDictionary<string, T> dynamicProperties;

        /// <summary>
        /// Gets a singleton read-only empty array instance.
        /// </summary>
        public static readonly ASArray Empty = new ASArray(EmptyArray<IASValue>.Instance, EmptyDictionary<string, IASValue>.Instance);

        /// <summary>
        /// Creates an empty mixed list where both the indexed value and dynamic property collections
        /// are mutable and of variable length.
        /// </summary>
        public MixedArray()
        {
            indexedValues = new List<T>();
            dynamicProperties = new Dictionary<string, T>();
        }

        /// <summary>
        /// Creates an array with the specified indexed values and dynamic properties.
        /// </summary>
        /// <param name="indexedValues">The indexed values list</param>
        /// <param name="dynamicProperties">The dynamic properties dictionary</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="indexedValues"/> or
        /// <paramref name="dynamicProperties"/> is null</exception>
        public MixedArray(IList<T> indexedValues, IDictionary<string, T> dynamicProperties)
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
        public IList<T> IndexedValues
        {
            get
            {
                return indexedValues;
            }
        }

        /// <summary>
        /// Gets the non-numerically indexed key/value pairs of the ActionScript array.
        /// </summary>
        /// <returns>The properties of the ActionScript object</returns>
        public IDictionary<string, T> DynamicProperties
        {
            get
            {
                return dynamicProperties;
            }
        }

        /// <summary>
        /// Gets or sets an indexed value.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The value</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of bounds</exception>
        public T this[int index]
        {
            get
            {
                return indexedValues[index];
            }
            set
            {
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
        public T this[string key]
        {
            get
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                return dynamicProperties[key];
            }
            set
            {
                if (key == null)
                    throw new ArgumentNullException("key");

                dynamicProperties[key] = value;
            }
        }
    }
}
