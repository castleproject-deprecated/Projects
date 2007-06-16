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
    /// Represents a boolean value.
    /// </summary>
    public sealed class ASBoolean : BaseASValue
    {
        private bool value;

        /// <summary>
        /// Gets the singleton boolean instance with value true.
        /// </summary>
        public static readonly ASBoolean True = new ASBoolean(true);

        /// <summary>
        /// Gets the singleton boolean instance with value false.
        /// </summary>
        public static readonly ASBoolean False = new ASBoolean(false);

        private ASBoolean(bool value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets a <see cref="ASBoolean" /> value for the given value. 
        /// </summary>
        /// <param name="value">The boolean value</param>
        /// <returns>The boolean value instance</returns>
        public static ASBoolean ToASBoolean(bool value)
        {
            return value ? True : False;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public bool Value
        {
            get { return value; }
        }

        /// <inheritdoc />
        public override ASTypeKind Kind
        {
            get { return ASTypeKind.Boolean; }
        }

        /// <inheritdoc />
        public override void AcceptVisitor(IActionScriptSerializer serializer, IASValueVisitor visitor)
        {
            visitor.VisitBoolean(serializer, value);
        }

        /// <inheritdoc />
        public override object GetNativeValue(Type nativeType)
        {
            return nativeType == typeof(bool) ? (object)value : null;
        }
    }
}
