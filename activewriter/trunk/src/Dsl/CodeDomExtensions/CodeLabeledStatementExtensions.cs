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

	public static class CodeLabeledStatementExtensions
    {
        public static CodeLabeledStatement Clone(this CodeLabeledStatement statement)
        {
            if (statement == null) return null;
            CodeLabeledStatement s = new CodeLabeledStatement();
            s.EndDirectives.AddRange(statement.EndDirectives);
            s.Label = statement.Label;
            s.LinePragma = statement.LinePragma;
            s.StartDirectives.AddRange(statement.StartDirectives);
            s.Statement = statement.Statement.Clone();
            s.UserData.AddRange(statement.UserData);
            return s;
        }

        public static void ReplaceType(this CodeLabeledStatement statement, string oldType, string newType)
        {
            if (statement == null) return;
            statement.Statement.ReplaceType(oldType, newType);
        }
    }
}
