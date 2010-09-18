#region License
//  Copyright 2004-2010 Castle Project - http:www.castleproject.org/
//  
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//      http:www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// 
#endregion
namespace Castle.ActiveWriter.ARValidators
{
	using System;
	using System.CodeDom;
	using System.Collections.Generic;
	using System.ComponentModel;
	using CodeGeneration;

	[Serializable]
	public class ValidateGroupNotEmpty: AbstractValidation
	{
        private string _group;

        public ValidateGroupNotEmpty()
        {
            base.friendlyName = "Group Not Empty";
        }

        [Category("Group")]
        [Description("Name of the property group to validate together")]
        public string Group
        {
            get { return _group; }
            set { _group = value; }
        }

        public override bool IsValid(List<string> errorList)
        {
            if (string.IsNullOrEmpty(_group))
            {
                errorList.Add("ValidateGroupNotEmpty: Group is required");
                return false;
            }

            return true;
        }

        public override CodeAttributeDeclaration GetAttributeDeclaration()
        {
            List<string> errorList = new List<string>();
            if (!IsValid(errorList))
                throw new ArgumentException(errorList[0]);

            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("ValidateGroupNotEmpty");

            attribute.Arguments.Add(AttributeHelper.GetPrimitiveAttributeArgument(_group));

            base.AddAttributeArguments(attribute, ErrorMessagePlacement.UnOrdered);
            return attribute;
        }
	}
}
