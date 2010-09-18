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

namespace ICSharpCode.NRefactory
{
	using System;

	public class Comment : AbstractSpecial
	{
		public CommentType CommentType { get; set; }

		public string CommentText { get; set; }

		/// <value>
		/// Is true, when the comment is at line start or only whitespaces
		/// between line and comment start.
		/// </value>
		public bool CommentStartsLine {
			get;
			set;
		}
		
		public Comment(CommentType commentType, string comment, bool commentStartsLine, Location startPosition, Location endPosition)
			: base(startPosition, endPosition)
		{
			this.CommentType   = commentType;
			this.CommentText       = comment;
			CommentStartsLine = commentStartsLine;
		}
		
		public override string ToString()
		{
			return String.Format("[{0}: Type = {1}, Text = {2}, Start = {3}, End = {4}]",
			                     GetType().Name, CommentType, CommentText, StartPosition, EndPosition);
		}
		
		public override object AcceptVisitor(ISpecialVisitor visitor, object data)
		{
			return visitor.Visit(this, data);
		}
	}
}
