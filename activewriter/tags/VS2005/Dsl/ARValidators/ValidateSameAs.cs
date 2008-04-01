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
    public class ValidateSameAs : AbstractValidation
	{
	    private string _propertyToCompare;

        public ValidateSameAs()
        {
            base.friendlyName = "Same As";
        }

        [Category("Same As")]
        [Description("The property to compare.")]
        public string PropertyToCompare
	    {
            get { return _propertyToCompare; }
            set { _propertyToCompare = value; }
	    }

        public override bool IsValid(List<string> errorList)
        {
            if (errorList == null)
                throw new ArgumentNullException("errorList");

            if (string.IsNullOrEmpty(_propertyToCompare))
            {
                errorList.Add("ValidateSameAs: A property name to compare is not supplied.");
                return false;
            }

            return true;
        }
        public override CodeAttributeDeclaration GetAttributeDeclaration()
        {
            List<string> errorList = new List<string>();
            if (!IsValid(errorList))
                throw new ArgumentException(errorList[0]);

            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("ValidateSameAs");

            attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(_propertyToCompare));

            base.AddAttributeArguments(attribute, ErrorMessagePlacement.UnOrdered);
            return attribute;
        }
	}
}
