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

namespace Castle.FlexBridge.Serialization.Factories
{
    /// <summary>
    /// Constructs instances of <see cref="MixedArray{T}" />.
    /// </summary>
    public sealed class MixedArrayFactory : IMixedArrayFactory
    {
        /// <summary>
        /// Gets the singleton instance of the factory.
        /// </summary>
        public static readonly MixedArrayFactory Instance = new MixedArrayFactory();

        private MixedArrayFactory()
        {
        }

        public bool CanCreateInstance<T>(Type baseType)
        {
            return baseType.IsAssignableFrom(typeof(MixedArray<T>));
        }

        public IMixedArray<T> CreateInstance<T>(Type baseType, int initialIndexedValueCapacity,
            int initialDynamicPropertyCapacity)
        {
            return new MixedArray<T>(new List<T>(initialIndexedValueCapacity),
                new Dictionary<string, T>(initialDynamicPropertyCapacity));
        }
    }
}