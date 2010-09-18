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

namespace Castle.ActiveWriter.ARValidators
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.ComponentModel;
    using CodeGeneration;

    public enum RangeValidationType
    {
        Integer,
        DateTime,
        String
    }

    [Serializable]
	public class ValidateRange: AbstractValidation
	{
        private RangeValidationType _type = RangeValidationType.Integer;
        private string _min;
        private string _max;

        public ValidateRange()
        {
            base.friendlyName = "Range";
        }

        [Category("Range")]
        [Description("Range validation type for this validator.")]
        public RangeValidationType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        [Category("Range")]
        [Description("The minimum value. Leave empty if this should not be tested.")]
        public string MinValue
        {
            get { return _min; }
            set { _min = value; }
        }

        [Category("Range")]
        [Description("The maximum value. Leave empty if this should not be tested.")]
        public string MaxValue
        {
            get { return _max; }
            set { _max = value; }
        }

        public override bool IsValid(List<string> errorList)
        {
            if (errorList == null)
                throw new ArgumentNullException("errorList");

            switch(_type)
            {
                case RangeValidationType.Integer:
                    int minValue = int.MinValue;
                    int maxValue = int.MaxValue;
                    if (!string.IsNullOrEmpty(_min) && !int.TryParse(_min, out minValue))
                    {
                        errorList.Add("ValidateRange: Min Value is not a valid integer");
                        return false;
                    }
                    if (!string.IsNullOrEmpty(_max) && !int.TryParse(_max, out maxValue))
                    {
                        errorList.Add("ValidateRange: Max Value is not a valid integer");
                        return false;
                    }
                    if (minValue == int.MinValue && maxValue == int.MaxValue)
                    {
                        errorList.Add("ValidateRange: Both min and max were set in such a way that neither would be tested. At least one must be tested.");
                        return false;
                    }
                    if (minValue > maxValue)
                    {
                        errorList.Add("ValidateRange: The min parameter must be less than or equal to the max parameter.");
                        return false;
                    }
                    break;
                case RangeValidationType.DateTime:
                    DateTime minDateValue = DateTime.MinValue;
                    DateTime maxDateValue = DateTime.MaxValue;
                    if (!DateTime.TryParse(_min, out minDateValue))
                    {
                        errorList.Add("ValidateRange: Min Value is not a valid date");
                        return false;
                    }
                    if (!DateTime.TryParse(_max, out maxDateValue))
                    {
                        errorList.Add("ValidateRange: Max Value is not a valid date");
                        return false;
                    }
                    if (minDateValue == DateTime.MinValue && maxDateValue == DateTime.MaxValue)
                    {
                        errorList.Add("ValidateRange: Both min and max were set in such a way that neither would be tested. At least one must be tested.");
                        return false;
                    }
                    if (minDateValue > maxDateValue)
                    {
                        errorList.Add("ValidateRange: The min parameter must be less than or equal to the max parameter.");
                        return false;
                    }
                    break;
                case RangeValidationType.String:
                    if (String.IsNullOrEmpty(_min) && String.IsNullOrEmpty(_max))
                    {
                        errorList.Add("ValidateRange: Both min and max were set in such a way that neither would be tested. At least one must be tested.");
                        return false;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Type");
            }

            return true;
        }

        public override CodeAttributeDeclaration GetAttributeDeclaration()
        {
            List<string> errorList = new List<string>();
            if (!IsValid(errorList))
                throw new ArgumentException(errorList[0]);

            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("ValidateRange");

            switch(_type)
            {
                case RangeValidationType.Integer:
                    if (!string.IsNullOrEmpty(_min))
                        attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(int.Parse(_min)));
                    else
                        attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgumentUsingSnippet("Int32.MinValue"));

                    if (!string.IsNullOrEmpty(_max))
                        attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(int.Parse(_max)));
                    else
                        attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgumentUsingSnippet("Int32.MaxValue"));
                    break;
                case RangeValidationType.DateTime:
                    if (!string.IsNullOrEmpty(_min))
                        attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(DateTime.Parse(_min)));
                    else
                        attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgumentUsingSnippet("DateTime.MinValue"));

                    if (!string.IsNullOrEmpty(_max))
                        attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(DateTime.Parse(_max)));
                    else
                        attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgumentUsingSnippet("DateTime.MaxValue"));
                    break;
                case RangeValidationType.String:
                    if (!string.IsNullOrEmpty(_min))
                        attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(_min));
                    else
                        attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgumentUsingSnippet("String.Empty"));
                    
                    if (!string.IsNullOrEmpty(_max))
                        attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(_max));
                    else
                        attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgumentUsingSnippet("String.Empty"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Type");
            }

            base.AddAttributeArguments(attribute, ErrorMessagePlacement.UnOrdered);
            return attribute;
        }
	}
}
