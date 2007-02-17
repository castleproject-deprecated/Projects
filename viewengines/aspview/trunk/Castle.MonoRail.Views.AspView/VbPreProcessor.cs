// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MonoRail.Views.AspView
{
	using System;
    using System.IO;
    using System.Collections.Generic;

	public class VbPreProcessor : LanguagePreProcessor
	{
        protected override void ProcessProperties(string propertiesSection, Dictionary<string, ViewProperty> properties)
        {
            propertiesSection = StripRemarks(propertiesSection);
            string[] propertiesDeclerations = propertiesSection.Split(';');
            foreach (string propertiesDecleration in propertiesDeclerations)
            {
                string prop = propertiesDecleration.Trim();
                if (prop == string.Empty)
                    continue;
                string[] words = prop.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (words.Length != 4)
                    throw new Exception("Illegal property decleration: '" + prop + "'");
                string type = words[3];
                string name = words[1];
                if (name.EndsWith("()"))
                {
                    name = name.Remove(name.Length - 2);
                    type += "()";
                }
                properties.Add(name, new ViewProperty(name, type, null));
            }

        }
		protected override string ClassFileExtension
		{
            get { return ".vb"; }
        }

        protected override string StripRemarks(string source)
        {
            return source;
        }

        protected override void WriteNamespace(StringWriter writer, string assemblyNamespace)
        {
            writer.WriteLine("Namespace {0}", assemblyNamespace);
        }

        protected override void WriteImports(StringWriter writer, System.Collections.Generic.Dictionary<string, object> imports)
        {
            foreach (string import in imports.Keys)
                writer.WriteLine("Imports {0}", import);
        }

        protected override void WriteClassDecleration(StringWriter writer, string className)
        {
            writer.WriteLine("Public Class {0}",className);
            writer.WriteLine("Inherits AspViewBase");
        }

        protected override void WriteConstructorDefinition(StringWriter writer, string className)
        {
            writer.WriteLine(
                "Public Sub New(viewEngine As AspViewEngine, output As TextWriter, context As IRailsEngineContext, controller As Controller)");
            writer.WriteLine(
                "MyBase.New(viewEngine,output,context,controller)");
            writer.WriteLine("End Sub");
        }

        protected override void WriteProperties(StringWriter writer, Dictionary<string, ViewProperty> properties)
        {
            foreach (string name in properties.Keys)
            {
                ViewProperty prop = properties[name];
                writer.WriteLine("Private ReadOnly Property {0}() As {1}", prop.Name, prop.Type);
                writer.WriteLine("Get");
                writer.WriteLine("Return DirectCast(Properties(\"{0}\"), {1})", prop.Name, prop.Type);
                writer.WriteLine("End Get");
                writer.WriteLine("End Property");
            }
        }

        protected override void WriteRenderDecleration(StringWriter writer)
        {
            writer.WriteLine("Public Overloads Overrides Sub Render()");
        }

        protected override void WriteRenderBody(StringWriter writer, string viewBody)
        {
            int index = 0;
            while (index > -1)
            {
                if (viewBody.IndexOf("<%", index) == index)
                {
                    string code = GetCode(viewBody, ref index);
                    code = code.Replace("\"", "\"\"");
                    code = code.Replace("\r\n", "\" & _ \r\n \"");
                    if (code[0] == '=' && code.Length > 0)
                        code = "Output.Write(" + code.Substring(1) + ")";
                    writer.WriteLine(code);
                }
                else
                {
                    string markup = GetMarkup(viewBody, ref index);
                    writer.Write("Output.Write(\"");
                    markup = markup.Replace("\r\n", "\" & \"\" & Chr(13) & \"\" & Chr(10) & _ \r\n \"");
                    writer.Write(markup);
                    writer.WriteLine("\")");
                }
            }
        }

        protected override void WriteViewDirectoryProperty(StringWriter writer, string viewDirectory)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void WriteViewNameProperty(StringWriter writer, string viewName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void WriteOpenTag(StringWriter writer)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override void WriteSubCloseTag(StringWriter writer)
        {
            writer.WriteLine("End Sub");
        }

        /// <summary>
        /// Writes current language's close tag for class to the buffer
        /// </summary>
        /// <param name="writer">A StringWriter that writes the generated class</param>
        protected override void WriteClassCloseTag(StringWriter writer)
        {
            writer.WriteLine("End Class");
        }


        /// <summary>
        /// Writes current language's close tag for namespace to the buffer
        /// </summary>
        /// <param name="writer">A StringWriter that writes the generated class</param>
        protected override void WriteNamespaceCloseTag(StringWriter writer)
        {
            writer.WriteLine("End Namespace");
        }

        protected override string GetViewFilterCloseStatement()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override string GetLateBoundViewFilterOpenStatement(string filterName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override string GetEarlyBoundViewFilterOpenStatement(string filterName)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        protected override string GetSubViewStatement(string viewName, string parameters)
        {
            throw new Exception("The method or operation is not implemented.");
        }
		/// <summary>
		/// Writes handlers for the view component sections
		/// </summary>
		/// <param name="writer">A StringWriter that writes the generated class</param>
		protected override void WriteSectionHandlers(StringWriter writer)
		{
			throw new Exception("The method or operation is not implemented.");
		}
    }
}