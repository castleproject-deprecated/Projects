// Copyright 2006-2007 Ken Egozi http://www.kenegozi.com/
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

namespace ViewUpgrader
{
	public class Upgrader
	{
		private readonly string source;

		public Upgrader(string source)
		{
			this.source = source;
		}

		public Upgrader TransformPropertyDecleration()
		{
			return new Upgrader(Transformer.TransformPropertyDecleration(source));
		}

		public Upgrader TransformSubViewProperties()
		{
			return new Upgrader(Transformer.TransformSubViewProperties(source));
		}

		public override string ToString()
		{
			return source;
		}
	}
}
