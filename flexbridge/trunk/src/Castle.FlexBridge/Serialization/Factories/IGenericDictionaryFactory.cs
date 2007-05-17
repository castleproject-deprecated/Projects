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

namespace Castle.FlexBridge.Serialization.Factories
{
    /// <summary>
    /// Manufactures instances of <see cref="IDictionary{TKey, TValue}" />.
    /// </summary>
    public interface IGenericDictionaryFactory
    {
        /// <summary>
        /// Returns true if the factory can create an instance of <see cref="IDictionary{TKey, TValue}" />
        /// that is compatible with the specified type.
        /// </summary>
        /// <typeparam name="TKey">The dictionary key type</typeparam>
        /// <typeparam name="TValue">The dictionary value type</typeparam>
        /// <param name="baseType">The type to which the instance must be assignable</param>
        /// <returns>True if the factory can construct such a dictionary</returns>
        bool CanCreateInstance<TKey, TValue>(Type baseType);

        /// <summary>
        /// Constructs a dictionary that is assignable to the specified base type.
        /// </summary>
        /// <typeparam name="TKey">The dictionary key type</typeparam>
        /// <typeparam name="TValue">The dictionary value type</typeparam>
        /// <param name="baseType">The type to which the instance must be assignable</param>
        /// <param name="initialCapacity">The initial capacity of the collection (may be ignored by some implementations)</param>
        /// <returns>The new dictionary instance</returns>
        /// <exception cref="InvalidOperationException">Thrown if the collection cannot be created</exception>
        IDictionary<TKey, TValue> CreateInstance<TKey, TValue>(Type baseType, int initialCapacity);
    }
}