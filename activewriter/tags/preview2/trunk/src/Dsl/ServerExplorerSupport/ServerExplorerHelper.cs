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
    using System.Data.Common;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using EnvDTE;
    using Microsoft.VisualStudio.Data;
    using Microsoft.VisualStudio.Modeling;
    using Microsoft.VSDesigner.ServerExplorer;
    using IServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;
    
    internal static class ServerExplorerHelper
    {
        public static Guid ServerExplorer = new Guid("7494682C-37A0-11d2-A273-00C04F8EF4FF");
        public static Guid IUnknown = new Guid("00000000-0000-0000-c000-000000000046");

        public static DbConnection GetConnection(Store store, out string type)
        {
            type = null;
            try
            {
                string itemName = null;

                DTE dte = DTEHelper.GetDTE(store);

                if (dte != null)
                {
                    itemName = GetItemName(dte);

                    if (itemName != null)
                    {
                        DataConnection connection = GetDataConnection(dte, itemName);
                        if (connection != null && connection.ConnectionSupport.ProviderObject != null)
                        {
                            type = connection.ConnectionSupport.ProviderObject.GetType().FullName;
                            return (DbConnection) connection.ConnectionSupport.ProviderObject;
                        }
                    }
                }
            }
            catch
            {
            }

            return null;
        }

        private static DataConnection GetDataConnection(DTE dte, string itemName)
        {
            IntPtr ptr = IntPtr.Zero;
            try
            {
                if (((IServiceProvider) dte).QueryService(ref ServerExplorer, ref IUnknown, out ptr) >= 0)
                {
                    object o = Marshal.GetObjectForIUnknown(ptr);
                    if (o != null)
                    {
                        object dataConnectionsNode = GetField(o, "lastBrowseObjectNodeSite");
                        if (dataConnectionsNode != null)
                        {
                            INodeSite nodeSite =
                                (INodeSite) dataConnectionsNode;

                            INodeSite[] nodes = nodeSite.FindChildrenByLabel(itemName);
                            if (nodes != null && nodes.Length > 0)
                            {
                                object expNode = GetField(nodes[0], "expNode");
                                if (expNode != null)
                                {
                                    object nestedHierarchy = GetField(expNode, "nestedHierarchy");
                                    if (nestedHierarchy != null)
                                    {
                                        DataConnection connection =
                                            GetProperty(nestedHierarchy, "DataConnection") as DataConnection; // TODO: Fails on localized VS editions.
                                        if (connection != null)
                                        {
                                            return connection;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                Marshal.Release(ptr);
            }

            return null;
        }

        private static string GetItemName(DTE dte)
        {
            string itemName = null;

            UIHierarchyItem connectionsRoot = GetConnectionsRoot(dte);

            foreach (UIHierarchyItem item in connectionsRoot.UIHierarchyItems)
            {
                bool expanded = item.UIHierarchyItems.Expanded;
                // TODO: Prevent flicker of Server Explorer when searching? Can we get sub items without expanding?
                try
                {
                    if (IsHierarchySelected(item.UIHierarchyItems, true))
                    {
                        itemName = item.Name;
                        break;
                    }
                }
                finally
                {
                    item.UIHierarchyItems.Expanded = expanded;
                }
            }
            return itemName;
        }

        private static UIHierarchyItem GetConnectionsRoot(DTE dte)
        {
            UIHierarchy explorer = dte.Windows.Item(Constants.vsWindowKindServerExplorer).Object as UIHierarchy;
            return explorer.GetItem("Data Connections"); // TODO: Fails on localized VS editions?
        }

        private static object GetField(object o, string fieldName)
        {
            if (o != null && !String.IsNullOrEmpty(fieldName))
                return o.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(o);

            return null;
        }

        private static object GetProperty(object o, string fieldName)
        {
            if (o != null && !String.IsNullOrEmpty(fieldName))
                return o.GetType().GetProperty(fieldName).GetValue(o, null);

            return null;
        }

        private static bool IsHierarchySelected(UIHierarchyItems items, bool isTableLevel)
        {
            if (items != null)
            {
                bool expanded = items.Expanded;

                try
                {
                    foreach (UIHierarchyItem item in items)
                    {
                        if (isTableLevel)
                        {
                            if (item.Name == "Tables" && // TODO: Fails on localized VS editions.
                                (item.IsSelected || IsHierarchySelected(item.UIHierarchyItems, false)))
                                return true;
                        }
                        else
                        {
                            if (item.IsSelected || IsHierarchySelected(item.UIHierarchyItems, false))
                                return true;
                        }
                    }
                }
                finally
                {
                    items.Expanded = expanded;
                }
            }

            return false;
        }
    }
}