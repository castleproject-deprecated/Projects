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
using Castle.FlexBridge.Serialization;
using Castle.FlexBridge.Serialization.AMF;
using Castle.FlexBridge.Tests.UnitTests;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace Castle.FlexBridge.Tests.UnitTests.Serialization.AMF
{
    [TestFixture]
    [TestsOn(typeof(AMFDataInput))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class AMFDataInputTest : BaseUnitTest
    {
        private delegate void ReadExternalDelegate(IDataInput input);

        private AMFDataInput input;
        private MemoryStream stream;
        private IActionScriptSerializer serializer;

        public override void SetUp()
        {
            base.SetUp();

            stream = new MemoryStream();
            serializer = Mocks.CreateMock<IActionScriptSerializer>();
            input = new AMFDataInput(stream, serializer);
        }

        public override void TearDown()
        {
            // Ensures that the stream has been completely read.
            Assert.AreEqual(stream.Length, stream.Position, "Stream should have been fully read by the test.");

            base.TearDown();
        }

        [Test]
        public void InitialEncodingIsAMF0()
        {
            Assert.AreEqual(AMFObjectEncoding.AMF0, input.ObjectEncoding);
        }

        [Test]
        public void SerializerAccessibleViaGetter()
        {
            Assert.AreSame(serializer, input.Serializer);
        }

        [Test]
        public void ReadByte()
        {
            SetStreamContents(new byte[] { 1, 0, 42 });

            Assert.AreEqual(1, input.ReadByte());
            Assert.AreEqual(0, input.ReadByte());
            Assert.AreEqual(42, input.ReadByte());
        }

        [Test]
        [ExpectedException(typeof(EndOfStreamException))]
        public void ReadByte_ThrowsEndOfStreamExceptionWhenAtEnd()
        {
            input.ReadByte();
        }

        [Test]
        public void ReadBytes()
        {
            SetStreamContents(new byte[] { 3, 2 });

            byte[] bytes = new byte[5];
            input.ReadBytes(bytes, 2, 2);

            CollectionAssert.AreElementsEqual(new byte[] { 0, 0, 3, 2, 0 }, bytes);
        }

        [Test]
        [ExpectedException(typeof(EndOfStreamException))]
        public void ReadBytes_ThrowsEndOfStreamExceptionWhenAtEnd()
        {
            input.ReadBytes(new byte[1], 0, 1);
        }

        [RowTest]
        [Row(new byte[] { 1 }, true)]
        [Row(new byte[] { 0 }, false)]
        public void ReadBoolean(byte[] data, bool expectedValue)
        {
            SetStreamContents(data);

            Assert.AreEqual(expectedValue, input.ReadBoolean());
        }

        [Test]
        public void ReadDouble()
        {
            SetStreamContents(new byte[] { 0x3f, 0xf0, 0, 0, 0, 0, 0, 0 });

            Assert.AreEqual(1.0, input.ReadDouble());
        }

        [Test]
        public void ReadFloat()
        {
            SetStreamContents(new byte[] { 0x3f, 0x80, 0, 0 });

            Assert.AreEqual(1.0f, input.ReadFloat());
        }

        [RowTest]
        [Row(new byte[] { 0x12, 0x34, 0x56, 0x78 }, 0x12345678)]
        [Row(new byte[] { 0xed, 0xcb, 0xa9, 0x88 }, - 0x12345678)]
        public void ReadInt(byte[] data, int expectedValue)
        {
            SetStreamContents(data);

            Assert.AreEqual(expectedValue, input.ReadInt());
        }

        [RowTest]
        [Row(new byte[] { 0x12, 0x34 }, (short) 0x1234)]
        [Row(new byte[] { 0xed, 0xcc }, (short) -0x1234)]
        public void ReadShort(byte[] data, short expectedValue)
        {
            SetStreamContents(data);

            Assert.AreEqual(expectedValue, input.ReadShort());
        }

        [RowTest]
        [Row(new byte[] { 0x12, 0x34, 0x56, 0x78 }, (uint) 0x12345678)]
        [Row(new byte[] { 0xed, 0xcb, 0xa9, 0x88 }, (uint) 0xedcba988)]
        public void ReadUnsignedInt(byte[] data, uint expectedValue)
        {
            SetStreamContents(data);

            Assert.AreEqual(expectedValue, input.ReadUnsignedInt());
        }

        [RowTest]
        [Row(new byte[] { 0x12, 0x34 }, (ushort) 0x1234)]
        [Row(new byte[] { 0xed, 0xcc }, (ushort) 0xedcc )]
        public void ReadUnsignedShort(byte[] data, ushort expectedValue)
        {
            SetStreamContents(data);

            Assert.AreEqual(expectedValue, input.ReadUnsignedShort());
        }

        [RowTest]
        [Row(new byte[] { 0x00, 0x00 }, "")]
        [Row(new byte[] { 0x00, 0x05, 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        public void ReadUTF(byte[] data, string expectedValue)
        {
            SetStreamContents(data);

            Assert.AreEqual(expectedValue, input.ReadUTF());
        }

        [RowTest]
        [Row(new byte[] { }, "")]
        [Row(new byte[] { 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        public void ReadUTFBytes(byte[] data, string expectedValue)
        {
            SetStreamContents(data);

            Assert.AreEqual(expectedValue, input.ReadUTFBytes(data.Length));
        }

        [RowTest]
        [Row(new byte[] { 0x00, 0x00 }, "")]
        [Row(new byte[] { 0x00, 0x05, 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        public void ReadShortString(byte[] data, string expectedValue)
        {
            SetStreamContents(data);

            Assert.AreEqual(expectedValue, input.ReadShortString());
        }

        [RowTest]
        [Row(new byte[] { 0x00, 0x00, 0x00, 0x00 }, "")]
        [Row(new byte[] { 0x00, 0x00, 0x00, 0x05, 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        public void ReadLongString(byte[] data, string expectedValue)
        {
            SetStreamContents(data);

            Assert.AreEqual(expectedValue, input.ReadLongString());
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
        public void ReadVWInt29(byte[] data, int expectedValue)
        {
            SetStreamContents(data);

            Assert.AreEqual(expectedValue, input.ReadVWInt29());
        }

        [Test]
        public void BeginAndEndObjectStreamWorkAsAPair()
        {
            input.BeginObjectStream();
            input.EndObjectStream();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void BeginObjectStreamFailsIfAlreadyStarted()
        {
            input.BeginObjectStream();
            input.BeginObjectStream();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void EndbjectStreamFailsIfNotStarted()
        {
            input.EndObjectStream();
        }


        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Boolean, 1 }, true)]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Boolean, 0 }, false)]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.True }, true)]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.False }, false)]
        public void ReadObject_Booleans(AMFObjectEncoding objectEncoding, byte[] bytes, bool expectedValue)
        {
            SetStreamContents(bytes);

            input.BeginObjectStream();
            ASBoolean result = (ASBoolean)input.ReadObject();
            Assert.AreEqual(objectEncoding, input.ObjectEncoding);
            input.EndObjectStream();

            Assert.AreEqual(expectedValue, result.Value);
        }

        [Test]
        public void ReadObject_Ints_AMF3()
        {
            byte[] bytes = new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Integer, 0x01 };
            int expectedValue = 1;

            SetStreamContents(bytes);

            input.BeginObjectStream();
            ASInt29 result = (ASInt29)input.ReadObject();
            Assert.AreEqual(AMFObjectEncoding.AMF3, input.ObjectEncoding);
            input.EndObjectStream();

            Assert.AreEqual(expectedValue, result.Value);
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Number, 0x3f, 0xf0, 0, 0, 0, 0, 0, 0 }, 1.0)]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Number, 0x3f, 0xf0, 0, 0, 0, 0, 0, 0 }, 1.0)]
        public void ReadObject_Numbers(AMFObjectEncoding objectEncoding, byte[] bytes, double expectedValue)
        {
            SetStreamContents(bytes);

            input.BeginObjectStream();
            ASNumber result = (ASNumber)input.ReadObject();
            Assert.AreEqual(objectEncoding, input.ObjectEncoding);
            input.EndObjectStream();

            Assert.AreEqual(expectedValue, result.Value);
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Null })]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Null })]
        public void ReadObject_Nulls(AMFObjectEncoding objectEncoding, byte[] bytes)
        {
            SetStreamContents(bytes);

            input.BeginObjectStream();
            object result = input.ReadObject();
            Assert.AreEqual(objectEncoding, input.ObjectEncoding);
            input.EndObjectStream();

            Assert.IsNull(result);
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Undefined })]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Undefined })]
        public void ReadObject_UndefinedValues(AMFObjectEncoding objectEncoding, byte[] bytes)
        {
            SetStreamContents(bytes);

            input.BeginObjectStream();
            ASUndefined result = (ASUndefined) input.ReadObject();
            Assert.AreEqual(objectEncoding, input.ObjectEncoding);
            input.EndObjectStream();

            Assert.AreSame(ASUndefined.Value, result);
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Unsupported })]
        public void ReadObject_UnsupportedValues(AMFObjectEncoding objectEncoding, byte[] bytes)
        {
            SetStreamContents(bytes);

            input.BeginObjectStream();
            ASUnsupported result = (ASUnsupported) input.ReadObject();
            Assert.AreEqual(objectEncoding, input.ObjectEncoding);
            input.EndObjectStream();

            Assert.AreSame(ASUnsupported.Value, result);
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Xml, 0x00, 0x00, 0x00, 0x05, 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Xml, 0x0b, 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        public void ReadObject_XmlDocuments(AMFObjectEncoding objectEncoding, byte[] bytes, string expectedValue)
        {
            SetStreamContents(bytes);

            input.BeginObjectStream();
            ASXmlDocument result = (ASXmlDocument) input.ReadObject();
            Assert.AreEqual(objectEncoding, input.ObjectEncoding);
            input.EndObjectStream();

            Assert.AreEqual(expectedValue, result.XmlString);
        }

        [Test]
        public void ReadObject_Strings_LongFormat_AMF0()
        {
            byte[] bytes = new byte[100005];
            bytes[0] = (byte) AMF0ObjectTypeCode.LongString;
            bytes[1] = 0x00;
            bytes[2] = 0x01;
            bytes[3] = 0x86;
            bytes[4] = 0xa0;
            for (int i = 0; i < 100000; i++)
                bytes[i + 5] = 0x61;

            SetStreamContents(bytes);

            input.BeginObjectStream();
            ASString result = (ASString)input.ReadObject();
            Assert.AreEqual(AMFObjectEncoding.AMF0, input.ObjectEncoding);
            input.EndObjectStream();

            Assert.AreEqual(new String('a', 100000), result.Value);
        }

        /// <summary>
        /// In AMF3 strings are cached in lots of contexts.
        /// Tests a mixture of them.
        /// </summary>
        [Test]
        public void ReadObject_Strings_Caching_AMF3()
        {
            ASString empty = new ASString("");
            ASString valueA = new ASString("a");
            ASString valueB = new ASString("b");
            ASString valueC = new ASString("c");

            byte[] bytes =  new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data,
                    (byte)AMF3ObjectTypeCode.String, 0x01, // ""
                    (byte)AMF3ObjectTypeCode.String, 0x03, 0x61, // valueA
                    (byte)AMF3ObjectTypeCode.String, 0x01, // ""
                    (byte)AMF3ObjectTypeCode.String, 0x03, 0x62, // valueB
                    (byte)AMF3ObjectTypeCode.String, 0x00, // valueA (by ref)
                    (byte)AMF3ObjectTypeCode.Xml, 0x02, // valueB (by ref)
                    (byte)AMF3ObjectTypeCode.Array, 0x05, 0x02, (byte)AMF3ObjectTypeCode.String, 0x00, 0x01, (byte)AMF3ObjectTypeCode.String, 0x00, (byte)AMF3ObjectTypeCode.String, 0x02, // array
                    (byte)AMF3ObjectTypeCode.Object, 0x1b, 0x02, 0x03, 0x63, (byte)AMF3ObjectTypeCode.String, 0x04, 0x00, (byte)AMF3ObjectTypeCode.String, 0x02, 0x01 // object
            };
            SetStreamContents(bytes);

            input.BeginObjectStream();
            Assert.AreEqual(empty, input.ReadObject()); // empty strings are not cached
            Assert.AreEqual(valueA, input.ReadObject()); // will get string ref #0
            Assert.AreEqual(empty, input.ReadObject()); // empty strings are not cached
            Assert.AreEqual(valueB, input.ReadObject()); // will get string ref #1
            Assert.AreEqual(valueA, input.ReadObject()); // will use string ref #0
            ASXmlDocument xml = (ASXmlDocument)input.ReadObject();
            Assert.AreEqual(valueB.Value, xml.XmlString); // XML contents are same as valueB, will use ref #1
            ASArray array = (ASArray) input.ReadObject(); // Array contains valueA and valueB and mixed values with key valueB and value valueA
            CollectionAssert.AreElementsEqual(new object[] { valueA, valueB }, array.IndexedValues); 
            Assert.AreEqual(valueA, array.DynamicProperties[valueB.Value]);
            ASObject obj = (ASObject) input.ReadObject(); // Object has class name valueB contains member with key valueC and value valueA dynamic property with key valueA and value valueB
            CollectionAssert.AreElementsEqual(new object[] { valueC }, obj.MemberValues);
            Assert.AreEqual(valueB, obj.DynamicProperties[valueA.Value]);
            Assert.AreEqual(valueB.Value, obj.Class.ClassAlias);
            Assert.AreEqual(ASClassLayout.Dynamic, obj.Class.Layout);
            CollectionAssert.AreElementsEqual(new string[] { valueC.Value }, obj.Class.MemberNames);

            Assert.AreEqual(AMFObjectEncoding.AMF3, input.ObjectEncoding);
            input.EndObjectStream();
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.ShortString, 0x00, 0x05, 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.String, 0x0b, 0x61, 0x00, 0xe3, 0x8c, 0xb3 }, "a\0\u3333")]
        public void ReadObject_Strings(AMFObjectEncoding objectEncoding, byte[] bytes, string expectedValue)
        {
            SetStreamContents(bytes);

            input.BeginObjectStream();
            ASString result = (ASString)input.ReadObject();
            Assert.AreEqual(objectEncoding, input.ObjectEncoding);
            input.EndObjectStream();

            Assert.AreEqual(expectedValue, result.Value);
        }

        [RowTest]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Date,
            0x3f, 0xf0, 0, 0, 0, 0, 0, 0, // 0x01 as a double
            0x00, 0x00 // timezone is GMT
            }, // 0x01 as a double
            1, 0)]
        [Row(AMFObjectEncoding.AMF0, new byte[] { (byte)AMF0ObjectTypeCode.Date,
            0x3f, 0xf0, 0, 0, 0, 0, 0, 0, // 0x01 as a double
            0x00, 0x3c // timezone is 1 hr
            }, // 0x01 as a double
            1, 60)]
        [Row(AMFObjectEncoding.AMF3, new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte) AMF3ObjectTypeCode.Date, 0x01,
            0x3f, 0xf0, 0, 0, 0, 0, 0, 0 },
            1, 0)]
        public void ReadObject_Dates(AMFObjectEncoding objectEncoding, byte[] bytes, int millisecondsSinceEpoch, int timezoneOffsetMinutes)
        {
            SetStreamContents(bytes);

            input.BeginObjectStream();
            ASDate result = (ASDate)input.ReadObject();
            Assert.AreEqual(objectEncoding, input.ObjectEncoding);
            input.EndObjectStream();

            Assert.AreEqual(millisecondsSinceEpoch, result.MillisecondsSinceEpoch);
            Assert.AreEqual(timezoneOffsetMinutes, result.TimeZoneOffsetMinutes);
        }

        [Test]
        public void ReadObject_ByteArrays_AMF3()
        {
            byte[] bytes = new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte) AMF3ObjectTypeCode.ByteArray,
                0x1f, 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };
            byte[] expectedValue = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0a, 0x0b, 0x0c, 0x0d, 0x0e, 0x0f };

            SetStreamContents(bytes);

            input.BeginObjectStream();
            ASByteArray result = (ASByteArray)input.ReadObject();
            Assert.AreEqual(AMFObjectEncoding.AMF3, input.ObjectEncoding);
            input.EndObjectStream();

            CollectionAssert.AreElementsEqual(expectedValue, result.Bytes.Array);
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
        public void ReadObject_Arrays(AMFObjectEncoding objectEncoding, byte[] bytes, string[] indexedValues, string[] mixedKeysAndValues)
        {
            SetStreamContents(bytes);

            input.BeginObjectStream();
            ASArray result = (ASArray)input.ReadObject();
            Assert.AreEqual(objectEncoding, input.ObjectEncoding);
            input.EndObjectStream();

            CollectionAssert.AreElementsEqual(WrapStrings(indexedValues), result.IndexedValues);

            Assert.AreEqual(mixedKeysAndValues.Length / 2, result.DynamicProperties.Count);
            for (int i = 0; i < mixedKeysAndValues.Length; i += 2)
                Assert.AreEqual(new ASString(mixedKeysAndValues[i + 1]), result.DynamicProperties[mixedKeysAndValues[i]]);
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
            "class", ASClassLayout.Dynamic,
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
            "class", ASClassLayout.Dynamic,
            new string[] { },
            new string[] { },
            new string[] { "x", "1", "y", "2" })]
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
            new string[] { },
            new string[] { },
            new string[] { "x", "1", "y", "2", "a", "3", "b", "4" })]
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
        public void ReadObject_Objects(AMFObjectEncoding objectEncoding, byte[] bytes,
            string className, ASClassLayout classLayout,
            string[] memberNames, string[] memberValues, string[] dynamicKeysAndValues)
        {
            SetStreamContents(bytes);

            input.BeginObjectStream();
            ASObject result = (ASObject)input.ReadObject();
            Assert.AreEqual(objectEncoding, input.ObjectEncoding);
            input.EndObjectStream();

            Assert.AreEqual(classLayout, result.Class.Layout);
            Assert.AreEqual(className, result.Class.ClassAlias);
            CollectionAssert.AreElementsEqual(memberNames, result.Class.MemberNames);

            CollectionAssert.AreElementsEqual(WrapStrings(memberValues), result.MemberValues);

            Assert.AreEqual(dynamicKeysAndValues.Length / 2, result.DynamicProperties.Count);
            for (int i = 0; i < dynamicKeysAndValues.Length; i += 2)
                Assert.AreEqual(new ASString(dynamicKeysAndValues[i + 1]), result.DynamicProperties[dynamicKeysAndValues[i]]);
        }

        [Test]
        public void ReadObject_Objects_Externalizable_AMF3()
        {
            IExternalizable externalizableValue = Mocks.CreateMock<IExternalizable>();
            externalizableValue.ReadExternal(input);
            LastCall.Do((ReadExternalDelegate)delegate(IDataInput inputToUse)
            {
                // Note: inputToUse will be the same instance of AMFDataInput which we've already
                // tested so we don't need to try all combinations here.  Just a few as a sanity check.
                Assert.AreEqual("abc", inputToUse.ReadUTF());
                Assert.AreEqual(10, inputToUse.ReadInt());
                Assert.AreEqual(new ASString("def"), inputToUse.ReadObject());
            });

            Expect.Call(serializer.CreateExternalizableInstance("class")).Return(externalizableValue);

            Mocks.ReplayAll();

            SetStreamContents(new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Object, 0x07,
                0x0b, 0x63, 0x6c, 0x61, 0x73, 0x73, // class def
                0x00, 0x03, 0x61, 0x62, 0x63, // write utf "abc"
                0x00, 0x00, 0x00, 0x0a, // write int 10
                (byte) AMF3ObjectTypeCode.String, 0x07, 0x64, 0x65, 0x66 // write object "def"
            });

            input.BeginObjectStream();
            ASExternalizableObject obj = (ASExternalizableObject) input.ReadObject();
            Assert.AreEqual(AMFObjectEncoding.AMF3, input.ObjectEncoding);
            input.EndObjectStream();

            Assert.AreEqual("class", obj.Class.ClassAlias);
            Assert.AreEqual(ASClassLayout.Externalizable, obj.Class.Layout);
            Assert.AreSame(externalizableValue, obj.ExternalizableValue);
        }

        [Test]
        public void ReadObject_Objects_ClassDefinitionCaching_AMF3()
        {
            // Read out two untyped dynamic objects and two typed normal
            // objects and ensures the class definition is reused.
            SetStreamContents(new byte[] { (byte)AMF0ObjectTypeCode.AMF3Data,
                (byte)AMF3ObjectTypeCode.Object, 0x0b, 0x01, 0x01, // untyped1
                (byte)AMF3ObjectTypeCode.Object, 0x03, 0x0b, 0x63, 0x6c, 0x61, 0x73, 0x73, // typed1
                (byte)AMF3ObjectTypeCode.Object, 0x01, 0x01, // untyped2 using cached class definition
                (byte)AMF3ObjectTypeCode.Object, 0x05, // typed2 using cached class definition
            });

            input.BeginObjectStream();
            ASObject untyped1 = (ASObject) input.ReadObject();
            ASObject typed1 = (ASObject) input.ReadObject();
            ASObject untyped2 = (ASObject) input.ReadObject();
            ASObject typed2 = (ASObject) input.ReadObject();
            Assert.AreEqual(AMFObjectEncoding.AMF3, input.ObjectEncoding);
            input.EndObjectStream();

            Assert.AreSame(ASClass.UntypedDynamicClass, untyped1.Class);
            Assert.AreSame(ASClass.UntypedDynamicClass, untyped2.Class);
            Assert.AreEqual("class", typed1.Class.ClassAlias);
            Assert.AreEqual(ASClassLayout.Normal, typed1.Class.Layout);
            Assert.AreEqual(0, typed1.Class.MemberNames.Count);
            Assert.AreEqual(0, typed1.MemberValues.Count);
            Assert.AreSame(typed1.Class, typed2.Class);
        }

        [Test]
        public void ReadObject_Objects_ReferenceCaching_AMF0()
        {
            // Read an array with multiple copies of each kind of object and including a self-reference
            SetStreamContents(new byte[] {
                (byte)AMF0ObjectTypeCode.MixedArray, 0x00, 0x00, 0x00, 0x04,
                0x00, 0x01, 0x61, (byte)AMF0ObjectTypeCode.Reference, 0x00, 0x01, // mixed array by reference
                0x00, 0x01, 0x30, (byte)AMF0ObjectTypeCode.Object, 0x00, 0x00, (byte)AMF0ObjectTypeCode.EndOfObject, // obj
                0x00, 0x01, 0x31, (byte)AMF0ObjectTypeCode.Array, 0x00, 0x00, 0x00, 0x00, // regular array
                0x00, 0x01, 0x32, (byte)AMF0ObjectTypeCode.Reference, 0x00, 0x02, // obj by reference
                0x00, 0x01, 0x33, (byte)AMF0ObjectTypeCode.Reference, 0x00, 0x03, // regular array by reference
                0x00, 0x00, (byte)AMF0ObjectTypeCode.EndOfObject
            });

            input.BeginObjectStream();
            ASArray mixedArray = (ASArray) input.ReadObject();
            Assert.AreEqual(AMFObjectEncoding.AMF0, input.ObjectEncoding);
            input.EndObjectStream();

            Assert.AreEqual(4, mixedArray.IndexedValues.Count);
            Assert.AreEqual(1, mixedArray.DynamicProperties.Count);

            ASObject obj = (ASObject) mixedArray.IndexedValues[0];
            ASArray regularArray = (ASArray) mixedArray.IndexedValues[1];
            Assert.AreSame(obj, mixedArray.IndexedValues[2]);
            Assert.AreSame(regularArray, mixedArray.IndexedValues[3]);
            Assert.AreSame(mixedArray, mixedArray.DynamicProperties["a"]);
        }

        [Test]
        public void ReadObject_Objects_ReferenceCaching_AMF3()
        {
            // Read an array with multiple copies of each kind of object and including a self-reference
            SetStreamContents(new byte[] {
                (byte)AMF0ObjectTypeCode.AMF3Data, (byte)AMF3ObjectTypeCode.Array, 0x0d,
                0x03, 0x61, (byte)AMF3ObjectTypeCode.Array, 0x00, // array by reference
                0x01, // end of mixed values
                (byte)AMF3ObjectTypeCode.Object, 0x0b, 0x01, 0x01, // obj
                (byte)AMF3ObjectTypeCode.ByteArray, 0x01, // byte array
                (byte)AMF3ObjectTypeCode.Date, 0x01, 0x3f, 0xf0, 0, 0, 0, 0, 0, 0, // date
                (byte)AMF3ObjectTypeCode.Object, 0x02, // obj by reference
                (byte)AMF3ObjectTypeCode.ByteArray, 0x04, // byte array by reference
                (byte)AMF3ObjectTypeCode.Date, 0x06, // date by reference
            });

            input.BeginObjectStream();
            ASArray array = (ASArray) input.ReadObject();
            Assert.AreEqual(AMFObjectEncoding.AMF3, input.ObjectEncoding);
            input.EndObjectStream();

            Assert.AreEqual(6, array.IndexedValues.Count);
            Assert.AreEqual(1, array.DynamicProperties.Count);

            ASObject obj = (ASObject) array.IndexedValues[0];
            ASByteArray byteArray = (ASByteArray) array.IndexedValues[1];
            ASDate date = (ASDate) array.IndexedValues[2];
            Assert.AreSame(obj, array.IndexedValues[3]);
            Assert.AreSame(byteArray, array.IndexedValues[4]);
            Assert.AreSame(date, array.IndexedValues[5]);
            Assert.AreSame(array, array.DynamicProperties["a"]);
        }

        /// <summary>
        /// Initializes the contents of the stream to the specified bytes
        /// and resets the stream reading position to 0.
        /// </summary>
        /// <param name="bytes">The bytes to put in the stream.</param>
        private void SetStreamContents(byte[] bytes)
        {
            stream.SetLength(bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            stream.Write(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
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