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
    /// Used to get and set dynamic properties during ActionScript serialization.
    /// </summary>
    /// <remarks>
    /// When a class implements this interface, it indicates that it supports
    /// "dynamic" properties that are represented as key/value pairs.  This
    /// is the .Net counterpart of the ActionScript "dynamic" class modifier.
    /// </remarks>
    public interface IDynamic
    {
        /// <summary>
        /// Gets the dynamic properties of the class as an enumeration of key/value pairs.
        /// </summary>
        /// <param name="serializer">The ActionScript serializer to use</param>
        /// <returns>The enumeration of property key/value pairs, never null</returns>
        IEnumerable<KeyValuePair<string, IASValue>> GetDynamicProperties(IActionScriptSerializer serializer);

        /// <summary>
        /// Set a dynamic property of the class.
        /// </summary>
        /// <param name="serializer">The ActionScript serializer to use</param>
        /// <param name="propertyName">The name of the property to set, never null</param>
        /// <param name="value">The value to set</param>
        void SetDynamicProperty(IActionScriptSerializer serializer, string propertyName, IASValue value);
    }
}
