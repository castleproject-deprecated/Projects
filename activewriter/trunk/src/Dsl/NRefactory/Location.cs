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
	/// A line/column position.
	/// NRefactory lines/columns are counting from one.
	/// </summary>
	public struct Location : IComparable<Location>, IEquatable<Location>
	{
		public static readonly Location Empty = new Location(-1, -1);
		
		public Location(int column, int line)
		{
			x = column;
			y = line;
		}
		
		int x, y;
		
		public int X {
			get { return x; }
			set { x = value; }
		}
		
		public int Y {
			get { return y; }
			set { y = value; }
		}
		
		public int Line {
			get { return y; }
			set { y = value; }
		}
		
		public int Column {
			get { return x; }
			set { x = value; }
		}
		
		public bool IsEmpty {
			get {
				return x <= 0 && y <= 0;
			}
		}
		
		public override string ToString()
		{
			return string.Format("(Line {1}, Col {0})", x, y);
		}
		
		public override int GetHashCode()
		{
			return unchecked (87 * x.GetHashCode() ^ y.GetHashCode());
		}
		
		public override bool Equals(object obj)
		{
			if (!(obj is Location)) return false;
			return (Location)obj == this;
		}
		
		public bool Equals(Location other)
		{
			return this == other;
		}
		
		public static bool operator ==(Location a, Location b)
		{
			return a.x == b.x && a.y == b.y;
		}
		
		public static bool operator !=(Location a, Location b)
		{
			return a.x != b.x || a.y != b.y;
		}
		
		public static bool operator <(Location a, Location b)
		{
			if (a.y < b.y)
				return true;
			else if (a.y == b.y)
				return a.x < b.x;
			else
				return false;
		}
		
		public static bool operator >(Location a, Location b)
		{
			if (a.y > b.y)
				return true;
			else if (a.y == b.y)
				return a.x > b.x;
			else
				return false;
		}
		
		public static bool operator <=(Location a, Location b)
		{
			return !(a > b);
		}
		
		public static bool operator >=(Location a, Location b)
		{
			return !(a < b);
		}
		
		public int CompareTo(Location other)
		{
			if (this == other)
				return 0;
			if (this < other)
				return -1;
			else
				return 1;
		}
	}
}
