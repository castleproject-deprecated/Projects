// Copyright 2006 Gokhan Castle - http://altinoren.com/
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

namespace Castle.ActiveWriter.ServerExplorerSupport
{
    using System;
    using Microsoft.VisualStudio.Data.Interop;
    
    internal class DSRefNode
    {
        private IntPtr _pointer;
        private DSRefNavigator _navigator = null;
        private string _name = null;
        private string _owner = null;
        private __DSREFTYPE _type = __DSREFTYPE.DSREFTYPE_NULL;

        public bool HasName
        {
            get { return ((DSRefType & __DSREFTYPE.DSREFTYPE_HASNAME) == __DSREFTYPE.DSREFTYPE_HASNAME); }
        }

        public bool IsTable
        {
            get { return ((DSRefType & __DSREFTYPE.DSREFTYPE_TABLE) == __DSREFTYPE.DSREFTYPE_TABLE); }
        }

      public bool IsConnection {
        get { return ((DSRefType & __DSREFTYPE.DSREFTYPE_DATABASE) == __DSREFTYPE.DSREFTYPE_DATABASE); }
      }

        public __DSREFTYPE DSRefType
        {
            get
            {
                if (_type == __DSREFTYPE.DSREFTYPE_NULL)
                    _type = _navigator.Consumer.GetType(_pointer);

                return _type;
            }
        }

        public string Name
        {
            get
            {
                if (_name == null)
                    _name = _navigator.Consumer.GetName(_pointer);

                return _name;
            }
        }

        public string Owner
        {
            get
            {
                if (_owner == null)
                    _owner = _navigator.Consumer.GetOwner(_pointer);

                return _owner;
            }
        }

        public IntPtr Pointer
        {
            get { return _pointer; }
        }

        public DSRefNode(IntPtr pointer, DSRefNavigator navigator)
        {
            _pointer = pointer;
            _navigator = navigator;
        }

        public DSRefNode GetFirstChildNode()
        {
            IntPtr child = _navigator.Consumer.GetFirstChildNode(_pointer);
            if (child != IntPtr.Zero)
                return new DSRefNode(child, _navigator);
            else
                return null;
        }

        public DSRefNode GetNextSiblingNode()
        {
            IntPtr sibling = _navigator.Consumer.GetNextSiblingNode(_pointer);
            if (sibling != IntPtr.Zero)
                return new DSRefNode(sibling, _navigator);
            else
                return null;
        }
    }
}
