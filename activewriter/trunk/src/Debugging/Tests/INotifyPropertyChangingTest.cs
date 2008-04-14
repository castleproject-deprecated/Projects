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

using System.ComponentModel;

namespace Debugging.Tests
{
    using System;
    using System.Reflection;
    using NUnit.Framework;
    using Castle.ActiveRecord;

    [TestFixture]
    public class INotifyPropertyChangingTest
    {
        [Test]
        public void CanGenerateClassImplementingINotifyPropertyChanging()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.NotifyChangingClass");
            Assert.IsNotNull(type.GetInterface("System.ComponentModel.INotifyPropertyChanging"), "Generated clas does not implement INotifyPropertyChanging.");
        }

        [Test]
        public void DoesClassImplementingINotifyPropertyChangingWorks()
        {
            string propertyName = null;
            NotifyChangingClass cls = new NotifyChangingClass();
            cls.PropertyChanging +=
                delegate(object sender, PropertyChangingEventArgs e) { propertyName = e.PropertyName; };
            cls.NotifyChangingProperty = "dummy";
            Assert.AreEqual(propertyName, "NotifyChangingProperty", "Event not triggered succesfully through INotifyPropertyChanging.");
        }
    }
}
