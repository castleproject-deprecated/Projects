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

using Castle.Components.Validator.Contrib;
using Castle.Components.Validator.Contrib.Validators;
using NUnit.Framework;

namespace Castle.ActiveRecord.Validation.Tests
{
    [TestFixture]
    public class PostCodeValidatorTestCase
    {
        [Test]
        [ExpectedException(typeof (MissingRegionSpecificValidatorException))]
        public void ExceptionThrownForCulturesWithoutASpecifiedExpression()
        {
            PostCodeValidator validator = new PostCodeValidator("NI");
            validator.IsValid(this, "abc");
        }
        
        [Test]
        public void ValidationPassesForValidPostCodes()
        {
            PostCodeValidator validator = new PostCodeValidator("GB");

            Assert.IsTrue(validator.IsValid(this, "DY13 9SB"));
            Assert.IsTrue(validator.IsValid(this, "S11 7EZ"));
            Assert.IsTrue(validator.IsValid(this, "SW12 0HF"));            
        }
        
        [Test]
        public void ValidationFailsForInvalidPostCodes()
        {
            PostCodeValidator validator = new PostCodeValidator("GB");

            Assert.IsFalse(validator.IsValid(this, "A 1"));
            Assert.IsFalse(validator.IsValid(this, "AB 1A"));
            Assert.IsFalse(validator.IsValid(this, "1AB 2AB"));
        }
    }
}