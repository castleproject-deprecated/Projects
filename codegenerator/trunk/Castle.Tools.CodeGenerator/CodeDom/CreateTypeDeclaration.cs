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

namespace Castle.Tools.CodeGenerator.CodeDom
{
	using System.CodeDom;
	using System.Reflection;

	public class CreateTypeDeclaration
	{
		public CreateTypeDeclaration(string name)
		{
			Type = new CodeTypeDeclaration(name);
		}

		public static CreateTypeDeclaration Called(string name)
		{
			return new CreateTypeDeclaration(name);
		}

		public CreateTypeDeclaration AsPartial
		{
			get
			{
				Type.IsPartial = true;
				return this;
			}
		}

		public CreateTypeDeclaration WithAttributes(TypeAttributes attributes)
		{
			Type.TypeAttributes = attributes;
			return this;
		}

		public CreateTypeDeclaration WithCustomAttributes(CodeAttributeDeclaration attribute,
		                                                  params CodeAttributeDeclaration[] attributes)
		{
			Type.CustomAttributes.Add(attribute);

			foreach (var a in attributes)
				Type.CustomAttributes.Add(a);

			return this;
		}

		public CodeTypeDeclaration Type { get; private set; }
	}
}