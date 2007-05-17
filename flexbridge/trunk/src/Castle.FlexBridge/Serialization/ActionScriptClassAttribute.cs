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
using Castle.FlexBridge.Serialization.Mapping;

namespace Castle.FlexBridge.Serialization
{
    /// <summary>
    /// Maps a .Net class to its ActionScript counterpart.
    /// Instances of the class will be serialized along with the ActionScript class
    /// name such that instances of the corresponding ActionScript will be
    /// recreated on the other end.  When omitted then unless a custom <see cref="IASMapperFactory" />
    /// handle is registered to handle the class the contents of its instances will be mapped to untyped
    /// ActionScript objects in the form of key/value pairs.
    /// <seealso cref="ActionScriptPropertyAttribute"/>
    /// <seealso cref="ActionScriptIgnoreAttribute"/>
    /// </summary>
    /// <remarks>
    /// This attribute must be used in every class that implements <see cref="IExternalizable" />
    /// because the client uses the class alias to decide how to deserialize the object.
    /// 
    /// A class indicates that it supports dynamic properties by implementing <see cref="IDynamic" />.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public sealed class ActionScriptClassAttribute : Attribute
    {
        private string classAlias;

        /// <summary>
        /// Associates an empty class name with the .Net class so that its instances will be mapped to
        /// untyped ActionScript objects with its properties represented as key/values pairs.
        /// </summary>
        public ActionScriptClassAttribute()
        {
            classAlias = "";
        }

        /// <summary>
        /// Associates the decorated .Net class with the specified ActionScript class.
        /// </summary>
        /// <remarks>
        /// The value of <paramref name="classAlias"/> must be non-empty for all <see cref="IExternalizable" />
        /// classes.
        /// </remarks>
        /// <param name="classAlias">The ActionScript class alias or empty if none.
        /// If non-empty the value must be registered on the client side with "flash.net.registerClassAlias".</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="classAlias"/> is null</exception>
        public ActionScriptClassAttribute(string classAlias)
        {
            if (classAlias == null)
                throw new ArgumentNullException("classAlias");

            this.classAlias = classAlias;
        }

        /// <summary>
        /// Gets the the ActionScript class alias.  Must be registered
        /// on the client side with "flash.net.registerClassAlias".
        /// </summary>
        public string ClassAlias
        {
            get { return classAlias; }
        }
    }
}
