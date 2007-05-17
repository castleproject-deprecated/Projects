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
using System.Globalization;
using System.Reflection;
using Castle.FlexBridge.ActionScript;

namespace Castle.FlexBridge.Serialization.Mapping
{
    /// <summary>
    /// Provides mappers for mapping objects property by property using reflection.
    /// New .Net object instances are created by consulting a <see cref="ActionScriptMappingTable" />.
    /// </summary>
    /// <seealso cref="ActionScriptClassAttribute"/>
    /// <seealso cref="ActionScriptPropertyAttribute"/>
    /// <seealso cref="ActionScriptIgnoreAttribute"/>
    public sealed class ObjectMapperFactory : BaseASMapperFactory
    {
        private ActionScriptMappingTable mappingTable;

        /// <summary>
        /// Creates an object mapper factory with the specified mapping table.
        /// </summary>
        /// <param name="mappingTable">The mapping table</param>
        public ObjectMapperFactory(ActionScriptMappingTable mappingTable)
        {
            this.mappingTable = mappingTable;
        }

        /// <summary>
        /// Gets the mapping table.
        /// </summary>
        public ActionScriptMappingTable MappingTable
        {
            get { return mappingTable; }
        }

        public override IASSourceMapper GetASSourceMapper(ASSourceMappingDescriptor descriptor)
        {
            if (descriptor.SourceKind == ASTypeKind.Object)
            {
                ActionScriptClassMapping classMapping = mappingTable.GetClassMappingByAlias(descriptor.SourceClassAlias);
                if (classMapping != null && descriptor.TargetNativeType.IsAssignableFrom(classMapping.NativeType))
                {
                    return new ObjectMapper(classMapping);
                }
            }

            return null;
        }

        public override IASTargetMapper GetASTargetMapper(ASTargetMappingDescriptor descriptor)
        {
            ActionScriptClassMapping classMapping = mappingTable.GetClassMappingByType(descriptor.SourceNativeType);
            if (classMapping != null)
            {
                return new ObjectMapper(classMapping);
            }

            return null;
        }

        /// <summary>
        /// Maps objects property by property using reflection.
        /// </summary>
        private sealed class ObjectMapper : BaseASMapper, IASNativeObjectMapper
        {
            private ActionScriptClassMapping classMapping;
            private Dictionary<string, ActionScriptPropertyMapping> propertyMappings;
            private ActionScriptPropertyMapping[] memberPropertyMappings;
            private ActionScriptPropertyMapping[] dynamicPropertyMappings;

            public ObjectMapper(ActionScriptClassMapping classMapping)
            {
                this.classMapping = classMapping;

                PopulateTables();
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                // Externalizable classes require less work and we already have a good special
                // case ASValue implementation to support the AMF deserializer so let's use it.
                ASClass @class = classMapping.ASClass;
                if (@class.Layout == ASClassLayout.Externalizable)
                    return new ASExternalizableObject(@class, (IExternalizable)nativeValue);

                // Generate the full mapping on demand.
                return new ASNativeObject(@class, nativeValue, this);
            }

            void IASNativeObjectMapper.AcceptVisitor(IActionScriptSerializer serializer, ASClass @class, object nativeObject,
                IASValueVisitor visitor)
            {
                // Note: We can assume that @class == classMapping.ASClass because we created
                //       the ASNativeObject instance that way ourselves.
                //       We also know that the object is not IExternalizable for the same reason.
                try
                {
                    visitor.VisitObject(serializer, @class, GetMappedMemberValues(serializer, nativeObject),
                        GetMappedDynamicProperties(serializer, nativeObject), null);
                }
                catch (Exception ex)
                {
                    throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                        "An error occurred while mapping an instance of type '{0}' to ActionScript.",
                        classMapping.NativeType.FullName), ex);
                }
            }

            ASValueContentFlags IASNativeObjectMapper.GetContentFlags(object nativeObject)
            {
                return classMapping.ASClass.Layout == ASClassLayout.Dynamic ? ASValueContentFlags.HasDynamicProperties : ASValueContentFlags.None;
            }

            private IEnumerable<IASValue> GetMappedMemberValues(IActionScriptSerializer serializer, object instance)
            {
                foreach (ActionScriptPropertyMapping mapping in memberPropertyMappings)
                {
                    yield return GetMappedPropertyOrField(serializer, instance, mapping.NativePropertyOrField);
                }
            }

            private IEnumerable<KeyValuePair<string, IASValue>> GetMappedDynamicProperties(IActionScriptSerializer serializer, object instance)
            {
                foreach (ActionScriptPropertyMapping mapping in dynamicPropertyMappings)
                {
                    yield return new KeyValuePair<string, IASValue>(mapping.ASPropertyName,
                        GetMappedPropertyOrField(serializer, instance, mapping.NativePropertyOrField));
                }

                IDynamic dynamic = instance as IDynamic;
                if (dynamic != null)
                {
                    // This somewhat contorted bit of code serves to catch and report
                    // failures while getting dynamic properties from a class in a somewhat
                    // more default fashion than would be possible using a plain foreach loop
                    // because yield return cannot be used inside a try/catch.
                    IEnumerator<KeyValuePair<string, IASValue>> en;
                    try
                    {
                        en = dynamic.GetDynamicProperties(serializer).GetEnumerator();
                    }
                    catch (Exception ex)
                    {
                        throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                            "Error while getting values of dynamic properties from class '{0}'",
                            dynamic.GetType().FullName), ex);
                    }

                    for (; ; )
                    {
                        KeyValuePair<string, IASValue> pair;
                        try
                        {
                            if (!en.MoveNext())
                                break;
                            pair = en.Current;
                        }
                        catch (Exception ex)
                        {
                            throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                                "Error while getting values of dynamic properties from class '{0}'",
                                dynamic.GetType().FullName), ex);
                        }

                        yield return pair;
                    }
                }
            }

            protected override object MapObjectToNative(IActionScriptSerializer serializer, Type nativeType,
                ASClass @class, IEnumerable<IASValue> memberValues, IEnumerable<KeyValuePair<string, IASValue>> dynamicProperties, IExternalizable externalizableValue)
            {
                try
                {
                    object instance = Activator.CreateInstance(classMapping.NativeType);

                    IDynamic dynamic = instance as IDynamic;

                    // Set values from class members.
                    // Note: The layout of the AS class we receive from the client may differ
                    //       from what we generated as part of the mapper.  The only thing that
                    //       matters is the property name and value regardless of whether it appears as a member
                    //       or as a dynamic property.
                    IList<string> memberNames = @class.MemberNames;
                    int memberIndex = 0;
                    foreach (IASValue memberValue in memberValues)
                    {
                        string memberName = memberNames[memberIndex];
                        SetMappedMemberOrDynamicProperty(serializer, instance, dynamic, memberName, memberValue);
                        memberIndex += 1;
                    }

                    // Set value from dynamic properties.
                    foreach (KeyValuePair<string, IASValue> pair in dynamicProperties)
                    {
                        SetMappedMemberOrDynamicProperty(serializer, instance, dynamic, pair.Key, pair.Value);
                    }

                    return instance;
                }
                catch (Exception ex)
                {
                    throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                        "An error occurred while mapping an ActionScript value to an instance of type '{0}'.",
                        classMapping.NativeType.FullName), ex);
                }
            }

            /// <summary>
            /// Populates lookup tables and lists.
            /// </summary>
            private void PopulateTables()
            {
                ICollection<ActionScriptPropertyMapping> properties = classMapping.Properties;
                List<ActionScriptPropertyMapping> tempMemberPropertyMappings = new List<ActionScriptPropertyMapping>(properties.Count);
                List<ActionScriptPropertyMapping> tempDynamicPropertyMappings = new List<ActionScriptPropertyMapping>(properties.Count);
                propertyMappings = new Dictionary<string, ActionScriptPropertyMapping>(properties.Count);

                foreach (ActionScriptPropertyMapping propertyMapping in properties)
                {
                    propertyMappings.Add(propertyMapping.ASPropertyName, propertyMapping);

                    if (propertyMapping.IsDynamic)
                        tempDynamicPropertyMappings.Add(propertyMapping);
                    else
                        tempMemberPropertyMappings.Add(propertyMapping);
                }

                memberPropertyMappings = tempMemberPropertyMappings.ToArray();
                dynamicPropertyMappings = tempDynamicPropertyMappings.ToArray();
            }

            /// <summary>
            /// Sets a mapped property or field value or a dynamic property depending on the mapping.
            /// </summary>
            /// <param name="serializer"></param>
            /// <param name="instance"></param>
            /// <param name="dynamic"></param>
            /// <param name="propertyName"></param>
            /// <param name="value"></param>
            private void SetMappedMemberOrDynamicProperty(IActionScriptSerializer serializer, object instance, IDynamic dynamic, string propertyName, IASValue value)
            {
                // Set the mapped property, if there is one.
                ActionScriptPropertyMapping propertyMapping;
                if (propertyMappings.TryGetValue(propertyName, out propertyMapping))
                {
                    SetMappedPropertyOrField(serializer, instance, propertyMapping.NativePropertyOrField, value);

                    // If the property is dynamic then also set it as a dynamic property if possible.
                    if (dynamic != null && propertyMapping.IsDynamic)
                    {
                        SetDynamicProperty(dynamic, serializer, propertyName, value);
                    }
                }
                else if (dynamic != null)
                {
                    SetDynamicProperty(dynamic, serializer, propertyName, value);
                }
                else
                {
                    throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                        "Unable to set value for property '{0}' during ActionScript object deserialization because no suitable mapping is defined and type '{1}' is not dynamic.",
                        propertyName, classMapping.NativeType.FullName));
                }
            }

            /// <summary>
            /// Gets a mapped property or field value using reflection.
            /// </summary>
            /// <param name="serializer"></param>
            /// <param name="instance"></param>
            /// <param name="propertyOrField"></param>
            /// <returns></returns>
            private static IASValue GetMappedPropertyOrField(IActionScriptSerializer serializer, object instance, MemberInfo propertyOrField)
            {
                try
                {
                    // Try to get the property.
                    PropertyInfo property = propertyOrField as PropertyInfo;
                    if (property != null)
                    {
                        object propertyValue = property.GetValue(instance, null);
                        IASValue mappedPropertyValue = serializer.ToASValue(propertyValue);
                        return mappedPropertyValue;
                    }

                    // Oh, it must be a field then.
                    FieldInfo field = (FieldInfo)propertyOrField;
                    object fieldValue = field.GetValue(instance);
                    IASValue mappedFieldValue = serializer.ToASValue(fieldValue);
                    return mappedFieldValue;
                }
                catch (Exception ex)
                {
                    throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                        "Error while getting value of property or field named '{0}' on class '{1}'",
                        propertyOrField.Name, propertyOrField.ReflectedType.FullName), ex);
                }
            }

            /// <summary>
            /// Sets a mapped property or field value using reflection.
            /// </summary>
            /// <param name="serializer"></param>
            /// <param name="instance"></param>
            /// <param name="propertyOrField"></param>
            /// <param name="value"></param>
            private static void SetMappedPropertyOrField(IActionScriptSerializer serializer, object instance, MemberInfo propertyOrField, IASValue value)
            {
                try
                {
                    // Try to set the property.
                    PropertyInfo property = propertyOrField as PropertyInfo;
                    if (property != null)
                    {
                        object propertyValue = serializer.ToNative(value, property.PropertyType);
                        property.SetValue(instance, propertyValue, null);
                        return;
                    }

                    // Oh, it must be a field then.
                    FieldInfo field = (FieldInfo)propertyOrField;
                    object fieldValue = serializer.ToNative(value, field.FieldType);
                    field.SetValue(instance, fieldValue);
                }
                catch (Exception ex)
                {
                    throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                        "Error while setting value of property or field named '{0}' on class '{1}'",
                        propertyOrField.Name, propertyOrField.ReflectedType.FullName), ex);
                }
            }

            private static void SetDynamicProperty(IDynamic dynamic, IActionScriptSerializer serializer,
                string propertyName, IASValue value)
            {
                try
                {
                    dynamic.SetDynamicProperty(serializer, propertyName, value);
                }
                catch (Exception ex)
                {
                    throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                        "Error while setting value of dynamic property '{0}' on class '{1}'",
                        propertyName, dynamic.GetType().FullName), ex);
                }
            }
        }
    }
}
