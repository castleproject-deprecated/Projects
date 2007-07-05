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
	using System.Data;
	using Microsoft.VisualStudio.Data;
	using Microsoft.VisualStudio.Data.Interop;
	using Microsoft.VisualStudio.Shell;
	using Microsoft.VisualStudio.Shell.Interop;

	internal static class ServerExplorerHelper
	{
		public static Guid IUnknown = new Guid("00000000-0000-0000-c000-000000000046");
		public static Guid ServerExplorer = new Guid("7494682C-37A0-11d2-A273-00C04F8EF4FF");

		public static IDbConnection GetConnection(DSRefNavigator navigator, out string type)
		{
			type = null;
			try
			{
				if (navigator != null)
				{
					IVsDataConnectionsService dataConnectionsService =
						(IVsDataConnectionsService) Package.GetGlobalService(typeof(IVsDataConnectionsService));
					string itemName = navigator.GetConnectionName();

					if (itemName != null)
					{
						int iConn; // = dataConnectionsService.GetConnectionIndex(itemName);
						DataViewHierarchyAccessor dataViewHierarchy = null;

						for(iConn = 0; iConn < dataConnectionsService.Count; iConn++)
						{
							DataViewHierarchyAccessor hierarchyAccessor =
								new DataViewHierarchyAccessor((IVsUIHierarchy) dataConnectionsService.GetConnectionHierarchy(iConn));
							try
							{
								if (hierarchyAccessor.Connection.DisplayConnectionString == itemName)
								{
									dataViewHierarchy = hierarchyAccessor;
									break;
								}
							}
							catch(Exception ex)
							{
							}
						}
						if (dataViewHierarchy != null)
						{
							DataConnection connection = dataViewHierarchy.Connection;
							if (connection != null && connection.ConnectionSupport.ProviderObject != null)
							{
								type = connection.ConnectionSupport.ProviderObject.GetType().FullName;
								return (IDbConnection) connection.ConnectionSupport.ProviderObject;
							}
						}
					}
				}
			}
			catch
			{
			}

			return null;
		}
	}
}