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

    [Serializable]
    public class ValidateLength : AbstractValidation
	{
	    private int _exactLength = int.MinValue;
	    private int _minLength = int.MinValue;
	    private int _maxLenght = int.MaxValue;

        public ValidateLength()
        {
            base.friendlyName = "Length";
        }

        [Category("Length")]
        [Description("The exact length required.")]
	    public int ExactLength
	    {
	        get { return _exactLength; }
	        set { _exactLength = value; }
	    }

        [Category("Length")]
        [Description("The minimum length, or int.MinValue if this should not be tested.")]
	    public int MinLength
	    {
	        get { return _minLength; }
	        set { _minLength = value; }
	    }

        [Category("Length")]
        [Description("The maximum length, or int.MaxValue if this should not be tested.")]
	    public int MaxLenght
	    {
	        get { return _maxLenght; }
	        set { _maxLenght = value; }
	    }
	}
}
