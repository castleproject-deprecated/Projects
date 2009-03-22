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

namespace Altinoren.ActiveWriter
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Text;
    using System.Xml.Serialization;
    using ARValidators;

    public partial class ModelProperty
    {
        private string validatorPropertyStorage = null;
        private readonly static Type[] _availableTypes = new Type[]
            {
            	typeof(ValidateCollectionNotEmpty),
            	typeof(ValidateCreditCard),
            	typeof(ValidateDate),
            	typeof(ValidateDateTime),
            	typeof(ValidateDecimal),
            	typeof(ValidateDouble),
            	typeof(ValidateEmail),
            	typeof(ValidateGroupNotEmpty),
            	typeof(ValidateInteger),
            	typeof(ValidateLength),
            	typeof(ValidateNonEmpty),
            	typeof(ValidateRange),
            	typeof(ValidateRegExp),
            	typeof(ValidateSameAs),
            	typeof(ValidateSet),
            	typeof(ValidateSingle)
            };

        public bool IsValidatorSet()
        {
            return !string.IsNullOrEmpty(this.validatorPropertyStorage);
        }

        public string GetValidatorValue()
        {
            // TODO: Will show up in properties window. Very ugly. How can we display some other text other than the actual
            // in the properties window?
            return this.validatorPropertyStorage;
        }

        public void SetValidatorValue(string newValue)
        {
            this.validatorPropertyStorage = newValue;
        }

        public static ArrayList DeserializeValidatorList(string value)
        {
            StringReader reader = new StringReader(value);
            XmlSerializer serializer =
                new XmlSerializer(typeof (ArrayList), _availableTypes);
            return serializer.Deserialize(reader) as ArrayList;
        }

        public static string SerializeValidatorList(ArrayList list)
        {
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);
            XmlSerializer serializer = new XmlSerializer(typeof(ArrayList), _availableTypes);
            serializer.Serialize(writer, list);
            return writer.ToString();
        }
    }
}
