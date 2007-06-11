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

namespace Altinoren.ActiveWriter.ARValidators
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.ComponentModel;
    using CodeGeneration;

    [Serializable]
    public class ValidateSet : AbstractValidation
    {
        private string[] _set;
        private string _type;

        public ValidateSet()
        {
            base.friendlyName = "Set";
        }

        [Category("Set")]
        [Description("The set of values to compare against.")]
        public string[] Set
        {
            get { return _set; }
            set { _set = value; }
        }

        [Category("Set")]
        [Description("The System.Type name of an enum class. The enum names will be added to the contents of the set.")]
        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public override bool IsValid(List<string> errorList)
        {
            if (errorList == null)
                throw new ArgumentNullException("errorList");

            if ((_set == null || Set.Length == 0) && string.IsNullOrEmpty(_type))
            {
                errorList.Add("ValidateSet: Either a set of values or an enum type should be provided.");
                return false;
            }

            if ((_set != null && Set.Length > 0) && !string.IsNullOrEmpty(_type))
            {
                errorList.Add("ValidateSet: Either a set of values or an enum type, but noth both, should be provided.");
                return false;
            }

            return true;
        }

        public override CodeAttributeDeclaration GetAttributeDeclaration()
        {
            List<string> errorList = new List<string>();
            if (!IsValid(errorList))
                throw new ArgumentException(errorList[0]);

            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("ValidateSet");

            if (_set != null && _set.Length > 0)
            {
            	attribute.Arguments.Add(AttributeHelper.GetStringArrayAttributeArgument(_set));
				base.AddAttributeArguments(attribute, ErrorMessagePlacement.First);
            }
            else
            {
            	attribute.Arguments.Add(AttributeHelper.GetPrimitiveTypeAttributeArgument(_type));
				base.AddAttributeArguments(attribute, ErrorMessagePlacement.UnOrdered);
            }
            
            return attribute;
        }
	}
}
