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
    /// A singleton value used to represent ActionScript values that are not supported
    /// for serialization.  This value may appear when the client attempts to serialize
    /// an object that is not supported by the remoting protocol.
    /// </summary>
    public sealed class ASUnsupported : BaseASValue
    {
        /// <summary>
        /// Gets the singleton Unsupported Value instance.
        /// </summary>
        public static ASUnsupported Value = new ASUnsupported();

        private ASUnsupported()
        {
        }

        /// <inheritdoc />
        public override ASTypeKind Kind
        {
            get { return ASTypeKind.Unsupported; }
        }

        /// <inheritdoc />
        public override void AcceptVisitor(IActionScriptSerializer serializer, IASValueVisitor visitor)
        {
            visitor.VisitUnsupported(serializer);
        }

        /// <inheritdoc />
        public override object GetNativeValue(Type nativeType)
        {
            return nativeType == typeof(ASUnsupported) ? ASUnsupported.Value : null;
        }
    }
}
