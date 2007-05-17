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
using System.Xml;
using Castle.FlexBridge.Serialization;

namespace Castle.FlexBridge.ActionScript
{
    /// <summary>
    /// Adapts a <see cref="DateTime" /> for ActionScript serialization.
    /// </summary>
    public sealed class ASDate : BaseASValue
    {
        private double millisecondsSinceEpoch;
        private int timeZoneOffsetMinutes;

        /// <summary>
        /// Creates a date using the specified <see cref="DateTime" />.
        /// If the date's kind is not <see cref="DateTimeKind.Utc" /> then the local system
        /// timezone offset is used to populate <see cref="TimeZoneOffsetMinutes" />,
        /// otherwise an offset of 0 is recorded.
        /// </summary>
        /// <param name="dateTime">The date</param>
        public ASDate(DateTime dateTime)
        {
            this.millisecondsSinceEpoch = (dateTime - ASConstants.Epoch).TotalMilliseconds;

            if (dateTime.Kind != DateTimeKind.Utc)
            {
                this.timeZoneOffsetMinutes = (int)TimeZone.CurrentTimeZone.GetUtcOffset(dateTime).TotalMinutes;
                this.millisecondsSinceEpoch += timeZoneOffsetMinutes * 60000;
            }
        }

        /// <summary>
        /// Creates a date using the specified <see cref="DateTime" /> and timezone offset.
        /// </summary>
        /// <param name="dateTime">The date</param>
        /// <param name="timeZoneOffsetMinutes">The timezone offset from Utc in minutes</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeZoneOffsetMinutes"/>
        /// is less than -720 or greater than 720</exception>
        public ASDate(DateTime dateTime, int timeZoneOffsetMinutes)
        {
            if (timeZoneOffsetMinutes < -720 || timeZoneOffsetMinutes > 720)
                throw new ArgumentOutOfRangeException("timeZoneOffsetMinutes", "Time zone offset minutes must be between -720 and 720.");

            this.millisecondsSinceEpoch = (dateTime - ASConstants.Epoch).TotalMilliseconds;
            this.timeZoneOffsetMinutes = timeZoneOffsetMinutes;
        }

        /// <summary>
        /// Creates a date specified as number of milliseconds since the Epoch and a timezone offset.
        /// </summary>
        /// <param name="millisecondsSinceEpoch">The number of milliseconds since <see cref="ASConstants.Epoch" /></param>
        /// <param name="timeZoneOffsetMinutes">The timezone offset from Utc in minutes</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="timeZoneOffsetMinutes"/>
        /// is less than -720 or greater than 720</exception>
        public ASDate(double millisecondsSinceEpoch, int timeZoneOffsetMinutes)
        {
            if (timeZoneOffsetMinutes < -720 || timeZoneOffsetMinutes > 720)
                throw new ArgumentOutOfRangeException("timeZoneOffsetMinutes", "Time zone offset minutes must be between -720 and 720.");

            this.millisecondsSinceEpoch = millisecondsSinceEpoch;
            this.timeZoneOffsetMinutes = timeZoneOffsetMinutes;
        }

        /// <summary>
        /// Converts the specified date value to a <see cref="DateTime" />.
        /// </summary>
        /// <param name="millisecondsSinceEpoch">The number of milliseconds since <see cref="ASConstants.Epoch" /></param>
        /// <param name="timeZoneOffsetMinutes">The timezone offset from Utc in minutes</param>
        /// <returns></returns>
        public static DateTime ToDateTime(double millisecondsSinceEpoch, int timeZoneOffsetMinutes)
        {
            return ASConstants.Epoch.AddMilliseconds(millisecondsSinceEpoch).AddMinutes(-timeZoneOffsetMinutes);
        }

        /// <summary>
        /// Gets the number of milliseconds since <see cref="ASConstants.Epoch" />.
        /// </summary>
        public double MillisecondsSinceEpoch
        {
            get { return millisecondsSinceEpoch; }
        }

        /// <summary>
        /// Gets the timezone offset from Utc in minutes.
        /// </summary>
        public int TimeZoneOffsetMinutes
        {
            get { return timeZoneOffsetMinutes; }
        }

        /// <summary>
        /// Gets an equivalent <see cref="DateTime" /> value in Utc after compensating for
        /// the time zone offset if any.
        /// </summary>
        /// <returns>The equivalent date/time</returns>
        public DateTime ToDateTime()
        {
            return ToDateTime(millisecondsSinceEpoch, timeZoneOffsetMinutes);
        }

        public override int GetHashCode()
        {
            return millisecondsSinceEpoch.GetHashCode() ^ timeZoneOffsetMinutes;
        }

        public override bool Equals(object obj)
        {
            ASDate other = obj as ASDate;
            return other != null && other.millisecondsSinceEpoch == millisecondsSinceEpoch
                && other.timeZoneOffsetMinutes == timeZoneOffsetMinutes;
        }

        public override ASTypeKind Kind
        {
            get { return ASTypeKind.Date; }
        }

        public override void AcceptVisitor(IActionScriptSerializer serializer, IASValueVisitor visitor)
        {
            visitor.VisitDate(serializer, millisecondsSinceEpoch, timeZoneOffsetMinutes);
        }

        public override object GetNativeValue(Type nativeType)
        {
            return nativeType == typeof(DateTime) ? (object) ToDateTime() : null;
        }
    }
}