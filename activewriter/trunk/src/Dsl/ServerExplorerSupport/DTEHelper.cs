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

using System;

namespace Altinoren.ActiveWriter.ServerExplorerSupport
{
    using EnvDTE;
    using Microsoft.VisualStudio.Modeling;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes; 

    public static class DTEHelper
    {
        [DllImport("ole32.dll")]
        public static extern int GetRunningObjectTable(int reserved, out IRunningObjectTable prot);

        [DllImport("ole32.dll")]
        public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc); 
        
        [CLSCompliant(false)]
        public static DTE GetDTE(Store store)
        {
            return store.GetService(typeof (_DTE)) as DTE;
        }

        [CLSCompliant(false)]
        public static DTE GetDTE(string processID)
        {
            IRunningObjectTable prot;
            IEnumMoniker pMonkEnum;

            string progID = Common.vsProgIdBase + processID;

            GetRunningObjectTable(0, out prot);
            prot.EnumRunning(out pMonkEnum);
            pMonkEnum.Reset();

            IntPtr fetched = IntPtr.Zero;
            IMoniker[] pmon = new IMoniker[1];
            while (pMonkEnum.Next(1, pmon, fetched) == 0)
            {
                IBindCtx pCtx;
                CreateBindCtx(0, out pCtx);
                string str;
                pmon[0].GetDisplayName(pCtx, null, out str);
                if (str == progID)
                {
                    object objReturnObject;
                    prot.GetObject(pmon[0], out objReturnObject);
                    DTE ide = (DTE)objReturnObject;
                    return ide;
                }
            }

            return null;
        }

        [CLSCompliant(false)]
        public static CodeLanguage GetProjectLanguage(Project project)
        {
            switch (project.Kind)
            {
                case VSLangProj.PrjKind.prjKindCSharpProject:
                //case "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}":
                    return CodeLanguage.CSharp;
                case VSLangProj.PrjKind.prjKindVBProject:
                //case "{164B10B9-B200-11D0-8C61-00A0C91E29D5}":
                    return CodeLanguage.VB;
                default:
                    throw new ArgumentException(
                        "Unsupported project type. ActiveWriter currently supports C# and Visual Basic.NET projects.");
            }
        }

		[CLSCompliant(false)]
		public static string GetAssemblyName(Project project)
		{
			Property property = project.Properties.Item("AssemblyName");
			if (property != null)
				return property.Value.ToString();
			else
				return Common.InMemoryCompiledAssemblyName;
		}
    }
}