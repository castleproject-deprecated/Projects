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
	using Model.TreeNodes;
	using NUnit.Framework;

	[TestFixture]
	public class RouteMapGeneratorActionTests : RouteMapGeneratorTests
	{
		private AreaTreeNode root;
		private ControllerTreeNode controller;

		public override void Setup()
		{
			base.Setup();
			root = new AreaTreeNode("Root");
			controller = new ControllerTreeNode("HomeController", "ControllerNamespace");
			root.AddChild(controller);
		}

		[Test]
		public void VisitRouteNode_NoParameters_CreatesMethod()
		{
			var node = new StaticRouteTreeNode("Index", "index");
			var actionTreeNode = new ActionTreeNode("action");
			actionTreeNode.AddChild(node);
			controller.AddChild(actionTreeNode);

			generator.Visit(controller);

			CodeDomAssert.AssertHasField(source.Ccu.Namespaces[0].Types[0], "_services");
			CodeDomAssert.AssertHasMethod(source.Ccu.Namespaces[0].Types[2], "Index");
		}

		[Test]
		public void VisitRouteNode_OneParameters_CreatesMethod()
		{
			var node = new StaticRouteTreeNode("AuthenticateLogIn", "login/authenticate/<userName:string>/<password:string>");
			var actionTreeNode = new ActionTreeNode("action");
			actionTreeNode.AddChild(node);
			controller.AddChild(actionTreeNode);
			node.AddChild(new ParameterTreeNode("userName", "System.String"));

			generator.Visit(controller);

			CodeDomAssert.AssertHasField(source.Ccu.Namespaces[0].Types[0], "_services");
			CodeDomAssert.AssertHasMethod(source.Ccu.Namespaces[0].Types[2], "AuthenticateLogIn");
		}
	}
}