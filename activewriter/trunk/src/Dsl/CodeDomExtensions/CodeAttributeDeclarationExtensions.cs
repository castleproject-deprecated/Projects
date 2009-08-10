using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altinoren.ActiveWriter.CodeDomExtensions
{
    public static class CodeAttributeDeclarationExtensions
    {
        public static CodeAttributeDeclaration Clone(this CodeAttributeDeclaration attribute)
        {
            if (attribute == null) return null;
            CodeAttributeDeclaration a = new CodeAttributeDeclaration(attribute.AttributeType.Clone());
            a.Arguments.AddRange(attribute.Arguments.Clone());
            a.Name = attribute.Name;
            return a;
        }

        public static void ReplaceType(this CodeAttributeDeclaration attribute, string oldType, string newType)
        {
            if (attribute == null) return;
            attribute.Arguments.ReplaceType(oldType, newType);
            attribute.AttributeType.ReplaceType(oldType, newType);
        }
    }
}
