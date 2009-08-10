using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeAttributeArgumentExtensions
    {
        public static CodeAttributeArgument Clone(this CodeAttributeArgument argument)
        {
            if (argument == null) return null;
            CodeAttributeArgument a = new CodeAttributeArgument();
            a.Name = argument.Name;
            a.Value = argument.Value.Clone();
            return a;
        }

        public static void ReplaceType(this CodeAttributeArgument argument, string oldType, string newType)
        {
            if (argument == null) return;
            argument.Value.ReplaceType(oldType, newType);
        }
    }
}
