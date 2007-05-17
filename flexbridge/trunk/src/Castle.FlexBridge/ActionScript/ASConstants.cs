// Copyright 2007 Castle Project - http://www.castleproject.org/
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
using System.Collections.Generic;
using System.Text;

namespace Castle.FlexBridge.ActionScript
{
    /// <summary>
    /// Contains various useful ActionScript related constants.
    /// </summary>
    public static class ASConstants
    {
        /// <summary>
        /// The ActionScript Epoch.
        /// </summary>
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// The smallest value that can be represented by a signed 29-bit integer.
        /// </summary>
        public const int Int29MinValue = - 0x10000000;

        /// <summary>
        /// The largest value that can be represented by a signed 29-bit integer.
        /// </summary>
        public const int Int29MaxValue = 0x0fffffff;
    }
}
