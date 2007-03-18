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

// Big TODO: Combine with CodeGenerationHelper validations in a seperate structure

namespace Altinoren.ActiveWriter
{
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Modeling.Validation;

    [ValidationState(ValidationState.Enabled)]
    public partial class Model
    {
        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateClassNameAndDuplication(ValidationContext context)
        {
            if (Classes.Count > 0)
            {
                Dictionary<string, ModelClass> classes = new Dictionary<string, ModelClass>();

                foreach (ModelClass cls in Classes)
                {
                    if (string.IsNullOrEmpty(cls.Name))
                    {
                        // Since we need the name for uniqueness validation, we check the existance of the name here.
                        context.LogError("Class must have a name", "AW001ValidateClassNameError", cls);
                    }
                    else
                    {
                        if (classes.ContainsKey(cls.Name))
                            context.LogError("Class must have a unique name", "AW001ValidateDuplicateClassNameError",
                                             cls, classes[cls.Name]);
                        else
                            classes.Add(cls.Name, cls);
                    }
                }
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateMonoRailProjectName(ValidationContext context)
        {
            if (this.GenerateMonoRailProject && string.IsNullOrEmpty(this.MonoRailProjectName))
            {
                context.LogError("MonoRail project must have a name", "AW001ValidateMonoRailProjectName", this);
            }
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateMonoRailProjectPath(ValidationContext context)
        {
            if (this.GenerateMonoRailProject && string.IsNullOrEmpty(this.MonoRailProjectPath))
            {
                context.LogError("MonoRail project must have a path defined", "AW001ValidateMonoRailProjectPath", this);
            }
        }
    }
}