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

namespace Castle.Tools.CodeGenerator.External
{
	using System;
	using System.IO;
	using External;

	public class ControllerViewReference : IControllerViewReference
	{
		public ControllerViewReference(ICodeGeneratorServices services, Type controllerType, string areaName,
									   string controllerName, string actionName)
		{
			if (services == null) throw new ArgumentNullException("services");
			if (controllerType == null) throw new ArgumentNullException("controllerType");
			if (String.IsNullOrEmpty(controllerName)) throw new ArgumentNullException("controllerName");
			if (String.IsNullOrEmpty(actionName)) throw new ArgumentNullException("actionName");
			
			Services = services;
			ControllerType = controllerType;
			ControllerName = controllerName;
			AreaName = areaName;
			ActionName = actionName;
		}

		public string ActionName { get; private set; }
		public string AreaName { get; private set; }
		public string ControllerName { get; private set; }
		public Type ControllerType { get; private set; }
		public ICodeGeneratorServices Services { get; private set; }

		public virtual void Render(bool skiplayout)
		{
			var controller = ControllerName;
			
			if (!String.IsNullOrEmpty(AreaName))
				controller = Path.Combine(AreaName, ControllerName);
			
			Services.Controller.RenderView(controller, ActionName, skiplayout);
		}

		public virtual void Render()
		{
			Render(false);
		}
	}
}