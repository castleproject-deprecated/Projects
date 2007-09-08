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
    using Castle.Components.Validator;

    [TestFixture]
    public class GeneratedCodeTest
    {
        [Test]
        public void CanGenerateGenericClass()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.SimpleGenericClass");
            Assert.IsNotNull(type, "Could not get the generated generic class.");
            object[] attributes = type.GetCustomAttributes(typeof (ActiveRecordAttribute), false);
            Assert.IsTrue(attributes.Length == 1);

            Type baseType = type.BaseType;
            Assert.IsTrue(baseType.IsGenericType, "Generated class does not have a generic base class.");
            Assert.IsTrue(baseType.Name == "ActiveRecordBase`1", "Generated class does not derived from ActiveRecordBase<T>");
        }

        [Test]
        public void CanGenerateExplicitGenericClassInGenericModel()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.ExplicitGenericClassInGenericModel");
            Assert.IsNotNull(type, "Could not get the generated explicit generic class.");
            object[] attributes = type.GetCustomAttributes(typeof(ActiveRecordAttribute), false);
            Assert.IsTrue(attributes.Length == 1);

            Type baseType = type.BaseType;
            Assert.IsTrue(baseType.IsGenericType, "Generated explicit generic class does not have a generic base class.");
            Assert.IsTrue(baseType.Name == "ActiveRecordBase`1", "Generated class does not derived from ActiveRecordBase<T>");
        }

        [Test]
        public void CanGenerateExplicitNonGenericClassInGenericModel()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.ExplicitNonGenericClassInGenericModel");
            Assert.IsNotNull(type, "Could not get the generated explicit non-generic class.");
            object[] attributes = type.GetCustomAttributes(typeof(ActiveRecordAttribute), false);
            Assert.IsTrue(attributes.Length == 1);

            Type baseType = type.BaseType;
            Assert.IsFalse(baseType.IsGenericType, "Generated explicit non-generic class have a generic base class.");
            Assert.IsTrue(baseType.Name == "ActiveRecordBase", "Generated class does not derived from ActiveRecordBase");
        }

        [Test]
        public void CanGenerateNonGenericClass()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.SimpleClass");
            Assert.IsNotNull(type, "Could not get the generated non-generic class.");
            object[] attributes = type.GetCustomAttributes(typeof(ActiveRecordAttribute), false);
            Assert.IsTrue(attributes.Length == 1);

            Type baseType = type.BaseType;
            Assert.IsFalse(baseType.IsGenericType, "Generated class have a generic base class.");
            Assert.IsTrue(baseType.Name == "ActiveRecordBase", "Generated class does not derived from ActiveRecordBase");
        }

        [Test]
        public void CanGenerateExplicitNonGenericClassInNonGenericModel()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.ExplicitNonGenericClassInNonGenericModel");
            Assert.IsNotNull(type, "Could not get the generated explicit non-generic class.");
            object[] attributes = type.GetCustomAttributes(typeof(ActiveRecordAttribute), false);
            Assert.IsTrue(attributes.Length == 1);

            Type baseType = type.BaseType;
            Assert.IsFalse(baseType.IsGenericType, "Generated explicit non-generic class have a generic base class.");
            Assert.IsTrue(baseType.Name == "ActiveRecordBase", "Generated class does not derived from ActiveRecordBase");
        }

        [Test]
        public void CanGenerateExplicitGenericClassInNonGenericModel()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.ExplicitGenericClassInNonGenericModel");
            Assert.IsNotNull(type, "Could not get the generated explicit generic class.");
            object[] attributes = type.GetCustomAttributes(typeof(ActiveRecordAttribute), false);
            Assert.IsTrue(attributes.Length == 1);

            Type baseType = type.BaseType;
            Assert.IsTrue(baseType.IsGenericType, "Generated explicit generic class does not have a generic base class.");
            Assert.IsTrue(baseType.Name == "ActiveRecordBase`1", "Generated class does not derived from ActiveRecordBase<T>");
        }

        [Test]
        public void CanGenerateProperty()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.ClassWithProperties");
            
            PropertyInfo property = type.GetProperty("SimpleProperty");
            Assert.IsNotNull(property, "Property not generated");
            object[] propertyAttributes = property.GetCustomAttributes(typeof(PropertyAttribute), false);
            Assert.IsTrue(propertyAttributes.Length == 1, "Did not generate PropertyAttribute for property.");
            PropertyAttribute propertyAttribute = propertyAttributes[0] as PropertyAttribute;
            Assert.IsNotNull(propertyAttribute, "Did not generate PropertyAttribute for property.");
        }
        
        [Test]
        public void CanGenerateCustomizedProperty()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.ClassWithProperties");
            PropertyInfo renamedProperty = type.GetProperty("CustomizedProperty");
            PropertyAttribute attribute = renamedProperty.GetCustomAttributes(typeof(PropertyAttribute), false)[0] as PropertyAttribute;
            
            Assert.IsTrue(attribute.Column == "column");
            Assert.IsTrue(attribute.ColumnType == "Int32");
            Assert.IsTrue(attribute.CustomAccess == "customAccess");
            Assert.IsTrue(attribute.Formula == "formula");
            Assert.IsFalse(attribute.Insert);
            Assert.IsTrue(attribute.Length == 1);
            Assert.IsTrue(attribute.NotNull);
            Assert.IsTrue(attribute.Unique);
            Assert.IsFalse(attribute.Update);
            Assert.IsTrue(attribute.UniqueKey == "uniqueKey");
            Assert.IsTrue(attribute.Index=="index");
            Assert.IsTrue(attribute.SqlType=="sqlType");
            Assert.IsTrue(attribute.Check=="check");
        }

        [Test]
        public void CanGenerateValidators()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.ClassWithProperties");
            PropertyInfo property = type.GetProperty("PropertyWithValidators");

            ValidateCreditCardAttribute ccAttribute = property.GetCustomAttributes(typeof(ValidateCreditCardAttribute), false)[0] as ValidateCreditCardAttribute;
            Assert.IsNotNull(ccAttribute);

            ValidateEmailAttribute emailAttribute = property.GetCustomAttributes(typeof(ValidateEmailAttribute), false)[0] as ValidateEmailAttribute;
            Assert.IsNotNull(emailAttribute);

            ValidateRegExpAttribute regExpAttribute = property.GetCustomAttributes(typeof(ValidateRegExpAttribute), false)[0] as ValidateRegExpAttribute;
            Assert.IsNotNull(regExpAttribute);

            ValidateLengthAttribute lengthAttribute = property.GetCustomAttributes(typeof(ValidateLengthAttribute), false)[0] as ValidateLengthAttribute;
            Assert.IsNotNull(lengthAttribute);

            ValidateDateAttribute dateAttribute = property.GetCustomAttributes(typeof(ValidateDateAttribute), false)[0] as ValidateDateAttribute;
            Assert.IsNotNull(dateAttribute);

            ValidateDateTimeAttribute dateTimeAttribute = property.GetCustomAttributes(typeof(ValidateDateTimeAttribute), false)[0] as ValidateDateTimeAttribute;
            Assert.IsNotNull(dateTimeAttribute);

            ValidateDecimalAttribute decimalAttribute = property.GetCustomAttributes(typeof(ValidateDecimalAttribute), false)[0] as ValidateDecimalAttribute;
            Assert.IsNotNull(decimalAttribute);

            ValidateDoubleAttribute doubleAttribute = property.GetCustomAttributes(typeof(ValidateDoubleAttribute), false)[0] as ValidateDoubleAttribute;
            Assert.IsNotNull(doubleAttribute);

            ValidateIntegerAttribute integerAttribute = property.GetCustomAttributes(typeof(ValidateIntegerAttribute), false)[0] as ValidateIntegerAttribute;
            Assert.IsNotNull(integerAttribute);

            ValidateNonEmptyAttribute nonEmptyAttribute = property.GetCustomAttributes(typeof(ValidateNonEmptyAttribute), false)[0] as ValidateNonEmptyAttribute;
            Assert.IsNotNull(nonEmptyAttribute);

            ValidateRangeAttribute rangeAttribute = property.GetCustomAttributes(typeof(ValidateRangeAttribute), false)[0] as ValidateRangeAttribute;
            Assert.IsNotNull(rangeAttribute);

            ValidateSameAsAttribute sameAsAttribute = property.GetCustomAttributes(typeof(ValidateSameAsAttribute), false)[0] as ValidateSameAsAttribute;
            Assert.IsNotNull(sameAsAttribute);

            ValidateSetAttribute setAttribute = property.GetCustomAttributes(typeof(ValidateSetAttribute), false)[0] as ValidateSetAttribute;
            Assert.IsNotNull(setAttribute);

            ValidateSingleAttribute singleAttribute = property.GetCustomAttributes(typeof(ValidateSingleAttribute), false)[0] as ValidateSingleAttribute;
            Assert.IsNotNull(singleAttribute);
        }

        [Test]
        public void CanGeneratePrimaryKey()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.ClassWithPK");
            PropertyInfo property = type.GetProperty("PrimaryKeyProperty");

            object[] propertyAttributes = property.GetCustomAttributes(typeof(PrimaryKeyAttribute), false);
            Assert.IsTrue(propertyAttributes.Length == 1, "Did not generate PrimaryKeyAttribute.");

            PrimaryKeyAttribute attribute = propertyAttributes[0] as PrimaryKeyAttribute;
            Assert.IsTrue(attribute.Generator == PrimaryKeyType.Native);
            Assert.IsTrue(attribute.Params == "params");
            Assert.IsTrue(attribute.UnsavedValue == "unsavedValue");
        }

        [Test]
        public void CanGenerateCompositeKey()
        {
            string expectedClassName = "ClassWithCompositeKey";

            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests." + expectedClassName);
            PropertyInfo property = type.GetProperty(expectedClassName + "CompositeKey");
            object[] propertyAttributes = property.GetCustomAttributes(typeof(CompositeKeyAttribute), false);
            Assert.IsTrue(propertyAttributes.Length == 1, "Did not generate CompositeKeyAttribute.");
            CompositeKeyAttribute attribute = propertyAttributes[0] as CompositeKeyAttribute;
            Assert.IsNotNull(attribute, "Did not generate CompositeKeyAttribute.");
            Assert.IsTrue(property.GetGetMethod().ReturnType.Name == expectedClassName + "CompositeKey");

            Type ckType =
                Assembly.GetExecutingAssembly().GetType("Debugging.Tests." + expectedClassName + "CompositeKey");
            Assert.IsNotNull(ckType, "Did not generate Helper class for composite key.");

            PropertyInfo keyProperty1 = ckType.GetProperty("Key1");
            object[] keyAttributes = keyProperty1.GetCustomAttributes(typeof(KeyPropertyAttribute), false);
            Assert.IsTrue(keyAttributes.Length == 1, "Did not generate KeyPropertyAttribute.");
            
            KeyPropertyAttribute keyAttribute = keyAttributes[0] as KeyPropertyAttribute;
            Assert.IsNotNull(keyAttribute, "Did not generate KeyPropertyAttribute.");

            ClassWithCompositeKeyCompositeKey ck = new ClassWithCompositeKeyCompositeKey();
            ck.Key1 = 1;
            ck.Key2 = 2;
            Assert.AreEqual(ck.ToString(), "1:2");
            Assert.IsFalse(ck.Equals(null));
            Assert.IsFalse(ck.Equals(new object()));
            Assert.IsFalse(ck.Equals(new ClassWithCompositeKeyCompositeKey()));
            ClassWithCompositeKeyCompositeKey ck2 = new ClassWithCompositeKeyCompositeKey();
            ck2.Key1 = 1;
            ck2.Key2 = 2;
            Assert.IsTrue(ck.Equals(ck2));
        }

        [Test]
        public void CanGenerateManyToOneRelation()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.ManyToOne_One");
            Type type2 = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.ManyToOne_Many");
            PropertyInfo property = type.GetProperty("TargetProperties");

            object[] propertyAttributes = property.GetCustomAttributes(typeof(HasManyAttribute), false);
            Assert.IsTrue(propertyAttributes.Length == 1, "Did not generate HasManyAttribute.");

            HasManyAttribute attribute = propertyAttributes[0] as HasManyAttribute;
            Assert.IsTrue(attribute.Table == "Posts");
            Assert.IsTrue(attribute.ColumnKey == "post_blogid");
            Assert.IsTrue(attribute.Cascade == ManyRelationCascadeEnum.All);
            Assert.IsTrue(attribute.Cache == CacheEnum.ReadOnly);
            Assert.IsTrue(attribute.CustomAccess == "TargetCustomAccess");
            Assert.IsTrue(attribute.Inverse);
            Assert.IsTrue(attribute.Lazy);
            Assert.IsTrue(attribute.OrderBy == "TargetOrderBy");
            Assert.IsTrue(attribute.RelationType == RelationType.Bag);
            Assert.IsTrue(attribute.Schema == "TargetSchema");
            Assert.IsTrue(attribute.Where == "TargetWhere");
            Assert.IsTrue(attribute.NotFoundBehaviour == NotFoundBehaviour.Exception);
            Assert.IsTrue(attribute.Element == "TargetElement");
            Assert.AreEqual(attribute.MapType, type2);

            PropertyInfo property2 = type2.GetProperty("SourceProperty");
            object[] propertyAttributes2 = property2.GetCustomAttributes(typeof(BelongsToAttribute), false);
            Assert.IsTrue(propertyAttributes2.Length == 1, "Did not generate BelongsToAttribute.");

            BelongsToAttribute attribute2 = propertyAttributes2[0] as BelongsToAttribute;
            Assert.IsTrue(attribute2.Column == "post_blogid");
            Assert.IsTrue(attribute2.Cascade == CascadeEnum.All);
            Assert.IsTrue(attribute2.NotNull == true);
            Assert.IsTrue(attribute2.CustomAccess == "SourceCustomAccss");
            Assert.IsTrue(attribute2.OuterJoin == OuterJoinEnum.True);
            Assert.IsTrue(attribute2.NotFoundBehaviour == NotFoundBehaviour.Ignore);
            Assert.IsTrue(attribute2.Unique);
            Assert.IsFalse(attribute2.Insert);
            Assert.IsFalse(attribute2.Update);
            Assert.AreEqual(attribute2.Type, type2);
        }

        [Test]
        public void CanGenerateManyToManyRelation()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.ManyToMany_First");
            Type type2 = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.ManyToMany_Second");
            PropertyInfo property = type.GetProperty("TargetProperties");

            object[] propertyAttributes = property.GetCustomAttributes(typeof(HasAndBelongsToManyAttribute), false);
            Assert.IsTrue(propertyAttributes.Length == 1, "Did not generate HasAndBelongsToManyAttribute.");

            HasAndBelongsToManyAttribute attribute = propertyAttributes[0] as HasAndBelongsToManyAttribute;
            Assert.IsTrue(attribute.Table == "FirstSecond");
            Assert.IsTrue(attribute.ColumnKey == "post_id");
            Assert.IsTrue(attribute.ColumnRef == "tag_id");
            Assert.IsTrue(attribute.Cascade == ManyRelationCascadeEnum.All);
            Assert.IsTrue(attribute.Cache == CacheEnum.ReadOnly);
            Assert.IsTrue(attribute.CustomAccess == "TargetCustomAccess");
            Assert.IsTrue(attribute.Inverse);
            Assert.IsTrue(attribute.Lazy);
            Assert.IsTrue(attribute.OrderBy == "TargetOrderBy");
            Assert.IsTrue(attribute.RelationType == RelationType.Bag);
            Assert.IsTrue(attribute.Schema == "dbo");
            Assert.IsTrue(attribute.Where == "TargetWhere");
            Assert.IsTrue(attribute.NotFoundBehaviour == NotFoundBehaviour.Exception);
            Assert.AreEqual(attribute.MapType, type2);

            PropertyInfo property2 = type2.GetProperty("SourceProperties");
            object[] propertyAttributes2 = property2.GetCustomAttributes(typeof(HasAndBelongsToManyAttribute), false);
            Assert.IsTrue(propertyAttributes2.Length == 1, "Did not generate HasAndBelongsToManyAttribute.");

            HasAndBelongsToManyAttribute attribute2 = propertyAttributes2[0] as HasAndBelongsToManyAttribute;
            Assert.IsTrue(attribute2.Table == "FirstSecond");
            Assert.IsTrue(attribute2.ColumnKey == "tag_id");
            Assert.IsTrue(attribute2.ColumnRef == "post_id");
            Assert.IsTrue(attribute2.Cascade == ManyRelationCascadeEnum.All);
            Assert.IsTrue(attribute2.Cache == CacheEnum.ReadOnly);
            Assert.IsTrue(attribute2.CustomAccess == "SourceCustomAccess");
            Assert.IsTrue(attribute2.Inverse);
            Assert.IsTrue(attribute2.Lazy);
            Assert.IsTrue(attribute2.OrderBy == "SourceOrderBy");
            Assert.IsTrue(attribute2.RelationType == RelationType.Bag);
            Assert.IsTrue(attribute2.Schema == "dbo");
            Assert.IsTrue(attribute2.Where == "SourceWhere");
            Assert.IsTrue(attribute2.NotFoundBehaviour == NotFoundBehaviour.Exception);
            Assert.AreEqual(attribute2.MapType, type);
        }

        [Test]
        public void CanGenerateOneToOneRelation()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.OneToOne_Target");
            Type type2 = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.OneToOne_Source");
            PropertyInfo property = type.GetProperty("OneToOne_Source");

            object[] propertyAttributes = property.GetCustomAttributes(typeof(OneToOneAttribute), false);
            Assert.IsTrue(propertyAttributes.Length == 1, "Did not generate OneToOneAttribute.");

            PropertyInfo property2 = type2.GetProperty("OneToOne_Target");
            object[] propertyAttributes2 = property2.GetCustomAttributes(typeof(OneToOneAttribute), false);
            Assert.IsTrue(propertyAttributes2.Length == 1, "Did not generate OneToOneAttribute.");
        }

        [Test]
        public void CanGenerateLazyOneToOneRelation()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.LazyOneToOne_Target");
            PropertyInfo property = type.GetProperty("LazyOneToOne_Source");
            object[] propertyAttributes = property.GetCustomAttributes(typeof(BelongsToAttribute), false);

            Assert.IsTrue(propertyAttributes.Length == 1, "Did not generate BelongsToAttribute for lazy one to one relation on the target.");
        }

        [Test]
        public void CanGenerateInClassMetaData()
        {
            Assert.IsTrue(InClassMetaDataType.MyPKProperty == "MyPK", "Generated metadata for primary key does not match property name.");
            Assert.IsTrue(InClassMetaDataType.MyFieldProperty == "MyField", "Generated metadata for field does not match property name.");
            Assert.IsTrue(InClassMetaDataType.MyPropertyProperty == "MyProperty", "Generated metadata for property does not match property name.");
            Assert.IsTrue(InClassMetaDataType.MyVersionProperty == "MyVersion", "Generated metadata for version does not match property name.");
            Assert.IsTrue(InClassMetaDataType.MyTimestampProperty == "MyTimestamp", "Generated metadata for timestamp does not match property name.");
        }

        [Test]
        public void CanGenerateSubClassMetaData()
        {
            Assert.IsTrue(SubClassMetaDataType.Properties.MyPK == "MyPK", "Generated metadata for primary key does not match property name.");
            Assert.IsTrue(SubClassMetaDataType.Properties.MyField == "MyField", "Generated metadata for field does not match property name.");
            Assert.IsTrue(SubClassMetaDataType.Properties.MyProperty == "MyProperty", "Generated metadata for property does not match property name.");
            Assert.IsTrue(SubClassMetaDataType.Properties.MyVersion == "MyVersion", "Generated metadata for version does not match property name.");
            Assert.IsTrue(SubClassMetaDataType.Properties.MyTimestamp == "MyTimestamp", "Generated metadata for timestamp does not match property name.");
        }

        [Test]
        public void CanGenerateExplicitlyGenericDeclaredField()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.ExplicitGenericRelationDecleration_HasMany");
            PropertyInfo property = type.GetProperty("ExplicitGenericRelationDecleration_BelongsToes");
            Assert.IsTrue(property.PropertyType.IsGenericType, "Generated type is not generic");
            Assert.AreEqual(property.PropertyType.GetGenericArguments()[0].Name, "String", "First generic does not match.");
            Assert.AreEqual(property.PropertyType.GetGenericArguments()[1].Name, "ExplicitGenericRelationDecleration_BelongsTo", "First generic does not match.");
        }

        [Test]
        public void CanGenerateDebuggerDisplayAttribute()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.DebuggerDisplayAndDefaultMemberTestClass");
            object[] attributes = type.GetCustomAttributes(typeof(System.Diagnostics.DebuggerDisplayAttribute), false);
            Assert.IsTrue(attributes.Length == 1, "Did not generate DebuggerDisplayAttribute.");
        }

        [Test]
        public void CanGenerateDefaultMemberAttribute()
        {
            Type type = Assembly.GetExecutingAssembly().GetType("Debugging.Tests.DebuggerDisplayAndDefaultMemberTestClass");
            object[] attributes = type.GetCustomAttributes(typeof(DefaultMemberAttribute), false);
            Assert.IsTrue(attributes.Length == 1, "Did not generate DefaultMemberAttribute.");
        }
    }
}
