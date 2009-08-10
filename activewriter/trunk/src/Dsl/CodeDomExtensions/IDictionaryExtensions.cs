using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class IDictionaryExtensions
    {
        public static void AddRange(this IDictionary toDictionary, IDictionary fromDictionary)
        {
            foreach (var key in fromDictionary.Keys)
                toDictionary[key] = fromDictionary[key];
        }
    }
}
