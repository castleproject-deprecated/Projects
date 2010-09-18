#region License
//  Copyright 2004-2010 Castle Project - http:www.castleproject.org/
//  
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//      http:www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// 
#endregion

namespace ICSharpCode.NRefactory.Parser.VB
{
	using Ast;

	internal class ParamModifierList
	{
		ParameterModifiers cur;
		Parser   parser;
		
		public ParameterModifiers Modifier {
			get {
				return cur;
			}
		}
		
		public ParamModifierList(Parser parser)
		{
			this.parser = parser;
			cur         = ParameterModifiers.None;
		}
		
		public bool isNone { get { return cur == ParameterModifiers.None; } }
		
		public void Add(ParameterModifiers m) 
		{
			if ((cur & m) == 0) {
				cur |= m;
			} else {
				parser.Error("param modifier " + m + " already defined");
			}
		}
		
		public void Add(ParamModifierList m)
		{
			Add(m.cur);
		}
		
		public void Check()
		{
			if((cur & ParameterModifiers.In) != 0 && 
			   (cur & ParameterModifiers.Ref) != 0) {
				parser.Error("ByRef and ByVal are not allowed at the same time.");
			}
		}
	}
}
