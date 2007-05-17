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

namespace Castle.FlexBridge.Serialization
{
    /// <summary>
    /// Indicates that a given property or field should be ignored during ActionScript
    /// serialization.
    /// <seealso cref="ActionScriptClassAttribute"/>
    /// <seealso cref="ActionScriptPropertyAttribute"/>
    /// </summary>
    /// <remarks>
    /// This attribute should not be used in a class that implements <see cref="IExternalizable" />
    /// because the properties and fields of externalizable classes are not mapped directly.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class ActionScriptIgnoreAttribute : Attribute
    {
    }
}
