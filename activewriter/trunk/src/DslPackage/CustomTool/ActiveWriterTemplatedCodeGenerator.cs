// Copyright 2006 Gokhan Castle - http://altinoren.com/
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

namespace Castle.ActiveWriter.CustomTool
{
    using System;
    using ServerExplorerSupport;
    using EnvDTE;

    using System.IO;
    using System.Resources;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.TextTemplating.VSHost;
    
    [Guid("2E39FF45-B117-4126-9073-7DDCBEC3E113")]
    internal class ActiveWriterTemplatedCodeGenerator : TemplatedCodeGenerator
    {
        protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            string fileExtension;

            DTE dte = DTEHelper.GetDTE(currentProcess.Id.ToString());

            ProjectItem projectItem = dte.Solution.FindProjectItem(inputFileName);

            switch (DTEHelper.GetProjectLanguage(projectItem.ContainingProject))
            {
                case CodeLanguage.CSharp:
                    fileExtension = "cs";
                    break;
                case CodeLanguage.VB:
                    fileExtension = "vb";
                    break;
                default:
                    throw new ArgumentException(
                        "Unsupported project type. ActiveWriter currently supports C# and Visual Basic.NET projects.");
            }

            ResourceManager manager =
                new ResourceManager("Castle.ActiveWriter.VSPackage1",
                                    typeof (ActiveWriterTemplatedCodeGenerator).Assembly);
            FileInfo fi = new FileInfo(inputFileName);
                        inputFileContent =
                                            manager.GetObject("ActiveWriterReport").ToString()
                    .Replace("%MODELFILE%", fi.Name)
                    .Replace("%MODELFILEFULLNAME%", fi.FullName)
                    .Replace("%NAMESPACE%", FileNamespace)
                    .Replace("%EXT%", fileExtension)
                    .Replace("%PID%", currentProcess.Id.ToString());

            // Implemenentation of the text templating creates separate AddDomin. Invoking DTE
            // from non-default AddDomain is incorrect. As a workaround we run code generation in
            // separate thread to have all calls to DTE be automatically marshaled by COM to the
            // main thread and default AppDomain.
            GenerateCodeDelegate del = BaseGenerateCode;
            IAsyncResult async = del.BeginInvoke(inputFileName, inputFileContent, null, null);
            async.AsyncWaitHandle.WaitOne();
            byte[] data = del.EndInvoke(async);
            return data;
        }

        private delegate byte[] GenerateCodeDelegate(string inputFileName, string inputFileContent);

        private byte[] BaseGenerateCode(string inputFileName, string inputFileContent)
        {
            return base.GenerateCode(inputFileName, inputFileContent);
        }
    }
}
