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
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Collections;
using MbUnit.Framework;

namespace Castle.FlexBridge.Tests.UnitTests.ActionScript
{
    [TestFixture]
    [TestsOn(typeof(ASArray))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class ASArrayTest
    {
        [Test]
        public void HasSingletonEmptyArray()
        {
            Assert.AreEqual(0, ASArray.Empty.IndexedValues.Count);
            Assert.IsTrue(ASArray.Empty.IndexedValues.IsReadOnly);

            Assert.AreEqual(0, ASArray.Empty.DynamicProperties.Count);
            Assert.IsTrue(ASArray.Empty.DynamicProperties.IsReadOnly);
        }

        [Test]
        public void ConstructorWithLength()
        {
            ASArray array = new ASArray(10);

            Assert.AreEqual(10, array.IndexedValues.Count);
            Assert.AreEqual(0, array.DynamicProperties.Count);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorWithLengthThrowsIfNegative()
        {
            new ASArray(-1);
        }

        [Test]
        public void ConstructorWithIndexedValues()
        {
            IASValue[] indexedValues = new IASValue[] { null, null, null };
            ASArray array = new ASArray(indexedValues);

            Assert.AreSame(indexedValues, array.IndexedValues);
            Assert.AreEqual(0, array.DynamicProperties.Count);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithIndexedValuesThrowsIfNull()
        {
            new ASArray(null);
        }

        [Test]
        public void ConstructorWithIndexedAndMixedValues()
        {
            IASValue[] indexedValues = new IASValue[] { null, null, null };
            IDictionary<string, IASValue> mixedValues = new Dictionary<string, IASValue>();

            ASArray array = new ASArray(indexedValues, mixedValues);

            Assert.AreSame(indexedValues, array.IndexedValues);
            Assert.AreSame(mixedValues, array.DynamicProperties);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithIndexedAndMixedValuesThrowsIfIndexedValuesNull()
        {
            new ASArray(null, EmptyDictionary<string, IASValue>.Instance);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithIndexedAndMixedValuesThrowsIfMixedValuesNull()
        {
            new ASArray(EmptyArray<IASValue>.Instance, null);
        }

        [Test]
        public void IndexedValueIndexer()
        {
            ASArray array = new ASArray(new IASValue[] { null, new ASString("abc"), null });

            Assert.AreEqual(new ASString("abc"), array[1]);
            array[1] = new ASString("def");
            Assert.AreEqual(new ASString("def"), array[1]);
        }

        [RowTest]
        [Row(-1, ExpectedException=typeof(ArgumentOutOfRangeException))]
        [Row(3, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void IndexedValueGetIndexerThrowsIfOutOfRange(int index)
        {
            ASArray array = new ASArray(new IASValue[] { null, null, null });
            GC.KeepAlive(array[index]);
        }

        [RowTest]
        [Row(-1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(3, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void IndexedValueSetIndexerThrowsIfOutOfRange(int index)
        {
            ASArray array = new ASArray(new IASValue[] { null, null, null });
            array[index] = new ASString("abc");
        }

        [Test]
        public void MixedValueIndexer()
        {
            ASArray array = new ASArray(0);

            array["abc"] = new ASInt29(1);
            Assert.AreEqual(new ASInt29(1), array["abc"]);

            // do it twice to be sure
            array["abc"] = new ASInt29(2);
            Assert.AreEqual(new ASInt29(2), array["abc"]);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MixedValueGetIndexerThrowsIfNull()
        {
            GC.KeepAlive(ASArray.Empty[null]);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void MixedValueSetIndexerThrowsIfNull()
        {
            ASArray.Empty[null] = new ASInt29(42);
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void MixedValueGetIndexerThrowsIfAbsent()
        {
            GC.KeepAlive(ASArray.Empty["abc"]);
        }

        [Test]
        public void CreateUninitializedInstanceAndSetProperties()
        {
            ASArray array = ASArray.CreateUninitializedInstance();
            array.SetProperties(EmptyArray<IASValue>.Instance, EmptyDictionary<string, IASValue>.Instance);

            Assert.AreSame(EmptyArray<IASValue>.Instance, array.IndexedValues);
            Assert.AreSame(EmptyDictionary<string, IASValue>.Instance, array.DynamicProperties);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetPropertiesWithInitializedInstanceThrows()
        {
            ASArray array = new ASArray(1);
            array.SetProperties(EmptyArray<IASValue>.Instance, EmptyDictionary<string, IASValue>.Instance);
        }
    }
}