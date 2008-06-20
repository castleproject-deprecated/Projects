#region license
// Copyright 2008 Ken Egozi http://www.kenegozi.com/blog
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
#endregion

namespace Castle.Tools.StaticMapGenerator.Tests
{
	using GeneratedClassMetadata;
	using Services;
	using NUnit.Framework;

	[TestFixture]
	public class DescriptorToClassServiceFixture
	{
		[Test]
		public void Execute_WhenHasClassBasedMember_Works()
		{
			ClassDescriptor cls = new ClassDescriptor();
			cls.Name = "the_class";
			cls.Members.Add(new MemberDescriptor("the_type", "member_name"));

			IDescriptorToClassService service = new DescriptorToClassService();

			string classDef = service.Execute(cls);

			Assert.IsTrue(classDef.IndexOf("class the_class") > -1);
			Assert.IsTrue(classDef.IndexOf("the_type member_name = new the_type();") > -1);
		}

		[Test]
		public void Execute_WhenHasStringMember_Works()
		{
			ClassDescriptor cls = new ClassDescriptor();
			cls.Name = "the_class";
			cls.Members.Add(new MemberDescriptor("string", "member_name", "\"value\""));

			IDescriptorToClassService service = new DescriptorToClassService();

			string classDef = service.Execute(cls);

			Assert.IsTrue(classDef.IndexOf("class the_class") > -1);
			Assert.IsTrue(classDef.IndexOf("string member_name = \"value\";") > -1);
		}
	}
}
