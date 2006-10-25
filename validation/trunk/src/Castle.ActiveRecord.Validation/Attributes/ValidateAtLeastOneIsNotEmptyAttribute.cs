// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

using System;
using Castle.ActiveRecord.Validation.Validators.CultureIndependant;

namespace Castle.ActiveRecord.Validation.Attributes
{
    /// <summary>
    /// Validates a group of field to ensure that at least one of the fields is not empty.
    /// </summary>
    [Serializable, CLSCompliant(false)]
    public class ValidateAtLeastOneIsNotEmptyAttribute : AbstractValidationAttribute
    {
        private string group;
        
        public ValidateAtLeastOneIsNotEmptyAttribute(string group) : base(new AtLeastOneIsNotEmptyValidator(group))
        {
            this.group = group;
        }

        public string Group
        {
            get { return group; }
        }
    }
}