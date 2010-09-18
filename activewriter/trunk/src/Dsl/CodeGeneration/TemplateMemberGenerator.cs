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

namespace Castle.ActiveWriter.CodeGeneration
{
	using System;
	using System.CodeDom;
	using System.IO;
	using CodeDomExtensions;
	using ICSharpCode.NRefactory;
	using ICSharpCode.NRefactory.Visitors;

	public class TemplateMemberGenerator
    {
        private CodeGenerationContext Context { get; set; }
        private CodeCompileUnit TemplateCompileUnit { get; set; }

        public TemplateMemberGenerator(CodeGenerationContext context)
        {
            Context = context;

            if (!String.IsNullOrEmpty(context.Model.MemberTemplateFile))
            {
                string templateFile = context.Model.MemberTemplateFile;
                if (!Path.IsPathRooted(templateFile))
                    templateFile = Helper.FindFile(context.ModelFilePath, templateFile);

                if (templateFile == null || !File.Exists(templateFile))
                    throw new FileNotFoundException("Template file not found", context.Model.MemberTemplateFile);

                SupportedLanguage language;
                if (templateFile.EndsWith(".cs", StringComparison.InvariantCultureIgnoreCase))
                    language = SupportedLanguage.CSharp;
                else if (templateFile.EndsWith(".vb", StringComparison.InvariantCultureIgnoreCase))
                    language = SupportedLanguage.VBNet;
                else
                    throw new Exception("Only .cs and .vb files are supported for MemberTemplateFile.");

                using (StreamReader reader = new StreamReader(templateFile))
                {
                    IParser parser = ParserFactory.CreateParser(language, reader);
                    parser.Parse();

                    if (parser.Errors.Count > 0)
                        throw new Exception("Error detected parsing MemberTemplateFile.");

                    CodeDomVisitor visit = new CodeDomVisitor();

                    visit.VisitCompilationUnit(parser.CompilationUnit, null);

                    // Remove Unsed Namespaces
                    for (int i = visit.codeCompileUnit.Namespaces.Count - 1; i >= 0; i--)
                    {
                        if (visit.codeCompileUnit.Namespaces[i].Types.Count == 0)
                        {
                            visit.codeCompileUnit.Namespaces.RemoveAt(i);
                        }
                    }

                    TemplateCompileUnit = visit.codeCompileUnit;
                }
            }
        }

        public void AddTemplateUsings()
        {
            if (TemplateCompileUnit != null)
            {
                foreach (CodeNamespaceImport import in TemplateCompileUnit.Namespaces[0].Imports)
                {
                    bool importPresent = false;
                    foreach (CodeNamespaceImport existingImport in Context.CompileUnit.Namespaces[0].Imports)
                        if (existingImport.Namespace == import.Namespace)
                            importPresent = true;

                    if (!importPresent)
                        Context.CompileUnit.Namespaces[0].Imports.Add(import);
                }
            }
        }

        /* The following are the parts of the CodeDOM class hierarchy that may not have have type replacement methods written.
              o System.CodeDom.CodeAttributeArgument (CodeExpression)
              o System.CodeDom.CodeAttributeDeclaration
              o System.CodeDom.CodeNamespaceImportCollection --- System.Collections.IList, System.Collections.ICollection, System.Collections.IEnumerable
              o System.CodeDom.CodeObject
                    + System.CodeDom.CodeComment
                    + System.CodeDom.CodeCompileUnit
                          # System.CodeDom.CodeSnippetCompileUnit
                    + System.CodeDom.CodeNamespace
                    + System.CodeDom.CodeNamespaceImport
                    + System.CodeDom.CodeTypeMember
                          # System.CodeDom.CodeMemberEvent
                          # System.CodeDom.CodeMemberField
                          # System.CodeDom.CodeMemberMethod
                                * System.CodeDom.CodeConstructor
                                * System.CodeDom.CodeEntryPointMethod
                                * System.CodeDom.CodeTypeConstructor
                          # System.CodeDom.CodeMemberProperty
                          # System.CodeDom.CodeSnippetTypeMember
                          # System.CodeDom.CodeTypeDeclaration
                                * System.CodeDom.CodeTypeDelegate
                    + System.CodeDom.CodeTypeReference
              o System.Collections.CollectionBase
                    + System.CodeDom.CodeAttributeArgumentCollection
                    + System.CodeDom.CodeAttributeDeclarationCollection
                    + System.CodeDom.CodeCommentStatementCollection
                    + System.CodeDom.CodeNamespaceCollection
                    + System.CodeDom.CodeParameterDeclarationExpressionCollection
                    + System.CodeDom.CodeTypeDeclarationCollection
                    + System.CodeDom.CodeTypeMemberCollection
        */

        public void AddTemplateMembers(ModelClass modelClass, CodeTypeDeclaration classDeclaration)
        {
            if (TemplateCompileUnit != null)
            {
                CodeTypeMemberCollection methods = new CodeTypeMemberCollection();
                CodeTypeMemberCollection properties = new CodeTypeMemberCollection();

                foreach (CodeTypeMember member in TemplateCompileUnit.Namespaces[0].Types[0].Members)
                {
                    if (member is CodeMemberMethod)
                    {
                        CodeMemberMethod method = ((CodeMemberMethod)member).Clone();


                        // If type is generic, replace all type parameters with the generated class name.
                        if (TemplateCompileUnit.Namespaces[0].Types[0].TypeParameters.Count > 0)
                        {
                            string typeParameter = TemplateCompileUnit.Namespaces[0].Types[0].TypeParameters[0].Name;

                            // If there's a generic parameter, we only need to add new if the parameter signature is the same.
                            if (modelClass.IsSubclass && !method.Parameters.ContainsType(typeParameter))
                                method.Attributes |= MemberAttributes.New;

                            method.ReplaceType(typeParameter, modelClass.Name);
                        }
                        else
                        {
                            // If there's no type argument, the signature will surely match and we need to add new or something similar.
                            if (modelClass.IsSubclass)
                                method.Attributes |= MemberAttributes.New;
                        }

                        methods.Add(method);
                    }
                    else if (member is CodeMemberProperty)
                    {
                        CodeMemberProperty property = ((CodeMemberProperty)member).Clone();

                        if (modelClass.IsSubclass)
                            property.Attributes |= MemberAttributes.New;

                        // If type is generic, replace all type parameters with the generated class name.
                        if (TemplateCompileUnit.Namespaces[0].Types[0].TypeParameters.Count > 0)
                        {
                            string typeParameter = TemplateCompileUnit.Namespaces[0].Types[0].TypeParameters[0].Name;

                            property.ReplaceType(typeParameter, modelClass.Name);
                        }

                        properties.Add(property);
                    }
                }

                AddRegionMarkers(methods, "Methods imported from Member Template File");
                classDeclaration.Members.AddRange(methods);

                AddRegionMarkers(properties, "Properties imported from Member Template File");
                classDeclaration.Members.AddRange(properties);
            }
        }

        private void AddRegionMarkers(CodeTypeMemberCollection members, string description)
        {
            if (members.Count > 0)
            {
                members[0].StartDirectives.Insert(0, new CodeRegionDirective(CodeRegionMode.Start, description));
                members[members.Count - 1].EndDirectives.Add(new CodeRegionDirective(CodeRegionMode.End, null));
            }
        }
    }
}
