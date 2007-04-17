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
using System.Globalization;
using System.Threading;
using Castle.Components.Validator.Contrib.Validators;

namespace Castle.Components.Validator.Contrib.Attributes
{
    /// <summary>
    /// Validate the post code.
    /// </summary>
    [Serializable, CLSCompliant(false)]
    public class ValidatePostCodeAttribute : AbstractValidationAttribute
    {
        private string region;
        
        /// <summary>
        /// Validates the property as a post code for the region associated with the current thread's culture.
        /// </summary>
        public ValidatePostCodeAttribute()
        {            
        }
        
        /// <summary>
        /// Validates the property as a post code for the specified region.
        /// </summary>
        /// <param name="region">A 2 or 3 letter ISO 3166 code describing the region against whose 
        /// post code format the property should be tested.</param>
        public ValidatePostCodeAttribute(string region)
        {
            this.region = region;
        }

        /// <summary>
        /// Validates the property as a post code for the specified region.
        /// </summary>
        /// <param name="region">A 2 or 3 letter ISO 3166 code describing the region against whose  
        /// post code format the property should be tested.</param>
        /// <param name="errorMessage">The error message.</param>
        public ValidatePostCodeAttribute(string region, string errorMessage) : base(errorMessage)
        {
            this.region = region;
        }

        public override IValidator Build()
        {
            IValidator validator = string.IsNullOrEmpty(region)
                                       ? new PostCodeValidator(new RegionInfo(Thread.CurrentThread.CurrentCulture.LCID))
                                       : new PostCodeValidator(region);

            ConfigureValidatorMessage(validator);

            return validator;
        }
    }
}