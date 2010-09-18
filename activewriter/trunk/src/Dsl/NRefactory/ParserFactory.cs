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
	using System.IO;
	using System.Text;

	public enum SupportedLanguage {
		CSharp,
		VBNet
	}
	
	/// <summary>
	/// Static helper class that constructs lexer and parser objects.
	/// </summary>
	public static class ParserFactory
	{
		public static Parser.ILexer CreateLexer(SupportedLanguage language, TextReader textReader)
		{
			switch (language) {
				case SupportedLanguage.CSharp:
					return new ICSharpCode.NRefactory.Parser.CSharp.Lexer(textReader);
				case SupportedLanguage.VBNet:
					return new ICSharpCode.NRefactory.Parser.VB.Lexer(textReader);
			}
			throw new System.NotSupportedException(language + " not supported.");
		}
		
		public static IParser CreateParser(SupportedLanguage language, TextReader textReader)
		{
			Parser.ILexer lexer = CreateLexer(language, textReader);
			switch (language) {
				case SupportedLanguage.CSharp:
					return new ICSharpCode.NRefactory.Parser.CSharp.Parser(lexer);
				case SupportedLanguage.VBNet:
					return new ICSharpCode.NRefactory.Parser.VB.Parser(lexer);
			}
			throw new System.NotSupportedException(language + " not supported.");
		}
		
		public static IParser CreateParser(string fileName)
		{
			return CreateParser(fileName, Encoding.UTF8);
		}
		
		public static IParser CreateParser(string fileName, Encoding encoding)
		{
			string ext = Path.GetExtension(fileName);
			if (ext.Equals(".cs", StringComparison.OrdinalIgnoreCase))
				return CreateParser(SupportedLanguage.CSharp, new StreamReader(fileName, encoding));
			if (ext.Equals(".vb", StringComparison.OrdinalIgnoreCase))
				return CreateParser(SupportedLanguage.VBNet, new StreamReader(fileName, encoding));
			return null;
		}
	}
}
