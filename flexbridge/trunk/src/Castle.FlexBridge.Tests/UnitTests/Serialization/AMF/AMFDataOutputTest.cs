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
using System.IO;
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Collections;
using Castle.FlexBridge.Serialization;
using Castle.FlexBridge.Serialization.AMF;
using Castle.FlexBridge.Tests.UnitTests;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace Castle.FlexBridge.Tests.UnitTests.Serialization.AMF
{
    [TestFixture]
    [TestsOn(typeof(AMFDataOutput))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class AMFDataOutputTest : BaseUnitTest
    {
        private delegate void WriteExternalDelegate(IDataOutput output);
        private delegate void AcceptVisitorDelegate(IActionScriptSerializer serializer, IASValueVisitor visitor);

        private AMFDataOutput output;
        private MemoryStream stream;
        private IActionScriptSerializer serializer;

        public override void SetUp()
        {
            base.SetUp();

            stream = new MemoryStream();
            serializer = Mocks.CreateMock<IActionScriptSerializer>();
            output = new AMFDataOutput(stream, serializer);
        }

        [Test]
        public void InitialEncodingIsAMF0()
        {
            Assert.AreEqual(AMFObjectEncoding.AMF0, output.ObjectEncoding);
        }

        [Test]
        public void CanGetAndSetEncoding()
        {
            output.ObjectEncoding = AMFObjectEncoding.AMF3;
            Assert.AreEqual(AMFObjectEncoding.AMF3, output.ObjectEncoding);
        }

        [Test]
        public void SerializerAccessibleViaGetter()
        {
            Assert.AreSame(serializer, output.Serializer);
        }

        [Test]
        public void WriteByte()
        {
            output.WriteByte(1);
            output.WriteByte(0);
            output.WriteByte(42);

            CollectionAssert.AreElementsEqual(new byte[] { 1, 0, 42 }, stream.ToArray());
        }

        [Test]
        public void WriteBytes()
        {
            output.WriteBytes(new byte[] { 5, 4, 3, 2, 1 });
            CollectionAssert.AreElementsEqual(new byte[] { 5, 4, 3, 2, 1 }, stream.ToArray());
        }

        [Test]
        public void WriteBytesWithOffsetAndCount()
        {
            output.WriteBytes(new byte[] { 5, 4, 3, 2, 1 }, 2, 2);
            CollectionAssert.AreElementsEqual(new byte[] { 3, 2 }, stream.ToArray());
        }

        [RowTest]
        [Row(new byte[] { 1 }, true)]
        [Row(new byte[] { 0 }, false)]
        public void WriteBoolean(byte[] expected, bool value)
        {
            output.WriteBoolean(value);
            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [Test]
        public void WriteDouble()
        {
            output.WriteDouble(1);
            CollectionAssert.AreElementsEqual(new byte[] { 0x3f, 0xf0, 0, 0, 0, 0, 0, 0 }, stream.ToArray());
        }

        [Test]
        public void WriteFloat()
        {
            output.WriteFloat(1);
            CollectionAssert.AreElementsEqual(new byte[] { 0x3f, 0x80, 0, 0 }, stream.ToArray());
        }

        [RowTest]
        [Row(new byte[] { 0x12, 0x34, 0x56, 0x78 }, 0x12345678)]
        [Row(new byte[] { 0xed, 0xcb, 0xa9, 0x88 }, - 0x12345678)]
        public void WriteInt(byte[] expected, int value)
        {
            output.WriteInt(value);
            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(new byte[] { 0x12, 0x34 }, (short) 0x1234)]
        [Row(new byte[] { 0xed, 0xcc }, (short) -0x1234)]
        public void WriteShort(byte[] expected, short value)
        {
            output.WriteShort(value);
            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(new byte[] { 0x12, 0x34, 0x56, 0x78 }, (uint) 0x12345678)]
        [Row(new byte[] { 0xed, 0xcb, 0xa9, 0x88 }, (uint) 0xedcba988)]
        public void WriteUnsignedInt(byte[] expected, uint value)
        {
            output.WriteUnsignedInt(value);
            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(new byte[] { 0x12, 0x34 }, (ushort) 0x1234)]
        [Row(new byte[] { 0xed, 0xcc }, (ushort) 0xedcc )]
        public void WriteUnsignedShort(byte[] expected, ushort value)
        {
            output.WriteUnsignedShort(value);
            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(new byte[] { 0x00, 0x00 }, "")]
        [Row(new byte[] { 0x00, 0x05, 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        public void WriteUTF(byte[] expected, string value)
        {
            output.WriteUTF(value);
            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [Test]
        [ExpectedException(typeof(AMFException))]
        public void WriteUTF_ShouldThrowWhenStringTooLongForUInt16SizePrefix()
        {
            output.WriteUTF(new string('a', 65537));
        }

        [RowTest]
        [Row(new byte[] { }, "")]
        [Row(new byte[] { 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        public void WriteUTFBytes(byte[] expected, string value)
        {
            output.WriteUTFBytes(value);
            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(new byte[] { 0x00, 0x00 }, "")]
        [Row(new byte[] { 0x00, 0x05, 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        public void WriteShortString(byte[] expected, string value)
        {
            output.WriteShortString(value);
            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [Test]
        [ExpectedException(typeof(AMFException))]
        public void WriteShortString_ShouldThrowWhenStringTooLongForUInt16SizePrefix()
        {
            output.WriteShortString(new string('a', 65537));
        }

        [Test]
        public void IsShortString()
        {
            Assert.IsTrue(output.IsShortString(""));
            Assert.IsTrue(output.IsShortString("abcdef"));
            Assert.IsFalse(output.IsShortString(new string('\u3333', 25000))); // each char requires 3 bytes when UTF8 encoded
        }

        [RowTest]
        [Row(new byte[] { 0x00, 0x00, 0x00, 0x00 }, "")]
        [Row(new byte[] { 0x00, 0x00, 0x00, 0x05, 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        public void WriteLongString(byte[] expected, string value)
        {
            output.WriteLongString(value);
            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [Test]
        public void WriteLongString_FineForReallyLongString()
        {
            output.WriteLongString(new string('a', 100000)); // more than UInt16.MaxValue
            Assert.AreEqual(4 /*length*/ + 100000 /*value*/, stream.Length);
        }

        [RowTest]
        [Row(new byte[] { 0x00 }, 0x00)]
        [Row(new byte[] { 0x01 }, 0x01)]
        [Row(new byte[] { 0x7f }, 0x7f)]
        [Row(new byte[] { 0x81, 0x00 }, 0x80)]
        [Row(new byte[] { 0xa4, 0x34 }, 0x1234)]
        [Row(new byte[] { 0x81, 0x80, 0x00 }, 0x4000)]
        [Row(new byte[] { 0x81, 0x8a, 0x67 }, 0x4567)]
        [Row(new byte[] { 0x80, 0xc0, 0x80, 0x00 }, 0x200000)]
        [Row(new byte[] { 0xa0, 0xc6, 0xc5, 0x67 }, 0x8234567)]
        [Row(new byte[] { 0xff, 0xff, 0xff, 0xff }, -1)]
        [Row(new byte[] { 0xff, 0xff, 0xff, 0x80 }, -0x80)]
        [Row(new byte[] { 0xff, 0xff, 0xff, 0x00 }, -0x100)]
        [Row(new byte[] { 0xff, 0xff, 0xc0, 0x00 }, -0x4000)]
        public void WriteVWInt29(byte[] expected, int value)
        {
            output.WriteVWInt29(value);
            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(new byte[] { 0x01 }, "")]
        [Row(new byte[] { 0x0b, 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        public void WriteVWInt29StringWithLSBSetTo1(byte[] expected, string value)
        {
            output.WriteVWInt29StringWithLSBSetTo1(value);
            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { })]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte) AMF0ObjectTypeCode.AMF3Data })]
        public void BeginAndEndObjectStream(AMFObjectEncoding encoding, byte[] expected)
        {
            output.ObjectEncoding = encoding;
            output.BeginObjectStream();
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BeginObjectStreamFailsIfAlreadyStarted()
        {
            output.BeginObjectStream();
            output.BeginObjectStream();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EndbjectStreamFailsIfNotStarted()
        {
            output.EndObjectStream();
        }

        [Test]
        public void EncodingChangeTakesEffectAtNextBeginObjectStream()
        {
            Mocks.ReplayAll();

            output.BeginObjectStream();
            output.ObjectEncoding = AMFObjectEncoding.AMF3;
            output.WriteObject(ASBoolean.True); // write boolean with AMF0 since AMF3 hasn't taken effect yet
            output.EndObjectStream();

            output.BeginObjectStream();
            output.ObjectEncoding = AMFObjectEncoding.AMF0;
            output.WriteObject(ASBoolean.True); // write boolean with AMF3 since AMF0 hasn't taken effect yet
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(new byte[] {
                (byte) AMF0ObjectTypeCode.Boolean, 1,
                (byte) AMF0ObjectTypeCode.AMF3Data, (byte) AMF3ObjectTypeCode.True }, stream.ToArray());
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Boolean, 1 }, true)]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Boolean, 0 }, false)]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.True }, true)]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.False }, false)]
        public void WriteObject_Booleans(AMFObjectEncoding objectEncoding, byte[] expected, bool value)
        {
            output.ObjectEncoding = objectEncoding;
            output.BeginObjectStream();
            output.WriteObject(ASBoolean.ToASBoolean(value));
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Number, 0x3f, 0xf0, 0, 0, 0, 0, 0, 0 }, 1)]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Integer, 0x01 }, 1)]
        public void WriteObject_Ints(AMFObjectEncoding objectEncoding, byte[] expected, int value)
        {
            output.ObjectEncoding = objectEncoding;
            output.BeginObjectStream();
            output.WriteObject(new ASInt29(value));
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Number, 0x3f, 0xf0, 0, 0, 0, 0, 0, 0 }, 1.0)]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Number, 0x3f, 0xf0, 0, 0, 0, 0, 0, 0 }, 1.0)]
        public void WriteObject_Numbers(AMFObjectEncoding objectEncoding, byte[] expected, double value)
        {
            output.ObjectEncoding = objectEncoding;
            output.BeginObjectStream();
            output.WriteObject(new ASNumber(value));
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Null })]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Null })]
        public void WriteObject_Nulls(AMFObjectEncoding objectEncoding, byte[] expected)
        {
            output.ObjectEncoding = objectEncoding;
            output.BeginObjectStream();
            output.WriteObject(null);
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Undefined })]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Undefined })]
        public void WriteObject_UndefinedValues(AMFObjectEncoding objectEncoding, byte[] expected)
        {
            output.ObjectEncoding = objectEncoding;
            output.BeginObjectStream();
            output.WriteObject(ASUndefined.Value);
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Unsupported })]
        [Row(AMFObjectEncoding.AMF3, new byte[] { }, ExpectedException=typeof(AMFException))] // Not supported in AMF3
        public void WriteObject_UnsupportedValues(AMFObjectEncoding objectEncoding, byte[] expected)
        {
            output.ObjectEncoding = objectEncoding;
            output.BeginObjectStream();
            output.WriteObject(ASUnsupported.Value);
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Xml, 0x00, 0x00, 0x00, 0x05, 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Xml, 0x0b, 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        public void WriteObject_XmlDocuments(AMFObjectEncoding objectEncoding, byte[] expected, string value)
        {
            ASXmlDocument xmlDocument = new ASXmlDocument(value);

            output.ObjectEncoding = objectEncoding;
            output.BeginObjectStream();
            output.WriteObject(xmlDocument);
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [Test]
        public void WriteObject_Strings_LongFormat_AMF0()
        {
            ASString value = new ASString(new string('a', 100000));

            output.BeginObjectStream();
            output.WriteObject(value);
            output.EndObjectStream();

            Assert.AreEqual(1 /*code*/ + 4 /*length*/ + 100000 /*value*/, stream.Length);
        }

        /// <summary>
        /// In AMF3 strings are cached in lots of contexts.
        /// Tests a mixture of them.
        /// </summary>
        [Test]
        public void WriteObject_Strings_Caching_AMF3()
        {
            ASString empty = new ASString("");
            ASString valueA = new ASString("a");
            ASString valueB = new ASString("b");
            ASString valueC = new ASString("c");
            ASXmlDocument xml = new ASXmlDocument(valueB.Value);
            ASArray array = new ASArray(new IASValue[] { valueA, valueB });
            array.DynamicProperties[valueB.Value] = valueA;
            ASClass clazz = new ASClass(valueB.Value, ASClassLayout.Dynamic, new string[] { valueC.Value });
            ASObject obj = new ASObject(clazz);
            obj.MemberValues[0] = valueC;
            obj.DynamicProperties[valueA.Value] = valueB;

            output.ObjectEncoding = AMFObjectEncoding.AMF3;
            output.BeginObjectStream();
            output.WriteObject(empty); // empty strings are not cached
            output.WriteObject(valueA); // will get string ref #0
            output.WriteObject(empty); // empty strings are not cached
            output.WriteObject(valueB); // will get string ref #1
            output.WriteObject(valueA); // will use string ref #0
            output.WriteObject(xml); // XML contents are same as valueB, will use ref #1
            output.WriteObject(array); // Array contains valueA and valueB and mixed values with key valueB and value valueA
            output.WriteObject(obj); // Object has class name valueB contains member with key valueC and value valueA dynamic property with key valueA and value valueB
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(
                new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data,
                    (byte)AMF3ObjectTypeCode.String, 0x01, // ""
                    (byte)AMF3ObjectTypeCode.String, 0x03, 0x61, // valueA
                    (byte)AMF3ObjectTypeCode.String, 0x01, // ""
                    (byte)AMF3ObjectTypeCode.String, 0x03, 0x62, // valueB
                    (byte)AMF3ObjectTypeCode.String, 0x00, // valueA (by ref)
                    (byte)AMF3ObjectTypeCode.Xml, 0x02, // valueB (by ref)
                    (byte)AMF3ObjectTypeCode.Array, 0x05, 0x02, (byte)AMF3ObjectTypeCode.String, 0x00, 0x01, (byte)AMF3ObjectTypeCode.String, 0x00, (byte)AMF3ObjectTypeCode.String, 0x02, // array
                    (byte)AMF3ObjectTypeCode.Object, 0x1b, 0x02, 0x03, 0x63, (byte)AMF3ObjectTypeCode.String, 0x04, 0x00, (byte)AMF3ObjectTypeCode.String, 0x02, 0x01 // object
                }, stream.ToArray());
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.ShortString, 0x00, 0x05, 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.String, 0x0b, 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        public void WriteObject_Strings(AMFObjectEncoding objectEncoding, byte[] expected, string value)
        {
            output.ObjectEncoding = objectEncoding;
            output.BeginObjectStream();
            output.WriteObject(new ASString(value));
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Date,
            0x3f, 0xf0, 0, 0, 0, 0, 0, 0, // 0x01 as a double
            0x00, 0x00 // timezone is GMT
            }, // 0x01 as a double
            1)]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte) AMF3ObjectTypeCode.Date, 0x01,
            0x3f, 0xf0, 0, 0, 0, 0, 0, 0 },
            1)]
        public void WriteObject_Dates(AMFObjectEncoding objectEncoding, byte[] expected, int millisecondsSinceEpoch)
        {
            ASDate date = new ASDate(1, 0);

            output.ObjectEncoding = objectEncoding;
            output.BeginObjectStream();
            output.WriteObject(date);
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Array, 0x00, 0x00, 0x00, 0x02,
            (byte)AMF0ObjectTypeCode.Number, 0x3f, 0xf0, 0, 0, 0, 0, 0, 0, // 0x01 as a double
            (byte)AMF0ObjectTypeCode.Number, 0x3f, 0xf0, 0, 0, 0, 0, 0, 0}, // 0x01 as a double
            new byte[] { 0x01, 0x01 })]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte) AMF3ObjectTypeCode.ByteArray,
            0x1f, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f },
            new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f })]
        public void WriteObject_ByteArrays(AMFObjectEncoding objectEncoding, byte[] expected, byte[] bytes)
        {
            ASByteArray byteArray = new ASByteArray(bytes);

            output.ObjectEncoding = objectEncoding;
            output.BeginObjectStream();
            output.WriteObject(byteArray);
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Array, 0x00, 0x00, 0x00, 0x02,
            (byte)AMF0ObjectTypeCode.Number, 0x3f, 0xf0, 0, 0, 0, 0, 0, 0, // 0x01 as a double
            (byte)AMF0ObjectTypeCode.Number, 0x3f, 0xf0, 0, 0, 0, 0, 0, 0}, // 0x01 as a double
            new byte[] { 0x01, 0x01 })]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte) AMF3ObjectTypeCode.ByteArray,
            0x1f, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f },
            new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f })]
        public void WriteObject_ByteArrays_WrittenInSegments(AMFObjectEncoding objectEncoding, byte[] expected, byte[] bytes)
        {
            IASValue byteArray = Mocks.CreateMock<IASValue>();
            byteArray.AcceptVisitor(serializer, null);
            LastCall.IgnoreArguments().Do((AcceptVisitorDelegate) delegate(IActionScriptSerializer theSerializer, IASValueVisitor visitor)
            {
                ArraySegment<byte>[] segments = new ArraySegment<byte>[bytes.Length];
                for (int i = 0; i < bytes.Length; i++)
                {
                    segments[i] = new ArraySegment<byte>(bytes, i, 1);
                }

                visitor.VisitByteArray(serializer, bytes.Length, segments);
            });

            Mocks.ReplayAll();

            output.ObjectEncoding = objectEncoding;
            output.BeginObjectStream();
            output.WriteObject(byteArray);
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        // empty arrays
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Array, 0x00, 0x00, 0x00, 0x00 },
            new string[] { },
            new string[] { })]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Array, 0x01, 0x01 },
            new string[] { },
            new string[] { })]
        // arrays with only indexed values
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Array, 0x00, 0x00, 0x00, 0x02,
            (byte)AMF0ObjectTypeCode.ShortString, 0x00, 0x03, 0x61, 0x62, 0x63,
            (byte)AMF0ObjectTypeCode.ShortString, 0x00, 0x03, 0x64, 0x65, 0x66 },
            new string[] { "abc", "def" },
            new string[] { })]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte) AMF3ObjectTypeCode.Array, 0x05,
            0x01, // no mixed key/value pairs
            (byte)AMF3ObjectTypeCode.String, 0x07, 0x61, 0x62, 0x63,
            (byte)AMF3ObjectTypeCode.String, 0x07, 0x64, 0x65, 0x66 },
            new string[] { "abc", "def" },
            new string[] { })]
        // arrays with mixed and indexed values
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.MixedArray, 0x00, 0x00, 0x00, 0x02,
            0x00, 0x01, 0x78, (byte)AMF0ObjectTypeCode.ShortString, 0x00, 0x01, 0x31, // "x" = "1"
            0x00, 0x01, 0x79, (byte)AMF0ObjectTypeCode.ShortString, 0x00, 0x01, 0x32, // "y" = "2"
            0x00, 0x01, 0x30, (byte)AMF0ObjectTypeCode.ShortString, 0x00, 0x03, 0x61, 0x62, 0x63, // 0 = "abc"
            0x00, 0x01, 0x31, (byte)AMF0ObjectTypeCode.ShortString, 0x00, 0x03, 0x64, 0x65, 0x66, // 1 = "def"
            0x00, 0x00, (byte)AMF0ObjectTypeCode.EndOfObject },
            new string[] { "abc", "def" },
            new string[] { "x", "1", "y", "2" })]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte) AMF3ObjectTypeCode.Array, 0x05,
            0x03, 0x78, (byte)AMF3ObjectTypeCode.String, 0x03, 0x31, // "x" = "1"
            0x03, 0x79, (byte)AMF3ObjectTypeCode.String, 0x03, 0x32, // "y" = "2"
            0x01,
            (byte)AMF3ObjectTypeCode.String, 0x07, 0x61, 0x62, 0x63,
            (byte)AMF3ObjectTypeCode.String, 0x07, 0x64, 0x65, 0x66 },
            new string[] { "abc", "def" },
            new string[] { "x", "1", "y", "2" })]
        // arrays with only mixed values
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.MixedArray, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x01, 0x78, (byte)AMF0ObjectTypeCode.ShortString, 0x00, 0x01, 0x31, // "x" = "1"
            0x00, 0x01, 0x79, (byte)AMF0ObjectTypeCode.ShortString, 0x00, 0x01, 0x32, // "y" = "2"
            0x00, 0x00, (byte)AMF0ObjectTypeCode.EndOfObject },
            new string[] { },
            new string[] { "x", "1", "y", "2" })]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte) AMF3ObjectTypeCode.Array, 0x01,
            0x03, 0x78, (byte)AMF3ObjectTypeCode.String, 0x03, 0x31, // "x" = "1"
            0x03, 0x79, (byte)AMF3ObjectTypeCode.String, 0x03, 0x32, // "y" = "2"
            0x01 },
            new string[] { },
            new string[] { "x", "1", "y", "2" })]
        public void WriteObject_Arrays(AMFObjectEncoding objectEncoding, byte[] expected, string[] values, string[] mixedKeysAndValues)
        {
            ASArray array = new ASArray(WrapStrings(values));
            for (int i = 0; i < mixedKeysAndValues.Length; i += 2)
                array.DynamicProperties[mixedKeysAndValues[i]] = new ASString(mixedKeysAndValues[i + 1]);

            output.ObjectEncoding = objectEncoding;
            output.BeginObjectStream();
            output.WriteObject(array);
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        // empty dynamic untyped objects
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Object,
            0x00, 0x00, (byte)AMF0ObjectTypeCode.EndOfObject },
            "", ASClassLayout.Dynamic,
            new string[] { },
            new string[] { },
            new string[] { })]
        [Row(AMFObjectEncoding.AMF3, new byte[] {
            (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Object, 0x0b,
            0x01, // class def
            0x01, // end of dynamic values
            },
            "", ASClassLayout.Dynamic,
            new string[] { },
            new string[] { },
            new string[] { })]
        // empty normal typed objects
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.TypedObject,
            0x00, 0x05, 0x63, 0x6c, 0x61, 0x73, 0x73, // class name
            0x00, 0x00, (byte)AMF0ObjectTypeCode.EndOfObject },
            "class", ASClassLayout.Normal,
            new string[] { },
            new string[] { },
            new string[] { })]
        [Row(AMFObjectEncoding.AMF3, new byte[] {
            (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Object, 0x03,
            0x0b, 0x63, 0x6c, 0x61, 0x73, 0x73 // class def
            },
            "class", ASClassLayout.Normal,
            new string[] { },
            new string[] { },
            new string[] { })]
        // normal typed objects with member values
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.TypedObject,
            0x00, 0x05, 0x63, 0x6c, 0x61, 0x73, 0x73, // class name
            0x00, 0x01, 0x78, (byte)AMF0ObjectTypeCode.ShortString, 0x00, 0x01, 0x31, // x = "1"
            0x00, 0x01, 0x79, (byte)AMF0ObjectTypeCode.ShortString, 0x00, 0x01, 0x32, // y = "2"
            0x00, 0x00, (byte)AMF0ObjectTypeCode.EndOfObject },
            "class", ASClassLayout.Normal,
            new string[] { "x", "y" },
            new string[] { "1", "2", },
            new string[] { })]
        [Row(AMFObjectEncoding.AMF3, new byte[] {
            (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Object, 0x23,
            0x0b, 0x63, 0x6c, 0x61, 0x73, 0x73, 0x03, 0x78, 0x03, 0x79, // class def
            (byte)AMF3ObjectTypeCode.String, 0x03, 0x31, // x = "1"
            (byte)AMF3ObjectTypeCode.String, 0x03, 0x32, // y = "2"
            },
            "class", ASClassLayout.Normal,
           new string[] { "x", "y" },
           new string[] { "1", "2", },
           new string[] { })]
        // dynamic typed objects with member values and dynamic properties
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.TypedObject,
            0x00, 0x05, 0x63, 0x6c, 0x61, 0x73, 0x73, // class name
            0x00, 0x01, 0x78, (byte)AMF0ObjectTypeCode.ShortString, 0x00, 0x01, 0x31, // x = "1"
            0x00, 0x01, 0x79, (byte)AMF0ObjectTypeCode.ShortString, 0x00, 0x01, 0x32, // y = "2"
            0x00, 0x01, 0x61, (byte)AMF0ObjectTypeCode.ShortString, 0x00, 0x01, 0x33, // a = "3"
            0x00, 0x01, 0x62, (byte)AMF0ObjectTypeCode.ShortString, 0x00, 0x01, 0x34, // b = "4"
            0x00, 0x00, (byte)AMF0ObjectTypeCode.EndOfObject },
            "class", ASClassLayout.Dynamic,
            new string[] { "x", "y" },
            new string[] { "1", "2", },
            new string[] { "a", "3", "b", "4" })]
        [Row(AMFObjectEncoding.AMF3, new byte[] {
            (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Object, 0x2b,
            0x0b, 0x63, 0x6c, 0x61, 0x73, 0x73, 0x03, 0x78, 0x03, 0x79, // class def
            (byte)AMF3ObjectTypeCode.String, 0x03, 0x31, // x = "1"
            (byte)AMF3ObjectTypeCode.String, 0x03, 0x32, // y = "2"
            0x03, 0x61, (byte)AMF3ObjectTypeCode.String, 0x03, 0x33, // a = "3"
            0x03, 0x62, (byte)AMF3ObjectTypeCode.String, 0x03, 0x34, // b = "4"
            0x01, // end of dynamic properties
            },
            "class", ASClassLayout.Dynamic,
            new string[] { "x", "y" },
            new string[] { "1", "2", },
            new string[] { "a", "3", "b", "4" })]
        public void WriteObject_Objects(AMFObjectEncoding objectEncoding, byte[] expected,
            string className, ASClassLayout classLayout,
            string[] memberNames, string[] memberValues, string[] dynamicKeysAndValues)
        {
            ASClass @class = new ASClass(className, classLayout, memberNames);
            ASObject obj = new ASObject(@class, WrapStrings(memberValues), new Dictionary<string, IASValue>());

            for (int i = 0; i < dynamicKeysAndValues.Length; i += 2)
                obj.DynamicProperties[dynamicKeysAndValues[i]] = new ASString(dynamicKeysAndValues[i + 1]);

            output.ObjectEncoding = objectEncoding;
            output.BeginObjectStream();
            output.WriteObject(obj);
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [Test]
        public void WriteObject_Objects_Externalizable_AMF3()
        {
            IExternalizable externalizableValue = Mocks.CreateMock<IExternalizable>();
            externalizableValue.WriteExternal(output);
            LastCall.Do((WriteExternalDelegate)delegate(IDataOutput outputToUse)
            {
                // Note: outputToUse will be the same instance as output which we've already
                // tested so we don't need to try all combinations here.  Just a few as a sanity check.
                outputToUse.WriteUTF("abc");
                outputToUse.WriteInt(10);
                outputToUse.WriteObject(new ASString("def"));
            });

            ASClass @class = new ASClass("class", ASClassLayout.Externalizable, EmptyArray<string>.Instance);
            ASExternalizableObject obj = new ASExternalizableObject(@class, externalizableValue);

            Mocks.ReplayAll();

            output.ObjectEncoding = AMFObjectEncoding.AMF3;
            output.BeginObjectStream();
            output.WriteObject(obj);
            output.EndObjectStream();

            byte[] expected = new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Object, 0x07,
                0x0b, 0x63, 0x6c, 0x61, 0x73, 0x73, // class def
                0x00, 0x03, 0x61, 0x62, 0x63, // write utf "abc"
                0x00, 0x00, 0x00, 0x0a, // write int 10
                (byte) AMF3ObjectTypeCode.String, 0x07, 0x64, 0x65, 0x66 // write object "def"
            };

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [Test]
        public void WriteObject_Objects_ClassDefinitionCaching_AMF3()
        {
            // Write out two untyped dynamic objects and two typed normal
            // objects and ensure the class definition is reused.
            ASObject untyped1 = new ASObject();
            ASObject untyped2 = new ASObject();
            ASClass @class = new ASClass("class", ASClassLayout.Normal, EmptyArray<string>.Instance);
            ASObject typed1 = new ASObject(@class);
            ASObject typed2 = new ASObject(@class);

            Mocks.ReplayAll();

            output.ObjectEncoding = AMFObjectEncoding.AMF3;
            output.BeginObjectStream();
            output.WriteObject(untyped1);
            output.WriteObject(typed1);
            output.WriteObject(untyped2);
            output.WriteObject(typed2);
            output.EndObjectStream();

            byte[] expected = new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data,
                (byte)AMF3ObjectTypeCode.Object, 0x0b, 0x01, 0x01, // untyped1
                (byte)AMF3ObjectTypeCode.Object, 0x03, 0x0b, 0x63, 0x6c, 0x61, 0x73, 0x73, // typed1
                (byte)AMF3ObjectTypeCode.Object, 0x01, 0x01, // untyped2 using cached class definition
                (byte)AMF3ObjectTypeCode.Object, 0x05, // typed2 using cached class definition
            };

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] {
            (byte)AMF0ObjectTypeCode.MixedArray, 0x00, 0x00, 0x00, 0x06,
            0x00, 0x01, 0x61, (byte)AMF0ObjectTypeCode.Reference, 0x00, 0x01, // array by reference
            0x00, 0x01, 0x30, (byte)AMF0ObjectTypeCode.Object, 0x00, 0x00, (byte)AMF0ObjectTypeCode.EndOfObject, // obj
            0x00, 0x01, 0x31, (byte)AMF0ObjectTypeCode.Array, 0x00, 0x00, 0x00, 0x00, // byte array
            0x00, 0x01, 0x32, (byte)AMF0ObjectTypeCode.Date, 0x3f, 0xf0, 0, 0, 0, 0, 0, 0, 0x00, 0x00, // date
            0x00, 0x01, 0x33, (byte)AMF0ObjectTypeCode.Reference, 0x00, 0x02, // obj by reference
            0x00, 0x01, 0x34, (byte)AMF0ObjectTypeCode.Reference, 0x00, 0x03, // byte array by reference
            0x00, 0x01, 0x35, (byte)AMF0ObjectTypeCode.Date, 0x3f, 0xf0, 0, 0, 0, 0, 0, 0, 0x00, 0x00, // date (not cached!)
            0x00, 0x00, (byte)AMF0ObjectTypeCode.EndOfObject
            })]
        [Row(AMFObjectEncoding.AMF3, new byte[] {
            (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Array, 0x0d,
            0x03, 0x61, (byte)AMF3ObjectTypeCode.Array, 0x00, // array by reference
            0x01, // end of mixed values
            (byte)AMF3ObjectTypeCode.Object, 0x0b, 0x01, 0x01, // obj
            (byte)AMF3ObjectTypeCode.ByteArray, 0x01, // byte array
            (byte)AMF3ObjectTypeCode.Date, 0x01, 0x3f, 0xf0, 0, 0, 0, 0, 0, 0, // date
            (byte)AMF3ObjectTypeCode.Object, 0x02, // obj by reference
            (byte)AMF3ObjectTypeCode.ByteArray, 0x04, // byte array by reference
            (byte)AMF3ObjectTypeCode.Date, 0x06, // date by reference
            })]
        public void WriteObject_Objects_ReferenceCaching(AMFObjectEncoding objectEncoding, byte[] expected)
        {
            // Create an array with multiple copies of each kind of object and including a self-reference
            ASObject obj = new ASObject();
            ASByteArray byteArray = new ASByteArray(new byte[0]);
            ASDate date = new ASDate(1, 0);
            ASArray array = new ASArray(6);

            array.IndexedValues[0] = obj;
            array.IndexedValues[1] = byteArray;
            array.IndexedValues[2] = date;
            array.IndexedValues[3] = obj;
            array.IndexedValues[4] = byteArray;
            array.IndexedValues[5] = date;
            array.DynamicProperties["a"] = array;

            output.ObjectEncoding = objectEncoding;
            output.BeginObjectStream();
            output.WriteObject(array);
            output.EndObjectStream();

            CollectionAssert.AreElementsEqual(expected, stream.ToArray());
        }

        private static ASString[] WrapStrings(string[] strings)
        {
            return Array.ConvertAll<string, ASString>(strings, delegate(string value)
            {
                return new ASString(value);
            });
        }
    }
}