package castle.flexbridge.collections
{
	import mx.collections.IList;
	import mx.collections.ICollectionView;
	import mx.collections.ArrayCollection;
	import flash.utils.getQualifiedClassName;
	import mx.collections.IViewCursor;
	import mx.collections.CursorBookmark;
	import flash.errors.IllegalOperationError;
	
	/**
	 * Provides utility functions for manipulating ILists.
	 */
	public final class ListUtils
	{
		/**
		 * Adds all of the items from the array to the list.
		 */
		public static function addAll(list:IList, items:Array):void
		{
			for each (var item:Object in items)
				list.addItem(item);
		}
		
		/**
		 * Adapts collections of various kinds to an IList.
		 */
		public static function toList(collection:*):IList
		{
			var array:Array = collection as Array;
			if (array != null)
				return new ArrayCollection(array);
			
			var list:IList = collection as IList;
			if (list != null)
				return list;
				
			var collectionView:ICollectionView = collection as ICollectionView;
			if (collectionView != null)
			{
				var cursor:IViewCursor = collectionView.createCursor();
				cursor.seek(CursorBookmark.FIRST, -1);
				
				list = new ArrayCollection();
				while (cursor.moveNext())
					list.addItem(cursor.current);
					
				return list;
			}
			
			if (collection == null)
				return null;
				
			throw new IllegalOperationError("Cannot convert collection of type "
				 + getQualifiedClassName(collection) + " to an IList");
		}
		
		/**
		 * Applies the converter function to each element of the input
		 * list and stores its results in the same order in an output
		 * array of the same length.
		 * 
		 * @param input The input list.
		 * @param converter The converter function with signature function(elem:Object):Object.
		 * @return The Output array.
		 */
		public static function convertAllToArray(input:IList, converter:Function):Array
		{
			var output:Array = new Array(input.length);
			
			for (var i:int = 0; i < input.length; i++)
				output[i] = converter(input.getItemAt(i));
				
			return output;
		}
	}
}