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
    /// Constructs instances of <see cref="Dictionary{TKey, TValue}" />.
    /// </summary>
    public sealed class GenericDictionaryFactory : IGenericDictionaryFactory
    {
        /// <summary>
        /// Gets the singleton instance of the factory.
        /// </summary>
        public static readonly GenericDictionaryFactory Instance = new GenericDictionaryFactory();

        private GenericDictionaryFactory()
        {
        }

        public bool CanCreateInstance<TKey, TValue>(Type baseType)
        {
            return baseType.IsAssignableFrom(typeof(Dictionary<TKey, TValue>));
        }

        public IDictionary<TKey, TValue> CreateInstance<TKey, TValue>(Type baseType, int initialCapacity)
        {
            return new Dictionary<TKey, TValue>();
        }
    }
}