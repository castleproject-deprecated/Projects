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
using Castle.FlexBridge.ActionScript;

namespace Castle.FlexBridge.Serialization.Mapping
{
    /// <summary>
    /// Abstract base implementation of <see cref="IASSourceMapper" /> and <see cref="IASTargetMapper" />.
    /// The default implementations throw <see cref="ActionScriptException" /> for unsupported mappings.
    /// </summary>
    public abstract class BaseASMapper : IASSourceMapper, IASTargetMapper
    {
        /// <summary>
        /// Converts the native value to an ActionScript value.
        /// </summary>
        /// <remarks>
        /// The default implementation throws an <see cref="ActionScriptException" />.
        /// </remarks>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="nativeValue">The native value to convert</param>
        /// <returns>The ActionScript value</returns>
        /// <exception cref="ActionScriptException">Thrown if the mapping is not supported</exception>
        public virtual IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
        {
            throw ToASValueNotSupported(nativeValue);
        }

        /// <summary>
        /// Converts the ActionScript value to a native value of the specified type.
        /// </summary>
        /// <remarks>
        /// The default implementation visits the <paramref name="asValue"/> and forwards the
        /// data to one of the specialized MapToXXX methods accordingly.
        /// </remarks>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="asValue">The ActionScript value to convert</param>
        /// <param name="nativeType">The native type to produce</param>
        /// <returns>The native value</returns>
        /// <exception cref="ActionScriptException">Thrown if the mapping is not supported</exception>
        public virtual object ToNative(IActionScriptSerializer serializer, IASValue asValue, Type nativeType)
        {
            Visitor visitor = new Visitor(this, nativeType);
            asValue.AcceptVisitor(serializer, visitor);
            return visitor.Result;
        }

        /// <summary>
        /// Maps a <see cref="ASTypeKind.Null" /> value.
        /// </summary>
        /// <remarks>
        /// The default implementation throws an <see cref="ActionScriptException" />.
        /// </remarks>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="nativeType">The native type to produce</param>
        protected virtual object MapNullToNative(IActionScriptSerializer serializer, Type nativeType)
        {
            throw ToNativeNotSupported(ASTypeKind.Null, nativeType);
        }

        /// <summary>
        /// Maps a <see cref="ASTypeKind.Undefined" /> value.
        /// </summary>
        /// <remarks>
        /// The default implementation throws an <see cref="ActionScriptException" />.
        /// </remarks>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="nativeType">The native type to produce</param>
        protected virtual object MapUndefinedToNative(IActionScriptSerializer serializer, Type nativeType)
        {
            throw ToNativeNotSupported(ASTypeKind.Undefined, nativeType);
        }

        /// <summary>
        /// Maps a <see cref="ASTypeKind.Unsupported" /> value.
        /// </summary>
        /// <remarks>
        /// The default implementation throws an <see cref="ActionScriptException" />.
        /// </remarks>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="nativeType">The native type to produce</param>
        protected virtual object MapUnsupportedToNative(IActionScriptSerializer serializer, Type nativeType)
        {
            throw ToNativeNotSupported(ASTypeKind.Unsupported, nativeType);
        }

        /// <summary>
        /// Maps a <see cref="ASTypeKind.Boolean" /> value.
        /// </summary>
        /// <remarks>
        /// The default implementation throws an <see cref="ActionScriptException" />.
        /// </remarks>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="nativeType">The native type to produce</param>
        /// <param name="value">The value</param>
        protected virtual object MapBooleanToNative(IActionScriptSerializer serializer, Type nativeType, bool value)
        {
            throw ToNativeNotSupported(ASTypeKind.Boolean, nativeType);
        }

        /// <summary>
        /// Maps a <see cref="ASTypeKind.Date" /> value.
        /// </summary>
        /// <remarks>
        /// The default implementation throws an <see cref="ActionScriptException" />.
        /// </remarks>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="nativeType">The native type to produce</param>
        /// <param name="millisecondsSinceEpoch">The number of milliseconds since <see cref="ASConstants.Epoch" /></param>
        /// <param name="timeZoneOffsetMinutes">The timezone offset from Utc in minutes</param>
        protected virtual object MapDateToNative(IActionScriptSerializer serializer, Type nativeType,
            double millisecondsSinceEpoch, int timeZoneOffsetMinutes)
        {
            throw ToNativeNotSupported(ASTypeKind.Date, nativeType);
        }

        /// <summary>
        /// Maps a <see cref="ASTypeKind.Number" /> value.
        /// </summary>
        /// <remarks>
        /// The default implementation throws an <see cref="ActionScriptException" />.
        /// </remarks>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="nativeType">The native type to produce</param>
        /// <param name="value">The value</param>
        protected virtual object MapNumberToNative(IActionScriptSerializer serializer, Type nativeType, double value)
        {
            throw ToNativeNotSupported(ASTypeKind.Number, nativeType);
        }

        /// <summary>
        /// Maps a <see cref="ASTypeKind.Int29" /> value.
        /// </summary>
        /// <remarks>
        /// The default implementation throws an <see cref="ActionScriptException" />.
        /// </remarks>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="nativeType">The native type to produce</param>
        /// <param name="value">The value</param>
        protected virtual object MapInt29ToNative(IActionScriptSerializer serializer, Type nativeType, int value)
        {
            throw ToNativeNotSupported(ASTypeKind.Int29, nativeType);
        }

        /// <summary>
        /// Maps a <see cref="ASTypeKind.String" /> value.
        /// </summary>
        /// <remarks>
        /// The default implementation throws an <see cref="ActionScriptException" />.
        /// </remarks>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="nativeType">The native type to produce</param>
        /// <param name="value">The value</param>
        protected virtual object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
        {
            throw ToNativeNotSupported(ASTypeKind.String, nativeType);
        }

        /// <summary>
        /// Maps a <see cref="ASTypeKind.ByteArray" /> value.
        /// </summary>
        /// <remarks>
        /// The default implementation throws an <see cref="ActionScriptException" />.
        /// </remarks>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="nativeType">The native type to produce</param>
        /// <param name="length">The length of the array</param>
        /// <param name="segments">An enumeration of byte segments that when traversed in
        /// sequential order yields the entire contents of the byte array</param>
        protected virtual object MapByteArrayToNative(IActionScriptSerializer serializer, Type nativeType, int length,
            IEnumerable<ArraySegment<byte>> segments)
        {
            throw ToNativeNotSupported(ASTypeKind.ByteArray, nativeType);
        }

        /// <summary>
        /// Maps a <see cref="ASTypeKind.Array" /> value.
        /// </summary>
        /// <remarks>
        /// The default implementation throws an <see cref="ActionScriptException" />.
        /// </remarks>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="nativeType">The native type to produce</param>
        /// <param name="indexedLength">The indexed length of the array</param>
        /// <param name="indexedValues">The indexed values enumeration in index order</param>
        /// <param name="dynamicProperties">The dynamic properties of the array in no particular order</param>
        protected virtual object MapArrayToNative(IActionScriptSerializer serializer, Type nativeType, int indexedLength,
            IEnumerable<IASValue> indexedValues,
            IEnumerable<KeyValuePair<string, IASValue>> dynamicProperties)
        {
            throw ToNativeNotSupported(ASTypeKind.Array, nativeType);
        }

        /// <summary>
        /// Maps a <see cref="ASTypeKind.Object" /> value.
        /// </summary>
        /// <remarks>
        /// The default implementation throws an <see cref="ActionScriptException" />.
        /// </remarks>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="nativeType">The native type to produce</param>
        /// <param name="class">The class of the object</param>
        /// <param name="memberValues">The member values of the object in member index order</param>
        /// <param name="dynamicProperties">The dynamic properties of the object in no particular order</param>
        /// <param name="externalizableValue">The externalizable value, or null if none</param>
        protected virtual object MapObjectToNative(IActionScriptSerializer serializer, Type nativeType, ASClass @class,
            IEnumerable<IASValue> memberValues,
            IEnumerable<KeyValuePair<string, IASValue>> dynamicProperties,
            IExternalizable externalizableValue)
        {
            throw ToNativeNotSupported(ASTypeKind.Object, nativeType);
        }

        /// <summary>
        /// Maps a <see cref="ASTypeKind.Xml" /> value.
        /// </summary>
        /// <remarks>
        /// The default implementation throws an <see cref="ActionScriptException" />.
        /// </remarks>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="nativeType">The native type to produce</param>
        /// <param name="xmlString">The XML contents represented as a string</param>
        protected virtual object MapXmlToNative(IActionScriptSerializer serializer, Type nativeType, string xmlString)
        {
            throw ToNativeNotSupported(ASTypeKind.Xml, nativeType);
        }

        /// <summary>
        /// Creates an exception to indicate that the specified mapping to a native value is not supported.
        /// </summary>
        /// <param name="kind">The kind of ActionScript value to be mapped</param>
        /// <param name="nativeType">The native type</param>
        /// <returns>A suitable exception</returns>
        protected static ActionScriptException ToNativeNotSupported(ASTypeKind kind, Type nativeType)
        {
            return new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                "This mapper does not support mapping from an ActionScript '{0}' to an instance of '{1}'.", kind, nativeType.FullName));
        }

        /// <summary>
        /// Creates an exception to indicate that the specified mapping to an ActionScript value is not supported.
        /// </summary>
        /// <param name="nativeValue">The native value to be mapped</param>
        /// <returns>A suitable exception</returns>
        protected static ActionScriptException ToASValueNotSupported(object nativeValue)
        {
            return new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                "This mapper does not support mapping from an instance of '{0}' to an ActionScript value.", nativeValue.GetType().FullName));
        }

        /// <summary>
        /// A special visitor that redirects back to the mapper.
        /// </summary>
        private sealed class Visitor : IASValueVisitor
        {
            private BaseASMapper mapper;
            private Type nativeType;
            private object result;

            /// <summary>
            /// Creates a visitor for the specified mapper.
            /// </summary>
            /// <param name="mapper"></param>
            /// <param name="nativeType"></param>
            public Visitor(BaseASMapper mapper, Type nativeType)
            {
                this.mapper = mapper;
                this.nativeType = nativeType;
            }

            /// <summary>
            /// Gets the result.
            /// </summary>
            public object Result
            {
                get { return result; }
            }

            void IASValueVisitor.VisitNull(IActionScriptSerializer serializer)
            {
                result = mapper.MapNullToNative(serializer, nativeType);
            }

            void IASValueVisitor.VisitUndefined(IActionScriptSerializer serializer)
            {
                result = mapper.MapUndefinedToNative(serializer, nativeType);
            }

            void IASValueVisitor.VisitUnsupported(IActionScriptSerializer serializer)
            {
                result = mapper.MapUnsupportedToNative(serializer, nativeType);
            }

            void IASValueVisitor.VisitBoolean(IActionScriptSerializer serializer, bool value)
            {
                result = mapper.MapBooleanToNative(serializer, nativeType, value);
            }

            void IASValueVisitor.VisitDate(IActionScriptSerializer serializer, double millisecondsSinceEpoch, int timeZoneOffsetMinutes)
            {
                result = mapper.MapDateToNative(serializer, nativeType, millisecondsSinceEpoch, timeZoneOffsetMinutes);
            }

            void IASValueVisitor.VisitNumber(IActionScriptSerializer serializer, double value)
            {
                result = mapper.MapNumberToNative(serializer, nativeType, value);
            }

            void IASValueVisitor.VisitInt29(IActionScriptSerializer serializer, int value)
            {
                result = mapper.MapInt29ToNative(serializer, nativeType, value);
            }

            void IASValueVisitor.VisitString(IActionScriptSerializer serializer, string value)
            {
                result = mapper.MapStringToNative(serializer, nativeType, value);
            }

            void IASValueVisitor.VisitByteArray(IActionScriptSerializer serializer, int length, IEnumerable<ArraySegment<byte>> segments)
            {
                result = mapper.MapByteArrayToNative(serializer, nativeType, length, segments);
            }

            void IASValueVisitor.VisitArray(IActionScriptSerializer serializer, int indexedLength, IEnumerable<IASValue> indexedValues,
                IEnumerable<KeyValuePair<string, IASValue>> dynamicProperties)
            {
                result = mapper.MapArrayToNative(serializer, nativeType, indexedLength, indexedValues, dynamicProperties);
            }

            void IASValueVisitor.VisitObject(IActionScriptSerializer serializer, ASClass @class, IEnumerable<IASValue> memberValues,
                IEnumerable<KeyValuePair<string, IASValue>> dynamicProperties, IExternalizable externalizableValue)
            {
                result = mapper.MapObjectToNative(serializer, nativeType, @class, memberValues, dynamicProperties, externalizableValue);
            }

            void IASValueVisitor.VisitXml(IActionScriptSerializer serializer, string xmlString)
            {
                result = mapper.MapXmlToNative(serializer, nativeType, xmlString);
            }
        }
    }
}