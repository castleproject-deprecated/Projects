// Copyright 2006 Gokhan Altinoren - http://altinoren.com/
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

namespace Altinoren.ActiveWriter
{
    using System.CodeDom;
    using CodeGeneration;

    public partial class OneToOneRelation
    {
        public CodeAttributeDeclaration GetOneToOneAttributeForSource()
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("OneToOne");

            if (SourceAccess != PropertyAccess.Property)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess", SourceAccess));
            if (SourceCascade != CascadeEnum.None)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Cascade", "CascadeEnum", SourceCascade));
            if (SourceConstrained)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Constrained", SourceConstrained));
            if (!string.IsNullOrEmpty(SourceCustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", SourceCustomAccess));
            if (SourceOuterJoin != OuterJoinEnum.Auto)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("OuterJoin", "OuterJoinEnum", SourceOuterJoin));

            return attribute;
        }

        public CodeAttributeDeclaration GetOneToOneAttributeForTarget()
        {
            CodeAttributeDeclaration attribute = null;

			if (!this.Lazy)
        	{
				attribute = new CodeAttributeDeclaration("OneToOne");

        		if (TargetAccess != PropertyAccess.Property)
        			attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess", TargetAccess));
        		if (TargetConstrained)
        			attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Constrained", TargetConstrained));
        	}
			else
				attribute = new CodeAttributeDeclaration("BelongsTo");

			if (TargetCascade != CascadeEnum.None)
				attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Cascade", "CascadeEnum", TargetCascade));
			if (!string.IsNullOrEmpty(TargetCustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", TargetCustomAccess));
            if (TargetOuterJoin != OuterJoinEnum.Auto)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("OuterJoin", "OuterJoinEnum", TargetOuterJoin));

            return attribute;
        }
    }
}
