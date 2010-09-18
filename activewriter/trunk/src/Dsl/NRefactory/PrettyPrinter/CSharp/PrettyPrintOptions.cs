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
namespace ICSharpCode.NRefactory.PrettyPrinter
{
	public enum BraceStyle {
		EndOfLine,
		EndOfLineWithoutSpace,
		NextLine,
		NextLineShifted,
		NextLineShifted2
	}
	
	/// <summary>
	/// Description of PrettyPrintOptions.	
	/// </summary>
	public class PrettyPrintOptions : AbstractPrettyPrintOptions
	{
		#region BraceStyle
		BraceStyle namespaceBraceStyle = BraceStyle.NextLine;
		BraceStyle classBraceStyle     = BraceStyle.NextLine;
		BraceStyle interfaceBraceStyle = BraceStyle.NextLine;
		BraceStyle structBraceStyle    = BraceStyle.NextLine;
		BraceStyle enumBraceStyle      = BraceStyle.NextLine;
		
		BraceStyle constructorBraceStyle  = BraceStyle.NextLine;
		BraceStyle destructorBraceStyle   = BraceStyle.NextLine;
		BraceStyle methodBraceStyle       = BraceStyle.NextLine;
		
		BraceStyle propertyBraceStyle     = BraceStyle.EndOfLine;
		bool      allowPropertyGetBlockInline = true;
		BraceStyle propertyGetBraceStyle  = BraceStyle.EndOfLine;
		bool      allowPropertySetBlockInline = true;
		BraceStyle propertySetBraceStyle  = BraceStyle.EndOfLine;
		
		BraceStyle eventBraceStyle        = BraceStyle.EndOfLine;
		bool      allowEventAddBlockInline = true;
		BraceStyle eventAddBraceStyle     = BraceStyle.EndOfLine;
		bool      allowEventRemoveBlockInline = true;
		BraceStyle eventRemoveBraceStyle  = BraceStyle.EndOfLine;
		
		BraceStyle statementBraceStyle = BraceStyle.EndOfLine;
		
		public BraceStyle StatementBraceStyle {
			get {
				return statementBraceStyle;
			}
			set {
				statementBraceStyle = value;
			}
		}
		
		public BraceStyle NamespaceBraceStyle {
			get {
				return namespaceBraceStyle;
			}
			set {
				namespaceBraceStyle = value;
			}
		}
		
		public BraceStyle ClassBraceStyle {
			get {
				return classBraceStyle;
			}
			set {
				classBraceStyle = value;
			}
		}
		
		public BraceStyle InterfaceBraceStyle {
			get {
				return interfaceBraceStyle;
			}
			set {
				interfaceBraceStyle = value;
			}
		}
		
		public BraceStyle StructBraceStyle {
			get {
				return structBraceStyle;
			}
			set {
				structBraceStyle = value;
			}
		}
		
		public BraceStyle EnumBraceStyle {
			get {
				return enumBraceStyle;
			}
			set {
				enumBraceStyle = value;
			}
		}
		
		
		public BraceStyle ConstructorBraceStyle {
			get {
				return constructorBraceStyle;
			}
			set {
				constructorBraceStyle = value;
			}
		}
		
		public BraceStyle DestructorBraceStyle {
			get {
				return destructorBraceStyle;
			}
			set {
				destructorBraceStyle = value;
			}
		}
		
		public BraceStyle MethodBraceStyle {
			get {
				return methodBraceStyle;
			}
			set {
				methodBraceStyle = value;
			}
		}
		
		public BraceStyle PropertyBraceStyle {
			get {
				return propertyBraceStyle;
			}
			set {
				propertyBraceStyle = value;
			}
		}
		public BraceStyle PropertyGetBraceStyle {
			get {
				return propertyGetBraceStyle;
			}
			set {
				propertyGetBraceStyle = value;
			}
		}
		
		public bool AllowPropertyGetBlockInline {
			get {
				return allowPropertyGetBlockInline;
			}
			set {
				allowPropertyGetBlockInline = value;
			}
		}
		
		public BraceStyle PropertySetBraceStyle {
			get {
				return propertySetBraceStyle;
			}
			set {
				propertySetBraceStyle = value;
			}
		}
		public bool AllowPropertySetBlockInline {
			get {
				return allowPropertySetBlockInline;
			}
			set {
				allowPropertySetBlockInline = value;
			}
		}
		
		public BraceStyle EventBraceStyle {
			get {
				return eventBraceStyle;
			}
			set {
				eventBraceStyle = value;
			}
		}
		
		public BraceStyle EventAddBraceStyle {
			get {
				return eventAddBraceStyle;
			}
			set {
				eventAddBraceStyle = value;
			}
		}
		public bool AllowEventAddBlockInline {
			get {
				return allowEventAddBlockInline;
			}
			set {
				allowEventAddBlockInline = value;
			}
		}
		
		public BraceStyle EventRemoveBraceStyle {
			get {
				return eventRemoveBraceStyle;
			}
			set {
				eventRemoveBraceStyle = value;
			}
		}
		public bool AllowEventRemoveBlockInline {
			get {
				return allowEventRemoveBlockInline;
			}
			set {
				allowEventRemoveBlockInline = value;
			}
		}
		#endregion
		
		#region Indentation
		bool indentNamespaceBody = true;
		bool indentClassBody     = true;
		bool indentInterfaceBody = true;
		bool indentStructBody    = true;
		bool indentEnumBody      = true;
		
		bool indentMethodBody    = true;
		bool indentPropertyBody  = true;
		bool indentEventBody     = true;
		bool indentBlocks        = true;
		
		bool indentSwitchBody = true;
		bool indentCaseBody   = true;
		bool indentBreakStatements = true;
		
		public bool IndentBlocks {
			get {
				return indentBlocks;
			}
			set {
				indentBlocks = value;
			}
		}

		public bool IndentClassBody {
			get {
				return indentClassBody;
			}
			set {
				indentClassBody = value;
			}
		}

		public bool IndentStructBody {
			get {
				return indentStructBody;
			}
			set {
				indentStructBody = value;
			}
		}
		
		public bool IndentPropertyBody {
			get {
				return indentPropertyBody;
			}
			set {
				indentPropertyBody = value;
			}
		}
		
		public bool IndentNamespaceBody {
			get {
				return indentNamespaceBody;
			}
			set {
				indentNamespaceBody = value;
			}
		}

		public bool IndentMethodBody {
			get {
				return indentMethodBody;
			}
			set {
				indentMethodBody = value;
			}
		}

		public bool IndentInterfaceBody {
			get {
				return indentInterfaceBody;
			}
			set {
				indentInterfaceBody = value;
			}
		}

		public bool IndentEventBody {
			get {
				return indentEventBody;
			}
			set {
				indentEventBody = value;
			}
		}
	
		public bool IndentEnumBody {
			get {
				return indentEnumBody;
			}
			set {
				indentEnumBody = value;
			}
		}
		
		public bool IndentBreakStatements {
			get {
				return indentBreakStatements;
			}
			set {
				indentBreakStatements = value;
			}
		}
		
		public bool IndentSwitchBody {
			get {
				return indentSwitchBody;
			}
			set {
				indentSwitchBody = value;
			}
		}

		public bool IndentCaseBody {
			get {
				return indentCaseBody;
			}
			set {
				indentCaseBody = value;
			}
		}

		#endregion
		
		#region NewLines
		bool placeCatchOnNewLine = true;
		public bool PlaceCatchOnNewLine {
			get {
				return placeCatchOnNewLine;
			}
			set {
				placeCatchOnNewLine  = value;
			}
		}
		
		bool placeFinallyOnNewLine = true;
		public bool PlaceFinallyOnNewLine {
			get {
				return placeFinallyOnNewLine;
			}
			set {
				placeFinallyOnNewLine  = value;
			}
		}
		
		bool placeElseOnNewLine = true;
		public bool PlaceElseOnNewLine {
			get {
				return placeElseOnNewLine;
			}
			set {
				placeElseOnNewLine  = value;
			}
		}
		
		bool placeWhileOnNewLine = true;
		public bool PlaceWhileOnNewLine {
			get {
				return placeWhileOnNewLine;
			}
			set {
				placeWhileOnNewLine  = value;
			}
		}
		#endregion
		
		#region Spaces
		
		#region Before Parentheses

		bool ifParentheses      = true;
		bool whileParentheses   = true;
		bool forParentheses     = true;
		bool foreachParentheses = true;
		bool catchParentheses   = true;
		bool switchParentheses  = true;
		bool lockParentheses    = true;
		bool usingParentheses   = true;
		bool fixedParentheses   = true;

		public bool CheckedParentheses { get; set; }

		public bool NewParentheses { get; set; }

		public bool SizeOfParentheses { get; set; }

		public bool TypeOfParentheses { get; set; }

		public bool UncheckedParentheses { get; set; }

		public bool BeforeConstructorDeclarationParentheses { get; set; }

		public bool BeforeDelegateDeclarationParentheses { get; set; }

		public bool BeforeMethodCallParentheses { get; set; }

		public bool BeforeMethodDeclarationParentheses { get; set; }

		public bool IfParentheses {
			get {
				return ifParentheses;
			}
			set {
				ifParentheses = value;
			}
		}
		
		public bool WhileParentheses {
			get {
				return whileParentheses;
			}
			set {
				whileParentheses = value;
			}
		}
		public bool ForeachParentheses {
			get {
				return foreachParentheses;
			}
			set {
				foreachParentheses = value;
			}
		}
		public bool LockParentheses {
			get {
				return lockParentheses;
			}
			set {
				lockParentheses = value;
			}
		}
		public bool UsingParentheses {
			get {
				return usingParentheses;
			}
			set {
				usingParentheses = value;
			}
		}
		
		public bool CatchParentheses {
			get {
				return catchParentheses;
			}
			set {
				catchParentheses = value;
			}
		}
		public bool FixedParentheses {
			get {
				return fixedParentheses;
			}
			set {
				fixedParentheses = value;
			}
		}
		public bool SwitchParentheses {
			get {
				return switchParentheses;
			}
			set {
				switchParentheses = value;
			}
		}
		public bool ForParentheses {
			get {
				return forParentheses;
			}
			set {
				forParentheses = value;
			}
		}
		
		#endregion
		
		#region AroundOperators
		bool aroundAssignmentParentheses = true;
		bool aroundLogicalOperatorParentheses = true;
		bool aroundEqualityOperatorParentheses = true;
		bool aroundRelationalOperatorParentheses = true;
		bool aroundBitwiseOperatorParentheses = true;
		bool aroundAdditiveOperatorParentheses = true;
		bool aroundMultiplicativeOperatorParentheses = true;
		bool aroundShiftOperatorParentheses = true;
		
		public bool AroundAdditiveOperatorParentheses {
			get {
				return aroundAdditiveOperatorParentheses;
			}
			set {
				aroundAdditiveOperatorParentheses = value;
			}
		}
		public bool AroundAssignmentParentheses {
			get {
				return aroundAssignmentParentheses;
			}
			set {
				aroundAssignmentParentheses = value;
			}
		}
		public bool AroundBitwiseOperatorParentheses {
			get {
				return aroundBitwiseOperatorParentheses;
			}
			set {
				aroundBitwiseOperatorParentheses = value;
			}
		}
		public bool AroundEqualityOperatorParentheses {
			get {
				return aroundEqualityOperatorParentheses;
			}
			set {
				aroundEqualityOperatorParentheses = value;
			}
		}
		public bool AroundLogicalOperatorParentheses {
			get {
				return aroundLogicalOperatorParentheses;
			}
			set {
				aroundLogicalOperatorParentheses = value;
			}
		}
		public bool AroundMultiplicativeOperatorParentheses {
			get {
				return aroundMultiplicativeOperatorParentheses;
			}
			set {
				aroundMultiplicativeOperatorParentheses = value;
			}
		}
		public bool AroundRelationalOperatorParentheses {
			get {
				return aroundRelationalOperatorParentheses;
			}
			set {
				aroundRelationalOperatorParentheses = value;
			}
		}
		public bool AroundShiftOperatorParentheses {
			get {
				return aroundShiftOperatorParentheses;
			}
			set {
				aroundShiftOperatorParentheses = value;
			}
		}
		#endregion
		
		#region WithinParentheses

		public bool WithinCheckedExpressionParantheses { get; set; }

		public bool WithinTypeOfParentheses { get; set; }

		public bool WithinSizeOfParentheses { get; set; }

		public bool WithinCastParentheses { get; set; }

		public bool WithinUsingParentheses { get; set; }

		public bool WithinLockParentheses { get; set; }

		public bool WithinSwitchParentheses { get; set; }

		public bool WithinCatchParentheses { get; set; }

		public bool WithinForEachParentheses { get; set; }

		public bool WithinForParentheses { get; set; }

		public bool WithinWhileParentheses { get; set; }

		public bool WithinIfParentheses { get; set; }

		public bool WithinMethodDeclarationParentheses { get; set; }

		public bool WithinMethodCallParentheses { get; set; }

		public bool WithinParentheses { get; set; }

		#endregion
		
		#region SpacesInConditionalOperator
		bool conditionalOperatorBeforeConditionSpace = true;
		bool conditionalOperatorAfterConditionSpace = true;
		
		bool conditionalOperatorBeforeSeparatorSpace = true;
		bool conditionalOperatorAfterSeparatorSpace = true;
		
		public bool ConditionalOperatorAfterConditionSpace {
			get {
				return conditionalOperatorAfterConditionSpace;
			}
			set {
				conditionalOperatorAfterConditionSpace = value;
			}
		}
		public bool ConditionalOperatorAfterSeparatorSpace {
			get {
				return conditionalOperatorAfterSeparatorSpace;
			}
			set {
				conditionalOperatorAfterSeparatorSpace = value;
			}
		}
		public bool ConditionalOperatorBeforeConditionSpace {
			get {
				return conditionalOperatorBeforeConditionSpace;
			}
			set {
				conditionalOperatorBeforeConditionSpace = value;
			}
		}
		public bool ConditionalOperatorBeforeSeparatorSpace {
			get {
				return conditionalOperatorBeforeSeparatorSpace;
			}
			set {
				conditionalOperatorBeforeSeparatorSpace = value;
			}
		}
		#endregion

		#region OtherSpaces
		bool spacesAfterComma     = true;
		public bool SpacesAfterComma {
			get {
				return spacesAfterComma;
			}
			set {
				spacesAfterComma = value;
			}
		}
		
		bool spacesAfterSemicolon = true;
		public bool SpacesAfterSemicolon {
			get {
				return spacesAfterSemicolon;
			}
			set {
				spacesAfterSemicolon = value;
			}
		}

		public bool SpacesAfterTypecast { get; set; }

		public bool SpacesBeforeComma { get; set; }

		public bool SpacesWithinBrackets { get; set; }

		#endregion
		#endregion
	}
}