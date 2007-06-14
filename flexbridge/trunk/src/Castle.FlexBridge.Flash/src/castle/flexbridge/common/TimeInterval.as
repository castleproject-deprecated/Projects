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

package castle.flexbridge.common
{
	/**
	 * A time interval represents the span between two Dates and times.
	 */
	public class TimeInterval
	{
		// ECMAScript spec says range is 100,000,000 days in before or after Jan 1 1970.
		private static const MIN_TICKS:Number = -100000000*86400;
		private static const MAX_TICKS:Number = 100000000*86400
		
		private var _startTime:Date;
		private var _endTime:Date;
		
		/**
		 * Creates a new time interval.
		 * 
		 * @param startTime The start time of the interval.
		 * @param endTime The end time of the interval, or null to make it
		 *     equal to the start time specified.
		 */
		public function TimeInterval(startTime:Date, endTime:Date = null)
		{
			if (! startTime)
				throw new ArgumentError("Start time must be non-null.");
			if (! endTime)
				endTime = startTime;
			
			_startTime = startTime;
			_endTime = endTime;
		}
		
		/**
		 * Gets the start time.
		 */
		public function get startTime():Date
		{
			return _startTime;
		}
		
		/**
		 * Sets the start time.
		 */
		public function set startTime(startTime:Date):void
		{
			if (! startTime)
				throw new ArgumentError("Start time must be non-null.");
			
			_startTime = startTime;
		}
		
		/**
		 * Gets the end time.
		 */
		public function get endTime():Date
		{
			return _endTime;
		}
		
		/**
		 * Sets the end time.
		 */
		public function set endTime(endTime:Date):void
		{
			if (! endTime)
				throw new ArgumentError("End time must be non-null.");
			
			_endTime = endTime;
		}
		
		/**
		 * Returns true if the interval represents all of time.
		 */
		public function get isEternity():Boolean
		{
			return _startTime.getTime() <= MIN_TICKS && _endTime.getTime() >= MAX_TICKS;
		}
		
		/**
		 * Returns true if the specified date is within the time interval.
		 * 
		 * @param date The date to check for containment within this time interval.
		 * @param excludeStartTime If true, excludes dates that exactly equal
		 *                         the start time.
		 * @param excludeEndTime If true, excludes dates that exactly equal
		 *                       the end time.
		 */
		public function contains(date:Date, excludeStartTime:Boolean = false, excludeEndTime:Boolean = false):Boolean
		{
			var startMillis:Number = _startTime.getTime();
			var endMillis:Number = _endTime.getTime();
			var dateMillis:Number = date.getTime();
			
			if (excludeStartTime && startMillis == dateMillis)
				return false;
			if (excludeEndTime && endMillis == dateMillis)
				return false;
			
			return startMillis <= dateMillis && endMillis >= dateMillis;
		}
		
		/**
		 * Gets a time interval that represents this instant.
		 */
		public static function now():TimeInterval
		{
			return new TimeInterval(new Date());
		}
		
		/**
		 * Gets a time interval that represents the widest possible timespan.
		 */
		public static function eternity():TimeInterval
		{
			return new TimeInterval(new Date(MIN_TICKS), new Date(MAX_TICKS));
		}
	}
}