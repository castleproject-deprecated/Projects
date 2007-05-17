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
using MbUnit.Framework;

namespace Castle.FlexBridge.Tests.UnitTests.Serialization.Mapping
{
    [TestFixture]
    [Author("Jeff", "jeff@ingenio.com")]
    public class PrimitiveMapperTest : BaseMapperTest
    {
        [Test]
        public void ASUndefined_FromASUndefined()
        {
            Assert.AreSame(ASUndefined.Value, ToNative<ASUndefined>(ASUndefined.Value));
        }

        [Test]
        public void ASUndefined_ToASValue()
        {
            Assert.AreSame(ASUndefined.Value, ToASValue(ASUndefined.Value));
        }

        [Test]
        public void ASUnsupported_FromASUnsupported()
        {
            Assert.AreSame(ASUnsupported.Value, ToNative<ASUnsupported>(ASUnsupported.Value));
        }

        [Test]
        public void ASUnsupported_ToASValue()
        {
            Assert.AreSame(ASUnsupported.Value, ToASValue(ASUnsupported.Value));
        }

        [Test]
        public void Boolean_FromASInt29()
        {
            Assert.AreEqual(true, ToNative<Boolean>(new ASInt29(42)));
            Assert.AreEqual(false, ToNative<Boolean>(new ASInt29(0)));
        }

        [Test]
        public void Boolean_FromASNumber()
        {
            Assert.AreEqual(true, ToNative<Boolean>(new ASNumber(42.0)));
            Assert.AreEqual(false, ToNative<Boolean>(new ASNumber(0.0)));
        }

        [Test]
        public void Boolean_FromASString()
        {
            Assert.AreEqual(true, ToNative<Boolean>(new ASString("true")));
            Assert.AreEqual(true, ToNative<Boolean>(new ASString("TRUE")));
            Assert.AreEqual(false, ToNative<Boolean>(new ASString("false")));
            Assert.AreEqual(false, ToNative<Boolean>(new ASString("FALSE")));
        }

        [Test]
        public void Boolean_ToASValue()
        {
            Assert.AreEqual(ASBoolean.True, ToASValue(true));
            Assert.AreEqual(ASBoolean.False, ToASValue(false));
        }

        [Test]
        public void Byte_FromASInt29()
        {
            Assert.AreEqual(42, ToNative<Byte>(new ASInt29(42)));
        }

        [Test]
        public void Byte_FromASNumber()
        {
            Assert.AreEqual(42, ToNative<Byte>(new ASNumber(42.0)));
        }

        [Test]
        public void Byte_FromASString()
        {
            Assert.AreEqual(42, ToNative<Byte>(new ASString("42")));
        }

        [Test]
        public void Byte_ToASValue()
        {
            Assert.AreEqual(new ASInt29(42), ToASValue((byte)42));
        }

        [Test]
        public void Char_FromASInt29()
        {
            Assert.AreEqual('*', ToNative<Char>(new ASInt29(42)));
        }

        [Test]
        public void Char_FromASNumber()
        {
            Assert.AreEqual('*', ToNative<Char>(new ASNumber(42.0)));
        }

        [Test]
        public void Char_FromASString()
        {
            Assert.AreEqual('*', ToNative<Char>(new ASString("*")));
        }

        [Test]
        public void Char_ToASValue()
        {
            Assert.AreEqual(new ASString("*"), ToASValue('*'));
        }

        [Test]
        public void DateTime_FromDate()
        {
            DateTime then = new DateTime(1970, 2, 1, 1, 2, 3, DateTimeKind.Utc);
            Assert.AreEqual(then, ToNative<DateTime>(new ASDate(then)));
        }

        [Test]
        public void DateTime_FromString()
        {
            DateTime then = new DateTime(1970, 2, 1, 0, 0, 0, DateTimeKind.Utc);
            Assert.AreEqual(then, ToNative<DateTime>(new ASString("1970/2/1")));
        }

        [Test]
        public void DateTime_ToASValue()
        {
            DateTime now = DateTime.UtcNow;
            Assert.AreEqual(new ASDate(now), ToASValue(now));
        }

        [Test]
        public void DBNull_FromASNull()
        {
            Assert.AreSame(DBNull.Value, ToNative<DBNull>(null));
            Assert.AreSame(DBNull.Value, ToNative<DBNull>(ASNull.Value));
        }

        [Test]
        public void DBNull_ToASValue()
        {
            Assert.AreSame(ASNull.Value, ToASValue(DBNull.Value));
        }

        [Test]
        public void Decimal_FromASInt29()
        {
            Assert.AreEqual(42.0m, ToNative<Decimal>(new ASInt29(42)));
        }

        [Test]
        public void Decimal_FromASNumber()
        {
            Assert.AreEqual(42.0m, ToNative<Decimal>(new ASNumber(42.0)));
        }

        [Test]
        public void Decimal_FromASString()
        {
            Assert.AreEqual(42.5m, ToNative<Decimal>(new ASString("42.5")));
        }

        [Test]
        public void Decimal_ToASValue()
        {
            Assert.AreEqual(new ASNumber(42.5), ToASValue(42.5m));
        }

        [Test]
        public void Double_FromASInt29()
        {
            Assert.AreEqual(42.0, ToNative<Double>(new ASInt29(42)));
        }

        [Test]
        public void Double_FromASNumber()
        {
            Assert.AreEqual(42.0, ToNative<Double>(new ASNumber(42.0)));
        }

        [Test]
        public void Double_FromASString()
        {
            Assert.AreEqual(42.5m, ToNative<Double>(new ASString("42.5")));
        }

        [Test]
        public void Double_ToASValue()
        {
            Assert.AreEqual(new ASNumber(42.5), ToASValue(42.5));
        }

        [Test]
        public void Int32Enum_FromASInt29()
        {
            Assert.AreEqual(Int32Enum.Zero, ToNative<Int32Enum>(new ASInt29(0)));
            Assert.AreEqual(Int32Enum.One, ToNative<Int32Enum>(new ASInt29(1)));
            Assert.AreEqual((Int32Enum) 2, ToNative<Int32Enum>(new ASInt29(2)));
        }

        [Test]
        public void Int32Enum_FromASNumber()
        {
            Assert.AreEqual(Int32Enum.Zero, ToNative<Int32Enum>(new ASNumber(0.0)));
            Assert.AreEqual(Int32Enum.One, ToNative<Int32Enum>(new ASNumber(1.0)));
            Assert.AreEqual((Int32Enum) 2, ToNative<Int32Enum>(new ASNumber(2.0)));
        }

        [Test]
        public void Int32Enum_FromASString()
        {
            Assert.AreEqual(Int32Enum.Zero, ToNative<Int32Enum>(new ASString("0")));
            Assert.AreEqual(Int32Enum.One, ToNative<Int32Enum>(new ASString("1")));
            Assert.AreEqual((Int32Enum) 2, ToNative<Int32Enum>(new ASString("2")));
            Assert.AreEqual(Int32Enum.Zero, ToNative<Int32Enum>(new ASString("Zero")));
            Assert.AreEqual(Int32Enum.One, ToNative<Int32Enum>(new ASString("One")));
        }

        [Test]
        public void Int32Enum_ToASValue()
        {
            Assert.AreEqual(new ASInt29(0), ToASValue(Int32Enum.Zero));
            Assert.AreEqual(new ASInt29(1), ToASValue(Int32Enum.One));
            Assert.AreEqual(new ASInt29(2), ToASValue((Int32Enum) 2));
        }

        [Test]
        public void UInt64Enum_FromASInt29()
        {
            Assert.AreEqual(UInt64Enum.Zero, ToNative<UInt64Enum>(new ASInt29(0)));
            Assert.AreEqual((UInt64Enum) 2, ToNative<UInt64Enum>(new ASInt29(2)));
        }

        [Test]
        public void UInt64Enum_FromASNumber()
        {
            Assert.AreEqual(UInt64Enum.Zero, ToNative<UInt64Enum>(new ASNumber(0.0)));
            Assert.AreEqual(UInt64Enum.Big, ToNative<UInt64Enum>(new ASNumber(0xffffffff00000000L)));
            Assert.AreEqual((UInt64Enum) 2, ToNative<UInt64Enum>(new ASNumber(2.0)));
        }

        [Test]
        public void UInt64Enum_FromASString()
        {
            Assert.AreEqual(UInt64Enum.Zero, ToNative<UInt64Enum>(new ASString("0")));
            Assert.AreEqual(UInt64Enum.Max, ToNative<UInt64Enum>(new ASString(ulong.MaxValue.ToString())));
            Assert.AreEqual((UInt64Enum) 2, ToNative<UInt64Enum>(new ASString("2")));
            Assert.AreEqual(UInt64Enum.Zero, ToNative<UInt64Enum>(new ASString("Zero")));
            Assert.AreEqual(UInt64Enum.Big, ToNative<UInt64Enum>(new ASString("Big")));
            Assert.AreEqual(UInt64Enum.Max, ToNative<UInt64Enum>(new ASString("Max")));
        }

        [Test]
        public void UInt64Enum_ToASValue()
        {
            Assert.AreEqual(new ASInt29(0), ToASValue(UInt64Enum.Zero));
            Assert.AreEqual(new ASInt29(2), ToASValue((UInt64Enum) 2));
            Assert.AreEqual(new ASNumber(0xffffffff00000000), ToASValue(UInt64Enum.Big));
        }

        [Test]
        [ExpectedException(typeof(ActionScriptException))]
        public void UInt64Enum_ToASValue_LossOfPrecision()
        {
            ToASValue((UInt64Enum) 0xffffffffffffffffL);
        }

        [Test]
        public void Int64Enum_FromASInt29()
        {
            Assert.AreEqual(Int64Enum.Zero, ToNative<Int64Enum>(new ASInt29(0)));
            Assert.AreEqual(Int64Enum.MinusOne, ToNative<Int64Enum>(new ASInt29(-1)));
            Assert.AreEqual((Int64Enum)2, ToNative<Int64Enum>(new ASInt29(2)));
        }

        [Test]
        public void Int64Enum_FromASNumber()
        {
            Assert.AreEqual(Int64Enum.Zero, ToNative<Int64Enum>(new ASNumber(0.0)));
            Assert.AreEqual(Int64Enum.MinusOne, ToNative<Int64Enum>(new ASNumber(-1.0)));
            Assert.AreEqual(Int64Enum.Big, ToNative<Int64Enum>(new ASNumber(0x7fffffff00000000L)));
            Assert.AreEqual((Int64Enum)2, ToNative<Int64Enum>(new ASNumber(2.0)));
        }

        [Test]
        public void Int64Enum_FromASString()
        {
            Assert.AreEqual(Int64Enum.Zero, ToNative<Int64Enum>(new ASString("0")));
            Assert.AreEqual(Int64Enum.Max, ToNative<Int64Enum>(new ASString(long.MaxValue.ToString())));
            Assert.AreEqual(Int64Enum.Min, ToNative<Int64Enum>(new ASString(long.MinValue.ToString())));
            Assert.AreEqual((Int64Enum)2, ToNative<Int64Enum>(new ASString("2")));
            Assert.AreEqual(Int64Enum.Zero, ToNative<Int64Enum>(new ASString("Zero")));
            Assert.AreEqual(Int64Enum.Max, ToNative<Int64Enum>(new ASString("Max")));
            Assert.AreEqual(Int64Enum.Min, ToNative<Int64Enum>(new ASString("Min")));
        }

        [Test]
        public void Int64Enum_ToASValue()
        {
            Assert.AreEqual(new ASInt29(0), ToASValue(Int64Enum.Zero));
            Assert.AreEqual(new ASInt29(2), ToASValue((Int64Enum)2));
            Assert.AreEqual(new ASNumber(0x7fffffff00000000L), ToASValue(Int64Enum.Big));
        }

        [Test]
        [ExpectedException(typeof(ActionScriptException))]
        public void Int64Enum_ToASValue_LossOfPrecisionOnNegativeValue()
        {
            // Important, we use this number rather than 0x8000000000000000
            // because it has 63 significant bits vs. 1 when encoded in
            // floating point form.
            ToASValue((Int64Enum) (- 0x7fffffffffffffffL));
        }

        [Test]
        [ExpectedException(typeof(ActionScriptException))]
        public void Int64Enum_ToASValue_LossOfPrecisionOnPositiveValue()
        {
            ToASValue((Int64Enum) 0x7fffffffffffffffL);
        }

        [Test]
        public void Int16_FromASInt29()
        {
            Assert.AreEqual(42, ToNative<Int16>(new ASInt29(42)));
        }

        [Test]
        public void Int16_FromASNumber()
        {
            Assert.AreEqual(42, ToNative<Int16>(new ASNumber(42.0)));
        }

        [Test]
        public void Int16_FromASString()
        {
            Assert.AreEqual(42, ToNative<Int16>(new ASString("42")));
        }

        [Test]
        public void Int16_ToASValue()
        {
            Assert.AreEqual(new ASInt29(42), ToASValue((Int16)42));
        }

        [Test]
        public void Int32_FromASInt29()
        {
            Assert.AreEqual(42, ToNative<Int32>(new ASInt29(42)));
        }

        [Test]
        public void Int32_FromASNumber()
        {
            Assert.AreEqual(42, ToNative<Int32>(new ASNumber(42.0)));
        }

        [Test]
        public void Int32_FromASString()
        {
            Assert.AreEqual(42, ToNative<Int32>(new ASString("42")));
        }

        [Test]
        public void Int32_ToASValue()
        {
            Assert.AreEqual(new ASInt29(42), ToASValue((Int32) 42));
            Assert.AreEqual(new ASInt29(ASConstants.Int29MaxValue), ToASValue((Int32) ASConstants.Int29MaxValue));
            Assert.AreEqual(new ASInt29(ASConstants.Int29MinValue), ToASValue((Int32) ASConstants.Int29MinValue));
            Assert.AreEqual(new ASNumber(Int32.MaxValue), ToASValue(Int32.MaxValue));
            Assert.AreEqual(new ASNumber(Int32.MinValue), ToASValue(Int32.MinValue));
        }

        [Test]
        public void Int64_FromASInt29()
        {
            Assert.AreEqual(42, ToNative<Int64>(new ASInt29(42)));
        }

        [Test]
        public void Int64_FromASNumber()
        {
            Assert.AreEqual(42, ToNative<Int64>(new ASNumber(42.0)));
        }

        [Test]
        public void Int64_FromASString()
        {
            Assert.AreEqual(42, ToNative<Int64>(new ASString("42")));
        }

        [Test]
        public void Int64_ToASValue()
        {
            Assert.AreEqual(new ASInt29(42), ToASValue((Int64)42));
            Assert.AreEqual(new ASInt29(ASConstants.Int29MaxValue), ToASValue((Int64)ASConstants.Int29MaxValue));
            Assert.AreEqual(new ASInt29(ASConstants.Int29MinValue), ToASValue((Int64)ASConstants.Int29MinValue));
            Assert.AreEqual(new ASNumber(Int64.MaxValue), ToASValue(Int64.MaxValue));
            Assert.AreEqual(new ASNumber(Int64.MinValue), ToASValue(Int64.MinValue));
        }

        [Test]
        public void SByte_FromASInt29()
        {
            Assert.AreEqual(-42, ToNative<SByte>(new ASInt29(-42)));
        }

        [Test]
        public void SByte_FromASNumber()
        {
            Assert.AreEqual(-42, ToNative<SByte>(new ASNumber(-42.0)));
        }

        [Test]
        public void SByte_FromASString()
        {
            Assert.AreEqual(-42, ToNative<SByte>(new ASString("-42")));
        }

        [Test]
        public void SByte_ToASValue()
        {
            Assert.AreEqual(new ASInt29(-42), ToASValue((sbyte)-42));
        }

        [Test]
        public void Single_FromASInt29()
        {
            Assert.AreEqual(42.0f, ToNative<Single>(new ASInt29(42)));
        }

        [Test]
        public void Single_FromASNumber()
        {
            Assert.AreEqual(42.5f, ToNative<Single>(new ASNumber(42.5)));
        }

        [Test]
        public void Single_FromASString()
        {
            Assert.AreEqual(42.5f, ToNative<Single>(new ASString("42.5")));
        }

        [Test]
        public void Single_ToASValue()
        {
            Assert.AreEqual(new ASNumber(42.5), ToASValue(42.5f));
        }

        [Test]
        public void String_FromASArray()
        {
            Assert.AreEqual("Array", ToNative<String>(new ASArray(5)));
        }

        [Test]
        public void String_FromASBoolean()
        {
            Assert.AreEqual("true", ToNative<String>(ASBoolean.True));
            Assert.AreEqual("false", ToNative<String>(ASBoolean.False));
        }

        [Test]
        public void String_FromASByteArray()
        {
            Assert.AreEqual("ByteArray", ToNative<String>(new ASByteArray(new byte[5])));
        }

        [Test]
        public void String_FromASDate()
        {
            Assert.AreEqual("1970/02/03 14:22:11.123", ToNative<String>(new ASDate(new DateTime(1970, 2, 3, 14, 22, 11, 123, DateTimeKind.Utc))));
        }

        [Test]
        public void String_FromASInt29()
        {
            Assert.AreEqual("42", ToNative<String>(new ASInt29(42)));
        }

        [Test]
        public void String_FromASNull()
        {
            Assert.IsNull(ToNative<String>(null));
            Assert.IsNull(ToNative<String>(ASNull.Value));
        }
        
        [Test]
        public void String_FromASNumber()
        {
            Assert.AreEqual("42.5", ToNative<String>(new ASNumber(42.5)));
        }

        [Test]
        public void String_FromASObject()
        {
            Assert.AreEqual("Object", ToNative<String>(new ASObject()));
        }

        [Test]
        public void String_FromASString()
        {
            Assert.AreEqual("This is a string.", ToNative<String>(new ASString("This is a string.")));
        }

        [Test]
        public void String_FromASUndefined()
        {
            Assert.AreEqual("undefined", ToNative<String>(ASUndefined.Value));
        }

        [Test]
        public void String_FromASUnsupported()
        {
            Assert.AreEqual("unsupported", ToNative<String>(ASUnsupported.Value));
        }

        [Test]
        public void String_FromASXml()
        {
            Assert.AreEqual("<root />", ToNative<String>(new ASXmlDocument("<root />")));
        }

        [Test]
        public void String_ToASValue()
        {
            Assert.AreEqual(new ASString("This is a string."), ToASValue("This is a string."));
        }

        [Test]
        public void UInt16_FromASInt29()
        {
            Assert.AreEqual(42, ToNative<UInt16>(new ASInt29(42)));
        }

        [Test]
        public void UInt16_FromASNumber()
        {
            Assert.AreEqual(42, ToNative<UInt16>(new ASNumber(42.0)));
        }

        [Test]
        public void UInt16_FromASString()
        {
            Assert.AreEqual(42, ToNative<UInt16>(new ASString("42")));
        }

        [Test]
        public void UInt16_ToASValue()
        {
            Assert.AreEqual(new ASInt29(42), ToASValue((UInt16)42));
        }

        [Test]
        public void UInt32_FromASInt29()
        {
            Assert.AreEqual(42, ToNative<UInt32>(new ASInt29(42)));
        }

        [Test]
        public void UInt32_FromASNumber()
        {
            Assert.AreEqual(42, ToNative<UInt32>(new ASNumber(42.0)));
        }

        [Test]
        public void UInt32_FromASString()
        {
            Assert.AreEqual(42, ToNative<UInt32>(new ASString("42")));
        }

        [Test]
        public void UInt32_ToASValue()
        {
            Assert.AreEqual(new ASInt29(42), ToASValue((UInt32)42));
            Assert.AreEqual(new ASInt29(ASConstants.Int29MaxValue), ToASValue((UInt32)ASConstants.Int29MaxValue));
            Assert.AreEqual(new ASNumber(UInt32.MaxValue), ToASValue(UInt32.MaxValue));
        }

        [Test]
        public void UInt64_FromASInt29()
        {
            Assert.AreEqual(42, ToNative<UInt64>(new ASInt29(42)));
        }

        [Test]
        public void UInt64_FromASNumber()
        {
            Assert.AreEqual(42, ToNative<UInt64>(new ASNumber(42.0)));
        }

        [Test]
        public void UInt64_FromASString()
        {
            Assert.AreEqual(42, ToNative<UInt64>(new ASString("42")));
        }

        [Test]
        public void UInt64_ToASValue()
        {
            Assert.AreEqual(new ASInt29(42), ToASValue((UInt64)42));
            Assert.AreEqual(new ASInt29(ASConstants.Int29MaxValue), ToASValue((UInt64)ASConstants.Int29MaxValue));
            Assert.AreEqual(new ASNumber(UInt64.MaxValue), ToASValue(UInt64.MaxValue));
        }

        private enum Int32Enum
        {
            Zero = 0,
            One = 1
        }
        private enum Int64Enum : long
        {
            Zero = 0,
            MinusOne = -1,
            Min = long.MinValue,
            Big = 0x7fffffff00000000, // msb is set which will show any sign
            // problems during conversion from numbers but the value is not
            // so precise as to lose precision during conversion to doubles
            Max = long.MaxValue
        }

        private enum UInt64Enum : ulong
        {
            Zero = 0,
            Big = 0xffffffff00000000, // msb is set which will show any sign
            // problems during conversion from numbers but the value is not
            // so precise as to lose precision during conversion to doubles
            Max = ulong.MaxValue
        }
    }
}
