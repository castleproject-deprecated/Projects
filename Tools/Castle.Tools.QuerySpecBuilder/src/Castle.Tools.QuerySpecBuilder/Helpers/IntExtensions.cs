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
	using System.Text;

	public static class IntExtensions
	{
		public static string Times(this int times, string input)
		{
			var buf = new StringBuilder(input.Length * times);
			for (int i = 0; i < times; i++)
			{
				buf.Append(input);
			}

			return buf.ToString();
		}

		public static string Times(this int times, char input)
		{
			return new string(input, times);
		}
	}
}