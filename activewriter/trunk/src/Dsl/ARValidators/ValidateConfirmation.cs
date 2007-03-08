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
    using System.ComponentModel;
    using System.Collections.Generic;

    [Serializable]
    public class ValidateConfirmation : AbstractValidation
	{
	    private string _confirmationFieldOrProperty;

        public ValidateConfirmation()
        {
            base.friendlyName = "Confirmation";
        }

        [Category("Confirmation")]
        [Description("The confirmation field or property that should be verified against this one.")]
        public string ConfirmationFieldOrProperty
        {
            get { return _confirmationFieldOrProperty; }
            set { _confirmationFieldOrProperty = value; }
        }

        public override bool IsValid(List<string> errorList)
        {
            if (errorList == null)
                throw new ArgumentNullException("errorList");

            if (string.IsNullOrEmpty(_confirmationFieldOrProperty))
            {
                errorList.Add("Confirmation validator requires a field or property name.");
                return false;
            }

            return true;
        }
	}
}
