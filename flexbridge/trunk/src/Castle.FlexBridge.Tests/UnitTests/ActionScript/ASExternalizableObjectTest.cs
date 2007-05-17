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
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Collections;
using Castle.FlexBridge.Serialization;
using Castle.FlexBridge.Tests.UnitTests;
using MbUnit.Framework;

namespace Castle.FlexBridge.Tests.UnitTests.ActionScript
{
    [TestFixture]
    [TestsOn(typeof(ASExternalizableObject))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class ASExternalizableObjectTest : BaseUnitTest
    {
        private ASClass externalizableClass;
        private IExternalizable externalizable;

        public override void SetUp()
        {
            base.SetUp();

            externalizableClass = new ASClass("extern", ASClassLayout.Externalizable, EmptyArray<string>.Instance);
            externalizable = Mocks.CreateMock<IExternalizable>();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsWhenClassIsNull()
        {
            new ASExternalizableObject(null, externalizable);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsWhenExternalizableIsNull()
        {
            new ASExternalizableObject(externalizableClass, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorThrowsWhenClassIsNotExternalizable()
        {
            new ASExternalizableObject(ASClass.UntypedDynamicClass, externalizable);
        }

        [Test]
        public void ConstructorSetsProperties()
        {
            ASExternalizableObject obj = new ASExternalizableObject(externalizableClass, externalizable);

            Assert.AreSame(externalizableClass, obj.Class);
            Assert.AreSame(externalizable, obj.ExternalizableValue);
        }

        [Test]
        public void CreateUninitializedInstanceAndSetProperties()
        {
            IExternalizable externalizableValue = Mocks.CreateMock<IExternalizable>();
            Mocks.ReplayAll();

            ASExternalizableObject obj = ASExternalizableObject.CreateUninitializedInstance(externalizableClass);
            Assert.AreSame(externalizableClass, obj.Class);

            obj.SetProperties(externalizableValue);
            Assert.AreSame(externalizableValue, obj.ExternalizableValue);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SetPropertiesWithInitializedInstanceThrows()
        {
            IExternalizable externalizableValue = Mocks.CreateMock<IExternalizable>();
            Mocks.ReplayAll();

            ASExternalizableObject obj = new ASExternalizableObject(externalizableClass, externalizableValue);
            obj.SetProperties(externalizableValue);
        }
    }
}