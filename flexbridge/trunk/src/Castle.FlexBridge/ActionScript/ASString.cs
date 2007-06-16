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
    /// Represents a string value.
    /// </summary>
    public sealed class ASString : BaseASValue
    {
        private string value;

        /// <summary>
        /// Creates a string value.
        /// </summary>
        /// <param name="value">The value</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public ASString(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            this.value = value;
        }

        /// <summary>
        /// Gets the string value.
        /// </summary>
        public string Value
        {
            get { return value; }
        }

        /// <inheritdoc />
        public override ASTypeKind Kind
        {
            get { return ASTypeKind.String; }
        }

        /// <inheritdoc />
        public override void AcceptVisitor(IActionScriptSerializer serializer, IASValueVisitor visitor)
        {
            visitor.VisitString(serializer, value);
        }

        /// <inheritdoc />
        public override object GetNativeValue(Type nativeType)
        {
            return nativeType == typeof(string) ? value : null;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            ASString other = obj as ASString;
            return other != null && value == other.value;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return value.GetHashCode();
        }
    }
}