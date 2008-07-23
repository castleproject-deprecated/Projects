// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.Tools.CodeGenerator.Services
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using Model.TreeNodes;

	public class ViewSourceMapper : IViewSourceMapper
	{
		private const string viewDirectory = @"Views";
		
		private readonly ILogger logger;
		private readonly ITreeCreationService treeService;
		
		public ViewSourceMapper(ILogger logger, ITreeCreationService treeService)
		{
			this.logger = logger;
			this.treeService = treeService;
		}

		public void AddViewSource(string path)
		{
			var viewName = Path.GetFileNameWithoutExtension(path);
			TreeNode node = null;
			
			foreach (var part in BreakPath(path))
			{
				var controllerName = part + "Controller";
				node = treeService.FindNode(controllerName);
				
				if (node == null)
				{
					var viewComponentName = part + "Component";
					node = treeService.FindNode(viewComponentName);
					
					if (node == null)
					{
						node = treeService.FindNode(part);
					
						if (node == null)
							continue;
					}
				}

				treeService.PushNode(node);
			}
			
			treeService.PopToRoot();
			
			if (node == null)
			{
				logger.LogInfo("Unable to map view: {0}", path);
				return;
			}

			node.AddChild(new ViewTreeNode(viewName));
		}

		private static string[] BreakPath(string path)
		{
			var parts = new List<string>(Path.GetDirectoryName(path).Split('\\', '/'));

			while (parts.Count > 0)
			{
				if (string.Compare(parts[0], viewDirectory, true) == 0)
				{
					parts.RemoveAt(0);
					return parts.ToArray();
				}
			
				parts.RemoveAt(0);
			}
			
			throw new ArgumentException("No Views directory in view path: " + path);
		}
	}
}