// Copyright 2006 Gokhan Altinoren - http://altinoren.com/
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

namespace Altinoren.ActiveWriter.CodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.CodeDom;
    using Microsoft.VisualStudio.Modeling;

    public static class CodeGenerationContext
    {
        private static bool isInitialized = false;
        private static Dictionary<ModelClass, CodeTypeDeclaration> classDeclarations;
        private static Dictionary<NestedClass, CodeTypeDeclaration> nestedClassDeclarations;
        private static List<string> generatedClassNames;

        public static void Initialize()
        {
            classDeclarations = new Dictionary<ModelClass, CodeTypeDeclaration>();
            nestedClassDeclarations = new Dictionary<NestedClass, CodeTypeDeclaration>();
            generatedClassNames = new List<string>();

            isInitialized = true;
        }

        public static void AddClass(ModelElement cls, CodeTypeDeclaration declaration)
        {
            if (!isInitialized)
                throw new ArgumentException("CodeGenerationContext must first be initialized");
            if (cls == null)
                throw new ArgumentNullException("No class supplied", "cls");
            if (declaration == null)
                throw new ArgumentNullException("No CodeTypeDeclaration supplied", "declaration");

            if (cls.GetType() == typeof(ModelClass))
            {
                ModelClass modelClass = (ModelClass)cls;
                if (!classDeclarations.ContainsKey(modelClass))
                    classDeclarations.Add(modelClass, declaration);
            }
            else if (cls.GetType() == typeof(NestedClass))
            {
                NestedClass nestedClass = (NestedClass)cls;
                if (!nestedClassDeclarations.ContainsKey(nestedClass))
                    nestedClassDeclarations.Add(nestedClass, declaration);
            }
            else
                throw new ArgumentException("Only ModelClass and NestedClass entities supported", "cls");

            generatedClassNames.Add(((NamedElement)cls).Name);
        }
        
        public static CodeTypeDeclaration GetTypeDeclaration(ModelElement cls)
        {
            if (!isInitialized)
                throw new ArgumentException("CodeGenerationContext must first be initialized");
            if (cls == null)
                throw new ArgumentNullException("No class supplied", "cls");

            if (cls.GetType() == typeof(ModelClass))
            {
                return classDeclarations[(ModelClass)cls];
            }
            else if (cls.GetType() == typeof(NestedClass))
            {
                return nestedClassDeclarations[(NestedClass)cls];
            }
            else
                throw new ArgumentException("Only ModelClass and NestedClass entities supported", "cls");
        }

        public static bool IsClassGenerated(ModelElement cls)
        {
            if (!isInitialized)
                throw new ArgumentException("CodeGenerationContext must first be initialized");
            if (cls == null)
                throw new ArgumentNullException("No class supplied", "cls");

            if (cls.GetType() == typeof(ModelClass))
            {
                return classDeclarations.ContainsKey((ModelClass)cls);
            }
            else if (cls.GetType() == typeof(NestedClass))
            {
                return nestedClassDeclarations.ContainsKey((NestedClass)cls);
            }
            else
                throw new ArgumentException("Only ModelClass and NestedClass entities supported", "cls");
        }

        public static bool IsClassWithSameNameGenerated(string name)
        {
            if (!isInitialized)
                throw new ArgumentException("CodeGenerationContext must first be initialized");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("Name not supplied", "name");

            return generatedClassNames.Contains(name);
        }
    }
}
