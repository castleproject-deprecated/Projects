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
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.ActionScript
{
    /// <summary>
    /// Represents an ActionScript value.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface represent values of various kinds that
    /// are represented in various ways.  The interface does not force a particular
    /// representation upon the value itself.  The value must simply be capable of
    /// providing a visitor with data according to its kind.
    /// </remarks>
    public interface IASValue
    {
        /// <summary>
        /// Returns true if the value is completely initialized.
        /// The value might not be completely initialized in circumstances involving circular
        /// references.  In such a case, an instance might be created as soon as type information
        /// became available but its properties might not be set until later.  That way the instance
        /// can be used to resolve references even though it is incompletely initialized.
        /// </summary>
        bool IsInitialized { get; }

        /// <summary>
        /// Gets the kind of value represented by this instance.
        /// </summary>
        ASTypeKind Kind { get; }

        /// <summary>
        /// Gets the class of the value, if it has one.
        /// Returns null if the value does not have a class.
        /// </summary>
        ASClass Class { get; }

        /// <summary>
        /// Gets flags that describe abstract properties of the contents of a value.
        /// </summary>
        ASValueContentFlags ContentFlags { get; }

        /// <summary>
        /// Invokes the method on the visitor that corresponds with the type of the value.
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="visitor">The visitor</param>
        void AcceptVisitor(IActionScriptSerializer serializer, IASValueVisitor visitor);

        /// <summary>
        /// Gets the value of the object as an instance of the specified type.
        /// If the object already has or can easily obtain a value of type <paramref name="nativeType"/>
        /// then it should return it.  Otherwise it should return null.
        /// </summary>
        /// <remarks>
        /// This is an optimization to avoid unnecessary mapping.
        /// </remarks>
        /// <param name="nativeType">The type of the object, must not be null</param>
        /// <returns>The native value, or null if not supported for the specified type
        /// or if the mapping cannot be performed because <see cref="IsInitialized"/> is false</returns>
        object GetNativeValue(Type nativeType);
    }
}
