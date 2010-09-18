// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveWriter.ARValidators
{
	using System;
	using System.CodeDom;
	using System.Collections;
	using System.ComponentModel;
	using CodeGeneration;

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
        [Description("An array of card numbers to skip checking for (eg. gateway test numbers).")]
        public string[] Exceptions
        {
            get { return _exceptions; }
            set { _exceptions = value; }
        }

        public override CodeAttributeDeclaration GetAttributeDeclaration()
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("ValidateCreditCard");

            if (this._allowedTypes != CardType.All)
            {
                ArrayList list = new ArrayList();

                if ((_allowedTypes & CardType.MasterCard) != 0)
                    list.Add(GetFieldReferenceForCreditCardEnum(CardType.MasterCard));
                if ((_allowedTypes & CardType.VISA) != 0)
                    list.Add(GetFieldReferenceForCreditCardEnum(CardType.VISA));
                if ((_allowedTypes & CardType.Amex) != 0)
                    list.Add(GetFieldReferenceForCreditCardEnum(CardType.Amex));
                if ((_allowedTypes & CardType.DinersClub) != 0)
                    list.Add(GetFieldReferenceForCreditCardEnum(CardType.DinersClub));
                if ((_allowedTypes & CardType.enRoute) != 0)
                    list.Add(GetFieldReferenceForCreditCardEnum(CardType.enRoute));
                if ((_allowedTypes & CardType.Discover) != 0)
                    list.Add(GetFieldReferenceForCreditCardEnum(CardType.Discover));
                if ((_allowedTypes & CardType.JCB) != 0)
                    list.Add(GetFieldReferenceForCreditCardEnum(CardType.JCB));
                if ((_allowedTypes & CardType.Unknown) != 0)
                    list.Add(GetFieldReferenceForCreditCardEnum(CardType.Unknown));

                attribute.Arguments.Add(new CodeAttributeArgument(CodeGenerationHelper.GetBinaryOr(list, 0)));

                if (_exceptions != null)
                {
                    // TODO: Add as array initializer 
                    attribute.Arguments.Add(AttributeHelper.GetStringArrayAttributeArgument(_exceptions));
                }
            }
            else if (_exceptions != null)
            {
                // TODO add as array init. as above :
                //attribute.Arguments.Add(GetPrimitiveAttributeArgument("CreditCardValidator.CardType", validator.Exceptions));
            }

            base.AddAttributeArguments(attribute, ErrorMessagePlacement.UnOrdered);
            return attribute;
        }

        private CodeFieldReferenceExpression GetFieldReferenceForCreditCardEnum(Enum value)
        {
            return new CodeFieldReferenceExpression(
                new CodeTypeReferenceExpression("CreditCardValidator.CardType"),
                value.ToString());
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
        All = Amex | DinersClub | Discover | Discover | enRoute | JCB | MasterCard | VISA
	}
}
