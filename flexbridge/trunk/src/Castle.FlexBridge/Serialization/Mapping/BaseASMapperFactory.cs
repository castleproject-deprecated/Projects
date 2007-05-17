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

namespace Castle.FlexBridge.Serialization.Mapping
{
    /// <summary>
    /// Abstract base implementation of an <see cref="IASMapperFactory" />.
    /// </summary>
    /// <remarks>
    /// The default implementation returns null when asked to obtain a mapper.
    /// </remarks>
    public abstract class BaseASMapperFactory : IASMapperFactory
    {
        /// <summary>
        /// Gets an ActionScript target mapper that satisfies the specified descriptor.
        /// </summary>
        /// <param name="descriptor">The target mapping descriptor</param>
        /// <returns>The ActionScript target mapper, or null if no compatible mapper can be obtained by this factory</returns>
        public virtual IASTargetMapper GetASTargetMapper(ASTargetMappingDescriptor descriptor)
        {
            return null;
        }

        /// <summary>
        /// Gets an ActionScript source mapper that satisfies the specified descriptor.
        /// </summary>
        /// <param name="descriptor">The source mapping descriptor</param>
        /// <returns>The ActionScript target mapper, or null if no compatible mapper can be obtained by this factory</returns>
        public virtual IASSourceMapper GetASSourceMapper(ASSourceMappingDescriptor descriptor)
        {
            return null;
        }
    }
}
