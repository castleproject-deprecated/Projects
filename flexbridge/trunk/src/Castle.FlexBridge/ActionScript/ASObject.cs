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
using System.Text;
using Castle.FlexBridge.Collections;
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.ActionScript
{
    /// <summary>
    /// Adapts a dictionary of property keys and values for serialization as an ActionScript object.
    /// </summary>
    public sealed class ASObject : BaseASValue
    {
        private ASClass @class;
        private IList<IASValue> memberValues;
        private IDictionary<string, IASValue> dynamicProperties;

        /// <summary>
        /// Gets a singleton read-only empty object instance.
        /// </summary>
        public static readonly ASObject Empty = new ASObject(ASClass.UntypedDynamicClass, EmptyArray<IASValue>.Instance,
            EmptyDictionary<string, IASValue>.Instance);

        /// <summary>
        /// Creates an uninitialized object.
        /// </summary>
        /// <param name="class">The ActionScript class</param>
        /// <param name="dummy">The value is ignored and is only used to disambiguate the constructor signature</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="class"/> is null
        /// (hint: use <see cref="ASClass.UntypedDynamicClass" /></exception>
        /// <exception cref="ArgumentException">Thrown if the layout of the class is not
        /// <see cref="ASClassLayout.Normal" /> or <see cref="ASClassLayout.Dynamic" /></exception>
        private ASObject(ASClass @class, bool dummy)
        {
            if (@class == null)
                throw new ArgumentNullException("class", "Class must not be null.  Did you mean to use ASClass.UntypedDynamicClass?");
            if (@class.Layout != ASClassLayout.Normal && @class.Layout != ASClassLayout.Dynamic)
                throw new ArgumentException("The class layout must be Normal or Dynamic.", "class");

            this.@class = @class;
        }

        /// <summary>
        /// Creates an untyped dynamic object with no member values or dynamic properties initially set.
        /// </summary>
        /// <remarks>
        /// The object will have an empty read-only list of member values and a read-write
        /// dictionary of dynamic properties.
        /// </remarks>
        public ASObject()
        {
            this.@class = ASClass.UntypedDynamicClass;
            this.memberValues = EmptyArray<IASValue>.Instance;
            this.dynamicProperties = new Dictionary<string, IASValue>();
        }

        /// <summary>
        /// Creates an object with the specified class and no member values or dynamic properties initially set.
        /// </summary>
        /// <remarks>
        /// The object will have a fixed-length list of member values with the same length as the
        /// list of member names defined by the class.
        /// 
        /// The object will have a read-only empty dynamic property dictionary if its
        /// class layout is <see cref="ASClassLayout.Normal" />, otherwise it will have
        /// a read-write dictionary.
        /// </remarks>
        /// <param name="class">The ActionScript class</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="class"/> is null
        /// (hint: use <see cref="ASClass.UntypedDynamicClass" /></exception>
        /// <exception cref="ArgumentException">Thrown if the layout of the class is not
        /// <see cref="ASClassLayout.Normal" /> or <see cref="ASClassLayout.Dynamic" /></exception>
        public ASObject(ASClass @class)
        {
            if (@class == null)
                throw new ArgumentNullException("class", "Class must not be null.  Did you mean to use ASClass.UntypedDynamicClass?");
            if (@class.Layout != ASClassLayout.Normal && @class.Layout != ASClassLayout.Dynamic)
                throw new ArgumentException("The class layout must be Normal or Dynamic.", "class");

            this.@class = @class;
            this.memberValues = @class.MemberNames.Count != 0 ? new IASValue[@class.MemberNames.Count] : EmptyArray<IASValue>.Instance;
            this.dynamicProperties = @class.Layout == ASClassLayout.Dynamic ? new Dictionary<string, IASValue>() : EmptyDictionary<string, IASValue>.Instance;
        }

        /// <summary>
        /// Creates an object with the specified class, member values and dynamic properties.
        /// </summary>
        /// <remarks>
        /// The object will have a read-only empty dynamic property dictionary if its
        /// class layout is <see cref="ASClassLayout.Normal" />, otherwise it will have
        /// a read-write dictionary.
        /// </remarks>
        /// <param name="class">The ActionScript class</param>
        /// <param name="memberValues">The member values of the object</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="class"/> is null
        /// (hint: use <see cref="ASClass.UntypedDynamicClass" /></exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="memberValues"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the layout of the class is not
        /// <see cref="ASClassLayout.Normal" /> or <see cref="ASClassLayout.Dynamic" /></exception>
        /// <exception cref="ArgumentException">Thrown if the number of members defined by the class
        /// differs from the count of <paramref name="memberValues"/></exception>
        public ASObject(ASClass @class, IList<IASValue> memberValues)
        {
            if (@class == null)
                throw new ArgumentNullException("class", "Class must not be null.  Did you mean to use ASClass.UntypedDynamicClass?");
            if (memberValues == null)
                throw new ArgumentNullException("memberValues");
            if (@class.Layout != ASClassLayout.Normal && @class.Layout != ASClassLayout.Dynamic)
                throw new ArgumentException("The class layout must be Normal or Dynamic.", "class");
            if (@class.MemberNames.Count != memberValues.Count)
                throw new ArgumentException("The number of member values provided must equal the number of members defined by the class.", "class");

            this.@class = @class;
            this.memberValues = memberValues;
            this.dynamicProperties = @class.Layout == ASClassLayout.Dynamic ? new Dictionary<string, IASValue>() : EmptyDictionary<string, IASValue>.Instance;
        }

        /// <summary>
        /// Creates an object with the specified class, member values and dynamic properties.
        /// </summary>
        /// <param name="class">The ActionScript class</param>
        /// <param name="memberValues">The member values of the object</param>
        /// <param name="dynamicProperties">The dynamic properties of the object</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="class"/> is null
        /// (hint: use <see cref="ASClass.UntypedDynamicClass" /></exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="memberValues"/> or
        /// <paramref name="dynamicProperties"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the layout of the class is not
        /// <see cref="ASClassLayout.Normal" /> or <see cref="ASClassLayout.Dynamic" /></exception>
        /// <exception cref="ArgumentException">Thrown if the number of members defined by the class
        /// differs from the count of <paramref name="memberValues"/></exception>
        public ASObject(ASClass @class, IList<IASValue> memberValues, IDictionary<string, IASValue> dynamicProperties)
        {
            if (@class == null)
                throw new ArgumentNullException("class", "Class must not be null.  Did you mean to use ASClass.UntypedDynamicClass?");
            if (memberValues == null)
                throw new ArgumentNullException("memberValues");
            if (dynamicProperties == null)
                throw new ArgumentNullException("properties");
            if (@class.Layout != ASClassLayout.Normal && @class.Layout != ASClassLayout.Dynamic)
                throw new ArgumentException("The class layout must be Normal or Dynamic.", "class");
            if (@class.MemberNames.Count != memberValues.Count)
                throw new ArgumentException("The number of member values provided must equal the number of members defined by the class.", "class");

            this.@class = @class;
            this.memberValues = memberValues;
            this.dynamicProperties = dynamicProperties;
        }

        /// <summary>
        /// Gets the ActionScript class, never null.
        /// </summary>
        public override ASClass Class
        {
            get { return @class; }
        }

        /// <summary>
        /// Gets the member values of the ActionScript object in the same order
        /// they appear in the member names collection of the class.
        /// </summary>
        /// <remarks>
        /// This collection must have the same number of values as the number of
        /// members defined by the class.
        /// </remarks>
        /// <exception cref="InvalidOperationException">Thrown if the object has not been initialized</exception>
        public IList<IASValue> MemberValues
        {
            get
            {
                ThrowIfNotInitialized();
                return memberValues;
            }
        }

        /// <summary>
        /// Gets the dynamic properties of the object.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the object has not been initialized</exception>
        public IDictionary<string, IASValue> DynamicProperties
        {
            get
            {
                ThrowIfNotInitialized();
                return dynamicProperties;
            }
        }

        /// <summary>
        /// Gets or sets a member value or dynamic property.
        /// If a member with the specified name has been defined, gets or
        /// sets its value.  Otherwise gets or sets a dynamic property value.
        /// </summary>
        /// <param name="memberOrDynamicPropertyName">The member or dynamic property name</param>
        /// <returns>The value</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="memberOrDynamicPropertyName"/> is null</exception>
        /// <exception cref="KeyNotFoundException">Thrown if the member or dynamic property was not found during a lookup</exception>
        /// <exception cref="NotSupportedException">Thrown if the value could not be set because the member value list or dynamic property dictionary is readonly</exception>
        /// <exception cref="InvalidOperationException">Thrown if the object has not been initialized</exception>
        public IASValue this[string memberOrDynamicPropertyName]
        {
            get
            {
                if (memberOrDynamicPropertyName == null)
                    throw new ArgumentNullException("memberOrDynamicPropertyName");

                ThrowIfNotInitialized();

                int memberIndex = @class.MemberNames.IndexOf(memberOrDynamicPropertyName);
                if (memberIndex >= 0)
                    return memberValues[memberIndex];
                else
                    return dynamicProperties[memberOrDynamicPropertyName];
            }
            set
            {
                if (memberOrDynamicPropertyName == null)
                    throw new ArgumentNullException("memberOrDynamicPropertyName");

                ThrowIfNotInitialized();

                int memberIndex = @class.MemberNames.IndexOf(memberOrDynamicPropertyName);
                if (memberIndex >= 0)
                    memberValues[memberIndex] = value;
                else
                    dynamicProperties[memberOrDynamicPropertyName] = value;
            }
        }

        /// <summary>
        /// Creates an instance whose initialization is to be deferred.
        /// </summary>
        /// <remarks>
        /// This special case is used to resolve circular references during the construction of
        /// object graphs.  The object should not be used until its properties have been initialized.
        /// </remarks>
        /// <param name="class">The ActionScript class</param>
        /// <returns>The uninitialized object</returns>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="class"/> is null
        /// (hint: use <see cref="ASClass.UntypedDynamicClass" /></exception>
        /// <exception cref="ArgumentException">Thrown if the layout of the class is not
        /// <see cref="ASClassLayout.Normal" /> or <see cref="ASClassLayout.Dynamic" /></exception>
        public static ASObject CreateUninitializedInstance(ASClass @class)
        {
            return new ASObject(@class, false);
        }

        /// <summary>
        /// Sets the properties of an instance created by <see cref="CreateUninitializedInstance" />.
        /// </summary>
        /// <remarks>
        /// This special case is used to resolve circular references during the construction of
        /// object graphs.  The object should not be used until its properties have been initialized.
        /// </remarks>
        /// <param name="memberValues">The member values of the object</param>
        /// <param name="dynamicProperties">The dynamic properties of the object</param>
        /// <exception cref="InvalidOperationException">Thrown if the object has already been initialized</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="memberValues"/> or
        /// <paramref name="dynamicProperties"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if the number of members defined by the class
        /// differs from the count of <paramref name="memberValues"/></exception>
        public void SetProperties(IList<IASValue> memberValues, IDictionary<string, IASValue> dynamicProperties)
        {
            if (IsInitialized)
                throw new InvalidOperationException("The object's properties may not be set once initialized.");

            if (memberValues == null)
                throw new ArgumentNullException("memberValues");
            if (dynamicProperties == null)
                throw new ArgumentNullException("properties");
            if (@class.MemberNames.Count != memberValues.Count)
                throw new ArgumentException("The number of member values provided must equal the number of members defined by the class.", "class");

            this.memberValues = memberValues;
            this.dynamicProperties = dynamicProperties;
        }

        public override bool IsInitialized
        {
            get { return memberValues != null; }
        }

        public override ASTypeKind Kind
        {
            get { return ASTypeKind.Object; }
        }

        public override ASValueContentFlags ContentFlags
        {
            get
            {
                ThrowIfNotInitialized();
                return dynamicProperties.Count != 0 ? ASValueContentFlags.HasDynamicProperties : ASValueContentFlags.None;
            }
        }

        public override void AcceptVisitor(IActionScriptSerializer serializer, IASValueVisitor visitor)
        {
            ThrowIfNotInitialized();

            visitor.VisitObject(serializer, @class, memberValues, dynamicProperties, null);
        }

        public override object GetNativeValue(Type nativeType)
        {
            if (IsInitialized)
            {
                // Note: We can only return the inner contents of this object when we're
                //       certain we won't want to apply any mapping to its elements at all.
                //       It's a pretty safe bet that this is the case if the native type
                //       is an array or list type directly involving IASValue but otherwise
                //       we can't really be sure.  So we check to make sure the native type
                //       really involves IASValue -- it can't be just "object" or something.
                if (memberValues.Count == 0
                    && nativeType.IsInstanceOfType(dynamicProperties)
                    && typeof(IDictionary<string, IASValue>).IsAssignableFrom(nativeType))
                    return dynamicProperties;
            }

            return null;
        }
    }
}
