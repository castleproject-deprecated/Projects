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
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.FlexBridge.Collections;

namespace Castle.FlexBridge.Collections
{
    /// <summary>
    /// A read-only empty dictionary.
    /// </summary>
    /// <typeparam name="TKey">The dictionary key type</typeparam>
    /// <typeparam name="TValue">The dictionary value type</typeparam>
    public class EmptyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        /// <summary>
        /// A read-only empty dictionary instance.
        /// </summary>
        public static readonly IDictionary<TKey, TValue> Instance =new EmptyDictionary<TKey, TValue>();

        public bool ContainsKey(TKey key)
        {
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            ThrowCollectionIsReadOnly();
        }

        public bool Remove(TKey key)
        {
            ThrowCollectionIsReadOnly();
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            value = default(TValue);
            return false;
        }

        public TValue this[TKey key]
        {
            get { throw new KeyNotFoundException(); }
            set { ThrowCollectionIsReadOnly(); }
        }

        public ICollection<TKey> Keys
        {
            get { return EmptyArray<TKey>.Instance; }
        }

        public ICollection<TValue> Values
        {
            get { return EmptyArray<TValue>.Instance; }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            ThrowCollectionIsReadOnly();
        }

        public void Clear()
        {
            ThrowCollectionIsReadOnly();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            ThrowCollectionIsReadOnly();
            return false;
        }

        public int Count
        {
            get { return 0; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            yield break;
        }

        public IEnumerator GetEnumerator()
        {
            yield break;
        }

        private static void ThrowCollectionIsReadOnly()
        {
            throw new NotSupportedException("Dictionary is read-only.");
        }
    }
}