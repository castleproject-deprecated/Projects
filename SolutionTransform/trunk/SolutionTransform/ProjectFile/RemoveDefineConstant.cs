// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace SolutionTransform.ProjectFile
{
	using System.Collections.Generic;
	using System.Xml;

	public class RemoveDefineConstant : DefineConstantTransform {
		private readonly string constant;

		public RemoveDefineConstant(string constant) {
			this.constant = constant;
		}

		protected override void Transform(XmlElement propertyGroup, IList<string> constants) {
			constants.Remove(constant);
		}
	}
}
