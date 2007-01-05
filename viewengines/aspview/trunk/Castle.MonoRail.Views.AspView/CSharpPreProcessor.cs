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

	public class CSharpPreProcessor : LanguagePreProcessor
	{
        protected override void ProcessProperties(string propertiesSection, Dictionary<string, string> properties)
        {
            propertiesSection = StripRemarks(propertiesSection);
            string[] propertiesDeclerations = propertiesSection.Split(';');
            foreach (string propertiesDecleration in propertiesDeclerations)
            {
                string prop = propertiesDecleration.Trim();
                if (prop == string.Empty)
                    continue;
                int lastSpace = prop.LastIndexOf(" ");
                if (lastSpace == -1)
                    throw new Exception("Illegal property decleration: '" + prop + "'");
                string type = prop.Substring(0, lastSpace).Trim();
                string name = prop.Substring(lastSpace).Trim();
                properties.Add(name, type);
            }

        }

		protected override string ClassFileExtension
		{
            get { return ".cs";}
		}

		protected override string StripRemarks(string source)
		{
			return source;
		}

		protected override void WriteNamespace(StringWriter writer, string assemblyNamespace)
		{
			writer.WriteLine("namespace {0}", assemblyNamespace);
		}

		protected override void WriteImports(StringWriter writer, System.Collections.Generic.Dictionary<string, object> imports)
		{
			foreach (string import in imports.Keys)
				writer.WriteLine("using {0};", import);
		}

		protected override void WriteClassDecleration(StringWriter writer, string className)
		{
			writer.WriteLine("public class {0} : AspViewBase", className);
		}

		protected override void WriteConstructorDefinition(StringWriter writer, string className)
		{
			writer.WriteLine(
				"public {0}(AspViewEngine viewEngine, TextWriter output, IRailsEngineContext context, Controller controller) : base(viewEngine,output,context,controller) {{ }}",
				className);
		}

		protected override void WriteProperties(StringWriter writer, Dictionary<string, string> properties)
		{
            foreach (string name in properties.Keys)
            {
                string type = properties[name];
                writer.WriteLine(
                    "private {0} {1} {{ get {{ return ({0})GetParameter(\"{1}\"); }} }}",
                    type, name);
            }
        }

		protected override void WriteRenderDecleration(StringWriter writer)
		{
            writer.WriteLine("public override void Render()");
        }

		protected override void WriteRenderBody(StringWriter writer, string viewBody)
		{
            int index = 0;
            while (index > -1)
            {
                if (viewBody.IndexOf("<%", index) == index)
                {
                    string code = GetCode(viewBody, ref index);
                    if (code[0] == '=' && code.Length > 0)
                        code = "Output(" + code.Substring(1) + ");";
                    writer.WriteLine(code);
                }
                else
                {
                    string markup = GetMarkup(viewBody, ref index);
                    markup = markup.Replace("\"", "\"\"");
                    markup = markup.Replace("~~", "\"); Output(fullSiteRoot); Output(@\"");
                    markup = markup.Replace("~", "\"); Output(siteRoot); Output(@\"");
                    writer.Write("Output(@\"");
                    writer.Write(markup);
                    writer.WriteLine("\");");
                }
            }
        }

        protected override void WriteViewDirectoryProperty(StringWriter writer, string viewDirectory)
        {
            writer.WriteLine(
                @"protected override string ViewDirectory {{ get {{ return ""{0}""; }} }}", viewDirectory.Replace("\\","\\\\"));
        }

        protected override void WriteViewNameProperty(StringWriter writer, string viewName)
        {
            writer.WriteLine(
                @"protected override string ViewName {{ get {{ return ""{0}""; }} }}", viewName.Replace("\\", "\\\\"));
        }

		protected override void WriteOpenTag(StringWriter writer)
		{
            writer.WriteLine("{");
		}

        protected override void WriteSubCloseTag(StringWriter writer)
        {
            writer.WriteLine("}");
        }

        /// <summary>
        /// Writes current language's close tag for class to the buffer
        /// </summary>
        /// <param name="writer">A StringWriter that writes the generated class</param>
        protected override void WriteClassCloseTag(StringWriter writer)
        {
            writer.WriteLine("}");
        }


        /// <summary>
        /// Writes current language's close tag for namespace to the buffer
        /// </summary>
        /// <param name="writer">A StringWriter that writes the generated class</param>
        protected override void WriteNamespaceCloseTag(StringWriter writer)
        {
            writer.WriteLine("}");
        }

    }
}