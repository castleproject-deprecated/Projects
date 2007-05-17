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
using Castle.FlexBridge.Serialization;
using Castle.FlexBridge.Serialization.Mapping;

namespace Castle.FlexBridge.Tests.UnitTests.Serialization.Mapping
{
    public abstract class BaseMapperTest : BaseUnitTest
    {
        private IActionScriptSerializer serializer;

        public override void SetUp()
        {
            base.SetUp();

            ActionScriptMappingTable mappingTable = new ActionScriptMappingTable();
            mappingTable.RegisterBuiltInMapperFactories();
            serializer = new MappedActionScriptSerializer(mappingTable);
        }

        protected T ToNative<T>(IASValue asValue)
        {
            return (T) serializer.ToNative(asValue, typeof(T));
        }

        protected IASValue ToASValue(object nativeValue)
        {
            return serializer.ToASValue(nativeValue);
        }
    }
}
