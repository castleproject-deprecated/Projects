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
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Collections;
using MbUnit.Framework;

namespace Castle.FlexBridge.Tests.UnitTests.ActionScript
{
    [TestFixture]
    [TestsOn(typeof(ASClass))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class ASClassTest
    {
        [Test]
        public void HasSingletonUntypedDynamicClass()
        {
            Assert.AreEqual("", ASClass.UntypedDynamicClass.ClassAlias);
            Assert.AreEqual(ASClassLayout.Dynamic, ASClass.UntypedDynamicClass.Layout);
            Assert.AreEqual(0, ASClass.UntypedDynamicClass.MemberNames.Count);
            Assert.IsTrue(ASClass.UntypedDynamicClass.MemberNames.IsReadOnly);
        }

        [RowTest]
        [Row("", ASClassLayout.Dynamic, new string[] { "abc", "def" })]
        [Row("normal", ASClassLayout.Normal, new string[] { "abc", "def" })]
        [Row("externalizable", ASClassLayout.Externalizable, new string[] { })]
        [Row(null, ASClassLayout.Normal, new string[] { }, ExpectedException = typeof(ArgumentNullException))]
        [Row("normal", ASClassLayout.Normal, null, ExpectedException = typeof(ArgumentNullException))]
        [Row("normal", ASClassLayout.Normal, null, ExpectedException = typeof(ArgumentNullException))]
        [Row("externalizable", ASClassLayout.Externalizable, new string[] { "abc", "def" }, ExpectedException=typeof(ArgumentException))]
        [Row("", ASClassLayout.Externalizable, new string[] { "abc", "def" }, ExpectedException = typeof(ArgumentException))]
        public void Constructor(string className, ASClassLayout classLayout, string[] memberNames)
        {
            ASClass @class = new ASClass(className, classLayout, memberNames);

            Assert.AreSame(className, @class.ClassAlias);
            Assert.AreEqual(classLayout, @class.Layout);
            Assert.AreSame(memberNames, @class.MemberNames);
        }

        [Test]
        public void GetHashCodeIsSane()
        {
            // Compare hashcodes of two identical classes.
            ASClass class1 = new ASClass("abc", ASClassLayout.Dynamic, new string[] { "abc", "def" });
            ASClass class2 = new ASClass("abc", ASClassLayout.Dynamic, new string[] { "abc", "def" });

            Assert.AreEqual(class1.GetHashCode(), class2.GetHashCode());
        }

        [Test]
        public void EqualsWithNullIsFalse()
        {
            ASClass class1 = new ASClass("abc", ASClassLayout.Dynamic, new string[] { "abc", "def" });
            Assert.AreNotEqual(class1, null);
        }

        [Test]
        public void EqualsWithSelfIsTrue()
        {
            ASClass class1 = new ASClass("abc", ASClassLayout.Dynamic, new string[] { "abc", "def" });
            Assert.AreEqual(class1, class1);
        }

        [Test]
        public void EqualsWithNonClassIsFalse()
        {
            ASClass class1 = new ASClass("abc", ASClassLayout.Dynamic, new string[] { "abc", "def" });
            Assert.AreNotEqual(class1, "abc");
        }

        [Test]
        public void EqualsWithIdenticalInstancesIsTrue()
        {
            ASClass class1 = new ASClass("abc", ASClassLayout.Dynamic, new string[] { "abc", "def" });
            ASClass class2 = new ASClass("abc", ASClassLayout.Dynamic, new string[] { "abc", "def" });

            Assert.AreEqual(class1, class2);
        }

        [Test]
        public void EqualsWithDifferentMemberOrderIsFalse()
        {
            ASClass class1 = new ASClass("abc", ASClassLayout.Dynamic, new string[] { "abc", "def" });
            ASClass class2 = new ASClass("abc", ASClassLayout.Dynamic, new string[] { "def", "abc" });

            Assert.AreNotEqual(class1, class2);
        }

        [Test]
        public void EqualsWithDifferentMemberCountIsFalse()
        {
            ASClass class1 = new ASClass("abc", ASClassLayout.Dynamic, new string[] { "abc", });
            ASClass class2 = new ASClass("abc", ASClassLayout.Dynamic, new string[] { "def", "abc" });

            Assert.AreNotEqual(class1, class2);
            Assert.AreNotEqual(class2, class1);
        }

        [Test]
        public void EqualsWithDifferentClassNameIsFalse()
        {
            ASClass class1 = new ASClass("abc", ASClassLayout.Dynamic, new string[] { "abc", "def" });
            ASClass class2 = new ASClass("def", ASClassLayout.Dynamic, new string[] { "abc", "def" });

            Assert.AreNotEqual(class1, class2);
        }

        [Test]
        public void EqualsWithDifferentClassLayoutIsFalse()
        {
            ASClass class1 = new ASClass("abc", ASClassLayout.Dynamic, new string[] { "abc", "def" });
            ASClass class2 = new ASClass("abc", ASClassLayout.Normal, new string[] { "abc", "def" });

            Assert.AreNotEqual(class1, class2);
        }

        [Test]
        public void EqualsWithSamePropertiesAndMemberNameArrayInstancesIsTrue()
        {
            ASClass class1 = new ASClass("abc", ASClassLayout.Normal, EmptyArray<string>.Instance);
            ASClass class2 = new ASClass("abc", ASClassLayout.Normal, EmptyArray<string>.Instance);

            Assert.AreNotEqual(class1, class2);
        }

        [Test]
        public void ToStringReturnsClassAliasIfNonEmpty()
        {
            ASClass class1 = new ASClass("abc", ASClassLayout.Normal, EmptyArray<string>.Instance);
            Assert.AreEqual("abc", class1.ToString());
        }

        [Test]
        public void ToStringReturnsUntypedIfClassAliasEmpty()
        {
            Assert.AreEqual("<untyped>", ASClass.UntypedDynamicClass.ToString());
        }
    }
}