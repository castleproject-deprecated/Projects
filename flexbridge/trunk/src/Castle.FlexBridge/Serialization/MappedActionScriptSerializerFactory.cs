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
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Serialization.Mapping;

namespace Castle.FlexBridge.Serialization
{
    /// <summary>
    /// A factory for <see cref="MappedActionScriptSerializer" />.
    /// All generated serializers share the same mapping table.
    /// </summary>
    public class MappedActionScriptSerializerFactory : IActionScriptSerializerFactory
    {
        private ActionScriptMappingTable mappingTable;

        /// <summary>
        /// Creates a factory with a new mapping table.
        /// </summary>
        /// <param name="registerBuiltInMapperFactories">If true, calls <see cref="ActionScriptMappingTable.RegisterBuiltInMapperFactories" />
        /// to register all built-in mappers automatically</param>
        public MappedActionScriptSerializerFactory(bool registerBuiltInMapperFactories)
        {
            mappingTable = new ActionScriptMappingTable();

            if (registerBuiltInMapperFactories)
                mappingTable.RegisterBuiltInMapperFactories();
        }

        /// <summary>
        /// Creates a factory with the specified mapping table.
        /// </summary>
        /// <param name="mappingTable">The mapping table</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="mappingTable"/> is null</exception>
        public MappedActionScriptSerializerFactory(ActionScriptMappingTable mappingTable)
        {
            if (mappingTable == null)
                throw new ArgumentNullException("mappingTable");

            this.mappingTable = mappingTable;
        }

        /// <summary>
        /// Gets the ActionScript mapping table.
        /// </summary>
        public ActionScriptMappingTable MappingTable
        {
            get { return mappingTable; }
        }

        /// <summary>
        /// Creates a new serializer.
        /// </summary>
        /// <returns>The serializer</returns>
        public IActionScriptSerializer CreateSerializer()
        {
            return new MappedActionScriptSerializer(mappingTable);
        }
    }
}
