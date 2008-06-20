//using System;
//using System.Collections;
//using System.Collections.Generic;
	
//namespace Castle.Tools.QuerySpecBuilder.Helpers
//{
//    public static class EnumerableExtensions
//    {
//        public static IEnumerable<U> Select<T, U>(this IEnumerable<T> source, Func<T, U> selector)
//        {
//            foreach (var item in source)
//            {
//                yield return selector(item);
//            }
//        }

//        public static IEnumerable<string> Select(this ArrayList source, Func<object, string> selector)
//        {
//            foreach (var item in source)
//            {
//                yield return selector(item);
//            }
//        }
//    }
//}