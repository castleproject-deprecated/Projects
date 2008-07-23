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

	public class ActionArgument
	{
		public ActionArgument(int index, string name, object value)
			: this(index, name, value.GetType(), value)
		{
		}

		public ActionArgument(int index, string name, Type type, object value)
		{
			Index = index;
			Name = name;
			Type = type;
			Value = value;
		}

		public int Index { get; private set; }
		public string Name { get; private set; }
		public Type Type { get; private set; }
		public object Value { get; private set; }
	}
}