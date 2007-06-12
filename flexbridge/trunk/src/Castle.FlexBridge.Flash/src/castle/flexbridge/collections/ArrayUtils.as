package castle.flexbridge.collections
{
	/**
	 * Provides utility functions for manipulating arrays.
	 */
	public final class ArrayUtils
	{
		/**
		 * Applies the converter function to each element of the input
		 * array and stores its results in the same order in an output
		 * array of the same length.
		 * 
		 * Correctly handles sparse arrays.
		 * 
		 * @param input The input array.
		 * @param converter The converter function with signature function(elem:Object):Object.
		 * @return The Output array.
		 */
		public static function convertAll(input:Array, converter:Function):Array
		{
			var output:Array = new Array(input.length);
			
			for (var indexProp:* in input)
				output[indexProp] = converter(input[indexProp]);
				
			return output;
		}
		
		/**
		 * Pushes an array of values onto the end of an array.
		 * 
		 * @param array The array to push into.
		 * @param value The array of values to push.
		 * @return The new length of the array.
		 */
		public static function pushArray(array:Array, values:Array):Number
		{
			var startIndex:int = array.length;
			var valueCount:int = values.length;
			
			// Optimization for small value arrays.
			if (valueCount == 0)
				return startIndex;
			else if (valueCount == 1)
				return array.push(values[0]);
			
			// Increase the size of the array and copy the value.
			var newLength:int = startIndex + valueCount;
			
			array.length = newLength;
			
			for (var i:int = 0; i < valueCount; i++)
				array[startIndex + i] = values[i];
				
			return newLength;
		}
		
		/**
		 * Equivalent to Array.splice but the values are specified as an array
		 * rather than as a variable argument list.
		 * 
		 * Correctly handles sparse arrays.
		 * 
		 * @param array The array to splice.
		 * @param startIndex The index of the element in the array where insertion
		 *                   or deletion begins.  If negative, specifies a position
		 *                   relative to the end of the array.
		 * @param deleteCount The number of elements to remove beginning at startIndex.
		 * @param values The array of values to insert at startIndex.
		 * @return The array of values that were removed from the array.
		 */
		public static function spliceArray(array:Array, startIndex:int, deleteCount:int, values:Array):Array
		{
			var valueCount:int = values.length;
			
			// Optimization for small value arrays.
			if (valueCount == 0)
				return array.splice(startIndex, deleteCount);
			else if (valueCount == 1)
				return array.splice(startIndex, deleteCount, values[0]);

			// Handle negative index.
			var arrayLength:int = array.length;
			if (startIndex < 0)
				startIndex += arrayLength;
			
			// Remove values.
			var removedValues:Array = array.slice(startIndex, startIndex + deleteCount);
			
			// Increase the size of the array and copy the values.
			array.length = arrayLength - deleteCount + valueCount;
			for (var i:int = 0; i < valueCount; i++)
			{
				if (i in values)
					array[startIndex + i] = values[i];
			}
			
			return removedValues;
		}
		
		/**
		 * Inserts an array of values at a given position into an array.
		 * 
		 * @param array The array to insert into.
		 * @param startIndex The index of the element in the array where insertion begins.
		 * @param values The array of values to insert at startIndex.
		 */
		public static function insertArray(array:Array, startIndex:int, values:Array):void
		{
			spliceArray(array, startIndex, 0, values);
		}
		
		/**
		 * Moves elements of the source array into the target array.
		 * The moved elements are deleted from the source array without changing
		 * its length (since arrays are sparse).  Correctly handles the case of moving
		 * elements among intersecting regions of the same array.
		 */
		public static function move(sourceArray:Array, sourceStartIndex:int,
			targetArray:Array, targetStartIndex:int, count:int):void
		{
			var i:int;
			var sourceIndex:int;
			
			if (sourceArray == targetArray)
			{
				if (sourceStartIndex == targetStartIndex)
				{
					// Regions overlap exactly in the same array so nothing to do.
					return;
				}
				else if (sourceStartIndex < targetStartIndex)
				{
					// Source region may overlap beginning of target region so
					// move elements from back to front to avoid interference.
					for (i = count - 1; i >= 0; i--)
					{
						sourceIndex = i + sourceStartIndex;
						
						if (sourceIndex in sourceArray)
						{
							targetArray[targetStartIndex + i] = sourceArray[sourceStartIndex + i];
							delete sourceArray[sourceStartIndex + i];
						}
					}
					
					return;
				}
			}
			
			// Otherwise move elements from front to back.
			for (i = 0; i < count; i++)
			{
				sourceIndex = i + sourceStartIndex;
				
				if (sourceIndex in sourceArray)
				{
					targetArray[targetStartIndex + i] = sourceArray[sourceStartIndex + i];
					delete sourceArray[sourceStartIndex + i];
				}
			}
		}
		
		/**
		 * Replaces all occurrences of the old value with the new value in an array.
		 * 
		 * @param array The array within which to perform the replacement.
		 * @param oldValue The old value to replace.
		 * @param newValue The new value to use as a replacement.
		 */
		public static function replace(array:Array, oldValue:*, newValue:*):void
		{
			var count:int = array.length;
			
			for (var i:int = 0; i < count; i++)
			{
				if (array[i] == oldValue)
					array[i] = newValue;
			}
		}
		
		/**
		 * Removes the first occurrence of the specified value from the array.
		 * Does nothing if the value was not found.
		 * 
		 * @param array The array from which to remove the value.
		 * @param value The value to remove.
		 */
		public static function remove(array:Array, value:*):void
		{
			var index:int = array.indexOf(value);
			if (index >= 0)
				array.splice(index, 1);
		}
		
		/**
		 * Finds the index of the first element of the array that matches the predicate.
		 * 
		 * @param array The array to search.
		 * @param predicate The predicate function to apply with signature
		 *   function(item:*):Boolean
		 * @return The index of the item found, or -1 if none.
		 */
		public static function find(array:Array, predicate:Function):int
		{
			var count:int = array.length;
			
			for (var i:int = 0; i < count; i++)
				if (predicate(array[i]))
					return i;
			
			return -1;
		}
	}
}