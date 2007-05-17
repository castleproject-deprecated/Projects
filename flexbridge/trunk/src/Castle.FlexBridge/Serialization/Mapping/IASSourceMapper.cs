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
using Castle.FlexBridge.ActionScript;

namespace Castle.FlexBridge.Serialization.Mapping
{
    /// <summary>
    /// Maps from <see cref="IASValue" /> values to their corresponding native .Net representations.
    /// The mapper instances may be cached by other code based on the criteria used to obtain them
    /// so they must be reusable and thread-safe.
    /// </summary>
    public interface IASSourceMapper
    {
        /// <summary>
        /// Converts the ActionScript value to a native value of the specified type.
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="asValue">The ActionScript value to convert</param>
        /// <param name="nativeType">The native type to produce</param>
        /// <returns>The native value</returns>
        /// <exception cref="ActionScriptException">Thrown if the mapping is not supported</exception>
        object ToNative(IActionScriptSerializer serializer, IASValue asValue, Type nativeType);
    }
}