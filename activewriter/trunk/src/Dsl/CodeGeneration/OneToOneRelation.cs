#region License
//  Copyright 2004-2010 Castle Project - http:www.castleproject.org/
//  
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//      http:www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// 
#endregion
namespace Castle.ActiveWriter
{
	using System.CodeDom;
	using CodeGeneration;

	public partial class OneToOneRelation
    {
        public PropertyAccess EffectiveSourceAccess
        {
            get
            {
                if (SourceAccess == InheritablePropertyAccess.Inherit)
                    return Source.EffectiveAccess;
                return SourceAccess.GetMatchingPropertyAccess();
            }
        }

        public PropertyAccess EffectiveTargetAccess
        {
            get
            {
                if (TargetAccess == InheritablePropertyAccess.Inherit)
                    return Target.EffectiveAccess;
                return TargetAccess.GetMatchingPropertyAccess();
            }
        }

        public CodeAttributeDeclaration GetOneToOneAttributeForSource()
        {
            CodeAttributeDeclaration attribute = new CodeAttributeDeclaration("OneToOne");

            if (SourceCascade != CascadeEnum.None)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Cascade", "CascadeEnum", SourceCascade));
            if (SourceConstrained)
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Constrained", SourceConstrained));

            if (!string.IsNullOrEmpty(SourceCustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", SourceCustomAccess));
            else if (EffectiveSourceAccess != PropertyAccess.Property)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess", EffectiveSourceAccess));

            if (SourceOuterJoin != OuterJoinEnum.Auto)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("OuterJoin", "OuterJoinEnum", SourceOuterJoin));

            return attribute;
        }

        public CodeAttributeDeclaration GetOneToOneAttributeForTarget()
        {
            CodeAttributeDeclaration attribute;

			if (!Lazy)
        	{
				attribute = new CodeAttributeDeclaration("OneToOne");

        		if (TargetConstrained)
        			attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("Constrained", TargetConstrained));
        	}
			else
				attribute = new CodeAttributeDeclaration("BelongsTo");

			if (TargetCascade != CascadeEnum.None)
				attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Cascade", "CascadeEnum", TargetCascade));

            if (!string.IsNullOrEmpty(TargetCustomAccess))
                attribute.Arguments.Add(AttributeHelper.GetNamedAttributeArgument("CustomAccess", TargetCustomAccess));
            // This was moved out of the above if(Lazy) block.  The properties themselves can be virtual and we can fill in the fields when the NHibernate object is loaded.
            else if (EffectiveTargetAccess != PropertyAccess.Property)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("Access", "PropertyAccess", EffectiveTargetAccess));

            if (TargetOuterJoin != OuterJoinEnum.Auto)
                attribute.Arguments.Add(AttributeHelper.GetNamedEnumAttributeArgument("OuterJoin", "OuterJoinEnum", TargetOuterJoin));

            return attribute;
        }
    }
}
