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

    [Serializable]
	public class ValidateSingle: AbstractValidation
	{
        public ValidateSingle()
        {
            base.friendlyName = "Single";
        }

        public override CodeAttributeDeclaration GetAttributeDeclaration()
        {
            CodeAttributeDeclaration attribute = base.GetAttributeDeclaration();
            attribute.Name = "ValidateSingle";

            return attribute;
        }
	}
}
