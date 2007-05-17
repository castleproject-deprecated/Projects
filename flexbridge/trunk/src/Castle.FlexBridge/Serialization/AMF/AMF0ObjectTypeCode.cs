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
    /// AMF0 data type codes.
    /// These code bytes appear as prefixes in the serialized data.
    /// </summary>
    internal enum AMF0ObjectTypeCode : byte
    {
        Number = 0x00,
        Boolean = 0x01,
        ShortString = 0x02,
        Object = 0x03,
        MovieClip = 0x04,
        Null = 0x05,
        Undefined = 0x06,
        Reference = 0x07,
        MixedArray = 0x08,
        EndOfObject = 0x09,
        Array = 0x0a,
        Date = 0x0b,
        LongString = 0x0c,
        Unsupported = 0x0d,
        RecordSet = 0x0e,
        Xml = 0x0f,
        TypedObject = 0x10,
        AMF3Data = 0x11
    }
}