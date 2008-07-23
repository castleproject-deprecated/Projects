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
	using Model.TreeNodes;
	using Rhino.Mocks;
	using NUnit.Framework;

	[TestFixture]
	public class ViewSourceMapperTests
	{
		private MockRepository mocks;
		private ViewSourceMapper mapper;
		private ITreeCreationService treeService;

		[SetUp]
		public void Setup()
		{
			mocks = new MockRepository();
			treeService = mocks.DynamicMock<ITreeCreationService>();
			mapper = new ViewSourceMapper(new NullLogger(), treeService);
		}

		[Test]
		[ExpectedException(typeof (ArgumentException))]
		public void AddViewSource_PathWithNoViewsDirectory_ThrowsException()
		{
			using (mocks.Unordered())
			{
			}

			mocks.ReplayAll();
			mapper.AddViewSource(@"Projects\Eleutian.Web.Site\Home\Index.brail");
			mocks.VerifyAll();
		}

		[Test]
		public void AddViewSource_ViewForMissingController_DoesNothing()
		{
			using (mocks.Unordered())
			{
				Expect.Call(treeService.FindNode("HomeController")).Return(null);
				Expect.Call(treeService.FindNode("HomeComponent")).Return(null);
				Expect.Call(treeService.FindNode("Home")).Return(null);
				treeService.PopToRoot();
			}

			mocks.ReplayAll();
			mapper.AddViewSource(@"Projects\Eleutian.Web.Site\Views\Home\Index.brail");
			mocks.VerifyAll();
		}

		[Test]
		public void AddViewSource_ViewForValidTopLevelController_AddsViewNode()
		{
			var node = new ControllerTreeNode("HomeController", "SomeNamespace");
			var root = new AreaTreeNode("Root");
			root.AddChild(node);

			using (mocks.Unordered())
			{
				Expect.Call(treeService.FindNode("HomeController")).Return(node);
				treeService.PushNode(node);
				treeService.PopToRoot();
			}

			mocks.ReplayAll();
			mapper.AddViewSource(@"Projects\Eleutian.Web.Site\Views\Home\Index.brail");
			mocks.VerifyAll();

			AssertHasViewNode(node);
		}

		[Test]
		public void AddViewSource_ViewForValidSubAreaController_AddsViewNode()
		{
			var node = new ControllerTreeNode("HomeController", "SomeNamespace");
			var root = new AreaTreeNode("Root");
			var area = new AreaTreeNode("Root");
			root.AddChild(area);
			area.AddChild(node);

			using (mocks.Unordered())
			{
				Expect.Call(treeService.FindNode("AreaController")).Return(null);
				Expect.Call(treeService.FindNode("AreaComponent")).Return(null);
				Expect.Call(treeService.FindNode("Area")).Return(area);
				treeService.PushNode(area);
				Expect.Call(treeService.FindNode("HomeController")).Return(node);
				treeService.PushNode(node);
				treeService.PopToRoot();
			}

			mocks.ReplayAll();
			mapper.AddViewSource(@"Projects\Eleutian.Web.Site\Views\Area\Home\Index.brail");
			mocks.VerifyAll();

			AssertHasViewNode(node);
		}

		private static void AssertHasViewNode(ControllerTreeNode node)
		{
			Assert.IsInstanceOfType(typeof (ViewTreeNode), node.Children[0]);
			Assert.AreEqual("Index", node.Children[0].Name);
		}
	}
}