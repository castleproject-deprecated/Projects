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
    /// Constructs instances of <see cref="List{T}" />.
    /// </summary>
    public sealed class GenericListFactory : IGenericCollectionFactory
    {
        /// <summary>
        /// Gets the singleton instance of the factory.
        /// </summary>
        public static readonly GenericListFactory Instance = new GenericListFactory();

        private GenericListFactory()
        {
        }

        public bool CanCreateInstance<T>(Type baseType)
        {
            return baseType.IsAssignableFrom(typeof(List<T>));
        }

        public ICollection<T> CreateInstance<T>(Type baseType, int initialCapacity)
        {
            return new List<T>(initialCapacity);
        }
    }
}
