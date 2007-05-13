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
    public class INotifyPropertyChangedTest
    {
        [Test]
        public void CanGenerateClassImplementingINotifyPropertyChanged()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.NotifyingClass");
            Assert.IsNotNull(type.GetInterface("System.ComponentModel.INotifyPropertyChanged"), "Generated clas does not implement INotifyPropertyChanged.");
        }

        [Test]
        public void DoesClassImplementingINotifyPropertyChangedWorks()
        {
            string propertyName = null;
            NotifyingClass cls = new NotifyingClass();
            cls.PropertyChanged +=
                delegate(object sender, PropertyChangedEventArgs e) { propertyName = e.PropertyName; };
            cls.PropertyWithNotify = "dummy";
            Assert.AreEqual(propertyName, "PropertyWithNotify", "Event not triggered succesfully through INotifyPropertyChanged.");
        }
    }
}
