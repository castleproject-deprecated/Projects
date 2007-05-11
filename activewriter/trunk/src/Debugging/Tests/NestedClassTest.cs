// Copyright 2006 Gokhan Altinoren - http://altinoren.com/
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

namespace Debugging.Tests
{
    using System;
    using System.Reflection;
    using NUnit.Framework;
    using Castle.ActiveRecord;

    [TestFixture]
    public class NestedClassTest
    {
        [Test]
        public void CanGenerateGenericClass()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.NestedClass1");
            Assert.IsNotNull(type, "Could not get the generated nested class.");
        }

        [Test]
        public void CanGenerateNestedProperty()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.NestingClass1");

            PropertyInfo property = type.GetProperty("NestedClass1");
            Assert.IsNotNull(property, "Nested property not generated");
            object[] nestedAttributes = property.GetCustomAttributes(typeof(NestedAttribute), false);
            Assert.IsTrue(nestedAttributes.Length == 1, "Did not generate NestedAttribute for nested property.");
            NestedAttribute nestedAttribute = nestedAttributes[0] as NestedAttribute;
            Assert.IsNotNull(nestedAttribute, "Did not generate NestedAttribute for nested property.");

            Assert.IsTrue(nestedAttribute.ColumnPrefix == "columnPrefix");
            Assert.IsTrue(nestedAttribute.Insert);
            Assert.IsTrue(nestedAttribute.Update);
            Assert.IsTrue(nestedAttribute.MapType == typeof(NestedClass1));
        }
    }
}
