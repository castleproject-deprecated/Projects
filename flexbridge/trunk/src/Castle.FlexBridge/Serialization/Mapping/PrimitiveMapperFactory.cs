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
using System.Globalization;
using System.Text;
using Castle.FlexBridge.ActionScript;

namespace Castle.FlexBridge.Serialization.Mapping
{
    /// <summary>
    /// Provides mappers for primitive types.
    /// Handles the following types:
    /// <list type="bullet">
    /// <item><see cref="ASUndefined"/></item>
    /// <item><see cref="ASUnsupported"/></item>
    /// <item><see cref="Boolean"/></item>
    /// <item><see cref="Byte" /></item>
    /// <item><see cref="Char" /></item>
    /// <item><see cref="DateTime" /></item>
    /// <item><see cref="DBNull" /></item>
    /// <item><see cref="Decimal" /></item>
    /// <item><see cref="Double" /></item>
    /// <item><see cref="Enum" /></item>
    /// <item><see cref="Int16" /></item>
    /// <item><see cref="Int32" /></item>
    /// <item><see cref="Int64" /></item>
    /// <item><see cref="SByte" /></item>
    /// <item><see cref="Single" /></item>
    /// <item><see cref="String" /></item>
    /// <item><see cref="UInt16" /></item>
    /// <item><see cref="UInt32" /></item>
    /// <item><see cref="UInt64" /></item>
    /// </list>
    /// </summary>
    public sealed class PrimitiveMapperFactory : BaseASMapperFactory
    {
        private List<PrimitiveMapper> mappers;

        /// <summary>
        /// Creates a primitive mapper factory.
        /// </summary>
        public PrimitiveMapperFactory()
        {
            mappers = new List<PrimitiveMapper>();

            RegisterMapper(new ASUndefinedMapper());
            RegisterMapper(new ASUnsupportedMapper());
            RegisterMapper(new BooleanMapper());
            RegisterMapper(new ByteMapper());
            RegisterMapper(new CharMapper());
            RegisterMapper(new DateTimeMapper());
            RegisterMapper(new DBNullMapper());
            RegisterMapper(new DecimalMapper());
            RegisterMapper(new DoubleMapper());
            RegisterMapper(new EnumMapper());
            RegisterMapper(new Int16Mapper());
            RegisterMapper(new Int32Mapper());
            RegisterMapper(new Int64Mapper());
            RegisterMapper(new SByteMapper());
            RegisterMapper(new SingleMapper());
            RegisterMapper(new StringMapper());
            RegisterMapper(new UInt16Mapper());
            RegisterMapper(new UInt32Mapper());
            RegisterMapper(new UInt64Mapper());
        }

        /// <inheritdoc />
        public override IASSourceMapper GetASSourceMapper(ASSourceMappingDescriptor descriptor)
        {
            foreach (PrimitiveMapper mapper in mappers)
                if (mapper.SupportsSourceMapping(descriptor))
                    return mapper;

            return null;
        }

        /// <inheritdoc />
        public override IASTargetMapper GetASTargetMapper(ASTargetMappingDescriptor descriptor)
        {
            foreach (PrimitiveMapper mapper in mappers)
                if (mapper.SupportsTargetMapping(descriptor))
                    return mapper;

            return null;
        }

        private void RegisterMapper(PrimitiveMapper mapper)
        {
            mappers.Add(mapper);
        }

        private abstract class PrimitiveMapper : BaseASMapper
        {
            private Type type;
            private ASTypeKind[] kinds;

            protected PrimitiveMapper(Type type, params ASTypeKind[] kinds)
            {
                this.type = type;
                this.kinds = kinds;
            }

            public virtual bool SupportsSourceMapping(ASSourceMappingDescriptor descriptor)
            {
                // Note: Use strict equality here because we don't want to use 
                //       most primitive mappings if the result type is underspecified.
                //       How would we know whether to map to "int" or "long"?  The mapping
                //       would be ambiguous!  Instead we handle this case with default native type
                //       mappings elsewhere.
                if (descriptor.TargetNativeType == type)
                {
                    foreach (ASTypeKind kind in kinds)
                        if (descriptor.SourceKind == kind)
                            return true;
                }

                return false;
            }

            public virtual bool SupportsTargetMapping(ASTargetMappingDescriptor descriptor)
            {
                if (type.IsAssignableFrom(descriptor.SourceNativeType))
                    return true;

                return false;
            }
        }

        private sealed class ASUndefinedMapper : PrimitiveMapper
        {
            public ASUndefinedMapper()
                : base(typeof(ASUndefined), ASTypeKind.Undefined)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                return ASUndefined.Value;
            }

            protected override object MapUndefinedToNative(IActionScriptSerializer serializer, Type nativeType)
            {
                return ASUndefined.Value;
            }
        }

        private sealed class ASUnsupportedMapper : PrimitiveMapper
        {
            public ASUnsupportedMapper()
                : base(typeof(ASUnsupported), ASTypeKind.Unsupported)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                return ASUnsupported.Value;
            }

            protected override object MapUnsupportedToNative(IActionScriptSerializer serializer, Type nativeType)
            {
                return ASUnsupported.Value;
            }
        }

        private sealed class BooleanMapper : PrimitiveMapper
        {
            public BooleanMapper()
                : base(typeof(Boolean), ASTypeKind.Int29, ASTypeKind.Number, ASTypeKind.String)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                Boolean value = (Boolean)nativeValue;
                return ASBoolean.ToASBoolean(value);
            }

            protected override object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
            {
                return Boolean.Parse(value);
            }

            protected override object MapInt29ToNative(IActionScriptSerializer serializer, Type nativeType, int value)
            {
                return value != 0;
            }

            protected override object MapNumberToNative(IActionScriptSerializer serializer, Type nativeType, double value)
            {
                return value != 0.0;
            }
        }

        private sealed class ByteMapper : PrimitiveMapper
        {
            public ByteMapper()
                : base(typeof(Byte), ASTypeKind.Int29, ASTypeKind.Number, ASTypeKind.String)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                Byte value = (Byte)nativeValue;
                return new ASInt29(value);
            }

            protected override object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
            {
                return Byte.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }

            protected override object MapInt29ToNative(IActionScriptSerializer serializer, Type nativeType, int value)
            {
                return (byte)value;
            }

            protected override object MapNumberToNative(IActionScriptSerializer serializer, Type nativeType, double value)
            {
                return (byte)value;
            }
        }

        private sealed class CharMapper : PrimitiveMapper
        {
            public CharMapper()
                : base(typeof(Char), ASTypeKind.Int29, ASTypeKind.Number, ASTypeKind.String)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                Char value = (Char)nativeValue;
                return new ASString(new string(value, 1));
            }

            protected override object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
            {
                if (value.Length != 1)
                    throw new ActionScriptException("Cannot map a string with length not equal to 1 to a Char.");

                return value[0];
            }

            protected override object MapInt29ToNative(IActionScriptSerializer serializer, Type nativeType, int value)
            {
                return (Char)value;
            }

            protected override object MapNumberToNative(IActionScriptSerializer serializer, Type nativeType, double value)
            {
                return (Char)value;
            }
        }

        private sealed class DateTimeMapper : PrimitiveMapper
        {
            public DateTimeMapper()
                : base(typeof(DateTime), ASTypeKind.Date, ASTypeKind.String)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                DateTime value = (DateTime)nativeValue;
                return new ASDate(value);
            }

            protected override object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
            {
                return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AdjustToUniversal);
            }

            protected override object MapDateToNative(IActionScriptSerializer serializer, Type nativeType, double millisecondsSinceEpoch, int timeZoneOffsetMinutes)
            {
                return ASDate.ToDateTime(millisecondsSinceEpoch, timeZoneOffsetMinutes);
            }
        }

        private sealed class DBNullMapper : PrimitiveMapper
        {
            public DBNullMapper()
                : base(typeof(DBNull), ASTypeKind.Null)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                return ASNull.Value;
            }

            protected override object MapNullToNative(IActionScriptSerializer serializer, Type nativeType)
            {
                return DBNull.Value;
            }
        }

        private sealed class DecimalMapper : PrimitiveMapper
        {
            public DecimalMapper()
                : base(typeof(Decimal), ASTypeKind.Int29, ASTypeKind.Number, ASTypeKind.String)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                Decimal value = (Decimal)nativeValue;
                return new ASNumber((double)value); // note: loss of precision may occur
            }

            protected override object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
            {
                return Decimal.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
            }

            protected override object MapInt29ToNative(IActionScriptSerializer serializer, Type nativeType, int value)
            {
                return (Decimal)value;
            }

            protected override object MapNumberToNative(IActionScriptSerializer serializer, Type nativeType, double value)
            {
                return (Decimal)value;
            }
        }

        private sealed class DoubleMapper : PrimitiveMapper
        {
            public DoubleMapper()
                : base(typeof(Double), ASTypeKind.Int29, ASTypeKind.Number, ASTypeKind.String)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                Double value = (Double)nativeValue;
                return new ASNumber(value);
            }

            protected override object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
            {
                return Double.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
            }

            protected override object MapInt29ToNative(IActionScriptSerializer serializer, Type nativeType, int value)
            {
                return (double)value;
            }

            protected override object MapNumberToNative(IActionScriptSerializer serializer, Type nativeType, double value)
            {
                return value;
            }
        }

        private sealed class EnumMapper : PrimitiveMapper
        {
            public EnumMapper()
                : base(typeof(Enum))
            {
            }

            public override bool SupportsSourceMapping(ASSourceMappingDescriptor descriptor)
            {
                ASTypeKind sourceKind = descriptor.SourceKind;
                return descriptor.TargetNativeType.IsEnum
                    && (sourceKind == ASTypeKind.Int29 || sourceKind == ASTypeKind.Number || sourceKind == ASTypeKind.String);
            }

            public override bool SupportsTargetMapping(ASTargetMappingDescriptor descriptor)
            {
                return descriptor.SourceNativeType.IsEnum;
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                Enum enumValue = (Enum)nativeValue;

                if (enumValue.GetTypeCode() == TypeCode.UInt64)
                {
                    ulong value = Convert.ToUInt64(enumValue);
                    if (value <= ASConstants.Int29MaxValue)
                        return new ASInt29(unchecked((int)value));

                    // Losing precision during conversion of 64bit enums is very bad.
                    // The wrong information will be received once the value has been munged through a double.
                    double doubleValue = value;
                    if ((ulong) doubleValue != value)
                        throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                            "Cannot serialize an enum with base type UInt64 to ActionScript because loss of precision will occur.  Enum value was '{0}'.",
                            enumValue));

                    return new ASNumber(doubleValue);
                }
                else
                {
                    long value = Convert.ToInt64(enumValue);
                    if (value >= ASConstants.Int29MinValue && value <= ASConstants.Int29MaxValue)
                        return new ASInt29(unchecked((int)value));

                    // Losing precision during conversion of 64bit enums is very bad.
                    // The wrong information will be received once the value has been munged through a double.
                    double doubleValue = value;
                    if ((long) doubleValue != value)
                        throw new ActionScriptException(String.Format(CultureInfo.CurrentCulture,
                            "Cannot serialize an enum with base type Int64 to ActionScript because loss of precision will occur.  Enum value was '{0}'.",
                            enumValue));

                    return new ASNumber(doubleValue);
                }
            }

            protected override object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
            {
                return Enum.Parse(nativeType, value, true);
            }

            protected override object MapInt29ToNative(IActionScriptSerializer serializer, Type nativeType, int value)
            {
                return Enum.ToObject(nativeType, value);
            }

            protected override object MapNumberToNative(IActionScriptSerializer serializer, Type nativeType, double value)
            {
                if (Enum.GetUnderlyingType(nativeType) == typeof(UInt64))
                    return Enum.ToObject(nativeType, unchecked((ulong)value)); // use unsigned long to ensure we don't drop the most significant bit
                else
                    return Enum.ToObject(nativeType, unchecked((long)value));
            }
        }

        private sealed class Int16Mapper : PrimitiveMapper
        {
            public Int16Mapper()
                : base(typeof(Int16), ASTypeKind.Int29, ASTypeKind.Number, ASTypeKind.String)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                Int16 value = (Int16)nativeValue;
                return new ASInt29(value);
            }

            protected override object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
            {
                return Int16.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }

            protected override object MapInt29ToNative(IActionScriptSerializer serializer, Type nativeType, int value)
            {
                return (Int16)value;
            }

            protected override object MapNumberToNative(IActionScriptSerializer serializer, Type nativeType, double value)
            {
                return (Int16)value;
            }
        }

        private sealed class Int32Mapper : PrimitiveMapper
        {
            public Int32Mapper()
                : base(typeof(Int32), ASTypeKind.Int29, ASTypeKind.Number, ASTypeKind.String)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                Int32 value = (Int32)nativeValue;
                if (value >= ASConstants.Int29MinValue && value <= ASConstants.Int29MaxValue)
                    return new ASInt29(value);

                return new ASNumber(value);
            }

            protected override object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
            {
                return Int32.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }

            protected override object MapInt29ToNative(IActionScriptSerializer serializer, Type nativeType, int value)
            {
                return value;
            }

            protected override object MapNumberToNative(IActionScriptSerializer serializer, Type nativeType, double value)
            {
                return (int)value;
            }
        }

        private sealed class Int64Mapper : PrimitiveMapper
        {
            public Int64Mapper()
                : base(typeof(Int64), ASTypeKind.Int29, ASTypeKind.Number, ASTypeKind.String)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                Int64 value = (Int64)nativeValue;
                if (value >= ASConstants.Int29MinValue && value <= ASConstants.Int29MaxValue)
                    return new ASInt29(unchecked((int)value));

                return new ASNumber(value); // note: loss of precision may occur
            }

            protected override object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
            {
                return Int64.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }

            protected override object MapInt29ToNative(IActionScriptSerializer serializer, Type nativeType, int value)
            {
                return (Int64)value;
            }

            protected override object MapNumberToNative(IActionScriptSerializer serializer, Type nativeType, double value)
            {
                return (Int64)value;
            }
        }

        private sealed class SByteMapper : PrimitiveMapper
        {
            public SByteMapper()
                : base(typeof(SByte), ASTypeKind.Int29, ASTypeKind.Number, ASTypeKind.String)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                SByte value = (SByte)nativeValue;
                return new ASInt29(value);
            }

            protected override object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
            {
                return SByte.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }

            protected override object MapInt29ToNative(IActionScriptSerializer serializer, Type nativeType, int value)
            {
                return (SByte)value;
            }

            protected override object MapNumberToNative(IActionScriptSerializer serializer, Type nativeType, double value)
            {
                return (SByte)value;
            }
        }

        private sealed class SingleMapper : PrimitiveMapper
        {
            public SingleMapper()
                : base(typeof(Single), ASTypeKind.Int29, ASTypeKind.Number, ASTypeKind.String)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                Single value = (Single)nativeValue;
                return new ASNumber(value);
            }

            protected override object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
            {
                return Single.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
            }

            protected override object MapInt29ToNative(IActionScriptSerializer serializer, Type nativeType, int value)
            {
                return (Single)value;
            }

            protected override object MapNumberToNative(IActionScriptSerializer serializer, Type nativeType, double value)
            {
                return (Single)value;
            }
        }

        private sealed class StringMapper : PrimitiveMapper
        {
            public StringMapper()
                : base(typeof(String), ASTypeKind.Array, ASTypeKind.Boolean, ASTypeKind.ByteArray,
                    ASTypeKind.Date, ASTypeKind.Int29, ASTypeKind.Null, ASTypeKind.Number, ASTypeKind.Object,
                    ASTypeKind.Null, ASTypeKind.String, ASTypeKind.Undefined, ASTypeKind.Unsupported, ASTypeKind.Xml)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                string value = (string)nativeValue;
                return new ASString(value);
            }

            protected override object MapArrayToNative(IActionScriptSerializer serializer, Type nativeType, int indexedLength, IEnumerable<IASValue> indexedValues, IEnumerable<KeyValuePair<string, IASValue>> dynamicProperties)
            {
                // FIXME: do something smarter perhaps?
                return "Array";
            }

            protected override object MapBooleanToNative(IActionScriptSerializer serializer, Type nativeType, bool value)
            {
                return value ? "true" : "false";
            }

            protected override object MapByteArrayToNative(IActionScriptSerializer serializer, Type nativeType, int length, IEnumerable<ArraySegment<byte>> segments)
            {
                return "ByteArray";
            }

            protected override object MapDateToNative(IActionScriptSerializer serializer, Type nativeType, double millisecondsSinceEpoch, int timeZoneOffsetMinutes)
            {
                return String.Format(CultureInfo.InstalledUICulture, "{0:yyyy/MM/dd HH:mm:ss.fff}",
                    ASDate.ToDateTime(millisecondsSinceEpoch, timeZoneOffsetMinutes));
            }

            protected override object MapInt29ToNative(IActionScriptSerializer serializer, Type nativeType, int value)
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }

            protected override object MapNullToNative(IActionScriptSerializer serializer, Type nativeType)
            {
                return null;
            }

            protected override object MapNumberToNative(IActionScriptSerializer serializer, Type nativeType, double value)
            {
                return value.ToString(CultureInfo.InvariantCulture);
            }

            protected override object MapObjectToNative(IActionScriptSerializer serializer, Type nativeType, ASClass @class, IEnumerable<IASValue> memberValues, IEnumerable<KeyValuePair<string, IASValue>> dynamicProperties, IExternalizable externalizableValue)
            {
                return "Object";
            }

            protected override object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
            {
                return value;
            }

            protected override object MapUndefinedToNative(IActionScriptSerializer serializer, Type nativeType)
            {
                return "undefined";
            }

            protected override object MapUnsupportedToNative(IActionScriptSerializer serializer, Type nativeType)
            {
                return "unsupported";
            }

            protected override object MapXmlToNative(IActionScriptSerializer serializer, Type nativeType, string xmlString)
            {
                return xmlString;
            }
        }

        private sealed class UInt16Mapper : PrimitiveMapper
        {
            public UInt16Mapper()
                : base(typeof(UInt16), ASTypeKind.Int29, ASTypeKind.Number, ASTypeKind.String)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                UInt16 value = (UInt16)nativeValue;
                return new ASInt29(value);
            }

            protected override object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
            {
                return UInt16.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }

            protected override object MapInt29ToNative(IActionScriptSerializer serializer, Type nativeType, int value)
            {
                return (UInt16)value;
            }

            protected override object MapNumberToNative(IActionScriptSerializer serializer, Type nativeType, double value)
            {
                return (UInt16)value;
            }
        }

        private sealed class UInt32Mapper : PrimitiveMapper
        {
            public UInt32Mapper()
                : base(typeof(UInt32), ASTypeKind.Int29, ASTypeKind.Number, ASTypeKind.String)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                UInt32 value = (UInt32)nativeValue;
                if (value <= ASConstants.Int29MaxValue)
                    return new ASInt29(unchecked((int)value));

                return new ASNumber(value);
            }

            protected override object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
            {
                return UInt32.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }

            protected override object MapInt29ToNative(IActionScriptSerializer serializer, Type nativeType, int value)
            {
                return (UInt32)value;
            }

            protected override object MapNumberToNative(IActionScriptSerializer serializer, Type nativeType, double value)
            {
                return (UInt32)value;
            }
        }

        private sealed class UInt64Mapper : PrimitiveMapper
        {
            public UInt64Mapper()
                : base(typeof(UInt64), ASTypeKind.Int29, ASTypeKind.Number, ASTypeKind.String)
            {
            }

            public override IASValue ToASValue(IActionScriptSerializer serializer, object nativeValue)
            {
                UInt64 value = (UInt64)nativeValue;
                if (value <= ASConstants.Int29MaxValue)
                    return new ASInt29(unchecked((int)value));

                return new ASNumber(value); // note: loss of precision may occur
            }

            protected override object MapStringToNative(IActionScriptSerializer serializer, Type nativeType, string value)
            {
                return UInt64.Parse(value, NumberStyles.Integer, CultureInfo.InvariantCulture);
            }

            protected override object MapInt29ToNative(IActionScriptSerializer serializer, Type nativeType, int value)
            {
                return (UInt64)value;
            }

            protected override object MapNumberToNative(IActionScriptSerializer serializer, Type nativeType, double value)
            {
                return (UInt64)value;
            }
        }

    }
}
