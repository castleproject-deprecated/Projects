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

namespace Castle.FlexBridge.Serialization
{
    /// <summary>
    /// Provides custom AMF serialization for objects.
    /// </summary>
    /// <remarks>
    /// The methods provided here parallel those defined on the client side
    /// in the "flash.utils.IExternalizable" class.
    /// </remarks>
    public interface IExternalizable
    {
        /// <summary>
        /// A class implements this method to decode itself from a data
        /// stream by calling the methods of the IDataInput interface.
        /// </summary>
        /// <param name="input">The data input stream</param>
        void ReadExternal(IDataInput input);

        /// <summary>
        /// A class implements this method to encode itself for a data
        /// stream by calling the methods of the IDataOutput interface.
        /// </summary>
        /// <param name="output">The data output stream</param>
        void WriteExternal(IDataOutput output);
    }
}
