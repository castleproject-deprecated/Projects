// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveWriter
{
    using Microsoft.VisualStudio.Modeling.Validation;
    using Castle.ActiveWriter.Validation;

    [ValidationState(ValidationState.Enabled)]
    public partial class NestedClass
    {
        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateExistanceOfPrimaryKeys(ValidationContext context)
        {
            if (Properties.Count > 0 && ValidationHelper.GetPrimaryKeyCount(Properties) > 0)
                context.LogError(
                    string.Format(
                        "Nested class {0} has primary key(s). Try moving the key to the nesting class instead.",
                        Name), "AW001ValidateExistanceOfPrimaryKeysError", this);
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateExistanceOfCompositeKeys(ValidationContext context)
        {
            if (Properties.Count > 0 && ValidationHelper.GetCompositeKeyCount(Properties) > 0)
                context.LogError(
                    string.Format(
                        "Nested class {0} has composite key(s). Try moving the key to the nesting class instead.",
                        Name), "AW001ValidateExistanceOfCompositeKeysError", this);
        }

        [ValidationMethod(ValidationCategories.Open | ValidationCategories.Save | ValidationCategories.Menu)]
        private void ValidateMultipleDebuggerDisplays(ValidationContext context)
        {
            if (Properties.Count > 0 && ValidationHelper.GetDebuggerDisplayCount(Properties) > 1)
                context.LogError(
                    string.Format(
                        "Class {0} has multiple debugger display attributes set. Only one is allowed per class.",
                        Name), "AW001ValidateMultipleDebuggerDisplaysError", this);
        }
    }
}
