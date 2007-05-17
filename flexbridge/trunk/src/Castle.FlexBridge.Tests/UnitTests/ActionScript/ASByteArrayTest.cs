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
using Castle.FlexBridge.Serialization;
using Castle.FlexBridge.Tests.UnitTests;
using MbUnit.Framework;

namespace Castle.FlexBridge.Tests.UnitTests.ActionScript
{
    [TestFixture]
    [TestsOn(typeof(ASByteArray))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class ASByteArrayTest : BaseUnitTest
    {
        [Test]
        public void HasSingletonEmptyByteArray()
        {
            Assert.AreEqual(0, ASByteArray.Empty.Bytes.Count);
            Assert.IsTrue(ASByteArray.Empty.Bytes.Array.IsFixedSize);
        }

        [RowTest]
        [Row(new byte[] { 1, 2, 3 })]
        [Row(null, ExpectedException = typeof(ArgumentNullException))]
        public void ConstructorWithByteArray(byte[] bytes)
        {
            ASByteArray byteArray = new ASByteArray(bytes);

            Assert.AreSame(bytes, byteArray.Bytes.Array);
            Assert.AreEqual(0, byteArray.Bytes.Offset);
            Assert.AreEqual(3, byteArray.Bytes.Count);
            Assert.AreEqual(3, byteArray.Count);
        }

        [Test]
        public void ConstructorWithByteArraySegment()
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(new byte[] { 1, 2, 3 }, 1, 1);
            ASByteArray byteArray = new ASByteArray(segment);

            Assert.AreEqual(segment, byteArray.Bytes);
            Assert.AreEqual(1, byteArray.Count);
        }

        [Test]
        public void IndexerGetAndSet()
        {
            ArraySegment<byte> segment = new ArraySegment<byte>(new byte[] { 1, 2, 3, 4, 5 }, 1, 3);
            ASByteArray byteArray = new ASByteArray(segment);

            Assert.AreEqual(3, byteArray[1]);
            byteArray[1] = 42;
            Assert.AreEqual(42, byteArray[1]);
        }

        [RowTest]
        [Row(-1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(3, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void IndexerGetThrowsWhenIndexOutOfRange(int index)
        {
            ASByteArray byteArray = new ASByteArray(new byte[] { 1, 2, 3 });
            GC.KeepAlive(byteArray[index]);
        }

        [RowTest]
        [Row(-1, ExpectedException = typeof(ArgumentOutOfRangeException))]
        [Row(3, ExpectedException = typeof(ArgumentOutOfRangeException))]
        public void IndexerSetThrowsWhenIndexOutOfRange(int index)
        {
            ASByteArray byteArray = new ASByteArray(new byte[] { 1, 2, 3 });
            byteArray[index] = 42;
        }
    }
}