// Copyright 2006 Gokhan Altinoren - http://altinoren.com/
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

namespace Altinoren.ActiveWriter.ServerExplorerSupport
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Microsoft.VisualStudio.Data.Interop;
    
    /// <summary>
    /// The DSRefNavigator is used to enumerate the DSRef Consumer data source
    /// and provide details about the Server Explorer's selection.
    /// </summary>
    internal class DSRefNavigator : IDisposable
    {
        #region Private Variables

        /// <summary>
        /// DSRef Consumer that provides a tree-like structure of the selected items
        /// </summary>
        private IDSRefConsumer _consumer;

        /// <summary>
        /// Root node of the DSRef Consumer selection result tree
        /// </summary>
        private static readonly IntPtr RootNode = IntPtr.Zero;

        #endregion

        #region Public Variables

        /// <summary>
        /// Clipboard format of the data source reference objects in the
        /// Server Explorer window of Visual Studio
        /// </summary>
        public static readonly string DataSourceReferenceFormat = "CF_DSREF";

        #endregion

        #region DLL Imports

        /// <summary>
        /// Create a COM stream from a pointer in unmanaged memory
        /// </summary>
        [DllImport("ole32.dll")]
        private static extern int CreateStreamOnHGlobal(IntPtr ptr, bool delete,
                                                        ref IntPtr stream);

        /// <summary>
        /// Load an OLE object from a COM stream
        /// </summary>
        [DllImport("ole32.dll")]
        private static extern int OleLoadFromStream(IntPtr stream, byte[] iid,
                                                    ref IntPtr obj);

        #endregion

        #region ctors

        /// <summary>
        /// Constructor to create the DSRef navigator from a stream
        /// </summary>
        /// <param name="data">Stream containing the DSRef consumer</param>
        public DSRefNavigator(Stream data)
        {
            _consumer = null;

            // Pointers to unmanaged resources
            IntPtr ptr = IntPtr.Zero;
            IntPtr stream = IntPtr.Zero;
            IntPtr native = IntPtr.Zero;

            // Get a reference to the DSRef Consumer from the stream
            try
            {
                // Read the stream containing the DSRef Consumer
                byte[] buffer = new byte[data.Length];
                data.Seek(0, SeekOrigin.Begin);
                data.Read(buffer, 0, buffer.Length);

                // Copy the DSRef Consumer to native memory
                native = Marshal.AllocHGlobal(buffer.Length);
                Marshal.Copy(buffer, 0, native, buffer.Length);

                // Create a COM stream from the memory
                int result = CreateStreamOnHGlobal(native, false, ref stream);
                Marshal.ThrowExceptionForHR(result);

                // Load the DSRef Consumer from the COM stream
                result = OleLoadFromStream(stream,
                                           typeof (IDSRefConsumer).GUID.ToByteArray(), ref ptr);
                Marshal.ThrowExceptionForHR(result);

                // Get the DSRef consumer
                _consumer = Marshal.GetObjectForIUnknown(ptr) as IDSRefConsumer;
            }
            finally
            {
                // Release all of the unmanaged resources
                Marshal.Release(ptr);
                Marshal.Release(stream);
                Marshal.Release(native);
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Dispose resources used by the navigator
        /// </summary>
        public void Dispose()
        {
            // Release the reference to the DSRef Consumer
            if (_consumer != null)
                Marshal.ReleaseComObject(_consumer);
        }

        #endregion

        #region Public Methods, Properties and Indexers

        /// <summary>
        /// Indicate whether or not the selection represented by this DSRef Consumer
        /// contains only tables as leaf nodes
        /// </summary>
        public bool ContainsOnlyTables
        {
            get
            {
                if (_consumer != null)
                {
                    // Determine if one of the leaf nodes has a table by querying
                    // the type of the root node
                    try
                    {
                        return (_consumer.GetType(RootNode) &
                                __DSREFTYPE.DSREFTYPE_TABLE) == __DSREFTYPE.DSREFTYPE_TABLE;
                    }
                    catch
                    {
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Enumerate the names of the tables in the selection represented by
        /// the DSRef Consumer
        /// </summary>
        public IEnumerable<string> Tables
        {
            get
            {
                foreach (DSRefNode child in ChildNodes)
                {
                    if (child.IsTable && child.HasName)
                        yield return child.Name;
                }
            }
        }

        public IDSRefConsumer Consumer
        {
            get { return _consumer; }
            set { _consumer = value; }
        }

        /// <summary>
        /// Enumerate all of the child nodes in the selection 
        /// </summary>
        /// <returns>Iterator for the child nodes in the selection</returns>
        public IEnumerable<DSRefNode> ChildNodes
        {
            get
            {
                // Instead of recursing, we'll just continually iterate over the queue
                Queue<DSRefNode> parents = new Queue<DSRefNode>();
                parents.Enqueue(new DSRefNode(RootNode, this));
                while (parents.Count > 0)
                {
                    DSRefNode parent = parents.Dequeue();
                    DSRefNode child = parent.GetFirstChildNode();
                    while (child != null)
                    {
                        yield return child;
                        parents.Enqueue(child);
                        child = child.GetNextSiblingNode();
                    }
                }
            }
        }

        public IEnumerable<DSRefNode> ChildTableNodes
        {
            get
            {
                // Instead of recursing, we'll just continually iterate over the queue
                Queue<DSRefNode> parents = new Queue<DSRefNode>();
                parents.Enqueue(new DSRefNode(RootNode, this));
                while (parents.Count > 0)
                {
                    DSRefNode parent = parents.Dequeue();
                    DSRefNode child = parent.GetFirstChildNode();
                    while (child != null)
                    {
                        if (child.IsTable && child.HasName)
                            yield return child;
                        parents.Enqueue(child);
                        child = child.GetNextSiblingNode();
                    }
                }
            }
        }


        public string GetConnectionName()
        {
            return null;
        }

        #endregion
    }
}