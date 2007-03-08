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
    public class ValidateCreditCard : AbstractValidation
	{
        private CardType _allowedTypes = CardType.All;
        private string[] _exceptions;

        public ValidateCreditCard()
        {
            base.friendlyName = "Credit Card";
        }

        [Category("Card")]
        [Description("The card types to accept.")]
        public CardType AllowedTypes
        {
            get { return _allowedTypes; }
            set { _allowedTypes = value; }
        }

        [Category("Card")]
        [Description("An array of card numbers to skip checking for (eg. gateway test numbers). Only digits should be provided for the exceptions.")]
        public string[] Exceptions
        {
            get { return _exceptions; }
            set { _exceptions = value; }
        }
	}

    [Flags, Serializable]
	public enum CardType
	{
		/// <summary>
		/// MasterCard Card
		/// </summary>
		MasterCard = 0x0001,
		/// <summary>
		/// VISA Card
		/// </summary>
		VISA = 0x0002,
		/// <summary>
		/// American Express Card
		/// </summary>
		Amex = 0x0004,
		/// <summary>
		/// Diners Club Card
		/// </summary>
		DinersClub = 0x0008,
		/// <summary>
		/// enRoute Card
		/// </summary>
		enRoute = 0x0010,
		/// <summary>
		/// Discover Card
		/// </summary>
		Discover = 0x0020,
		/// <summary>
		/// JCB Card
		/// </summary>
		JCB = 0x0040,
		/// <summary>
		/// Unkown card
		/// </summary>
		Unknown = 0x0080,
		/// <summary>
		/// All (known) cards
		/// </summary>
		All = Amex | DinersClub | Discover | Discover | enRoute | JCB | MasterCard | VISA,
	}
}
