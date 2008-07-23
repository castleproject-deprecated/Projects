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
	using System.Text;

	public class MethodSignature
	{
		public MethodSignature(Type type, string name, Type[] types)
		{
			Type = type;
			Name = name;
			Types = types;
		}

		public string Name { get; private set; }
		public Type Type { get; private set; }
		public Type[] Types { get; private set; }

		public override bool Equals(object obj)
		{
			var ms = obj as MethodSignature;

			if (ms == null)
				return base.Equals(obj);

			if (!Type.Equals(ms.Type) || !Name.Equals(ms.Name) || ms.Types.Length != Types.Length)
				return false;
			
			for (var i = 0; i < Types.Length; i++)
				if (!Types[i].Equals(ms.Types[i]))
					return false;
			
			return true;
		}

		public override int GetHashCode()
		{
			var hash = Type.GetHashCode() ^ Name.GetHashCode();
			
			foreach (var type in Types)
				hash ^= type.GetHashCode();
			
			return hash;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			
			foreach (var type in Types)
			{
				if (sb.Length != 0)
				{
					sb.Append(", ");
				}
				sb.Append(type);
			}
			
			var args = sb.ToString();
			sb = new StringBuilder();
			sb.Append(Type.FullName).Append(Name).Append("(").Append(args).Append(")");
			
			return sb.ToString();
		}
	}
}