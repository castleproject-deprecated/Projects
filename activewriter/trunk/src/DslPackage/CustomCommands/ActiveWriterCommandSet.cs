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

namespace Altinoren.ActiveWriter
{
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using Microsoft.VisualStudio.Modeling.Shell;
    using System;
    using ToolWindow;
    
    internal partial class ActiveWriterCommandSet
    {
        protected override IList<MenuCommand> GetMenuCommands()
        {
            IList <MenuCommand> commands = base.GetMenuCommands();

            // AW Class Details ToolWindow
            MenuCommand toolWindowMenuCommand = new CommandContextBoundMenuCommand(this.ServiceProvider,
                OnMenuViewClassDetails,
                Constants.ViewClassDetailsCommand,
                typeof(ActiveWriterEditorFactory).GUID);
            commands.Add(toolWindowMenuCommand);

            return commands;
        }

        internal void OnMenuViewClassDetails(object sender, EventArgs e)
        {
            ActiveWriterClassDetailsToolWindow classDetails = this.ActiveWriterClassDetailsToolWindow;
            if (classDetails != null)
            {
                classDetails.Show();
            }
        }

        protected ActiveWriterClassDetailsToolWindow ActiveWriterClassDetailsToolWindow
        {
            get
            {
                ActiveWriterClassDetailsToolWindow classDetails = null;
                ModelingPackage package = this.ServiceProvider.GetService(typeof(Microsoft.VisualStudio.Shell.Package)) as ModelingPackage;

                if (package != null)
                {
                    classDetails = package.GetToolWindow(typeof(ActiveWriterClassDetailsToolWindow), true) as ActiveWriterClassDetailsToolWindow;
                }

                return classDetails;
            }
        }
    }
}
