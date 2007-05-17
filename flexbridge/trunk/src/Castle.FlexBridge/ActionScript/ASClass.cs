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
using Castle.FlexBridge.Collections;

namespace Castle.FlexBridge.ActionScript
{
    /// <summary>
    /// Describes an ActionScript class.
    /// </summary>
    public sealed class ASClass
    {
        /// <summary>
        /// Gets an untyped and unnamed class with no members and only dynamic properties.
        /// </summary>
        public static readonly ASClass UntypedDynamicClass = new ASClass("", ASClassLayout.Dynamic, EmptyArray<string>.Instance);

        private string classAlias;
        private ASClassLayout layout;
        private IList<string> memberNames;

        /// <summary>
        /// Creates a class with the specified alias name, layout and members.
        /// </summary>
        /// <param name="classAlias">The class alias, or an empty string if none</param>
        /// <param name="layout">The class layout</param>
        /// <param name="memberNames">The member names</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="classAlias"/>
        /// or <paramref name="memberNames"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="memberNames"/> is
        /// non-empty and <paramref name="layout"/> is <see cref="ASClassLayout.Externalizable" /></exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="classAlias"/>
        /// is empty but <paramref name="layout"/> is <see cref="ASClassLayout.Externalizable" /></exception>
        public ASClass(string classAlias, ASClassLayout layout, IList<string> memberNames)
        {
            if (classAlias == null)
                throw new ArgumentNullException("classAlias");
            if (memberNames == null)
                throw new ArgumentNullException("memberNames");
            if (layout == ASClassLayout.Externalizable && memberNames.Count != 0)
                throw new ArgumentException("An externalizable class cannot have any members.", "memberNames");
            if (layout == ASClassLayout.Externalizable && classAlias.Length == 0)
                throw new ArgumentException("An externalizable class must have a non-empty class name.", "className");

            this.classAlias = classAlias;
            this.layout = layout;
            this.memberNames = memberNames;
        }

        /// <summary>
        /// Gets the ActionScript class alias, or an empty string if none.
        /// The class alias must have been registered on the client side using
        /// "flash.net.registerClassAlias".
        /// </summary>
        public string ClassAlias
        {
            get { return classAlias; }
        }

        /// <summary>
        /// Gets the layout of the class.
        /// </summary>
        public ASClassLayout Layout
        {
            get { return layout; }
        }

        /// <summary>
        /// Gets the list of member names of the class.
        /// </summary>
        public IList<string> MemberNames
        {
            get { return memberNames; }
        }

        public override int GetHashCode()
        {
            return classAlias.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;

            ASClass other = obj as ASClass;
            if (other != null && other.classAlias == classAlias && other.layout == layout)
            {
                IList<string> otherMemberNames = other.memberNames;
                if (otherMemberNames == memberNames)
                    return true;

                int count = memberNames.Count;
                if (otherMemberNames.Count == count)
                {
                    for (int i = 0; i < count; i++)
                        if (otherMemberNames[i] != memberNames[i])
                            return false;

                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            return classAlias.Length != 0 ? classAlias : "<untyped>";
        }
    }
}
