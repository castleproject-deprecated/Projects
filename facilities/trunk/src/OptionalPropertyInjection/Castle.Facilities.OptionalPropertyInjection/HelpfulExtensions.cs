using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace System {
    internal static class StringExtensions {
        private static Action<string> WriteAction = s => System.Diagnostics.Debug.WriteLine(s);
        public static void Output(this string str) {
            WriteAction(str);
        }
        public static string Use(this string unformatted, params object[] args) {
            return String.Format(unformatted, args);
        }
        public static string Join(this IEnumerable<string> strings, string separator) {
            return String.Join(separator, strings.ToArray());
        }
        public static bool IsEmpty(this string str) {
            return String.IsNullOrEmpty(str);
        }
    }

    internal static class HelpfulExtensions {
        public static List<T> AsList<T>(this T obj) {
            if (typeof(IEnumerable<T>).IsAssignableFrom(obj.GetType())) return (obj as IEnumerable<T>).ToList();
            return new List<T> { obj };
        }
        public static bool IsNull(this object obj) { return object.ReferenceEquals(null, obj); }
        public static bool IsNotNull(this object obj) { return !obj.IsNull(); }

        public static void DoIfNotNull<T>(this T obj, Action<T> action) { if (!obj.IsNull()) action(obj); }

        public static R IfNotNull<T, R>(this T obj, Func<T, R> func) {
            if (obj.IsNull()) return default(R);
            return func(obj);
        }

        public static T IfNullInitialize<T>(this T obj, Action<T> action) {
            if (!obj.IsNull()) action(obj);
            if (obj.IsNull()) throw new InvalidOperationException("Preformed initialization but instance is still null");
            return obj;
        }
        public static string ToSafeString(this object obj) { return obj.IsNull() ? string.Empty : obj.ToString(); }
        public static IList<VALUE> Flatten<KEY, VALUE>(this IDictionary<KEY, IList<VALUE>> hash) {
            return hash.Values.Aggregate((list, node) => list.Concat(node).ToList());
        }
        public static void AddIfNew<VALUE>(this ICollection<VALUE> list, VALUE item) {
            if (!list.Contains(item)) list.Add(item);
        }
        public static string MemberName<T, R>(this T obj, Expression<Func<T, R>> expresion) {
            if (expresion.Body is MemberExpression)
                return (expresion.Body as MemberExpression).Member.Name;
            if (expresion.Body is MethodCallExpression)
                return (expresion.Body as MethodCallExpression).Method.Name;
            throw new InvalidOperationException("Cannot derive member name");
        }
    }
}
