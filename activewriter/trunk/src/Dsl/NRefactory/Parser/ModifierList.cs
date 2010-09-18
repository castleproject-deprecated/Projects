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
	using Ast;

	internal class ModifierList
	{
		Modifiers cur;
		Location location = new Location(-1, -1);
		
		public Modifiers Modifier {
			get {
				return cur;
			}
		}
		
		public Location GetDeclarationLocation(Location keywordLocation)
		{
			if(location.IsEmpty) {
				return keywordLocation;
			}
			return location;
		}
		
//		public Location Location {
//			get {
//				return location;
//			}
//			set {
//				location = value;
//			}
//		}
		
		public bool isNone { get { return cur == Modifiers.None; } }
		
		public bool Contains(Modifiers m)
		{
			return ((cur & m) != 0);
		}
		
		public void Add(Modifiers m, Location tokenLocation) 
		{
			if(location.IsEmpty) {
				location = tokenLocation;
			}
			
			if ((cur & m) == 0) {
				cur |= m;
			} else {
//				parser.Error("modifier " + m + " already defined");
			}
		}
		
//		public void Add(Modifiers m)
//		{
//			Add(m.cur, m.Location);
//		}
		
		public void Check(Modifiers allowed)
		{
			Modifiers wrong = cur & ~allowed;
			if (wrong != Modifiers.None) {
//				parser.Error("modifier(s) " + wrong + " not allowed here");
			}
		}
	}
}
