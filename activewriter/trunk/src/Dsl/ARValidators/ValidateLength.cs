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

namespace Castle.ActiveWriter.ARValidators
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.ComponentModel;
    using CodeGeneration;

    [Serializable]
    public class ValidateLength : AbstractValidation
	{
	    private int _exactLength = int.MinValue;
	    private int _minLength = int.MinValue;
	    private int _maxLength = int.MaxValue;

        public ValidateLength()
        {
            base.friendlyName = "Length";
        }

        [Category("Length")]
        [Description("The exact length required.")]
	    public int ExactLength
	    {
	        get { return _exactLength; }
	        set { _exactLength = value; }
	    }

        [Category("Length")]
        [Description("The minimum length, or int.MinValue if this should not be tested.")]
	    public int MinLength
	    {
	        get { return _minLength; }
	        set { _minLength = value; }
	    }

        [Category("Length")]
        [Description("The maximum length, or int.MaxValue if this should not be tested.")]
	    public int MaxLength
	    {
	        get { return _maxLength; }
	        set { _maxLength = value; }
	    }

        public override bool IsValid(List<string> errorList)
        {
            bool result = true;

            if (_exactLength != int.MinValue && _exactLength < 0 && _minLength == int.MinValue && _maxLength == int.MaxValue)
            {
                errorList.Add("ValidateLength: Exact lenght should be set to a non-negative number.");
                result = false;
            }

            if (_exactLength == int.MinValue && _minLength == int.MinValue && _maxLength == int.MaxValue)
            {
                errorList.Add("ValidateLength: Both minLength and maxLength were set in such as way that neither would be tested. At least one must be tested.");
                result = false;
            }

            if (_exactLength != int.MinValue && _minLength != int.MinValue && _maxLength != int.MaxValue)
            {
                errorList.Add("ValidateLenght: Either exact lenght or a min / max combination should be set.");
            }

            if (_minLength > _maxLength)
            {
                errorList.Add("ValidateLenght: The maxLength parameter must be greater than the minLength parameter.");
                result = false;
            }

            if (_minLength != int.MinValue && _minLength < 0)
            {
                errorList.Add("ValidateLenght: The minLength parameter must be set to either int.MinValue or a non-negative number.");
                result = false;
            }
            
            if (_maxLength < 0)
            {
                errorList.Add("ValidateLenght: The maxLength parameter must be set to either int.MaxValue or a non-negative number.");
                result = false;
            }

            return result;
        }

        public override CodeAttributeDeclaration GetAttributeDeclaration()
        {
            List<string> errorList = new List<string>();
            if (!IsValid(errorList))
                throw new ArgumentException(errorList[0]);

            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("ValidateLength");

            if (_exactLength != int.MinValue)
                attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(_exactLength));
            else
            {
                if (_minLength != int.MinValue)
                    attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(_minLength));
                else
                    attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgumentUsingSnippet("Int32.MinValue"));
                if (_maxLength != int.MaxValue)
                    attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(_maxLength));
                else
                    attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgumentUsingSnippet("Int32.MaxValue"));
            }

            base.AddAttributeArguments(attribute, ErrorMessagePlacement.UnOrdered);
            return attribute;
        }
	}
}
