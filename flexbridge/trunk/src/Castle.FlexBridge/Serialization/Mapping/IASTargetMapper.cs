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
    /// Maps from native .Net values to their corresponding <see cref="IASValue" /> representations.
    /// The mapper instances may be cached by other code based on the criteria used to obtain them
    /// so they must be reusable and thread-safe.
    /// </summary>
    public interface IASTargetMapper
    {
        /// <summary>
        /// Converts the native value to an ActionScript value.
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="nativeValue">The native value to convert</param>
        /// <returns>The ActionScript value</returns>
        /// <exception cref="ActionScriptException">Thrown if the mapping is not supported</exception>
        IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue);
    }
}