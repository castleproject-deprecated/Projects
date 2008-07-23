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

namespace Castle.Tools.CodeGenerator.Model.TreeNodes
{
	using System;

	public class ControllerTreeNode : TreeNode
	{
		public ControllerTreeNode(string name, string controllerNamespace) : base(name)
		{
			Namespace = controllerNamespace;
		}

		public string Area
		{
			get { return CalculatePath(n => n.Name == RootName); }
		}

		public string FullName
		{
			get { return Namespace + "." + Name; }
		}

		public string Namespace { get; private set; }

		public override string ToString()
		{
			var area = Area;
			return !String.IsNullOrEmpty(area)
				? String.Format("Controller<{0}/{1}>", area, Name) 
				: String.Format("Controller<{0}>", Name);
		}
	}
}