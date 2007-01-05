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
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using Castle.MonoRail.Framework.Views;
    using System.Collections.Specialized;
    using System.IO;
    using Castle.MonoRail.Framework;

    public abstract class LanguagePreProcessor
    {
        #region static readonly members
        private static readonly Regex findImportsDirectives = new Regex("<%@\\s*Import\\s+Namespace\\s*=\\s*\"(?<namespace>.*)\"\\s*%>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex findSubViewTags = new Regex("<subView:(?<viewName>[\\w\\.]+)\\s*(?<attributes>.*)\\s*>\\s*</subView:\\k<viewName>>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex findAttributes = new Regex("\\s*(?<name>\\w+)\\s*=\\s*\"(?<value>\\w*)\"\\s*");
        private static readonly string assemblyNamespace = "CompiledViews";
        private static readonly string assemblyFileName = assemblyNamespace + ".dll";
        #endregion

		public LanguagePreProcessor()
        {
        }

        #region Process
        /// <summary>
        /// Transferres an AspView file to a concrete CLI compliant class in C# or VB.NET (VB.NET not implemented yet)
        /// </summary>
		/// <param name="file">An AspViewFile to be processed</param>
        public void Process(AspViewFile file)
        {
            int index = 0;

            Dictionary<string, object> imports = new Dictionary<string, object>();
            Dictionary<string, object> references = new Dictionary<string, object>();
            Dictionary<string, string> properties = new Dictionary<string, string>();

            string directivesSection = GetDirectivesSection(file.ViewSource, ref index);
			string propertiesSection = GetPropertiesSection(file.ViewSource, ref index);
			string viewBody = GetViewBody(file.ViewSource, ref index);

            ProcessDirectives(directivesSection, imports, references);
            ProcessProperties(propertiesSection, properties);

            viewBody = ProcessViewBody(viewBody);

			StringWriter writer = new StringWriter();
			WriteImports(writer, imports);
			WriteNamespace(writer, assemblyNamespace);
			WriteOpenTag(writer);
			WriteClassDecleration(writer, file.ClassName);
			WriteOpenTag(writer);
			WriteConstructorDefinition(writer, file.ClassName);
			WriteProperties(writer, properties);
			WriteRenderDecleration(writer);
			WriteOpenTag(writer);
			WriteRenderBody(writer, viewBody);
			WriteSubCloseTag(writer);
            WriteViewNameProperty(writer, file.ViewName);
            WriteViewDirectoryProperty(writer, GetDirectory(file.ViewName));
			WriteClassCloseTag(writer);
			WriteNamespaceCloseTag(writer);

			file.ConcreteClass = writer.GetStringBuilder().ToString();
        }

        private string ProcessViewBody(string viewBody)
        {
            return findSubViewTags.Replace(viewBody,SubViewTagHandler);
        }

        private string SubViewTagHandler(Match match)
        {
            string viewName = match.Groups["viewName"].Value.Replace('.', '/');
            string arguments = match.Groups["attributes"].Value;
            StringBuilder argumentsString = new StringBuilder();
            bool first = true;
            foreach (Match attribute in findAttributes.Matches(arguments))
            {
                argumentsString.AppendFormat(",\"{0}\", {1}",
                    attribute.Groups["name"].Value,
                    attribute.Groups["value"].Value);
            }
            return string.Format("<% OutputSubView(\"{0}\"{1}); %>",
                viewName,
                argumentsString.ToString());
        }


        private string GetDirectory(string viewName)
        {
            int lastSlash = viewName.LastIndexOf('\\');
            if (lastSlash == -1)
                return viewName;
            return viewName.Substring(0, lastSlash);
        }
        /// <summary>
        /// Writes the implementation for ViewDirectory Property
        /// </summary>
        /// <param name="writer">A StringWriter that writes the generated class</param>
        /// <param name="viewDirectory">The current view's directory</param>
        protected abstract void WriteViewDirectoryProperty(StringWriter writer, string viewDirectory);
        /// <summary>
        /// Writes the implementation for ViewName Property
        /// </summary>
        /// <param name="writer">A StringWriter that writes the generated class</param>
        /// <param name="viewName">The current view's name</param>
        protected abstract void WriteViewNameProperty(StringWriter writer, string viewName);

        /// <summary>
        /// Transferres an bunch of AspView files to a concrete CLI compliant classes in C# or VB.NET (VB.NET not implemented yet)
        /// </summary>
		/// <param name="files">A list of AspViewFiles to be processed</param>
        public void Process(List<AspViewFile> files)
        {
			foreach (AspViewFile file in files)
				Process(file);
        }

        /// <summary>
        /// Saves a source file to the output folder
        /// </summary>
        /// <param name="path">The path to the output folder</param>
        /// <param name="className">The name of the class that will be saved</param>
        /// <param name="processedFile">The class</param>
        private void SaveFile(string path, string className, string processedFile)
        {
            string fileName = Path.Combine(path, processedFile + ClassFileExtension);
            File.WriteAllText(fileName, processedFile);
        }

        /// <summary>
        /// Gets the generated classes file extension
        /// </summary>
        protected abstract string ClassFileExtension { get;}
        #endregion

        #region initial parsing
        /// <summary>
        /// Parses the source and gets the directives section
        /// </summary>
        /// <param name="viewSource">Source</param>
        /// <param name="index">The index to start searching. Will be updated to the end of the found section</param>
        /// <returns>The directive section</returns>
        protected string GetDirectivesSection(string viewSource, ref int index)
        {
            int start = viewSource.LastIndexOf("<%@");
            if (start == -1)
                return string.Empty;
            int end = viewSource.IndexOf("%>", start);
            if (end == -1)
                throw new Exception("%> expected");
            string directivesSection = viewSource.Substring(index, end - index + 2);
            index = end + 2;
            if (index > viewSource.Length - 1)
                index = -1;
            return directivesSection;
        }

        /// <summary>
        /// Parses the source and gets the properties section
        /// </summary>
        /// <param name="viewSource">Source</param>
        /// <param name="index">The index to start searching. Will be updated to the end of the found section</param>
        /// <returns>The properties section</returns>
        protected string GetPropertiesSection(string viewSource, ref int index)
        {
            int start = viewSource.IndexOf("<%", index);
            if (start == -1)
                throw new Exception("Properties section was not found.\r\nThere must be a properties section after the directives(<%@ %>) section and before the view body");
            int end = viewSource.IndexOf("%>", start);
            if (end == -1)
                throw new Exception("%> expected");
            string propertiesSection = viewSource.Substring(start + 2, end - start - 4);
            index = end + 2;
            if (index > viewSource.Length - 1)
                index = -1;
            return propertiesSection.Replace("\r", "").Replace("\n", "");
        }

        /// <summary>
        /// Parses the source and gets the view body
        /// </summary>
        /// <param name="viewSource">Source</param>
        /// <param name="index">The index to start searching. Will be updated to the end of the found section</param>
        /// <returns>The view body</returns>
        protected string GetViewBody(string viewSource, ref int index)
        {
            return viewSource.Substring(index);
        }
        #endregion

        #region initial processing
        /// <summary>
        /// Parses the direvtives (<%@ %>) into containers of imports (implemented) and references(not yet implemented)
        /// </summary>
        /// <param name="directivesSection">Source</param>
        /// <param name="imports">Imports container</param>
        /// <param name="references">References container</param>
        protected void ProcessDirectives(string directivesSection, Dictionary<string, object> imports, Dictionary<string, object> references)
        {
            MatchCollection matches = findImportsDirectives.Matches(directivesSection);
            imports["System"] = null;
            imports["System.IO"] = null;
            imports["System.Collections"] = null;
            imports["System.Collections.Generic"] = null;
            imports["Castle.MonoRail.Framework"] = null;
            imports["Castle.MonoRail.Views.AspView"] = null;
            foreach (Match match in matches)
            {
                string import = findImportsDirectives.Replace(match.Value, "${namespace}");
                imports[import] = null;
            }
        }

        /// <summary>
        /// Parses a raw properties section (<% string s; %>) into a container of <name, type>
        /// </summary>
        /// <param name="propertiesSection">The properties section from the source view</param>
        /// <param name="properties">The properties <name,type> container(</param>
        protected abstract void ProcessProperties(string propertiesSection, Dictionary<string, string> properties);

        /// <summary>
        /// Gets the next markup string from the view's source
        /// </summary>
        /// <param name="viewBody">The view's source</param>
        /// <param name="index">The index to start looking from, will be updated to the end of the found section</param>
        /// <returns>Markup string</returns>
        protected string GetMarkup(string viewBody, ref int index)
        {
            string markup = string.Empty;
            int end = viewBody.IndexOf("<%", index);
            if (end == -1)
            {
                markup = viewBody.Substring(index);
                index = -1;
            }
            else
            {
                markup = viewBody.Substring(index, end - index);
                index = end;
            }
            while (markup.IndexOf("\r\n") == 0)
                markup = markup.Substring(2);
//            while (markup.LastIndexOf("\r\n") == markup.Length - 2)
//                markup = markup.Substring(0, markup.Length - 2);
            return markup;
        }

        /// <summary>
        /// Gets the next code string from the view's source
        /// </summary>
        /// <param name="viewBody">The view's source</param>
        /// <param name="index">The index to start looking from, will be updated to the end of the found section</param>
        /// <returns>Code string</returns>
        protected string GetCode(string viewBody, ref int index)
        {
            int end = viewBody.IndexOf("%>", index);
            if (index == -1)
                throw new Exception("%> Expected");
            string code = viewBody.Substring(index + 2, end - index - 2);
            if (end + 2 == viewBody.Length)
                index = -1;
            else
                index = end + 2;
            return code.Trim();
        }

        /// <summary>
        /// Should strip remarks from a source block
        /// </summary>
        /// <param name="source">Source (with remarks)</param>
        /// <returns>the same source but without the remarks</returns>
        protected abstract string StripRemarks(string source);
        #endregion

        #region Writing code
        /// <summary>
        /// Writes namespace decleration to the buffer
        /// </summary>
		/// <param name="writer">A StringWriter that writes the generated class</param>
		/// <param name="properties">The namespace</param>
		protected abstract void WriteNamespace(StringWriter writer, string assemblyNamespace);

        /// <summary>
        /// Writes imports decleration to the buffer
        /// </summary>
		/// <param name="writer">A StringWriter that writes the generated class</param>
		/// <param name="properties">Imports container</param>
		protected abstract void WriteImports(StringWriter writer, Dictionary<string, object> imports);

        /// <summary>
        /// Writes current language's class decleration to the buffer
        /// </summary>
		/// <param name="writer">A StringWriter that writes the generated class</param>
		protected abstract void WriteClassDecleration(StringWriter writer, string className);

        /// <summary>
        /// Writes a constructor definition to the current string
        /// </summary>
		/// <param name="writer">A StringWriter that writes the generated class</param>
		/// <param name="className">The generated class name</param>
		protected abstract void WriteConstructorDefinition(StringWriter writer, string className);

        /// <summary>
        /// Writes properties decleration to the buffer
        /// </summary>
		/// <param name="writer">A StringWriter that writes the generated class</param>
		/// <param name="properties">Properties container</param>
		protected abstract void WriteProperties(StringWriter writer, Dictionary<string, string> properties);

        /// <summary>
        /// Writes the Render method decleration to the buffer
        /// </summary>
		/// <param name="writer">A StringWriter that writes the generated class</param>
		protected abstract void WriteRenderDecleration(StringWriter writer);

        /// <summary>
        /// Writes Output.Write statements according to the view body
        /// </summary>
		/// <param name="writer">A StringWriter that writes the generated class</param>
		/// <param name="viewBody">The view's source</param>
		protected abstract void WriteRenderBody(StringWriter writer, string viewBody);

        /// <summary>
        /// Writes current language's open tag to the buffer
        /// </summary>
		/// <param name="writer">A StringWriter that writes the generated class</param>
		protected abstract void WriteOpenTag(StringWriter writer);

        /// <summary>
        /// Writes current language's close tag for subroutines to the buffer
        /// </summary>
        /// <param name="writer">A StringWriter that writes the generated class</param>
        protected abstract void WriteSubCloseTag(StringWriter writer);

        /// <summary>
        /// Writes current language's close tag for class to the buffer
        /// </summary>
        /// <param name="writer">A StringWriter that writes the generated class</param>
        protected abstract void WriteClassCloseTag(StringWriter writer);

        /// <summary>
        /// Writes current language's close tag for namespace to the buffer
        /// </summary>
        /// <param name="writer">A StringWriter that writes the generated class</param>
        protected abstract void WriteNamespaceCloseTag(StringWriter writer);
        #endregion
    }
}