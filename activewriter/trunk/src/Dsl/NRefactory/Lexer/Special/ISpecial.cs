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

	/// <summary>
	/// Interface for all specials.
	/// </summary>
	public interface ISpecial
	{
		Location StartPosition { get; }
		Location EndPosition { get; }
		
		object AcceptVisitor(ISpecialVisitor visitor, object data);
	}
	
	public interface ISpecialVisitor
	{
		object Visit(ISpecial special, object data);
		object Visit(BlankLine special, object data);
		object Visit(Comment special, object data);
		object Visit(PreprocessingDirective special, object data);
	}
	
	public abstract class AbstractSpecial : ISpecial
	{
		public abstract object AcceptVisitor(ISpecialVisitor visitor, object data);
		
		protected AbstractSpecial(Location position)
		{
			StartPosition = position;
			EndPosition = position;
		}
		
		protected AbstractSpecial(Location startPosition, Location endPosition)
		{
			StartPosition = startPosition;
			EndPosition = endPosition;
		}
		
		public Location StartPosition { get; set; }
		public Location EndPosition { get; set; }
		
		public override string ToString()
		{
			return String.Format("[{0}: Start = {1}, End = {2}]",
			                     GetType().Name, StartPosition, EndPosition);
		}
	}
}
