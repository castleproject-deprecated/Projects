namespace Debugging.Tests
{
    using System;
    using System.Reflection;
    using NUnit.Framework;
    using Castle.ActiveRecord;

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

            ValidateConfirmationAttribute confirmationAttribute = property.GetCustomAttributes(typeof(ValidateConfirmationAttribute), false)[0] as ValidateConfirmationAttribute;
            Assert.IsNotNull(confirmationAttribute);

            ValidateCreditCardAttribute ccAttribute = property.GetCustomAttributes(typeof(ValidateCreditCardAttribute), false)[0] as ValidateCreditCardAttribute;
            Assert.IsNotNull(ccAttribute);

            ValidateEmailAttribute emailAttribute = property.GetCustomAttributes(typeof(ValidateEmailAttribute), false)[0] as ValidateEmailAttribute;
            Assert.IsNotNull(emailAttribute);

            ValidateIsUniqueAttribute uniqueAttribute = property.GetCustomAttributes(typeof(ValidateIsUniqueAttribute), false)[0] as ValidateIsUniqueAttribute;
            Assert.IsNotNull(uniqueAttribute);

            ValidateNotEmptyAttribute notEmptyAttribute = property.GetCustomAttributes(typeof(ValidateNotEmptyAttribute), false)[0] as ValidateNotEmptyAttribute;
            Assert.IsNotNull(notEmptyAttribute);

            ValidateRegExpAttribute regExpAttribute = property.GetCustomAttributes(typeof(ValidateRegExpAttribute), false)[0] as ValidateRegExpAttribute;
            Assert.IsNotNull(regExpAttribute);

            ValidateLengthAttribute lengthAttribute = property.GetCustomAttributes(typeof(ValidateLengthAttribute), false)[0] as ValidateLengthAttribute;
            Assert.IsNotNull(lengthAttribute);

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
            Assert.IsTrue(attribute2.NotNull == false);
            Assert.IsTrue(attribute2.CustomAccess == "SourceCustomAccss");
            Assert.IsTrue(attribute2.OuterJoin == OuterJoinEnum.True);
            Assert.IsTrue(attribute2.NotFoundBehaviour == NotFoundBehaviour.Ignore);
            Assert.IsTrue(attribute2.Unique);
            Assert.IsFalse(attribute2.Insert);
            Assert.IsFalse(attribute2.Update);
            Assert.AreEqual(attribute2.Type, type2);
        }
    }
}
