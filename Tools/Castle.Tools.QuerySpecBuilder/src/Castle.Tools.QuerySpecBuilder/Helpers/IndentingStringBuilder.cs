#region license

// Copyright 2008 Ken Egozi http://www.kenegozi.com/
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

namespace Castle.Tools.QuerySpecBuilder.Helpers
{
	using System;
    using System.Text;
    using Helpers;


	public class IndentingStringBuilder
	{
		readonly StringBuilder buf = new StringBuilder();
		int indent;
		readonly int initialIndentation;

		public IndentingStringBuilder()
			: this(0)
		{
		}

		public IndentingStringBuilder(int initialIndentation)
		{
			this.initialIndentation = initialIndentation;
			indent = initialIndentation;
		}

		public IndentingStringBuilder In(int amount)
		{
			indent += amount;
			return this;
		}

		public IndentingStringBuilder Out(int amount)
		{
			indent -= amount;
			if (indent < initialIndentation)
				indent = initialIndentation;
			return this;
		}

		public IndentingStringBuilder AppendLine(string format, params object[] args)
		{
			buf.AppendLine(ApplyFormatOn(format, args));
			return this;
		}

		public IndentingStringBuilder Append(string format, params object[] args)
		{
			buf.Append(ApplyFormatOn(format, args));
			return this;
		}

		string ApplyFormatOn(string format, params object[] args)
		{
			var indentation = indent.Times('\t');
			var rawTextToAdd = string.Format(format, args).TrimStart('\r', '\n').TrimEnd('\r', '\n');
			var indentedNewLine = Environment.NewLine + indentation;
			var textToAdd = rawTextToAdd.Replace(Environment.NewLine, indentedNewLine);
			return indentation + textToAdd;
		}

		
		public override string ToString()
		{
			return buf.ToString();
		}
	}
}