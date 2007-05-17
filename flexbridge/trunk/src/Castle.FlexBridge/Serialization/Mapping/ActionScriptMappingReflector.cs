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
using System.Text;
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Collections;

namespace Castle.FlexBridge.Serialization.Mapping
{
    /// <summary>
    /// Derives <see cref="ActionScriptClassMapping" /> data from types
    /// using reflection.
    /// </summary>
    public static class ActionScriptMappingReflector
    {
        /// <summary>
        /// Generates a class mapping for the specified type based on available meta-data.
        /// </summary>
        /// <remarks>
        /// The mapping incorporates declarative meta-data derived from attributes
        /// such as <see cref="ActionScriptClassAttribute" />.  The ActionScript class
        /// layout is determined based on whether the type implements <see cref="IDynamic" />
        /// or <see cref="IExternalizable" />.
        /// If the class is not decorated with a <see cref="ActionScriptClassAttribute" /> then
        /// you should specify a non-null value for <paramref name="classAliasOverride"/> if you want
        /// to include type information in the class mapping.
        /// </remarks>
        /// <param name="nativeType">The class for which to generate the mapping</param>
        /// <param name="classAliasOverride">The class alias to use or null to use the default.
        /// If <paramref name="classAliasOverride"/> is non-null then
        /// it overrides the value specified in the <see cref="ActionScriptClassAttribute" />.  If <paramref name="classAliasOverride"/>
        /// is an empty string then the class mapping will be untyped.</param>
        /// <returns>A new default class mapping for the specified type</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="nativeType"/> is null</exception>
        /// <exception cref="ActionScriptException">Thrown when an error occurs generating the class mapping</exception>
        public static ActionScriptClassMapping CreateClassMapping(Type nativeType, string classAliasOverride)
        {
            if (nativeType == null)
                throw new ArgumentNullException("nativeType");

            ActionScriptClassAttribute classAttribute = GetActionScriptClassAttribute(nativeType);
            return InternalCreateDefaultClassMapping(nativeType, classAliasOverride, classAttribute);
        }

        /// <summary>
        /// Generates class mappings for all public types in the specified assembly
        /// that are decorated with the <see cref="ActionScriptClassAttribute" />.
        /// </summary>
        /// <param name="assembly">The assembly to search for serializable types</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="assembly"/> is null</exception>
        /// <returns>The enumeration of class mappings</returns>
        public static IEnumerable<ActionScriptClassMapping> GetClassMappingsForTypesInAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            foreach (Type type in assembly.GetExportedTypes())
            {
                ActionScriptClassAttribute classAttribute = GetActionScriptClassAttribute(type);
                if (classAttribute != null)
                {
                    ActionScriptClassMapping classMapping = InternalCreateDefaultClassMapping(type, null, classAttribute);
                    yield return classMapping;
                }
            }
        }

        private static ActionScriptClassMapping InternalCreateDefaultClassMapping(Type nativeType, string classAliasOverride,
            ActionScriptClassAttribute classAttribute)
        {
            // Determine the class alias.
            string classAlias;

            if (classAliasOverride != null)
                classAlias = classAliasOverride;
            else if (classAttribute != null)
                classAlias = classAttribute.ClassAlias;
            else
                classAlias = "";

            // Determine the class layout.
            ASClassLayout classLayout = ASClassLayout.Normal;

            if (typeof(IDynamic).IsAssignableFrom(nativeType))
            {
                classLayout = ASClassLayout.Dynamic;
            }

            if (typeof(IExternalizable).IsAssignableFrom(nativeType))
            {
                if (classAlias.Length == 0)
                    throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                        "Type '{0}' implements IExternalizable but does not have a non-empty class alias name which is required by the externalizable serialization protocol.",
                        nativeType.FullName));

                if (classLayout == ASClassLayout.Dynamic)
                    throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                        "Type '{0}' implements IExternalizable as well as IDynamic which is not supported by the externalizable serialization protocol.",
                        nativeType.FullName));

                classLayout = ASClassLayout.Externalizable;
            }

            // Populate the list of property mappings.
            List<ActionScriptPropertyMapping> propertyMappings = new List<ActionScriptPropertyMapping>();
            List<string> memberNames = new List<string>();

            foreach (PropertyInfo property in nativeType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                AddPropertyMappingIfNeeded(property, propertyMappings, memberNames, ref classLayout);
            }

            foreach (FieldInfo field in nativeType.GetFields(BindingFlags.Instance | BindingFlags.Public))
            {
                AddPropertyMappingIfNeeded(field, propertyMappings, memberNames, ref classLayout);
            }

            // Finish up.
            // Convert the lists to arrays to reduce the memory footprint.
            string[] memberNamesArray = memberNames.Count == 0 ? EmptyArray<string>.Instance : memberNames.ToArray();
            ActionScriptPropertyMapping[] propertyMappingsArray = propertyMappings.Count == 0 ?
                EmptyArray<ActionScriptPropertyMapping>.Instance : propertyMappings.ToArray();

            ASClass classDefinition = ASClassCache.GetClass(classAlias, classLayout, memberNamesArray);
            ActionScriptClassMapping classMapping = new ActionScriptClassMapping(nativeType, classDefinition, propertyMappingsArray);

            return classMapping;
        }

        private static void AddPropertyMappingIfNeeded(MemberInfo propertyOrField,
            IList<ActionScriptPropertyMapping> propertyMappings,
            IList<string> memberNames,
            ref ASClassLayout classLayout)
        {
            // Handle ignored properties and fields.
            if (HasActionScriptIgnoreAttribute(propertyOrField))
                return;

            ActionScriptPropertyAttribute propertyAttribute = GetActionScriptPropertyAttribute(propertyOrField);

            // Sanity check for externalizable classes.  They shouldn't have decorated properties or fields.
            if (classLayout == ASClassLayout.Externalizable)
            {
                if (propertyAttribute != null)
                    throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                        "Type '{0}' implements IExternalizable but property or field '{1}' has been decorated with ActionScriptPropertyAttribute which is not supported by the externalizable serialization protocol.",
                        propertyOrField.DeclaringType.FullName,
                        propertyOrField.Name));

                return;
            }

            // Generate the property mapping.
            string propertyName;
            bool isDynamic;

            if (propertyAttribute != null)
            {
                propertyName = propertyAttribute.PropertyName;
                if (propertyName == null)
                    propertyName = propertyOrField.Name;

                isDynamic = propertyAttribute.IsDynamic;
            }
            else
            {
                propertyName = propertyOrField.Name;
                isDynamic = false;
            }

            if (!isDynamic)
                memberNames.Add(propertyName);

            propertyMappings.Add(new ActionScriptPropertyMapping(propertyOrField, propertyName, isDynamic));
        }

        private static ActionScriptClassAttribute GetActionScriptClassAttribute(Type type)
        {
            object[] attributes = type.GetCustomAttributes(typeof(ActionScriptClassAttribute), true);
            if (attributes.Length == 0)
                return null;

            if (attributes.Length != 1)
                throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                    "Type '{0}' has more than one ActionScriptClassAttribute.", type.FullName));

            return (ActionScriptClassAttribute)attributes[0];
        }

        private static ActionScriptPropertyAttribute GetActionScriptPropertyAttribute(MemberInfo propertyOrField)
        {
            object[] attributes = propertyOrField.GetCustomAttributes(typeof(ActionScriptPropertyAttribute), true);
            if (attributes.Length == 0)
                return null;

            if (attributes.Length != 1)
                throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                    "Property or field '{0}' in type '{1}' has more than one ActionScriptClassAttribute.",
                    propertyOrField.Name, propertyOrField.DeclaringType.FullName));

            return (ActionScriptPropertyAttribute)attributes[0];
        }

        private static bool HasActionScriptIgnoreAttribute(MemberInfo propertyOrField)
        {
            return propertyOrField.GetCustomAttributes(typeof(ActionScriptIgnoreAttribute), true).Length != 0;
        }
    }
}
