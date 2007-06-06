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

using System.Collections.Generic;

namespace Altinoren.ActiveWriter.ARValidators
{
    using System;
    using System.ComponentModel;

    [Flags]
    public enum RunWhen
    {
        /// <summary>
        /// Run all validations
        /// </summary>
        Everytime = 0x1,
        /// <summary>
        /// Only during an insertion phase
        /// </summary>
        Insert = 0x2,
        /// <summary>
        /// Only during an update phase
        /// </summary>
        Update = 0x4,
        /// <summary>
        /// Defines a custom phase
        /// </summary>
        Custom = 0x8,
    }

    [Serializable]
    [DefaultProperty("ErrorMessage")]
    public abstract class AbstractValidation
    {
        private string _errorMessage;
        private int executionOrder = 0;
        private RunWhen runWhen = RunWhen.Everytime;
        protected string friendlyName;
        private string castleFriendlyName;

        [CategoryAttribute("Errors")]
        [Description("The error message to display if invalid.")]
        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        [CategoryAttribute("Validation")]
        [Description("Defines when to run the validation. Defaults to \"Everytime\"")]
        public RunWhen RunWhen
        {
            get { return runWhen; }
            set { runWhen = value; }
        }

        [CategoryAttribute("Validation")]
        [Description("Validation execution order.")]
        public int ExecutionOrder
        {
            get { return executionOrder; }
            set { executionOrder = value; }
        }

        [CategoryAttribute("Validation")]
        [Description("Friendly name of the target property.")]
        public string FriendlyName
        {
            get { return castleFriendlyName; }
            set { castleFriendlyName = value; }
        }

        public override string ToString()
        {
            // Displayed in the ListBox in the editor window.
            if (!string.IsNullOrEmpty(friendlyName))
                return friendlyName;
            else
                return base.ToString();
        }

        public virtual bool IsValid(List<string> errorList)
        {
            return true;
        }
    }
}
