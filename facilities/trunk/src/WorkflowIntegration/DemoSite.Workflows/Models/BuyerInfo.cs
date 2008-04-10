// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace DemoSite.Workflows.Models
{
    using System;

    [Serializable]
	public class BuyerInfo
	{
        private string name;

        private string card;

        private int expiresYear;

        private int expiresMonth;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Card
        {
            get { return card; }
            set { card = value; }
        }

        public int ExpiresYear
        {
            get { return expiresYear; }
            set { expiresYear = value; }
        }

        public int ExpiresMonth
        {
            get { return expiresMonth; }
            set { expiresMonth = value; }
        }
	}
}
