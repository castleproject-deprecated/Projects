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
    public class TimeValidatorTestCase
    {
        [Test]
        public void ValidationPassesForValidTimes()
        {
            TimeValidator validator = new TimeValidator();

            Assert.IsTrue(validator.Perform(this, "00:00"));
            Assert.IsTrue(validator.Perform(this, "23:59"));
        }

        [Test]
        public void ValidationFailsForInvalidTimes()
        {
            TimeValidator validator = new TimeValidator();

            Assert.IsFalse(validator.Perform(this, "24:00"));
            Assert.IsFalse(validator.Perform(this, "00:60"));
            Assert.IsFalse(validator.Perform(this, "12"));
            Assert.IsFalse(validator.Perform(this, "23:"));
            Assert.IsFalse(validator.Perform(this, "abc"));
            Assert.IsFalse(validator.Perform(this, ":10"));
        }
    }
}