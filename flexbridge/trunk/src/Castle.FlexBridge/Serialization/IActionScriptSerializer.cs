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

namespace Castle.FlexBridge.Serialization
{
    /// <summary>
    /// The ActionScript serializer provides support for mapping native object graphs
    /// into ActionScript equivalents and vice-versa.  It supports extension by registering
    /// ActionScript mapper implementations for native types.
    /// </summary>
    public interface IActionScriptSerializer
    {
        /// <summary>
        /// Converts the native value to an ActionScript value.
        /// </summary>
        /// <param name="nativeValue">The native value to convert, possibly null</param>
        /// <returns>The ActionScript value or null if <paramref name="nativeValue"/> was null</returns>
        IASValue ToASValue(object nativeValue);

        /// <summary>
        /// Converts the ActionScript value to a native value of the specified type.
        /// If <paramref name="nativeType"/> is null, then returns the default native value
        /// of the object which is the form in which it would like to present its contents
        /// in native code in the absence of additional type information or mapping such as
        /// when the value is assigned to a field of type <see cref="Object" />.
        /// If <paramref name="nativeType"/> is not null then attempts to map the value
        /// to that particular type.  If this mapping is not possible throws
        /// <see cref="ActionScriptException" />.
        /// </summary>
        /// <param name="asValue">The ActionScript value to convert, possibly null</param>
        /// <param name="nativeType">The native type to produce or null to obtain the default
        /// native value of the object</param>
        /// <returns>The native value or null if <paramref name="asValue"/> was null</returns>
        /// <exception cref="ActionScriptException">Thrown if the mapping is not supported.</exception>
        /// <exception cref="ActionScriptException">Thrown if the value's <see cref="IASValue.IsInitialized" /> property is false
        /// and a mapping is required</exception>
        object ToNative(IASValue asValue, Type nativeType);

        /// <summary>
        /// Creates an instance of an externalizable object with the specified class alias.
        /// </summary>
        /// <param name="classAlias">The class alias</param>
        /// <returns>The externalizable object</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="classAlias"/> is null</exception>
        /// <exception cref="ActionScriptException">Thrown if the mapping is not supported</exception>
        IExternalizable CreateExternalizableInstance(string classAlias);

        /// <summary>
        /// Gets the class alias associated with the specified type or an empty string if none.
        /// </summary>
        /// <param name="nativeType">The type</param>
        /// <returns>The class alias associated with the type, or an empty string if none</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="nativeType"/> is null</exception>
        string GetClassAlias(Type nativeType);
    }
}
