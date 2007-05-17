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
    /// Used by <see cref="ASNativeObject" /> to map the properties of an object
    /// to ActionScript on demand.  The <see cref="ASNativeObject" /> delegates
    /// the task of invoking the appropriate method of <see cref="IASValueVisitor" />
    /// to this mapper.
    /// </summary>
    public interface IASNativeObjectMapper
    {
        /// <summary>
        /// Invokes the method on the visitor that corresponds with the type of the value
        /// once it has been mapped to ActionScript.
        /// </summary>
        /// <param name="serializer">The serializer to use</param>
        /// <param name="class">The class of the <see cref="ASNativeObject" /></param>
        /// <param name="nativeObject">The native object</param>
        /// <param name="visitor">The visitor</param>
        void AcceptVisitor(IActionScriptSerializer serializer, ASClass @class,
            object nativeObject, IASValueVisitor visitor);

        /// <summary>
        /// Gets flags that describe abstract properties of the contents of a value when serialized.
        /// </summary>
        /// <param name="nativeObject">The native object</param>
        /// <returns>The content flags</returns>
        ASValueContentFlags GetContentFlags(object nativeObject);
    }
}
