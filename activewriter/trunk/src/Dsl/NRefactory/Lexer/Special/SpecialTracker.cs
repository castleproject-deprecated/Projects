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
	using System;
	using System.Collections.Generic;
	using System.Text;

	public class SpecialTracker
	{
		List<ISpecial> currentSpecials = new List<ISpecial>();
		
		CommentType   currentCommentType;
		StringBuilder sb = new StringBuilder();
		Location         startPosition;
		bool commentStartsLine;
		
		public List<ISpecial> CurrentSpecials {
			get {
				return currentSpecials;
			}
		}
		
		public void InformToken(int kind)
		{
			
		}
		
		/// <summary>
		/// Gets the specials from the SpecialTracker and resets the lists.
		/// </summary>
		public List<ISpecial> RetrieveSpecials()
		{
			List<ISpecial> tmp = currentSpecials;
			currentSpecials = new List<ISpecial>();
			return tmp;
		}
		
		public void AddEndOfLine(Location point)
		{
			currentSpecials.Add(new BlankLine(point));
		}
		
		public void AddPreprocessingDirective(PreprocessingDirective directive)
		{
			if (directive == null)
				throw new ArgumentNullException("directive");
			currentSpecials.Add(directive);
		}
		
		// used for comment tracking
		public void StartComment(CommentType commentType, bool commentStartsLine, Location startPosition)
		{
			currentCommentType = commentType;
			this.startPosition      = startPosition;
			sb.Length          = 0;
			this.commentStartsLine  = commentStartsLine;
		}
		
		public void AddChar(char c)
		{
			sb.Append(c);
		}
		
		public void AddString(string s)
		{
			sb.Append(s);
		}
		
		public void FinishComment(Location endPosition)
		{
			currentSpecials.Add(new Comment(currentCommentType, sb.ToString(), commentStartsLine, startPosition, endPosition));
		}
	}
}
