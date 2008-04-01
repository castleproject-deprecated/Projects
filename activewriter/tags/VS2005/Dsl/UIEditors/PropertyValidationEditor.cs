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

namespace Altinoren.ActiveWriter.UIEditors
{
    using System;
    using System.Collections;
    using System.Drawing.Design;
    using System.Windows.Forms;
    using Microsoft.VisualStudio.Modeling.Design;

    public class PropertyValidationEditor : UITypeEditor
    {
        /// <summary>
        /// Display modal form as the editor
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            ElementPropertyDescriptor propertyDescriptor = context.PropertyDescriptor as ElementPropertyDescriptor;
            if (propertyDescriptor != null)
            {
                ModelProperty underlyingModelElement = propertyDescriptor.ModelElement as ModelProperty;

                ArrayList list = null;

                string actualValue = value as string;

                if (!string.IsNullOrEmpty(actualValue))
                    list = ModelProperty.DeserializeValidatorList(actualValue);

                PropertyValidationEditorForm editor = new PropertyValidationEditorForm(list);
                if (editor.ShowDialog() == DialogResult.OK)
                {
                    if (editor.Value != null)
                        value = ModelProperty.SerializeValidatorList(editor.Value);
                    else
                        value = null;
                }
            }

            return value;
        }
    }
}

