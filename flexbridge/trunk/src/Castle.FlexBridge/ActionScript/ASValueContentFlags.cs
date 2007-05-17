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
    /// Flags that describe abstract properties of the contents of a value.
    /// </summary>
    [Flags]
    public enum ASValueContentFlags
    {
        /// <summary>
        /// Indicates the absence of any special content flags.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies that the value is an array that has at least 1 indexed value.
        /// </summary>
        /// <remarks>
        /// If the array's contents are generated on demand when visited, then this property
        /// should return true if it is possible that the array will has a non-zero indexed length
        /// when that happens.
        /// </remarks>
        HasIndexedValues = 1,

        /// <summary>
        /// Specifies that the value is an array or object that has at least 1 dynamic property.
        /// </summary>
        /// <remarks>
        /// If the array's or object's contents are generated on demand when visited, then this property
        /// should return true if it is possible that the array will has a non-zero number of dynamic
        /// properties when that happens.
        /// </remarks>
        HasDynamicProperties = 2
    }
}
