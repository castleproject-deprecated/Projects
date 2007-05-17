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

namespace Castle.FlexBridge.Serialization.AMF
{
    /// <summary>
    /// AMF3 data type codes.
    /// These code bytes appear as prefixes in the serialized data.
    /// </summary>
    internal enum AMF3ObjectTypeCode : byte
    {
        Undefined = 0x00,
        Null = 0x01,
        False = 0x02,
        True = 0x03,
        Integer = 0x04,
        Number = 0x05,
        String = 0x06,
        Xml = 0x07,
        Date = 0x08,
        Array = 0x09,
        Object = 0x0a,
        XmlString = 0x0b,
        ByteArray = 0x0c
    }
}