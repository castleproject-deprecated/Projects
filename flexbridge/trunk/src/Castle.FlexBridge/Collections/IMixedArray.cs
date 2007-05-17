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

namespace Castle.FlexBridge.Collections
{
    /// <summary>
    /// A mixed array combines a list of indexed values with a dictionary of dynamic properties.
    /// It is used to deserialize ActionScript arrays that contain both indexed and dynamic properties.
    /// </summary>
    /// <typeparam name="T">The type of value to store in the mixed array</typeparam>
    public interface IMixedArray<T>
    {
        /// <summary>
        /// Gets the zero-based indexed values of the elements in the array.
        /// </summary>
        IList<T> IndexedValues { get; }

        /// <summary>
        /// Gets the non-numerically indexed key/value pairs of the ActionScript array.
        /// </summary>
        /// <returns>The properties of the ActionScript object</returns>
        IDictionary<string, T> DynamicProperties { get; }

        /// <summary>
        /// Gets or sets an indexed value.
        /// </summary>
        /// <param name="index">The index</param>
        /// <returns>The value</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the index is out of bounds</exception>
        T this[int index] { get; set; }

        /// <summary>
        /// Gets or sets a dynamic property value.
        /// </summary>
        /// <param name="key">The dynamic property key</param>
        /// <returns>The value</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="key"/> is null</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the dynamic property key was not found during a lookup</exception>
        /// <exception cref="NotSupportedException">Thrown if the value could not be set because the dynamic property dictionary is readonly</exception>
        T this[string key] { get; set; }
    }
}