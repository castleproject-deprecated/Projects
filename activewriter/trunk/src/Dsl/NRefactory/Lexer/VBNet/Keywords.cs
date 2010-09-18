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

namespace ICSharpCode.NRefactory.Parser.VB
{
	public static class Keywords
	{
		static readonly string[] keywordList = {
			"ADDHANDLER",
			"ADDRESSOF",
			"AGGREGATE",
			"ALIAS",
			"AND",
			"ANDALSO",
			"ANSI",
			"AS",
			"ASCENDING",
			"ASSEMBLY",
			"AUTO",
			"BINARY",
			"BOOLEAN",
			"BYREF",
			"BY",
			"BYTE",
			"BYVAL",
			"CALL",
			"CASE",
			"CATCH",
			"CBOOL",
			"CBYTE",
			"CCHAR",
			"CDATE",
			"CDBL",
			"CDEC",
			"CHAR",
			"CINT",
			"CLASS",
			"CLNG",
			"COBJ",
			"COMPARE",
			"CONST",
			"CONTINUE",
			"CSBYTE",
			"CSHORT",
			"CSNG",
			"CSTR",
			"CTYPE",
			"CUINT",
			"CULNG",
			"CUSHORT",
			"CUSTOM",
			"DATE",
			"DECIMAL",
			"DECLARE",
			"DEFAULT",
			"DELEGATE",
			"DESCENDING",
			"DIM",
			"DIRECTCAST",
			"DISTINCT",
			"DO",
			"DOUBLE",
			"EACH",
			"ELSE",
			"ELSEIF",
			"END",
			"ENDIF",
			"ENUM",
			"EQUALS",
			"ERASE",
			"ERROR",
			"EVENT",
			"EXIT",
			"EXPLICIT",
			"FALSE",
			"FINALLY",
			"FOR",
			"FRIEND",
			"FROM",
			"FUNCTION",
			"GET",
			"GETTYPE",
			"GLOBAL",
			"GOSUB",
			"GOTO",
			"GROUP",
			"HANDLES",
			"IF",
			"IMPLEMENTS",
			"IMPORTS",
			"IN",
			"INFER",
			"INHERITS",
			"INTEGER",
			"INTERFACE",
			"INTO",
			"IS",
			"ISNOT",
			"JOIN",
			"LET",
			"LIB",
			"LIKE",
			"LONG",
			"LOOP",
			"ME",
			"MOD",
			"MODULE",
			"MUSTINHERIT",
			"MUSTOVERRIDE",
			"MYBASE",
			"MYCLASS",
			"NAMESPACE",
			"NARROWING",
			"NEW",
			"NEXT",
			"NOT",
			"NOTHING",
			"NOTINHERITABLE",
			"NOTOVERRIDABLE",
			"OBJECT",
			"OF",
			"OFF",
			"ON",
			"OPERATOR",
			"OPTION",
			"OPTIONAL",
			"OR",
			"ORDER",
			"ORELSE",
			"OVERLOADS",
			"OVERRIDABLE",
			"OVERRIDES",
			"PARAMARRAY",
			"PARTIAL",
			"PRESERVE",
			"PRIVATE",
			"PROPERTY",
			"PROTECTED",
			"PUBLIC",
			"RAISEEVENT",
			"READONLY",
			"REDIM",
			"REM",
			"REMOVEHANDLER",
			"RESUME",
			"RETURN",
			"SBYTE",
			"SELECT",
			"SET",
			"SHADOWS",
			"SHARED",
			"SHORT",
			"SINGLE",
			"SKIP",
			"STATIC",
			"STEP",
			"STOP",
			"STRICT",
			"STRING",
			"STRUCTURE",
			"SUB",
			"SYNCLOCK",
			"TAKE",
			"TEXT",
			"THEN",
			"THROW",
			"TO",
			"TRUE",
			"TRY",
			"TRYCAST",
			"TYPEOF",
			"UINTEGER",
			"ULONG",
			"UNICODE",
			"UNTIL",
			"USHORT",
			"USING",
			"VARIANT",
			"WEND",
			"WHEN",
			"WHERE",
			"WHILE",
			"WIDENING",
			"WITH",
			"WITHEVENTS",
			"WRITEONLY",
			"XOR"
		};
		
		static LookupTable keywords = new LookupTable(false);
		
		static Keywords()
		{
			for (int i = 0; i < keywordList.Length; ++i) {
				keywords[keywordList[i]] = i + Tokens.AddHandler;
			}
		}
		
		public static int GetToken(string keyword)
		{
			return keywords[keyword];
		}
		
		public static bool IsNonIdentifierKeyword(string word)
		{
			int token = GetToken(word);
			if (token < 0)
				return false;
			return !Tokens.IdentifierTokens[token];
		}
	}
}
