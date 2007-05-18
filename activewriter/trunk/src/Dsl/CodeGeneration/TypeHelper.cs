// Copyright 2006 Gokhan Altinoren - http://altinoren.com/
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

namespace Altinoren.ActiveWriter.CodeGeneration
{
    using System;
    using System.CodeDom;
    using System.Globalization;

    public static class TypeHelper
    {
        public static bool IsNullable(NHibernateType type)
        {
            switch (type)
            {
                case NHibernateType.Int32:
                case NHibernateType.Boolean:
                case NHibernateType.TrueFalse:
                case NHibernateType.YesNo:
                case NHibernateType.DateTime:
                case NHibernateType.Ticks:
                case NHibernateType.Timestamp:
                case NHibernateType.Decimal:
                case NHibernateType.Double:
                case NHibernateType.Int16:
                case NHibernateType.Int64:
                case NHibernateType.Single:
                case NHibernateType.Byte:
                case NHibernateType.Guid:
                    return true;
                default:
                    return false;
            }
        }

        public static CodeTypeReference GetNullableTypeReferenceForHelper(NHibernateType type)
        {
            switch (type)
            {
                case NHibernateType.Int32:
                    return new CodeTypeReference("NullableInt32");
                case NHibernateType.Boolean:
                case NHibernateType.TrueFalse:
                case NHibernateType.YesNo:
                    return new CodeTypeReference("NullableBoolean");
                case NHibernateType.DateTime:
                case NHibernateType.Ticks:
                case NHibernateType.Timestamp:
                    return new CodeTypeReference("NullableDateTime");
                case NHibernateType.Decimal:
                    return new CodeTypeReference("NullableDecimal");
                case NHibernateType.Double:
                    return new CodeTypeReference("NullableDouble");
                case NHibernateType.Int16:
                    return new CodeTypeReference("NullableInt16");
                case NHibernateType.Int64:
                    return new CodeTypeReference("NullableInt64");
                case NHibernateType.Single:
                    return new CodeTypeReference("NullableSingle");
                case NHibernateType.Byte:
                    return new CodeTypeReference("NullableByte");
                case NHibernateType.Guid:
                    return new CodeTypeReference("NullableGuid");
                default:
                    return new CodeTypeReference(GetSystemType(type));
            }
        }

        public static CodeTypeReference GetNullableTypeReference(NHibernateType type)
        {
            switch (type)
            {
                case NHibernateType.Int32:
                case NHibernateType.Boolean:
                case NHibernateType.TrueFalse:
                case NHibernateType.YesNo:
                case NHibernateType.DateTime:
                case NHibernateType.Ticks:
                case NHibernateType.Timestamp:
                case NHibernateType.Decimal:
                case NHibernateType.Double:
                case NHibernateType.Int16:
                case NHibernateType.Int64:
                case NHibernateType.Single:
                case NHibernateType.Byte:
                case NHibernateType.Guid:
                    return GetNullableTypeReference(GetSystemType(type));
                default:
                    return new CodeTypeReference(GetSystemType(type));
            }
        }

        public static CodeTypeReference GetNullableTypeReference(Type type)
        {
            CodeTypeReference reference = new CodeTypeReference("System.Nullable");
            reference.TypeArguments.Add(type);

            return reference;
        }

        public static Type GetSystemType(NHibernateType type)
        {
            switch (type)
            {
                // TODO: Combine and order most likely asc
                case NHibernateType.AnsiChar:
                case NHibernateType.Char:
                    return typeof(string);
                case NHibernateType.Boolean:
                    return typeof(Boolean);
                case NHibernateType.Byte:
                    return typeof(Byte);
                case NHibernateType.DateTime:
                    return typeof(DateTime);
                case NHibernateType.Decimal:
                    return typeof(Decimal);
                case NHibernateType.Double:
                    return typeof(Double);
                case NHibernateType.Guid:
                    return typeof(Guid);
                case NHibernateType.Int16:
                    return typeof(Int16);
                case NHibernateType.Int32:
                    return typeof(Int32);
                case NHibernateType.Int64:
                    return typeof(Int64);
                case NHibernateType.Single:
                    return typeof(Single);
                case NHibernateType.Ticks:
                    return typeof(DateTime);
                case NHibernateType.TimeSpan:
                    return typeof(TimeSpan);
                case NHibernateType.Timestamp:
                    return typeof(DateTime);
                case NHibernateType.TrueFalse:
                    return typeof(Boolean);
                case NHibernateType.YesNo:
                    return typeof(Boolean);
                case NHibernateType.AnsiString:
                    return typeof(String);
                case NHibernateType.CultureInfo:
                    return typeof(CultureInfo);
                case NHibernateType.Binary:
                    return typeof(Byte[]);
                case NHibernateType.Type:
                    return typeof(Type);
                case NHibernateType.String:
                    return typeof(String);
                case NHibernateType.StringClob:
                    return typeof(String);
                case NHibernateType.BinaryBlob:
                    return typeof(Byte[]);
                default:
                    throw new ArgumentException("Unknown NHibernate type", type.ToString());
            }
        }
    }
}
