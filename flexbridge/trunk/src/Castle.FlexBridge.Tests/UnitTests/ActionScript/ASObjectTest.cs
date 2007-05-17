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
    [TestsOn(typeof(ASObject))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class ASObjectTest
    {
        [Test]
        public void HasSingletonEmptyObject()
        {
            Assert.AreSame(ASClass.UntypedDynamicClass, ASObject.Empty.Class);
            Assert.AreEqual(0, ASObject.Empty.MemberValues.Count);
            Assert.IsTrue(ASObject.Empty.MemberValues.IsReadOnly);
            Assert.AreEqual(0, ASObject.Empty.DynamicProperties.Count);
            Assert.IsTrue(ASObject.Empty.DynamicProperties.IsReadOnly);
        }

        [Test]
        public void DefaultConstructor()
        {
            ASObject obj = new ASObject();

            Assert.AreEqual(0, obj.MemberValues.Count);
            Assert.IsTrue(obj.MemberValues.IsReadOnly);

            Assert.AreEqual(0, obj.DynamicProperties.Count);
            Assert.IsFalse(obj.DynamicProperties.IsReadOnly);
        }

        [RowTest]
        [Row(ASClassLayout.Normal, new string[] { "member" })]
        [Row(ASClassLayout.Dynamic, new string[] { "member" })]
        [Row(ASClassLayout.Normal, new string[] { })]
        [Row(ASClassLayout.Dynamic, new string[] { })]
        [Row(ASClassLayout.Externalizable, new string[] { }, ExpectedException = typeof(ArgumentException))]
        public void ConstructorWithClass(ASClassLayout classLayout, string[] memberNames)
        {
            ASClass @class = new ASClass("class", classLayout, memberNames);
            ASObject obj = new ASObject(@class);

            Assert.AreSame(@class, obj.Class);
            Assert.AreEqual(memberNames.Length, obj.MemberValues.Count);
            Assert.AreEqual(0, obj.DynamicProperties.Count);

            Assert.AreEqual(classLayout == ASClassLayout.Normal, obj.DynamicProperties.IsReadOnly);
            if (memberNames.Length == 0)
                Assert.IsTrue(obj.MemberValues.IsReadOnly);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithClass_ThrowsIfNullClass()
        {
            new ASObject(null);
        }

        [RowTest]
        [Row(ASClassLayout.Normal, new string[] { "member" }, new int[] { 1 })]
        [Row(ASClassLayout.Dynamic, new string[] { "member" }, new int[] { 1 })]
        [Row(ASClassLayout.Normal, new string[] { }, new int[] { })]
        [Row(ASClassLayout.Dynamic, new string[] { }, new int[] { })]
        [Row(ASClassLayout.Normal, new string[] { "member" }, new int[] { 1, 2, 3 }, ExpectedException = typeof(ArgumentException))]
        [Row(ASClassLayout.Dynamic, new string[] { "member" }, new int[] { 1, 2, 3 }, ExpectedException = typeof(ArgumentException))]
        [Row(ASClassLayout.Externalizable, new string[] { }, new int[] { }, ExpectedException = typeof(ArgumentException))]
        public void ConstructorWithClassAndMemberValues(ASClassLayout classLayout, string[] memberNames, int[] memberValues)
        {
            IASValue[] asMemberValues = WrapInts(memberValues);

            ASClass @class = new ASClass("class", classLayout, memberNames);
            ASObject obj = new ASObject(@class, asMemberValues);

            Assert.AreSame(@class, obj.Class);
            Assert.AreSame(asMemberValues, obj.MemberValues);
            Assert.AreEqual(0, obj.DynamicProperties.Count);

            Assert.AreEqual(classLayout == ASClassLayout.Normal, obj.DynamicProperties.IsReadOnly);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithClassAndMemberValues_ThrowsIfNullClass()
        {
            new ASObject(null, EmptyArray<IASValue>.Instance);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithClassAndMemberValues_ThrowsIfNullMemberValues()
        {
            new ASObject(ASClass.UntypedDynamicClass, null);
        }

        [RowTest]
        [Row(ASClassLayout.Normal, new string[] { "member" }, new int[] { 1 })]
        [Row(ASClassLayout.Dynamic, new string[] { "member" }, new int[] { 1 })]
        [Row(ASClassLayout.Normal, new string[] { }, new int[] { })]
        [Row(ASClassLayout.Dynamic, new string[] { }, new int[] { })]
        [Row(ASClassLayout.Normal, new string[] { "member" }, new int[] { 1, 2, 3 }, ExpectedException = typeof(ArgumentException))]
        [Row(ASClassLayout.Dynamic, new string[] { "member" }, new int[] { 1, 2, 3 }, ExpectedException = typeof(ArgumentException))]
        [Row(ASClassLayout.Externalizable, new string[] { }, new int[] { }, ExpectedException = typeof(ArgumentException))]
        public void ConstructorWithClassAndMemberValuesAndDynamicValues(ASClassLayout classLayout,
            string[] memberNames, int[] memberValues)
        {
            IASValue[] asMemberValues = WrapInts(memberValues);

            IDictionary<string, IASValue> dynamicProperties = new Dictionary<string, IASValue>();
            ASClass @class = new ASClass("class", classLayout, memberNames);
            ASObject obj = new ASObject(@class, asMemberValues, dynamicProperties);

            Assert.AreSame(@class, obj.Class);
            Assert.AreSame(asMemberValues, obj.MemberValues);
            Assert.AreSame(dynamicProperties, obj.DynamicProperties);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithClassAndMemberValuesAndDynamicValues_ThrowsIfNullClass()
        {
            new ASObject(null, EmptyArray<IASValue>.Instance, EmptyDictionary<string, IASValue>.Instance);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithClassAndMemberValuesAndDynamicValues_ThrowsIfNullMemberValues()
        {
            new ASObject(ASClass.UntypedDynamicClass, null, EmptyDictionary<string, IASValue>.Instance);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorWithClassAndMemberValuesAndDynamicValues_ThrowsIfNullDynamicValues()
        {
            new ASObject(ASClass.UntypedDynamicClass, EmptyArray<IASValue>.Instance, null);
        }

        [Test]
        public void Indexer()
        {
            ASClass @class = new ASClass("class", ASClassLayout.Dynamic, new string[] { "member" });
            ASObject obj = new ASObject(@class);

            obj["member"] = new ASInt29(1);
            Assert.AreEqual(new ASInt29(1), obj["member"]);
            Assert.AreEqual(new ASInt29(1), obj.MemberValues[0]);

            obj["nonmember"] = new ASInt29(2);
            Assert.AreEqual(new ASInt29(2), obj["nonmember"]);
            Assert.AreEqual(new ASInt29(2), obj.DynamicProperties["nonmember"]);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetIndexerThrowsIfNull()
        {
            GC.KeepAlive(ASObject.Empty[null]);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SetIndexerThrowsIfNull()
        {
            ASObject.Empty[null] = new ASInt29(42);
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetIndexerThrowsIfAbsent()
        {
            GC.KeepAlive(ASObject.Empty["abc"]);
        }

        [Test]
        public void CreateUninitializedInstanceAndSetProperties()
        {
            ASObject obj = ASObject.CreateUninitializedInstance(ASClass.UntypedDynamicClass);
            obj.SetProperties(EmptyArray<IASValue>.Instance, EmptyDictionary<string, IASValue>.Instance);

            Assert.AreSame(ASClass.UntypedDynamicClass, obj.Class);
            Assert.AreSame(EmptyArray<IASValue>.Instance, obj.MemberValues);
            Assert.AreSame(EmptyDictionary<string, IASValue>.Instance, obj.DynamicProperties);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetPropertiesWithInitializedInstanceThrows()
        {
            ASObject obj = new ASObject();
            obj.SetProperties(EmptyArray<IASValue>.Instance, EmptyDictionary<string, IASValue>.Instance);
        }

        private static ASInt29[] WrapInts(int[] ints)
        {
            return Array.ConvertAll<int, ASInt29>(ints, delegate(int value)
            {
                return new ASInt29(value);
            });
        }
    }
}