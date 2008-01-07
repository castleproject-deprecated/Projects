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

using System;

namespace Castle.MonoRail.ViewComponents
{
    /// <summary>
    /// Represents a data item in a single data series chart
    /// </summary>
    public class ChartDataItem
    {
        private object _label;
        private decimal _value;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="label">The label for the data item, usually shown on the 'X' axis</param>
        /// <param name="value">The value of the data item</param>
        public ChartDataItem(object label, decimal value)
        {
            _label = label;
            _value = value;
        }

        /// <summary>
        /// The label for the data item, usually shown on the 'X' axis
        /// </summary>
        public object Label
        {
            get { return _label; }
            set { _label = value; }
        }

        /// <summary>
        /// The value of the data item
        /// </summary>
        public decimal Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}