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

namespace Castle.FlexBridge.Serialization.Factories
{
    /// <summary>
    /// Manufactures instances of <see cref="ICollection{T}" />.
    /// </summary>
    public interface IGenericCollectionFactory
    {
        /// <summary>
        /// Returns true if the factory can create an instance of <see cref="ICollection{T}" />
        /// that is compatible with the specified type.
        /// </summary>
        /// <typeparam name="T">The collection value type</typeparam>
        /// <param name="baseType">The type to which the instance must be assignable</param>
        /// <returns>True if the factory can construct such a collection</returns>
        bool CanCreateInstance<T>(Type baseType);

        /// <summary>
        /// Constructs a collection that is assignable to the specified base type.
        /// </summary>
        /// <typeparam name="T">The collection value type</typeparam>
        /// <param name="baseType">The type to which the instance must be assignable</param>
        /// <param name="initialCapacity">The initial capacity of the collection (may be ignored by some implementations)</param>
        /// <returns>The new collection instance</returns>
        /// <exception cref="InvalidOperationException">Thrown if the collection cannot be created</exception>
        ICollection<T> CreateInstance<T>(Type baseType, int initialCapacity);
    }
}
