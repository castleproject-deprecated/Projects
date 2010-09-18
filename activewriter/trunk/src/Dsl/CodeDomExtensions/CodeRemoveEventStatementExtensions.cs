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

	public static class CodeRemoveEventStatementExtensions
    {
        public static CodeRemoveEventStatement Clone(this CodeRemoveEventStatement statement)
        {
            if (statement == null) return null;
            CodeRemoveEventStatement s = new CodeRemoveEventStatement();
            s.EndDirectives.AddRange(statement.EndDirectives);
            s.Event = statement.Event.Clone();
            s.LinePragma = statement.LinePragma;
            s.Listener = statement.Listener.Clone();
            s.StartDirectives.AddRange(statement.StartDirectives);
            s.UserData.AddRange(statement.UserData);
            return s;
        }

        public static void ReplaceType(this CodeRemoveEventStatement statement, string oldType, string newType)
        {
            if (statement == null) return;
            statement.Event.ReplaceType(oldType, newType);
            statement.Listener.ReplaceType(oldType, newType);
        }
    }
}
