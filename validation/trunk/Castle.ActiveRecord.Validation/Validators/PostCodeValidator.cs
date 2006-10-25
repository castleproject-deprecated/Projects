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

using System.Globalization;
using Castle.ActiveRecord.Framework.Validators;

namespace Castle.ActiveRecord.Validation.Validators
{
    public class PostCodeValidator : RegularExpressionValidator
    {
        public PostCodeValidator(string region) : this(new RegionInfo(region))
        {
        }

        public PostCodeValidator(RegionInfo region) : base(GetValidatorExpression(region))
        {
        }

        private static string GetValidatorExpression(RegionInfo region)
        {
            switch(region.TwoLetterISORegionName)
            {
                case "GB" : return @"^[A-Za-z]{1,2}[\d]{1,2}([A-Za-z])?\s?[\d][A-Za-z]{2}$";
                default : throw new MissingRegionSpecificValidatorException("A post code validator does not exist for this region.");
            }
        }

        protected override string BuildErrorMessage()
        {
            return string.Format("Field {0} is not a valid post code.", Property.Name);
        }
    }
}