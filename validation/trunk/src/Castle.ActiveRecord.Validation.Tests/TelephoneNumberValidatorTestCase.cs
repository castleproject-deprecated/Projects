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

using Castle.ActiveRecord.Validation.Validators;
using NUnit.Framework;

namespace Castle.ActiveRecord.Validation.Tests
{
    [TestFixture]
    public class TelephoneNumberValidatorTestCase
    {
        [Test]
        [ExpectedException(typeof(MissingRegionSpecificValidatorException))]
        public void ExceptionThrownForCulturesWithoutASpecifiedExpression()
        {
            TelephoneNumberValidator validator = new TelephoneNumberValidator("NI");
            validator.Perform(this, "012345");
        }

        [Test]
        public void ValidationPassesForValidTelephoneNumbers()
        {
            TelephoneNumberValidator validator = new TelephoneNumberValidator("GB");

            Assert.IsTrue(validator.Perform(this, "01234567890"));
            Assert.IsTrue(validator.Perform(this, "012 3456 7890"));
            Assert.IsTrue(validator.Perform(this, "0123 4567890"));
        }

        [Test]
        public void ValidationFailsForInvalidTelephoneNumbers()
        {
            TelephoneNumberValidator validator = new TelephoneNumberValidator("GB");

            Assert.IsFalse(validator.Perform(this, "0000"));
            Assert.IsFalse(validator.Perform(this, "ABCDEFGH"));
        }
    }
}