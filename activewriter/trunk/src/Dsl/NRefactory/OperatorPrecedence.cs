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
	using System.Collections.Generic;
	using Ast;

	/// <summary>
	/// Stores the operator precedences for the output visitor.
	/// </summary>
	static class OperatorPrecedence
	{
		static readonly Dictionary<BinaryOperatorType, int> vbDict = MakePrecedenceTable(
			new BinaryOperatorType[] { BinaryOperatorType.Power },
			new BinaryOperatorType[] { BinaryOperatorType.Multiply, BinaryOperatorType.Divide },
			new BinaryOperatorType[] { BinaryOperatorType.DivideInteger },
			new BinaryOperatorType[] { BinaryOperatorType.Modulus },
			new BinaryOperatorType[] { BinaryOperatorType.Add, BinaryOperatorType.Subtract },
			new BinaryOperatorType[] { BinaryOperatorType.Concat },
			new BinaryOperatorType[] { BinaryOperatorType.ShiftLeft, BinaryOperatorType.ShiftRight },
			new BinaryOperatorType[] {
				BinaryOperatorType.Equality, BinaryOperatorType.InEquality,
				BinaryOperatorType.LessThan, BinaryOperatorType.LessThanOrEqual,
				BinaryOperatorType.GreaterThan, BinaryOperatorType.GreaterThanOrEqual,
				BinaryOperatorType.ReferenceEquality, BinaryOperatorType.ReferenceInequality,
				BinaryOperatorType.Like
			},
			new BinaryOperatorType[] { BinaryOperatorType.LogicalAnd, BinaryOperatorType.BitwiseAnd },
			new BinaryOperatorType[] { BinaryOperatorType.LogicalOr, BinaryOperatorType.BitwiseOr },
			new BinaryOperatorType[] { BinaryOperatorType.ExclusiveOr }
		);
		
		static readonly Dictionary<BinaryOperatorType, int> csharpDict = MakePrecedenceTable(
			new BinaryOperatorType[] { BinaryOperatorType.Multiply, BinaryOperatorType.Divide, BinaryOperatorType.Modulus },
			new BinaryOperatorType[] { BinaryOperatorType.Add, BinaryOperatorType.Subtract },
			new BinaryOperatorType[] { BinaryOperatorType.ShiftLeft, BinaryOperatorType.ShiftRight },
			new BinaryOperatorType[] {
				BinaryOperatorType.LessThan, BinaryOperatorType.LessThanOrEqual,
				BinaryOperatorType.GreaterThan, BinaryOperatorType.GreaterThanOrEqual,
			},
			new BinaryOperatorType[] { BinaryOperatorType.Equality, BinaryOperatorType.InEquality },
			new BinaryOperatorType[] { BinaryOperatorType.BitwiseAnd },
			new BinaryOperatorType[] { BinaryOperatorType.ExclusiveOr },
			new BinaryOperatorType[] { BinaryOperatorType.BitwiseOr },
			new BinaryOperatorType[] { BinaryOperatorType.LogicalAnd, BinaryOperatorType.LogicalOr },
			new BinaryOperatorType[] { BinaryOperatorType.NullCoalescing }
		);
		
		// create a dictionary operator->precedence (higher value = higher precedence)
		static Dictionary<BinaryOperatorType, int> MakePrecedenceTable(params BinaryOperatorType[][] input)
		{
			Dictionary<BinaryOperatorType, int> dict = new Dictionary<BinaryOperatorType, int>();
			for (int i = 0; i < input.Length; i++) {
				foreach (BinaryOperatorType op in input[i]) {
					dict.Add(op, input.Length - i);
				}
			}
			return dict;
		}
		
		public static int ComparePrecedenceVB(BinaryOperatorType op1, BinaryOperatorType op2)
		{
			int p1 = GetOperatorPrecedence(vbDict, op1);
			int p2 = GetOperatorPrecedence(vbDict, op2);
			return p1.CompareTo(p2);
		}
		
		public static int ComparePrecedenceCSharp(BinaryOperatorType op1, BinaryOperatorType op2)
		{
			int p1 = GetOperatorPrecedence(csharpDict, op1);
			int p2 = GetOperatorPrecedence(csharpDict, op2);
			return p1.CompareTo(p2);
		}
		
		static int GetOperatorPrecedence(Dictionary<BinaryOperatorType, int> dict, BinaryOperatorType op)
		{
			int p;
			dict.TryGetValue(op, out p);
			return p;
		}
	}
}
