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

namespace Castle.ActiveWriter.CodeDomExtensions
{
	using System.CodeDom;

	public static class CodeDirectionExpressionExtensions
    {
        public static CodeDirectionExpression Clone(this CodeDirectionExpression expression)
        {
            if (expression == null) return null;
            CodeDirectionExpression e = new CodeDirectionExpression();
            e.Direction = expression.Direction;
            e.Expression = expression.Expression.Clone();
            e.UserData.AddRange(expression.UserData);
            return e;
        }

        public static void ReplaceType(this CodeDirectionExpression expression, string oldType, string newType)
        {
            if (expression == null) return;
            expression.Expression.ReplaceType(oldType, newType);
        }
    }
}
