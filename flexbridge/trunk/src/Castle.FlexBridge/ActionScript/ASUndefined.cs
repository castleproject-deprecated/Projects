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
    /// A singleton value used to represent Undefined ActionScript values.
    /// This value may appear when an uninitialized variable is serialized.
    /// </summary>
    public sealed class ASUndefined : BaseASValue
    {
        /// <summary>
        /// Gets the singleton Undefined Value instance.
        /// </summary>
        public static ASUndefined Value = new ASUndefined();

        private ASUndefined()
        {
        }

        public override ASTypeKind Kind
        {
            get { return ASTypeKind.Undefined; }
        }

        public override void AcceptVisitor(IActionScriptSerializer serializer, IASValueVisitor visitor)
        {
            visitor.VisitUndefined(serializer);
        }

        public override object GetNativeValue(Type nativeType)
        {
            return nativeType == typeof(ASUndefined) ? ASUndefined.Value : null;
        }
    }
}
