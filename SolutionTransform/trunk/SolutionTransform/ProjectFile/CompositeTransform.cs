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
	using System.Linq;
	using System.Xml;

	public class CompositeTransform : ITransform
	{
		private readonly List<ITransform> transforms;

		public CompositeTransform(params ITransform[] transforms)
			: this((IEnumerable<ITransform>) transforms)
		{
			
		}

		public void Add(ITransform transform)
		{
			transforms.Add(transform);
		}


		public CompositeTransform(IEnumerable<ITransform> transforms)
		{
			this.transforms = transforms.ToList();
		}


		public void ApplyTransform(string path, XmlDocument document)
		{
			foreach (var transform in transforms)
			{
				transform.ApplyTransform(path, document);
			}
		}
	}
}
