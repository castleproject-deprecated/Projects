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
	using System.CodeDom;
	using System.Linq;
	using Model.TreeNodes;
	using NUnit.Framework;

	[TestFixture]
	public class ActionMapGeneratorActionTests : ActionMapGeneratorTests
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
		public void VisitActionNode_NoParameters_CreatesMethod()
		{
			var node = new ActionTreeNode("Index");
			controller.AddChild(node);

			generator.Visit(controller);

			CodeDomAssert.AssertHasField(source.Ccu.Namespaces[0].Types[0], "_services");
			CodeDomAssert.AssertHasMethod(source.Ccu.Namespaces[0].Types[0], "Index");
		}

		[Test]
		public void VisitActionNode_OneParameters_CreatesMethod()
		{
			var node = new ActionTreeNode("Index");
			controller.AddChild(node);
			node.AddChild(new ParameterTreeNode("id", "System.Int32"));

			generator.Visit(controller);

			CodeDomAssert.AssertHasField(source.Ccu.Namespaces[0].Types[0], "_services");
			CodeDomAssert.AssertHasMethod(source.Ccu.Namespaces[0].Types[0], "Index");
		}

		[Test]
		public void VisitActionNode_NullableParameter_CreatesMethod()
		{
			var node = new ActionTreeNode("Index");
			controller.AddChild(node);
			node.AddChild(new ParameterTreeNode("id", "System.Nullable<System.Int32>"));

			generator.Visit(controller);

			var type = source.Ccu.Namespaces[0].Types[0];
			CodeDomAssert.AssertHasField(type, "_services");
			CodeDomAssert.AssertHasMethod(type, "Index");

			var method = type.Members.OfType<CodeMemberMethod>().First(m => m.Name == "Index");
			CodeDomAssert.AssertHasParameter(method, "id");

			var parameter = method.Parameters.OfType<CodeParameterDeclarationExpression>().First(p => p.Name == "id");
			Assert.AreEqual("System.Nullable`1", parameter.Type.BaseType);
			Assert.AreEqual(1, parameter.Type.TypeArguments.Count);
			Assert.AreEqual("System.Int32", parameter.Type.TypeArguments[0].BaseType);
		}
	}
}