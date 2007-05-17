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
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.ActionScript
{
    /// <summary>
    /// A visitor for <see cref="IASValue" /> instances.
    /// </summary>
    public interface IASValueVisitor
    {
        /// <summary>
        /// Visits a <see cref="ASTypeKind.Null" /> value.
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        void VisitNull(IActionScriptSerializer serializer);

        /// <summary>
        /// Visits a <see cref="ASTypeKind.Undefined" /> value.
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        void VisitUndefined(IActionScriptSerializer serializer);

        /// <summary>
        /// Visits a <see cref="ASTypeKind.Unsupported" /> value.
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        void VisitUnsupported(IActionScriptSerializer serializer);

        /// <summary>
        /// Visits a <see cref="ASTypeKind.Boolean" /> value.
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="value">The value</param>
        void VisitBoolean(IActionScriptSerializer serializer, bool value);

        /// <summary>
        /// Visits a <see cref="ASTypeKind.Date" /> value.
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="millisecondsSinceEpoch">The number of milliseconds since <see cref="ASConstants.Epoch" /></param>
        /// <param name="timeZoneOffsetMinutes">The timezone offset from Utc in minutes</param>
        void VisitDate(IActionScriptSerializer serializer, double millisecondsSinceEpoch, int timeZoneOffsetMinutes);

        /// <summary>
        /// Visits a <see cref="ASTypeKind.Number" /> value.
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="value">The value</param>
        void VisitNumber(IActionScriptSerializer serializer, double value);

        /// <summary>
        /// Visits a <see cref="ASTypeKind.Int29" /> value.
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="value">The value</param>
        void VisitInt29(IActionScriptSerializer serializer, int value);

        /// <summary>
        /// Visits a <see cref="ASTypeKind.String" /> value.
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="value">The value</param>
        void VisitString(IActionScriptSerializer serializer, string value);

        /// <summary>
        /// Visits a <see cref="ASTypeKind.ByteArray" /> value.
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="length">The length of the array</param>
        /// <param name="segments">An enumeration of byte segments that when traversed in
        /// sequential order yields the entire contents of the byte array</param>
        void VisitByteArray(IActionScriptSerializer serializer, int length, IEnumerable<ArraySegment<byte>> segments);

        /// <summary>
        /// Visits a <see cref="ASTypeKind.Array" /> value.
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="indexedLength">The indexed length of the array</param>
        /// <param name="indexedValues">The indexed values enumeration in index order</param>
        /// <param name="dynamicProperties">The dynamic properties of the array in no particular order</param>
        void VisitArray(IActionScriptSerializer serializer, int indexedLength, IEnumerable<IASValue> indexedValues,
            IEnumerable<KeyValuePair<string, IASValue>> dynamicProperties);

        /// <summary>
        /// Visits a <see cref="ASTypeKind.Object" /> value.
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="class">The class of the object</param>
        /// <param name="memberValues">The member values of the object in member index order</param>
        /// <param name="dynamicProperties">The dynamic properties of the object in no particular order</param>
        /// <param name="externalizableValue">The externalizable value, or null if none</param>
        void VisitObject(IActionScriptSerializer serializer, ASClass @class, IEnumerable<IASValue> memberValues,
            IEnumerable<KeyValuePair<string, IASValue>> dynamicProperties,
            IExternalizable externalizableValue);

        /// <summary>
        /// Visits a <see cref="ASTypeKind.Xml" /> value.
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="xmlString">The XML contents represented as a string</param>
        void VisitXml(IActionScriptSerializer serializer, string xmlString);
    }
}
