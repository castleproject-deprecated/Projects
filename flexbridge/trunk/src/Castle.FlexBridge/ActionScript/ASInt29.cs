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
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.ActionScript
{
    /// <summary>
    /// Represents a 29-bit signed integer value.
    /// </summary>
    public sealed class ASInt29 : BaseASValue
    {
        private int value;

        /// <summary>
        /// Creates an instance of a 29-bit signed integer value.
        /// </summary>
        /// <param name="value">The value</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the integer is less than
        /// <see cref="ASConstants.Int29MinValue" /> or larger than <see cref="ASConstants.Int29MaxValue" /></exception>
        public ASInt29(int value)
        {
            if (value < ASConstants.Int29MinValue || value > ASConstants.Int29MaxValue)
                throw new ArgumentOutOfRangeException("value");

            this.value = value;
        }

        /// <summary>
        /// Gets the value as a 29-bit signed integer.
        /// </summary>
        public int Value
        {
            get { return value; }
        }

        public override ASTypeKind Kind
        {
            get { return ASTypeKind.Int29; }
        }

        public override void AcceptVisitor(IActionScriptSerializer serializer, IASValueVisitor visitor)
        {
            visitor.VisitInt29(serializer, value);
        }

        public override object GetNativeValue(Type nativeType)
        {
            return nativeType == typeof(int) ? (object)value : null;
        }

        public override bool Equals(object obj)
        {
            ASInt29 other = obj as ASInt29;
            return other != null && value == other.value;
        }

        public override int GetHashCode()
        {
            return value;
        }
    }
}