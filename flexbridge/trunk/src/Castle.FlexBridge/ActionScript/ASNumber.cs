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
    /// Represents a double-precision floating point value.
    /// </summary>
    public sealed class ASNumber : BaseASValue
    {
        private double value;

        /// <summary>
        /// Creates an instance of a number.
        /// </summary>
        /// <param name="value">The value</param>
        public ASNumber(double value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the value as a double-precision float.
        /// </summary>
        public double Value
        {
            get { return value; }
        }

        /// <inheritdoc />
        public override ASTypeKind Kind
        {
            get { return ASTypeKind.Number; }
        }

        /// <inheritdoc />
        public override void AcceptVisitor(IActionScriptSerializer serializer, IASValueVisitor visitor)
        {
            visitor.VisitNumber(serializer, value);
        }

        /// <inheritdoc />
        public override object GetNativeValue(Type nativeType)
        {
            return nativeType == typeof(double) ? (object)value : null;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            ASNumber other = obj as ASNumber;
            return other != null && value == other.value;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}