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
    /// Maintains a cache of <see cref="ASClass" /> instances that have been seen
    /// so that they can be reused across as many objects as possible.
    /// Uses the complete definition of the class for uniqueness, not just its name.
    /// </summary>
    public static class ASClassCache
    {
        private static Dictionary<ASClass, ASClass> cache;

        static ASClassCache()
        {
            cache = new Dictionary<ASClass, ASClass>();
            cache.Add(ASClass.UntypedDynamicClass, ASClass.UntypedDynamicClass);
        }

        /// <summary>
        /// Gets a shared and read-only class definition with the specified properties.
        /// The class is cached for reuse in subsequent requests.
        /// </summary>
        /// <remarks>
        /// This method is safe for concurrent access.
        /// </remarks>
        /// <param name="classAlias">The class alias</param>
        /// <param name="classLayout">The class layout</param>
        /// <param name="memberNames">The class members</param>
        /// <returns>The shared class definition</returns>
        public static ASClass GetClass(string classAlias, ASClassLayout classLayout, IList<string> memberNames)
        {
            lock (cache)
            {
                // Yes we create a new class instance in order to search the cache.
                // However, if we do find a match we will not return it to the caller so it
                // will get promptly garbage collected.
                ASClass key = new ASClass(classAlias, classLayout, memberNames);
                ASClass @class;
                if (cache.TryGetValue(key, out @class))
                    return @class;

                cache.Add(key, key);
                return key;
            }
        }
    }
}
