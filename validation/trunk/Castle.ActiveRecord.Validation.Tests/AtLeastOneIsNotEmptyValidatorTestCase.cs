// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

using Castle.ActiveRecord.Validation.Attributes;
using Castle.ActiveRecord.Validation.Validators.CultureIndependant;
using NUnit.Framework;

namespace Castle.ActiveRecord.Validation.Tests
{
    [TestFixture]
    public class AtLeastOneIsNotEmptyValidatorTestCase
    {
        private const string SinglePropertySingleGroupTestGroup = "SinglePropertySingleGroup";
        private const string MultiplePropertySingleGroupTestGroup = "MultiplePropertySingleGroup";
        private const string MultiplePropertyMultipleGroupTestGroupA = "MultiplePropertyMultipleGroupA";
        private const string MultiplePropertyMultipleGroupTestGroupB = "MultiplePropertyMultipleGroupB";
    
        [Test]
        public void ValidationPassesForASingleNonEmptyField()
        {
            AtLeastOneIsNotEmptyValidator validator = new AtLeastOneIsNotEmptyValidator(SinglePropertySingleGroupTestGroup);

            SinglePropertyTestClass testClass = new SinglePropertyTestClass("a value");
            Assert.IsTrue(validator.Perform(testClass, testClass.SingleProperty));
        }

        [Test]
        public void ValidationFailsForASingleNullOrEmptyField()
        {
            AtLeastOneIsNotEmptyValidator validator = new AtLeastOneIsNotEmptyValidator(SinglePropertySingleGroupTestGroup);

            SinglePropertyTestClass testClass = new SinglePropertyTestClass(null);
            Assert.IsFalse(validator.Perform(testClass, testClass.SingleProperty));

            testClass = new SinglePropertyTestClass(string.Empty);
            Assert.IsFalse(validator.Perform(testClass, testClass.SingleProperty));
        }
        
        [Test]
        public void ValidationPassesForANonEmptyFieldOnAClassWithMultiplePropertiesInASingleGroup()
        {
            AtLeastOneIsNotEmptyValidator validator = new AtLeastOneIsNotEmptyValidator(MultiplePropertySingleGroupTestGroup);
            
            MultiplePropertySingleGroupTestClass testClass = new MultiplePropertySingleGroupTestClass(null, "a value");
            Assert.IsTrue(validator.Perform(testClass, testClass.Field1));
            Assert.IsTrue(validator.Perform(testClass, testClass.Field2));

            testClass = new MultiplePropertySingleGroupTestClass("a value", string.Empty);
            Assert.IsTrue(validator.Perform(testClass, testClass.Field1));
            Assert.IsTrue(validator.Perform(testClass, testClass.Field2));
        }

        [Test]
        public void ValidationFailsForAllNullOrNonEmptyFieldsOnAClassWithMultiplePropertiesInASingleGroup()
        {
            AtLeastOneIsNotEmptyValidator validator = new AtLeastOneIsNotEmptyValidator(MultiplePropertySingleGroupTestGroup);

            MultiplePropertySingleGroupTestClass testClass = new MultiplePropertySingleGroupTestClass(null, null);
            Assert.IsFalse(validator.Perform(testClass, testClass.Field1));
            Assert.IsFalse(validator.Perform(testClass, testClass.Field2));

            testClass = new MultiplePropertySingleGroupTestClass(string.Empty, string.Empty);
            Assert.IsFalse(validator.Perform(testClass, testClass.Field1));
            Assert.IsFalse(validator.Perform(testClass, testClass.Field2));

            testClass = new MultiplePropertySingleGroupTestClass(null, string.Empty);
            Assert.IsFalse(validator.Perform(testClass, testClass.Field1));
            Assert.IsFalse(validator.Perform(testClass, testClass.Field2));
        }

        [Test]
        public void ValidationPassesForANonEmptyFieldOnAClassWithMultiplePropertiesInMultipleGroups()
        {
            AtLeastOneIsNotEmptyValidator validator = new AtLeastOneIsNotEmptyValidator(MultiplePropertyMultipleGroupTestGroupA);

            MultiplePropertyMultipleGroupTestClass testClass = new MultiplePropertyMultipleGroupTestClass(null, "a value", string.Empty, null);
            Assert.IsTrue(validator.Perform(testClass, testClass.Group1Field1));
            Assert.IsTrue(validator.Perform(testClass, testClass.Group1Field2));

            validator = new AtLeastOneIsNotEmptyValidator(MultiplePropertyMultipleGroupTestGroupB);
            
            testClass = new MultiplePropertyMultipleGroupTestClass(string.Empty, null, "a value", null);
            Assert.IsTrue(validator.Perform(testClass, testClass.Group2Field1));
            Assert.IsTrue(validator.Perform(testClass, testClass.Group2Field2));
        }

        [Test]
        public void ValidationFailsForAllNullOrNonEmptyFieldsOnAClassWithMultiplePropertiesInMultipleGroups()
        {
            AtLeastOneIsNotEmptyValidator validator = new AtLeastOneIsNotEmptyValidator(MultiplePropertyMultipleGroupTestGroupA);

            MultiplePropertyMultipleGroupTestClass testClass = new MultiplePropertyMultipleGroupTestClass(null, null, "a value", "another value");
            Assert.IsFalse(validator.Perform(testClass, testClass.Group1Field1));
            Assert.IsFalse(validator.Perform(testClass, testClass.Group1Field2));

            validator = new AtLeastOneIsNotEmptyValidator(MultiplePropertyMultipleGroupTestGroupB);
            
            testClass = new MultiplePropertyMultipleGroupTestClass("a value", "another value", string.Empty, string.Empty);
            Assert.IsFalse(validator.Perform(testClass, testClass.Group2Field1));
            Assert.IsFalse(validator.Perform(testClass, testClass.Group2Field2));
        }
        
        #region Single Property Test Class

        private class SinglePropertyTestClass
        {
            private string singleField;

            public SinglePropertyTestClass(string value)
            {
                singleField = value;    
            }
            
            [ValidateAtLeastOneIsNotEmpty(SinglePropertySingleGroupTestGroup)]
            public string SingleProperty
            {
                get { return singleField; }
            }
        }

        #endregion

        #region Multiple Property Single Group Test Class

        private class MultiplePropertySingleGroupTestClass
        {
            private string field1;
            private string field2;
            
            public MultiplePropertySingleGroupTestClass(string value1, string value2)
            {
                field1 = value1;
                field2 = value2;
            }

            [ValidateAtLeastOneIsNotEmpty(MultiplePropertySingleGroupTestGroup)]
            public string Field1
            {
                get { return field1; }
            }

            [ValidateAtLeastOneIsNotEmpty(MultiplePropertySingleGroupTestGroup)]
            public string Field2
            {
                get { return field2; }
            }
        }

        #endregion

        #region Multiple Property Multiple Group Test Class

        private class MultiplePropertyMultipleGroupTestClass
        {
            private string group1Field1;
            private string group1Field2;
            private string group2Field1;
            private string group2Field2;

            public MultiplePropertyMultipleGroupTestClass(string group1Value1, string group1Value2, string group2Value1, string group2Value2)
            {
                group1Field1 = group1Value1;
                group1Field2 = group1Value2;
                group2Field1 = group2Value1;
                group2Field2 = group2Value2;
            }

            [ValidateAtLeastOneIsNotEmpty(MultiplePropertyMultipleGroupTestGroupA)]
            public string Group1Field1
            {
                get { return group1Field1; }
            }

            [ValidateAtLeastOneIsNotEmpty(MultiplePropertyMultipleGroupTestGroupB)]
            public string Group2Field1
            {
                get { return group2Field1; }
            }

            [ValidateAtLeastOneIsNotEmpty(MultiplePropertyMultipleGroupTestGroupB)]
            public string Group2Field2
            {
                get { return group2Field2; }
            }
            
            [ValidateAtLeastOneIsNotEmpty(MultiplePropertyMultipleGroupTestGroupA)]
            public string Group1Field2
            {
                get { return group1Field2; }
            }            
        }

        #endregion
    }
}