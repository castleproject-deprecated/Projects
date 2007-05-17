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

namespace Castle.FlexBridge.ActionScript
{
    /// <summary>
    /// An enumeration of the kinds of ActionScript types.
    /// The kind of a type describes a given shape it must have.  For instance, arrays
    /// have length, indexed values and mixed values.
    /// </summary>
    public enum ASTypeKind
    {
        /// <summary>
        /// A null reference.
        /// </summary>
        Null,

        /// <summary>
        /// An undefined reference.
        /// </summary>
        Undefined,

        /// <summary>
        /// An unsupported object reference.  (AMF0 only)
        /// </summary>
        Unsupported,

        /// <summary>
        /// A boolean value.
        /// </summary>
        Boolean,

        /// <summary>
        /// A 29-bit signed integer.
        /// </summary>
        Int29,

        /// <summary>
        /// A double-precision floating point number.
        /// </summary>
        Number,

        /// <summary>
        /// A string.
        /// </summary>
        String,

        /// <summary>
        /// A date.
        /// </summary>
        Date,

        /// <summary>
        /// An array with indexed and mixed values.
        /// </summary>
        Array,

        /// <summary>
        /// A byte array.
        /// </summary>
        ByteArray,

        /// <summary>
        /// An object with static members and dynamic properties.
        /// </summary>
        Object,

        /// <summary>
        /// An XML element.
        /// </summary>
        Xml
    }
}
