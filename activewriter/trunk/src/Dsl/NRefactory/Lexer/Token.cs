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

namespace ICSharpCode.NRefactory.Parser
{
	public enum LiteralFormat : byte
	{
		None,
		DecimalNumber,
		HexadecimalNumber,
		OctalNumber,
		StringLiteral,
		VerbatimStringLiteral,
		CharLiteral,
		DateTimeLiteral
	}
	
	public class Token
	{
		internal readonly int kind;
		
		internal readonly int col;
		internal readonly int line;
		
		internal readonly LiteralFormat literalFormat;
		internal readonly object literalValue;
		internal readonly string val;
		internal Token next;
		readonly Location endLocation;
		
		public int Kind {
			get { return kind; }
		}
		
		public LiteralFormat LiteralFormat {
			get { return literalFormat; }
		}
		
		public object LiteralValue {
			get { return literalValue; }
		}
		
		public string Value {
			get { return val; }
		}
		
		public Location EndLocation {
			get { return endLocation; }
		}
		
		public Location Location {
			get {
				return new Location(col, line);
			}
		}
		
		public Token(int kind) : this(kind, 0, 0)
		{
		}
		
		public Token(int kind, int col, int line) : this (kind, col, line, null)
		{
		}
		
		public Token(int kind, int col, int line, string val)
		{
			this.kind         = kind;
			this.col          = col;
			this.line         = line;
			this.val          = val;
			endLocation  = new Location(col + (string.IsNullOrEmpty(val) ? 1 : val.Length), line);
		}
		
		internal Token(int kind, int x, int y, string val, object literalValue, LiteralFormat literalFormat)
			: this(kind, new Location(x, y), new Location(x + val.Length, y), val, literalValue, literalFormat)
		{
		}
		
		public Token(int kind, Location startLocation, Location endLocation, string val, object literalValue, LiteralFormat literalFormat)
		{
			this.kind         = kind;
			col          = startLocation.Column;
			line         = startLocation.Line;
			this.endLocation = endLocation;
			this.val          = val;
			this.literalValue = literalValue;
			this.literalFormat = literalFormat;
		}
		
		public override string ToString()
		{
			return string.Format("[C# {0}/VB {1} Location={3} EndLocation={4} val={5}]",
			                     CSharp.Tokens.GetTokenString(kind),
			                     VB.Tokens.GetTokenString(kind),
			                     Location, EndLocation, val);
			
		}
	}
}
