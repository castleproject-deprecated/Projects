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
namespace ICSharpCode.NRefactory.Visitors {
	using Ast;

	/// <summary>
	/// The NodeTrackingAstVisitor will iterate through the whole AST,
	/// just like the AbstractAstVisitor, and calls the virtual methods
	/// BeginVisit and EndVisit for each node being visited.
	/// </summary>
	/// <remarks>
	/// base.Visit(node, data) calls this.TrackedVisit(node, data), so if
	/// you want to visit child nodes using the default visiting behaviour,
	/// use base.TrackedVisit(parentNode, data).
	/// </remarks>
	public abstract class NodeTrackingAstVisitor : AbstractAstVisitor {
		
		protected virtual void BeginVisit(INode node) {
		}
		
		protected virtual void EndVisit(INode node) {
		}
		
		public sealed override object VisitAddHandlerStatement(AddHandlerStatement addHandlerStatement, object data) {
			BeginVisit(addHandlerStatement);
			object result = TrackedVisitAddHandlerStatement(addHandlerStatement, data);
			EndVisit(addHandlerStatement);
			return result;
		}
		
		public sealed override object VisitAddressOfExpression(AddressOfExpression addressOfExpression, object data) {
			BeginVisit(addressOfExpression);
			object result = TrackedVisitAddressOfExpression(addressOfExpression, data);
			EndVisit(addressOfExpression);
			return result;
		}
		
		public sealed override object VisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data) {
			BeginVisit(anonymousMethodExpression);
			object result = TrackedVisitAnonymousMethodExpression(anonymousMethodExpression, data);
			EndVisit(anonymousMethodExpression);
			return result;
		}
		
		public sealed override object VisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data) {
			BeginVisit(arrayCreateExpression);
			object result = TrackedVisitArrayCreateExpression(arrayCreateExpression, data);
			EndVisit(arrayCreateExpression);
			return result;
		}
		
		public sealed override object VisitAssignmentExpression(AssignmentExpression assignmentExpression, object data) {
			BeginVisit(assignmentExpression);
			object result = TrackedVisitAssignmentExpression(assignmentExpression, data);
			EndVisit(assignmentExpression);
			return result;
		}
		
		public sealed override object VisitAttribute(ICSharpCode.NRefactory.Ast.Attribute attribute, object data) {
			BeginVisit(attribute);
			object result = TrackedVisitAttribute(attribute, data);
			EndVisit(attribute);
			return result;
		}
		
		public sealed override object VisitAttributeSection(AttributeSection attributeSection, object data) {
			BeginVisit(attributeSection);
			object result = TrackedVisitAttributeSection(attributeSection, data);
			EndVisit(attributeSection);
			return result;
		}
		
		public sealed override object VisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data) {
			BeginVisit(baseReferenceExpression);
			object result = TrackedVisitBaseReferenceExpression(baseReferenceExpression, data);
			EndVisit(baseReferenceExpression);
			return result;
		}
		
		public sealed override object VisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data) {
			BeginVisit(binaryOperatorExpression);
			object result = TrackedVisitBinaryOperatorExpression(binaryOperatorExpression, data);
			EndVisit(binaryOperatorExpression);
			return result;
		}
		
		public sealed override object VisitBlockStatement(BlockStatement blockStatement, object data) {
			BeginVisit(blockStatement);
			object result = TrackedVisitBlockStatement(blockStatement, data);
			EndVisit(blockStatement);
			return result;
		}
		
		public sealed override object VisitBreakStatement(BreakStatement breakStatement, object data) {
			BeginVisit(breakStatement);
			object result = TrackedVisitBreakStatement(breakStatement, data);
			EndVisit(breakStatement);
			return result;
		}
		
		public sealed override object VisitCaseLabel(CaseLabel caseLabel, object data) {
			BeginVisit(caseLabel);
			object result = TrackedVisitCaseLabel(caseLabel, data);
			EndVisit(caseLabel);
			return result;
		}
		
		public sealed override object VisitCastExpression(CastExpression castExpression, object data) {
			BeginVisit(castExpression);
			object result = TrackedVisitCastExpression(castExpression, data);
			EndVisit(castExpression);
			return result;
		}
		
		public sealed override object VisitCatchClause(CatchClause catchClause, object data) {
			BeginVisit(catchClause);
			object result = TrackedVisitCatchClause(catchClause, data);
			EndVisit(catchClause);
			return result;
		}
		
		public sealed override object VisitCheckedExpression(CheckedExpression checkedExpression, object data) {
			BeginVisit(checkedExpression);
			object result = TrackedVisitCheckedExpression(checkedExpression, data);
			EndVisit(checkedExpression);
			return result;
		}
		
		public sealed override object VisitCheckedStatement(CheckedStatement checkedStatement, object data) {
			BeginVisit(checkedStatement);
			object result = TrackedVisitCheckedStatement(checkedStatement, data);
			EndVisit(checkedStatement);
			return result;
		}
		
		public sealed override object VisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data) {
			BeginVisit(classReferenceExpression);
			object result = TrackedVisitClassReferenceExpression(classReferenceExpression, data);
			EndVisit(classReferenceExpression);
			return result;
		}
		
		public sealed override object VisitCollectionInitializerExpression(CollectionInitializerExpression collectionInitializerExpression, object data) {
			BeginVisit(collectionInitializerExpression);
			object result = TrackedVisitCollectionInitializerExpression(collectionInitializerExpression, data);
			EndVisit(collectionInitializerExpression);
			return result;
		}
		
		public sealed override object VisitCompilationUnit(CompilationUnit compilationUnit, object data) {
			BeginVisit(compilationUnit);
			object result = TrackedVisitCompilationUnit(compilationUnit, data);
			EndVisit(compilationUnit);
			return result;
		}
		
		public sealed override object VisitConditionalExpression(ConditionalExpression conditionalExpression, object data) {
			BeginVisit(conditionalExpression);
			object result = TrackedVisitConditionalExpression(conditionalExpression, data);
			EndVisit(conditionalExpression);
			return result;
		}
		
		public sealed override object VisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data) {
			BeginVisit(constructorDeclaration);
			object result = TrackedVisitConstructorDeclaration(constructorDeclaration, data);
			EndVisit(constructorDeclaration);
			return result;
		}
		
		public sealed override object VisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data) {
			BeginVisit(constructorInitializer);
			object result = TrackedVisitConstructorInitializer(constructorInitializer, data);
			EndVisit(constructorInitializer);
			return result;
		}
		
		public sealed override object VisitContinueStatement(ContinueStatement continueStatement, object data) {
			BeginVisit(continueStatement);
			object result = TrackedVisitContinueStatement(continueStatement, data);
			EndVisit(continueStatement);
			return result;
		}
		
		public sealed override object VisitDeclareDeclaration(DeclareDeclaration declareDeclaration, object data) {
			BeginVisit(declareDeclaration);
			object result = TrackedVisitDeclareDeclaration(declareDeclaration, data);
			EndVisit(declareDeclaration);
			return result;
		}
		
		public sealed override object VisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data) {
			BeginVisit(defaultValueExpression);
			object result = TrackedVisitDefaultValueExpression(defaultValueExpression, data);
			EndVisit(defaultValueExpression);
			return result;
		}
		
		public sealed override object VisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data) {
			BeginVisit(delegateDeclaration);
			object result = TrackedVisitDelegateDeclaration(delegateDeclaration, data);
			EndVisit(delegateDeclaration);
			return result;
		}
		
		public sealed override object VisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data) {
			BeginVisit(destructorDeclaration);
			object result = TrackedVisitDestructorDeclaration(destructorDeclaration, data);
			EndVisit(destructorDeclaration);
			return result;
		}
		
		public sealed override object VisitDirectionExpression(DirectionExpression directionExpression, object data) {
			BeginVisit(directionExpression);
			object result = TrackedVisitDirectionExpression(directionExpression, data);
			EndVisit(directionExpression);
			return result;
		}
		
		public sealed override object VisitDoLoopStatement(DoLoopStatement doLoopStatement, object data) {
			BeginVisit(doLoopStatement);
			object result = TrackedVisitDoLoopStatement(doLoopStatement, data);
			EndVisit(doLoopStatement);
			return result;
		}
		
		public sealed override object VisitElseIfSection(ElseIfSection elseIfSection, object data) {
			BeginVisit(elseIfSection);
			object result = TrackedVisitElseIfSection(elseIfSection, data);
			EndVisit(elseIfSection);
			return result;
		}
		
		public sealed override object VisitEmptyStatement(EmptyStatement emptyStatement, object data) {
			BeginVisit(emptyStatement);
			object result = TrackedVisitEmptyStatement(emptyStatement, data);
			EndVisit(emptyStatement);
			return result;
		}
		
		public sealed override object VisitEndStatement(EndStatement endStatement, object data) {
			BeginVisit(endStatement);
			object result = TrackedVisitEndStatement(endStatement, data);
			EndVisit(endStatement);
			return result;
		}
		
		public sealed override object VisitEraseStatement(EraseStatement eraseStatement, object data) {
			BeginVisit(eraseStatement);
			object result = TrackedVisitEraseStatement(eraseStatement, data);
			EndVisit(eraseStatement);
			return result;
		}
		
		public sealed override object VisitErrorStatement(ErrorStatement errorStatement, object data) {
			BeginVisit(errorStatement);
			object result = TrackedVisitErrorStatement(errorStatement, data);
			EndVisit(errorStatement);
			return result;
		}
		
		public sealed override object VisitEventAddRegion(EventAddRegion eventAddRegion, object data) {
			BeginVisit(eventAddRegion);
			object result = TrackedVisitEventAddRegion(eventAddRegion, data);
			EndVisit(eventAddRegion);
			return result;
		}
		
		public sealed override object VisitEventDeclaration(EventDeclaration eventDeclaration, object data) {
			BeginVisit(eventDeclaration);
			object result = TrackedVisitEventDeclaration(eventDeclaration, data);
			EndVisit(eventDeclaration);
			return result;
		}
		
		public sealed override object VisitEventRaiseRegion(EventRaiseRegion eventRaiseRegion, object data) {
			BeginVisit(eventRaiseRegion);
			object result = TrackedVisitEventRaiseRegion(eventRaiseRegion, data);
			EndVisit(eventRaiseRegion);
			return result;
		}
		
		public sealed override object VisitEventRemoveRegion(EventRemoveRegion eventRemoveRegion, object data) {
			BeginVisit(eventRemoveRegion);
			object result = TrackedVisitEventRemoveRegion(eventRemoveRegion, data);
			EndVisit(eventRemoveRegion);
			return result;
		}
		
		public sealed override object VisitExitStatement(ExitStatement exitStatement, object data) {
			BeginVisit(exitStatement);
			object result = TrackedVisitExitStatement(exitStatement, data);
			EndVisit(exitStatement);
			return result;
		}
		
		public sealed override object VisitExpressionRangeVariable(ExpressionRangeVariable expressionRangeVariable, object data) {
			BeginVisit(expressionRangeVariable);
			object result = TrackedVisitExpressionRangeVariable(expressionRangeVariable, data);
			EndVisit(expressionRangeVariable);
			return result;
		}
		
		public sealed override object VisitExpressionStatement(ExpressionStatement expressionStatement, object data) {
			BeginVisit(expressionStatement);
			object result = TrackedVisitExpressionStatement(expressionStatement, data);
			EndVisit(expressionStatement);
			return result;
		}
		
		public sealed override object VisitExternAliasDirective(ExternAliasDirective externAliasDirective, object data) {
			BeginVisit(externAliasDirective);
			object result = TrackedVisitExternAliasDirective(externAliasDirective, data);
			EndVisit(externAliasDirective);
			return result;
		}
		
		public sealed override object VisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data) {
			BeginVisit(fieldDeclaration);
			object result = TrackedVisitFieldDeclaration(fieldDeclaration, data);
			EndVisit(fieldDeclaration);
			return result;
		}
		
		public sealed override object VisitFixedStatement(FixedStatement fixedStatement, object data) {
			BeginVisit(fixedStatement);
			object result = TrackedVisitFixedStatement(fixedStatement, data);
			EndVisit(fixedStatement);
			return result;
		}
		
		public sealed override object VisitForeachStatement(ForeachStatement foreachStatement, object data) {
			BeginVisit(foreachStatement);
			object result = TrackedVisitForeachStatement(foreachStatement, data);
			EndVisit(foreachStatement);
			return result;
		}
		
		public sealed override object VisitForNextStatement(ForNextStatement forNextStatement, object data) {
			BeginVisit(forNextStatement);
			object result = TrackedVisitForNextStatement(forNextStatement, data);
			EndVisit(forNextStatement);
			return result;
		}
		
		public sealed override object VisitForStatement(ForStatement forStatement, object data) {
			BeginVisit(forStatement);
			object result = TrackedVisitForStatement(forStatement, data);
			EndVisit(forStatement);
			return result;
		}
		
		public sealed override object VisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, object data) {
			BeginVisit(gotoCaseStatement);
			object result = TrackedVisitGotoCaseStatement(gotoCaseStatement, data);
			EndVisit(gotoCaseStatement);
			return result;
		}
		
		public sealed override object VisitGotoStatement(GotoStatement gotoStatement, object data) {
			BeginVisit(gotoStatement);
			object result = TrackedVisitGotoStatement(gotoStatement, data);
			EndVisit(gotoStatement);
			return result;
		}
		
		public sealed override object VisitIdentifierExpression(IdentifierExpression identifierExpression, object data) {
			BeginVisit(identifierExpression);
			object result = TrackedVisitIdentifierExpression(identifierExpression, data);
			EndVisit(identifierExpression);
			return result;
		}
		
		public sealed override object VisitIfElseStatement(IfElseStatement ifElseStatement, object data) {
			BeginVisit(ifElseStatement);
			object result = TrackedVisitIfElseStatement(ifElseStatement, data);
			EndVisit(ifElseStatement);
			return result;
		}
		
		public sealed override object VisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, object data) {
			BeginVisit(indexerDeclaration);
			object result = TrackedVisitIndexerDeclaration(indexerDeclaration, data);
			EndVisit(indexerDeclaration);
			return result;
		}
		
		public sealed override object VisitIndexerExpression(IndexerExpression indexerExpression, object data) {
			BeginVisit(indexerExpression);
			object result = TrackedVisitIndexerExpression(indexerExpression, data);
			EndVisit(indexerExpression);
			return result;
		}
		
		public sealed override object VisitInnerClassTypeReference(InnerClassTypeReference innerClassTypeReference, object data) {
			BeginVisit(innerClassTypeReference);
			object result = TrackedVisitInnerClassTypeReference(innerClassTypeReference, data);
			EndVisit(innerClassTypeReference);
			return result;
		}
		
		public sealed override object VisitInterfaceImplementation(InterfaceImplementation interfaceImplementation, object data) {
			BeginVisit(interfaceImplementation);
			object result = TrackedVisitInterfaceImplementation(interfaceImplementation, data);
			EndVisit(interfaceImplementation);
			return result;
		}
		
		public sealed override object VisitInvocationExpression(InvocationExpression invocationExpression, object data) {
			BeginVisit(invocationExpression);
			object result = TrackedVisitInvocationExpression(invocationExpression, data);
			EndVisit(invocationExpression);
			return result;
		}
		
		public sealed override object VisitLabelStatement(LabelStatement labelStatement, object data) {
			BeginVisit(labelStatement);
			object result = TrackedVisitLabelStatement(labelStatement, data);
			EndVisit(labelStatement);
			return result;
		}
		
		public sealed override object VisitLambdaExpression(LambdaExpression lambdaExpression, object data) {
			BeginVisit(lambdaExpression);
			object result = TrackedVisitLambdaExpression(lambdaExpression, data);
			EndVisit(lambdaExpression);
			return result;
		}
		
		public sealed override object VisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data) {
			BeginVisit(localVariableDeclaration);
			object result = TrackedVisitLocalVariableDeclaration(localVariableDeclaration, data);
			EndVisit(localVariableDeclaration);
			return result;
		}
		
		public sealed override object VisitLockStatement(LockStatement lockStatement, object data) {
			BeginVisit(lockStatement);
			object result = TrackedVisitLockStatement(lockStatement, data);
			EndVisit(lockStatement);
			return result;
		}
		
		public sealed override object VisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data) {
			BeginVisit(memberReferenceExpression);
			object result = TrackedVisitMemberReferenceExpression(memberReferenceExpression, data);
			EndVisit(memberReferenceExpression);
			return result;
		}
		
		public sealed override object VisitMethodDeclaration(MethodDeclaration methodDeclaration, object data) {
			BeginVisit(methodDeclaration);
			object result = TrackedVisitMethodDeclaration(methodDeclaration, data);
			EndVisit(methodDeclaration);
			return result;
		}
		
		public sealed override object VisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, object data) {
			BeginVisit(namedArgumentExpression);
			object result = TrackedVisitNamedArgumentExpression(namedArgumentExpression, data);
			EndVisit(namedArgumentExpression);
			return result;
		}
		
		public sealed override object VisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data) {
			BeginVisit(namespaceDeclaration);
			object result = TrackedVisitNamespaceDeclaration(namespaceDeclaration, data);
			EndVisit(namespaceDeclaration);
			return result;
		}
		
		public sealed override object VisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data) {
			BeginVisit(objectCreateExpression);
			object result = TrackedVisitObjectCreateExpression(objectCreateExpression, data);
			EndVisit(objectCreateExpression);
			return result;
		}
		
		public sealed override object VisitOnErrorStatement(OnErrorStatement onErrorStatement, object data) {
			BeginVisit(onErrorStatement);
			object result = TrackedVisitOnErrorStatement(onErrorStatement, data);
			EndVisit(onErrorStatement);
			return result;
		}
		
		public sealed override object VisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data) {
			BeginVisit(operatorDeclaration);
			object result = TrackedVisitOperatorDeclaration(operatorDeclaration, data);
			EndVisit(operatorDeclaration);
			return result;
		}
		
		public sealed override object VisitOptionDeclaration(OptionDeclaration optionDeclaration, object data) {
			BeginVisit(optionDeclaration);
			object result = TrackedVisitOptionDeclaration(optionDeclaration, data);
			EndVisit(optionDeclaration);
			return result;
		}
		
		public sealed override object VisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data) {
			BeginVisit(parameterDeclarationExpression);
			object result = TrackedVisitParameterDeclarationExpression(parameterDeclarationExpression, data);
			EndVisit(parameterDeclarationExpression);
			return result;
		}
		
		public sealed override object VisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data) {
			BeginVisit(parenthesizedExpression);
			object result = TrackedVisitParenthesizedExpression(parenthesizedExpression, data);
			EndVisit(parenthesizedExpression);
			return result;
		}
		
		public sealed override object VisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data) {
			BeginVisit(pointerReferenceExpression);
			object result = TrackedVisitPointerReferenceExpression(pointerReferenceExpression, data);
			EndVisit(pointerReferenceExpression);
			return result;
		}
		
		public sealed override object VisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data) {
			BeginVisit(primitiveExpression);
			object result = TrackedVisitPrimitiveExpression(primitiveExpression, data);
			EndVisit(primitiveExpression);
			return result;
		}
		
		public sealed override object VisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data) {
			BeginVisit(propertyDeclaration);
			object result = TrackedVisitPropertyDeclaration(propertyDeclaration, data);
			EndVisit(propertyDeclaration);
			return result;
		}
		
		public sealed override object VisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data) {
			BeginVisit(propertyGetRegion);
			object result = TrackedVisitPropertyGetRegion(propertyGetRegion, data);
			EndVisit(propertyGetRegion);
			return result;
		}
		
		public sealed override object VisitPropertySetRegion(PropertySetRegion propertySetRegion, object data) {
			BeginVisit(propertySetRegion);
			object result = TrackedVisitPropertySetRegion(propertySetRegion, data);
			EndVisit(propertySetRegion);
			return result;
		}
		
		public sealed override object VisitQueryExpression(QueryExpression queryExpression, object data) {
			BeginVisit(queryExpression);
			object result = TrackedVisitQueryExpression(queryExpression, data);
			EndVisit(queryExpression);
			return result;
		}
		
		public sealed override object VisitQueryExpressionAggregateClause(QueryExpressionAggregateClause queryExpressionAggregateClause, object data) {
			BeginVisit(queryExpressionAggregateClause);
			object result = TrackedVisitQueryExpressionAggregateClause(queryExpressionAggregateClause, data);
			EndVisit(queryExpressionAggregateClause);
			return result;
		}
		
		public sealed override object VisitQueryExpressionDistinctClause(QueryExpressionDistinctClause queryExpressionDistinctClause, object data) {
			BeginVisit(queryExpressionDistinctClause);
			object result = TrackedVisitQueryExpressionDistinctClause(queryExpressionDistinctClause, data);
			EndVisit(queryExpressionDistinctClause);
			return result;
		}
		
		public sealed override object VisitQueryExpressionFromClause(QueryExpressionFromClause queryExpressionFromClause, object data) {
			BeginVisit(queryExpressionFromClause);
			object result = TrackedVisitQueryExpressionFromClause(queryExpressionFromClause, data);
			EndVisit(queryExpressionFromClause);
			return result;
		}
		
		public sealed override object VisitQueryExpressionGroupClause(QueryExpressionGroupClause queryExpressionGroupClause, object data) {
			BeginVisit(queryExpressionGroupClause);
			object result = TrackedVisitQueryExpressionGroupClause(queryExpressionGroupClause, data);
			EndVisit(queryExpressionGroupClause);
			return result;
		}
		
		public sealed override object VisitQueryExpressionGroupJoinVBClause(QueryExpressionGroupJoinVBClause queryExpressionGroupJoinVBClause, object data) {
			BeginVisit(queryExpressionGroupJoinVBClause);
			object result = TrackedVisitQueryExpressionGroupJoinVBClause(queryExpressionGroupJoinVBClause, data);
			EndVisit(queryExpressionGroupJoinVBClause);
			return result;
		}
		
		public sealed override object VisitQueryExpressionGroupVBClause(QueryExpressionGroupVBClause queryExpressionGroupVBClause, object data) {
			BeginVisit(queryExpressionGroupVBClause);
			object result = TrackedVisitQueryExpressionGroupVBClause(queryExpressionGroupVBClause, data);
			EndVisit(queryExpressionGroupVBClause);
			return result;
		}
		
		public sealed override object VisitQueryExpressionJoinClause(QueryExpressionJoinClause queryExpressionJoinClause, object data) {
			BeginVisit(queryExpressionJoinClause);
			object result = TrackedVisitQueryExpressionJoinClause(queryExpressionJoinClause, data);
			EndVisit(queryExpressionJoinClause);
			return result;
		}
		
		public sealed override object VisitQueryExpressionJoinConditionVB(QueryExpressionJoinConditionVB queryExpressionJoinConditionVB, object data) {
			BeginVisit(queryExpressionJoinConditionVB);
			object result = TrackedVisitQueryExpressionJoinConditionVB(queryExpressionJoinConditionVB, data);
			EndVisit(queryExpressionJoinConditionVB);
			return result;
		}
		
		public sealed override object VisitQueryExpressionJoinVBClause(QueryExpressionJoinVBClause queryExpressionJoinVBClause, object data) {
			BeginVisit(queryExpressionJoinVBClause);
			object result = TrackedVisitQueryExpressionJoinVBClause(queryExpressionJoinVBClause, data);
			EndVisit(queryExpressionJoinVBClause);
			return result;
		}
		
		public sealed override object VisitQueryExpressionLetClause(QueryExpressionLetClause queryExpressionLetClause, object data) {
			BeginVisit(queryExpressionLetClause);
			object result = TrackedVisitQueryExpressionLetClause(queryExpressionLetClause, data);
			EndVisit(queryExpressionLetClause);
			return result;
		}
		
		public sealed override object VisitQueryExpressionLetVBClause(QueryExpressionLetVBClause queryExpressionLetVBClause, object data) {
			BeginVisit(queryExpressionLetVBClause);
			object result = TrackedVisitQueryExpressionLetVBClause(queryExpressionLetVBClause, data);
			EndVisit(queryExpressionLetVBClause);
			return result;
		}
		
		public sealed override object VisitQueryExpressionOrderClause(QueryExpressionOrderClause queryExpressionOrderClause, object data) {
			BeginVisit(queryExpressionOrderClause);
			object result = TrackedVisitQueryExpressionOrderClause(queryExpressionOrderClause, data);
			EndVisit(queryExpressionOrderClause);
			return result;
		}
		
		public sealed override object VisitQueryExpressionOrdering(QueryExpressionOrdering queryExpressionOrdering, object data) {
			BeginVisit(queryExpressionOrdering);
			object result = TrackedVisitQueryExpressionOrdering(queryExpressionOrdering, data);
			EndVisit(queryExpressionOrdering);
			return result;
		}
		
		public sealed override object VisitQueryExpressionPartitionVBClause(QueryExpressionPartitionVBClause queryExpressionPartitionVBClause, object data) {
			BeginVisit(queryExpressionPartitionVBClause);
			object result = TrackedVisitQueryExpressionPartitionVBClause(queryExpressionPartitionVBClause, data);
			EndVisit(queryExpressionPartitionVBClause);
			return result;
		}
		
		public sealed override object VisitQueryExpressionSelectClause(QueryExpressionSelectClause queryExpressionSelectClause, object data) {
			BeginVisit(queryExpressionSelectClause);
			object result = TrackedVisitQueryExpressionSelectClause(queryExpressionSelectClause, data);
			EndVisit(queryExpressionSelectClause);
			return result;
		}
		
		public sealed override object VisitQueryExpressionSelectVBClause(QueryExpressionSelectVBClause queryExpressionSelectVBClause, object data) {
			BeginVisit(queryExpressionSelectVBClause);
			object result = TrackedVisitQueryExpressionSelectVBClause(queryExpressionSelectVBClause, data);
			EndVisit(queryExpressionSelectVBClause);
			return result;
		}
		
		public sealed override object VisitQueryExpressionWhereClause(QueryExpressionWhereClause queryExpressionWhereClause, object data) {
			BeginVisit(queryExpressionWhereClause);
			object result = TrackedVisitQueryExpressionWhereClause(queryExpressionWhereClause, data);
			EndVisit(queryExpressionWhereClause);
			return result;
		}
		
		public sealed override object VisitRaiseEventStatement(RaiseEventStatement raiseEventStatement, object data) {
			BeginVisit(raiseEventStatement);
			object result = TrackedVisitRaiseEventStatement(raiseEventStatement, data);
			EndVisit(raiseEventStatement);
			return result;
		}
		
		public sealed override object VisitReDimStatement(ReDimStatement reDimStatement, object data) {
			BeginVisit(reDimStatement);
			object result = TrackedVisitReDimStatement(reDimStatement, data);
			EndVisit(reDimStatement);
			return result;
		}
		
		public sealed override object VisitRemoveHandlerStatement(RemoveHandlerStatement removeHandlerStatement, object data) {
			BeginVisit(removeHandlerStatement);
			object result = TrackedVisitRemoveHandlerStatement(removeHandlerStatement, data);
			EndVisit(removeHandlerStatement);
			return result;
		}
		
		public sealed override object VisitResumeStatement(ResumeStatement resumeStatement, object data) {
			BeginVisit(resumeStatement);
			object result = TrackedVisitResumeStatement(resumeStatement, data);
			EndVisit(resumeStatement);
			return result;
		}
		
		public sealed override object VisitReturnStatement(ReturnStatement returnStatement, object data) {
			BeginVisit(returnStatement);
			object result = TrackedVisitReturnStatement(returnStatement, data);
			EndVisit(returnStatement);
			return result;
		}
		
		public sealed override object VisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data) {
			BeginVisit(sizeOfExpression);
			object result = TrackedVisitSizeOfExpression(sizeOfExpression, data);
			EndVisit(sizeOfExpression);
			return result;
		}
		
		public sealed override object VisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data) {
			BeginVisit(stackAllocExpression);
			object result = TrackedVisitStackAllocExpression(stackAllocExpression, data);
			EndVisit(stackAllocExpression);
			return result;
		}
		
		public sealed override object VisitStopStatement(StopStatement stopStatement, object data) {
			BeginVisit(stopStatement);
			object result = TrackedVisitStopStatement(stopStatement, data);
			EndVisit(stopStatement);
			return result;
		}
		
		public sealed override object VisitSwitchSection(SwitchSection switchSection, object data) {
			BeginVisit(switchSection);
			object result = TrackedVisitSwitchSection(switchSection, data);
			EndVisit(switchSection);
			return result;
		}
		
		public sealed override object VisitSwitchStatement(SwitchStatement switchStatement, object data) {
			BeginVisit(switchStatement);
			object result = TrackedVisitSwitchStatement(switchStatement, data);
			EndVisit(switchStatement);
			return result;
		}
		
		public sealed override object VisitTemplateDefinition(TemplateDefinition templateDefinition, object data) {
			BeginVisit(templateDefinition);
			object result = TrackedVisitTemplateDefinition(templateDefinition, data);
			EndVisit(templateDefinition);
			return result;
		}
		
		public sealed override object VisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data) {
			BeginVisit(thisReferenceExpression);
			object result = TrackedVisitThisReferenceExpression(thisReferenceExpression, data);
			EndVisit(thisReferenceExpression);
			return result;
		}
		
		public sealed override object VisitThrowStatement(ThrowStatement throwStatement, object data) {
			BeginVisit(throwStatement);
			object result = TrackedVisitThrowStatement(throwStatement, data);
			EndVisit(throwStatement);
			return result;
		}
		
		public sealed override object VisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data) {
			BeginVisit(tryCatchStatement);
			object result = TrackedVisitTryCatchStatement(tryCatchStatement, data);
			EndVisit(tryCatchStatement);
			return result;
		}
		
		public sealed override object VisitTypeDeclaration(TypeDeclaration typeDeclaration, object data) {
			BeginVisit(typeDeclaration);
			object result = TrackedVisitTypeDeclaration(typeDeclaration, data);
			EndVisit(typeDeclaration);
			return result;
		}
		
		public sealed override object VisitTypeOfExpression(TypeOfExpression typeOfExpression, object data) {
			BeginVisit(typeOfExpression);
			object result = TrackedVisitTypeOfExpression(typeOfExpression, data);
			EndVisit(typeOfExpression);
			return result;
		}
		
		public sealed override object VisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data) {
			BeginVisit(typeOfIsExpression);
			object result = TrackedVisitTypeOfIsExpression(typeOfIsExpression, data);
			EndVisit(typeOfIsExpression);
			return result;
		}
		
		public sealed override object VisitTypeReference(TypeReference typeReference, object data) {
			BeginVisit(typeReference);
			object result = TrackedVisitTypeReference(typeReference, data);
			EndVisit(typeReference);
			return result;
		}
		
		public sealed override object VisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data) {
			BeginVisit(typeReferenceExpression);
			object result = TrackedVisitTypeReferenceExpression(typeReferenceExpression, data);
			EndVisit(typeReferenceExpression);
			return result;
		}
		
		public sealed override object VisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data) {
			BeginVisit(unaryOperatorExpression);
			object result = TrackedVisitUnaryOperatorExpression(unaryOperatorExpression, data);
			EndVisit(unaryOperatorExpression);
			return result;
		}
		
		public sealed override object VisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data) {
			BeginVisit(uncheckedExpression);
			object result = TrackedVisitUncheckedExpression(uncheckedExpression, data);
			EndVisit(uncheckedExpression);
			return result;
		}
		
		public sealed override object VisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data) {
			BeginVisit(uncheckedStatement);
			object result = TrackedVisitUncheckedStatement(uncheckedStatement, data);
			EndVisit(uncheckedStatement);
			return result;
		}
		
		public sealed override object VisitUnsafeStatement(UnsafeStatement unsafeStatement, object data) {
			BeginVisit(unsafeStatement);
			object result = TrackedVisitUnsafeStatement(unsafeStatement, data);
			EndVisit(unsafeStatement);
			return result;
		}
		
		public sealed override object VisitUsing(Using @using, object data) {
			BeginVisit(@using);
			object result = TrackedVisitUsing(@using, data);
			EndVisit(@using);
			return result;
		}
		
		public sealed override object VisitUsingDeclaration(UsingDeclaration usingDeclaration, object data) {
			BeginVisit(usingDeclaration);
			object result = TrackedVisitUsingDeclaration(usingDeclaration, data);
			EndVisit(usingDeclaration);
			return result;
		}
		
		public sealed override object VisitUsingStatement(UsingStatement usingStatement, object data) {
			BeginVisit(usingStatement);
			object result = TrackedVisitUsingStatement(usingStatement, data);
			EndVisit(usingStatement);
			return result;
		}
		
		public sealed override object VisitVariableDeclaration(VariableDeclaration variableDeclaration, object data) {
			BeginVisit(variableDeclaration);
			object result = TrackedVisitVariableDeclaration(variableDeclaration, data);
			EndVisit(variableDeclaration);
			return result;
		}
		
		public sealed override object VisitWithStatement(WithStatement withStatement, object data) {
			BeginVisit(withStatement);
			object result = TrackedVisitWithStatement(withStatement, data);
			EndVisit(withStatement);
			return result;
		}
		
		public sealed override object VisitYieldStatement(YieldStatement yieldStatement, object data) {
			BeginVisit(yieldStatement);
			object result = TrackedVisitYieldStatement(yieldStatement, data);
			EndVisit(yieldStatement);
			return result;
		}
		
		public virtual object TrackedVisitAddHandlerStatement(AddHandlerStatement addHandlerStatement, object data) {
			return base.VisitAddHandlerStatement(addHandlerStatement, data);
		}
		
		public virtual object TrackedVisitAddressOfExpression(AddressOfExpression addressOfExpression, object data) {
			return base.VisitAddressOfExpression(addressOfExpression, data);
		}
		
		public virtual object TrackedVisitAnonymousMethodExpression(AnonymousMethodExpression anonymousMethodExpression, object data) {
			return base.VisitAnonymousMethodExpression(anonymousMethodExpression, data);
		}
		
		public virtual object TrackedVisitArrayCreateExpression(ArrayCreateExpression arrayCreateExpression, object data) {
			return base.VisitArrayCreateExpression(arrayCreateExpression, data);
		}
		
		public virtual object TrackedVisitAssignmentExpression(AssignmentExpression assignmentExpression, object data) {
			return base.VisitAssignmentExpression(assignmentExpression, data);
		}
		
		public virtual object TrackedVisitAttribute(ICSharpCode.NRefactory.Ast.Attribute attribute, object data) {
			return base.VisitAttribute(attribute, data);
		}
		
		public virtual object TrackedVisitAttributeSection(AttributeSection attributeSection, object data) {
			return base.VisitAttributeSection(attributeSection, data);
		}
		
		public virtual object TrackedVisitBaseReferenceExpression(BaseReferenceExpression baseReferenceExpression, object data) {
			return base.VisitBaseReferenceExpression(baseReferenceExpression, data);
		}
		
		public virtual object TrackedVisitBinaryOperatorExpression(BinaryOperatorExpression binaryOperatorExpression, object data) {
			return base.VisitBinaryOperatorExpression(binaryOperatorExpression, data);
		}
		
		public virtual object TrackedVisitBlockStatement(BlockStatement blockStatement, object data) {
			return base.VisitBlockStatement(blockStatement, data);
		}
		
		public virtual object TrackedVisitBreakStatement(BreakStatement breakStatement, object data) {
			return base.VisitBreakStatement(breakStatement, data);
		}
		
		public virtual object TrackedVisitCaseLabel(CaseLabel caseLabel, object data) {
			return base.VisitCaseLabel(caseLabel, data);
		}
		
		public virtual object TrackedVisitCastExpression(CastExpression castExpression, object data) {
			return base.VisitCastExpression(castExpression, data);
		}
		
		public virtual object TrackedVisitCatchClause(CatchClause catchClause, object data) {
			return base.VisitCatchClause(catchClause, data);
		}
		
		public virtual object TrackedVisitCheckedExpression(CheckedExpression checkedExpression, object data) {
			return base.VisitCheckedExpression(checkedExpression, data);
		}
		
		public virtual object TrackedVisitCheckedStatement(CheckedStatement checkedStatement, object data) {
			return base.VisitCheckedStatement(checkedStatement, data);
		}
		
		public virtual object TrackedVisitClassReferenceExpression(ClassReferenceExpression classReferenceExpression, object data) {
			return base.VisitClassReferenceExpression(classReferenceExpression, data);
		}
		
		public virtual object TrackedVisitCollectionInitializerExpression(CollectionInitializerExpression collectionInitializerExpression, object data) {
			return base.VisitCollectionInitializerExpression(collectionInitializerExpression, data);
		}
		
		public virtual object TrackedVisitCompilationUnit(CompilationUnit compilationUnit, object data) {
			return base.VisitCompilationUnit(compilationUnit, data);
		}
		
		public virtual object TrackedVisitConditionalExpression(ConditionalExpression conditionalExpression, object data) {
			return base.VisitConditionalExpression(conditionalExpression, data);
		}
		
		public virtual object TrackedVisitConstructorDeclaration(ConstructorDeclaration constructorDeclaration, object data) {
			return base.VisitConstructorDeclaration(constructorDeclaration, data);
		}
		
		public virtual object TrackedVisitConstructorInitializer(ConstructorInitializer constructorInitializer, object data) {
			return base.VisitConstructorInitializer(constructorInitializer, data);
		}
		
		public virtual object TrackedVisitContinueStatement(ContinueStatement continueStatement, object data) {
			return base.VisitContinueStatement(continueStatement, data);
		}
		
		public virtual object TrackedVisitDeclareDeclaration(DeclareDeclaration declareDeclaration, object data) {
			return base.VisitDeclareDeclaration(declareDeclaration, data);
		}
		
		public virtual object TrackedVisitDefaultValueExpression(DefaultValueExpression defaultValueExpression, object data) {
			return base.VisitDefaultValueExpression(defaultValueExpression, data);
		}
		
		public virtual object TrackedVisitDelegateDeclaration(DelegateDeclaration delegateDeclaration, object data) {
			return base.VisitDelegateDeclaration(delegateDeclaration, data);
		}
		
		public virtual object TrackedVisitDestructorDeclaration(DestructorDeclaration destructorDeclaration, object data) {
			return base.VisitDestructorDeclaration(destructorDeclaration, data);
		}
		
		public virtual object TrackedVisitDirectionExpression(DirectionExpression directionExpression, object data) {
			return base.VisitDirectionExpression(directionExpression, data);
		}
		
		public virtual object TrackedVisitDoLoopStatement(DoLoopStatement doLoopStatement, object data) {
			return base.VisitDoLoopStatement(doLoopStatement, data);
		}
		
		public virtual object TrackedVisitElseIfSection(ElseIfSection elseIfSection, object data) {
			return base.VisitElseIfSection(elseIfSection, data);
		}
		
		public virtual object TrackedVisitEmptyStatement(EmptyStatement emptyStatement, object data) {
			return base.VisitEmptyStatement(emptyStatement, data);
		}
		
		public virtual object TrackedVisitEndStatement(EndStatement endStatement, object data) {
			return base.VisitEndStatement(endStatement, data);
		}
		
		public virtual object TrackedVisitEraseStatement(EraseStatement eraseStatement, object data) {
			return base.VisitEraseStatement(eraseStatement, data);
		}
		
		public virtual object TrackedVisitErrorStatement(ErrorStatement errorStatement, object data) {
			return base.VisitErrorStatement(errorStatement, data);
		}
		
		public virtual object TrackedVisitEventAddRegion(EventAddRegion eventAddRegion, object data) {
			return base.VisitEventAddRegion(eventAddRegion, data);
		}
		
		public virtual object TrackedVisitEventDeclaration(EventDeclaration eventDeclaration, object data) {
			return base.VisitEventDeclaration(eventDeclaration, data);
		}
		
		public virtual object TrackedVisitEventRaiseRegion(EventRaiseRegion eventRaiseRegion, object data) {
			return base.VisitEventRaiseRegion(eventRaiseRegion, data);
		}
		
		public virtual object TrackedVisitEventRemoveRegion(EventRemoveRegion eventRemoveRegion, object data) {
			return base.VisitEventRemoveRegion(eventRemoveRegion, data);
		}
		
		public virtual object TrackedVisitExitStatement(ExitStatement exitStatement, object data) {
			return base.VisitExitStatement(exitStatement, data);
		}
		
		public virtual object TrackedVisitExpressionRangeVariable(ExpressionRangeVariable expressionRangeVariable, object data) {
			return base.VisitExpressionRangeVariable(expressionRangeVariable, data);
		}
		
		public virtual object TrackedVisitExpressionStatement(ExpressionStatement expressionStatement, object data) {
			return base.VisitExpressionStatement(expressionStatement, data);
		}
		
		public virtual object TrackedVisitExternAliasDirective(ExternAliasDirective externAliasDirective, object data) {
			return base.VisitExternAliasDirective(externAliasDirective, data);
		}
		
		public virtual object TrackedVisitFieldDeclaration(FieldDeclaration fieldDeclaration, object data) {
			return base.VisitFieldDeclaration(fieldDeclaration, data);
		}
		
		public virtual object TrackedVisitFixedStatement(FixedStatement fixedStatement, object data) {
			return base.VisitFixedStatement(fixedStatement, data);
		}
		
		public virtual object TrackedVisitForeachStatement(ForeachStatement foreachStatement, object data) {
			return base.VisitForeachStatement(foreachStatement, data);
		}
		
		public virtual object TrackedVisitForNextStatement(ForNextStatement forNextStatement, object data) {
			return base.VisitForNextStatement(forNextStatement, data);
		}
		
		public virtual object TrackedVisitForStatement(ForStatement forStatement, object data) {
			return base.VisitForStatement(forStatement, data);
		}
		
		public virtual object TrackedVisitGotoCaseStatement(GotoCaseStatement gotoCaseStatement, object data) {
			return base.VisitGotoCaseStatement(gotoCaseStatement, data);
		}
		
		public virtual object TrackedVisitGotoStatement(GotoStatement gotoStatement, object data) {
			return base.VisitGotoStatement(gotoStatement, data);
		}
		
		public virtual object TrackedVisitIdentifierExpression(IdentifierExpression identifierExpression, object data) {
			return base.VisitIdentifierExpression(identifierExpression, data);
		}
		
		public virtual object TrackedVisitIfElseStatement(IfElseStatement ifElseStatement, object data) {
			return base.VisitIfElseStatement(ifElseStatement, data);
		}
		
		public virtual object TrackedVisitIndexerDeclaration(IndexerDeclaration indexerDeclaration, object data) {
			return base.VisitIndexerDeclaration(indexerDeclaration, data);
		}
		
		public virtual object TrackedVisitIndexerExpression(IndexerExpression indexerExpression, object data) {
			return base.VisitIndexerExpression(indexerExpression, data);
		}
		
		public virtual object TrackedVisitInnerClassTypeReference(InnerClassTypeReference innerClassTypeReference, object data) {
			return base.VisitInnerClassTypeReference(innerClassTypeReference, data);
		}
		
		public virtual object TrackedVisitInterfaceImplementation(InterfaceImplementation interfaceImplementation, object data) {
			return base.VisitInterfaceImplementation(interfaceImplementation, data);
		}
		
		public virtual object TrackedVisitInvocationExpression(InvocationExpression invocationExpression, object data) {
			return base.VisitInvocationExpression(invocationExpression, data);
		}
		
		public virtual object TrackedVisitLabelStatement(LabelStatement labelStatement, object data) {
			return base.VisitLabelStatement(labelStatement, data);
		}
		
		public virtual object TrackedVisitLambdaExpression(LambdaExpression lambdaExpression, object data) {
			return base.VisitLambdaExpression(lambdaExpression, data);
		}
		
		public virtual object TrackedVisitLocalVariableDeclaration(LocalVariableDeclaration localVariableDeclaration, object data) {
			return base.VisitLocalVariableDeclaration(localVariableDeclaration, data);
		}
		
		public virtual object TrackedVisitLockStatement(LockStatement lockStatement, object data) {
			return base.VisitLockStatement(lockStatement, data);
		}
		
		public virtual object TrackedVisitMemberReferenceExpression(MemberReferenceExpression memberReferenceExpression, object data) {
			return base.VisitMemberReferenceExpression(memberReferenceExpression, data);
		}
		
		public virtual object TrackedVisitMethodDeclaration(MethodDeclaration methodDeclaration, object data) {
			return base.VisitMethodDeclaration(methodDeclaration, data);
		}
		
		public virtual object TrackedVisitNamedArgumentExpression(NamedArgumentExpression namedArgumentExpression, object data) {
			return base.VisitNamedArgumentExpression(namedArgumentExpression, data);
		}
		
		public virtual object TrackedVisitNamespaceDeclaration(NamespaceDeclaration namespaceDeclaration, object data) {
			return base.VisitNamespaceDeclaration(namespaceDeclaration, data);
		}
		
		public virtual object TrackedVisitObjectCreateExpression(ObjectCreateExpression objectCreateExpression, object data) {
			return base.VisitObjectCreateExpression(objectCreateExpression, data);
		}
		
		public virtual object TrackedVisitOnErrorStatement(OnErrorStatement onErrorStatement, object data) {
			return base.VisitOnErrorStatement(onErrorStatement, data);
		}
		
		public virtual object TrackedVisitOperatorDeclaration(OperatorDeclaration operatorDeclaration, object data) {
			return base.VisitOperatorDeclaration(operatorDeclaration, data);
		}
		
		public virtual object TrackedVisitOptionDeclaration(OptionDeclaration optionDeclaration, object data) {
			return base.VisitOptionDeclaration(optionDeclaration, data);
		}
		
		public virtual object TrackedVisitParameterDeclarationExpression(ParameterDeclarationExpression parameterDeclarationExpression, object data) {
			return base.VisitParameterDeclarationExpression(parameterDeclarationExpression, data);
		}
		
		public virtual object TrackedVisitParenthesizedExpression(ParenthesizedExpression parenthesizedExpression, object data) {
			return base.VisitParenthesizedExpression(parenthesizedExpression, data);
		}
		
		public virtual object TrackedVisitPointerReferenceExpression(PointerReferenceExpression pointerReferenceExpression, object data) {
			return base.VisitPointerReferenceExpression(pointerReferenceExpression, data);
		}
		
		public virtual object TrackedVisitPrimitiveExpression(PrimitiveExpression primitiveExpression, object data) {
			return base.VisitPrimitiveExpression(primitiveExpression, data);
		}
		
		public virtual object TrackedVisitPropertyDeclaration(PropertyDeclaration propertyDeclaration, object data) {
			return base.VisitPropertyDeclaration(propertyDeclaration, data);
		}
		
		public virtual object TrackedVisitPropertyGetRegion(PropertyGetRegion propertyGetRegion, object data) {
			return base.VisitPropertyGetRegion(propertyGetRegion, data);
		}
		
		public virtual object TrackedVisitPropertySetRegion(PropertySetRegion propertySetRegion, object data) {
			return base.VisitPropertySetRegion(propertySetRegion, data);
		}
		
		public virtual object TrackedVisitQueryExpression(QueryExpression queryExpression, object data) {
			return base.VisitQueryExpression(queryExpression, data);
		}
		
		public virtual object TrackedVisitQueryExpressionAggregateClause(QueryExpressionAggregateClause queryExpressionAggregateClause, object data) {
			return base.VisitQueryExpressionAggregateClause(queryExpressionAggregateClause, data);
		}
		
		public virtual object TrackedVisitQueryExpressionDistinctClause(QueryExpressionDistinctClause queryExpressionDistinctClause, object data) {
			return base.VisitQueryExpressionDistinctClause(queryExpressionDistinctClause, data);
		}
		
		public virtual object TrackedVisitQueryExpressionFromClause(QueryExpressionFromClause queryExpressionFromClause, object data) {
			return base.VisitQueryExpressionFromClause(queryExpressionFromClause, data);
		}
		
		public virtual object TrackedVisitQueryExpressionGroupClause(QueryExpressionGroupClause queryExpressionGroupClause, object data) {
			return base.VisitQueryExpressionGroupClause(queryExpressionGroupClause, data);
		}
		
		public virtual object TrackedVisitQueryExpressionGroupJoinVBClause(QueryExpressionGroupJoinVBClause queryExpressionGroupJoinVBClause, object data) {
			return base.VisitQueryExpressionGroupJoinVBClause(queryExpressionGroupJoinVBClause, data);
		}
		
		public virtual object TrackedVisitQueryExpressionGroupVBClause(QueryExpressionGroupVBClause queryExpressionGroupVBClause, object data) {
			return base.VisitQueryExpressionGroupVBClause(queryExpressionGroupVBClause, data);
		}
		
		public virtual object TrackedVisitQueryExpressionJoinClause(QueryExpressionJoinClause queryExpressionJoinClause, object data) {
			return base.VisitQueryExpressionJoinClause(queryExpressionJoinClause, data);
		}
		
		public virtual object TrackedVisitQueryExpressionJoinConditionVB(QueryExpressionJoinConditionVB queryExpressionJoinConditionVB, object data) {
			return base.VisitQueryExpressionJoinConditionVB(queryExpressionJoinConditionVB, data);
		}
		
		public virtual object TrackedVisitQueryExpressionJoinVBClause(QueryExpressionJoinVBClause queryExpressionJoinVBClause, object data) {
			return base.VisitQueryExpressionJoinVBClause(queryExpressionJoinVBClause, data);
		}
		
		public virtual object TrackedVisitQueryExpressionLetClause(QueryExpressionLetClause queryExpressionLetClause, object data) {
			return base.VisitQueryExpressionLetClause(queryExpressionLetClause, data);
		}
		
		public virtual object TrackedVisitQueryExpressionLetVBClause(QueryExpressionLetVBClause queryExpressionLetVBClause, object data) {
			return base.VisitQueryExpressionLetVBClause(queryExpressionLetVBClause, data);
		}
		
		public virtual object TrackedVisitQueryExpressionOrderClause(QueryExpressionOrderClause queryExpressionOrderClause, object data) {
			return base.VisitQueryExpressionOrderClause(queryExpressionOrderClause, data);
		}
		
		public virtual object TrackedVisitQueryExpressionOrdering(QueryExpressionOrdering queryExpressionOrdering, object data) {
			return base.VisitQueryExpressionOrdering(queryExpressionOrdering, data);
		}
		
		public virtual object TrackedVisitQueryExpressionPartitionVBClause(QueryExpressionPartitionVBClause queryExpressionPartitionVBClause, object data) {
			return base.VisitQueryExpressionPartitionVBClause(queryExpressionPartitionVBClause, data);
		}
		
		public virtual object TrackedVisitQueryExpressionSelectClause(QueryExpressionSelectClause queryExpressionSelectClause, object data) {
			return base.VisitQueryExpressionSelectClause(queryExpressionSelectClause, data);
		}
		
		public virtual object TrackedVisitQueryExpressionSelectVBClause(QueryExpressionSelectVBClause queryExpressionSelectVBClause, object data) {
			return base.VisitQueryExpressionSelectVBClause(queryExpressionSelectVBClause, data);
		}
		
		public virtual object TrackedVisitQueryExpressionWhereClause(QueryExpressionWhereClause queryExpressionWhereClause, object data) {
			return base.VisitQueryExpressionWhereClause(queryExpressionWhereClause, data);
		}
		
		public virtual object TrackedVisitRaiseEventStatement(RaiseEventStatement raiseEventStatement, object data) {
			return base.VisitRaiseEventStatement(raiseEventStatement, data);
		}
		
		public virtual object TrackedVisitReDimStatement(ReDimStatement reDimStatement, object data) {
			return base.VisitReDimStatement(reDimStatement, data);
		}
		
		public virtual object TrackedVisitRemoveHandlerStatement(RemoveHandlerStatement removeHandlerStatement, object data) {
			return base.VisitRemoveHandlerStatement(removeHandlerStatement, data);
		}
		
		public virtual object TrackedVisitResumeStatement(ResumeStatement resumeStatement, object data) {
			return base.VisitResumeStatement(resumeStatement, data);
		}
		
		public virtual object TrackedVisitReturnStatement(ReturnStatement returnStatement, object data) {
			return base.VisitReturnStatement(returnStatement, data);
		}
		
		public virtual object TrackedVisitSizeOfExpression(SizeOfExpression sizeOfExpression, object data) {
			return base.VisitSizeOfExpression(sizeOfExpression, data);
		}
		
		public virtual object TrackedVisitStackAllocExpression(StackAllocExpression stackAllocExpression, object data) {
			return base.VisitStackAllocExpression(stackAllocExpression, data);
		}
		
		public virtual object TrackedVisitStopStatement(StopStatement stopStatement, object data) {
			return base.VisitStopStatement(stopStatement, data);
		}
		
		public virtual object TrackedVisitSwitchSection(SwitchSection switchSection, object data) {
			return base.VisitSwitchSection(switchSection, data);
		}
		
		public virtual object TrackedVisitSwitchStatement(SwitchStatement switchStatement, object data) {
			return base.VisitSwitchStatement(switchStatement, data);
		}
		
		public virtual object TrackedVisitTemplateDefinition(TemplateDefinition templateDefinition, object data) {
			return base.VisitTemplateDefinition(templateDefinition, data);
		}
		
		public virtual object TrackedVisitThisReferenceExpression(ThisReferenceExpression thisReferenceExpression, object data) {
			return base.VisitThisReferenceExpression(thisReferenceExpression, data);
		}
		
		public virtual object TrackedVisitThrowStatement(ThrowStatement throwStatement, object data) {
			return base.VisitThrowStatement(throwStatement, data);
		}
		
		public virtual object TrackedVisitTryCatchStatement(TryCatchStatement tryCatchStatement, object data) {
			return base.VisitTryCatchStatement(tryCatchStatement, data);
		}
		
		public virtual object TrackedVisitTypeDeclaration(TypeDeclaration typeDeclaration, object data) {
			return base.VisitTypeDeclaration(typeDeclaration, data);
		}
		
		public virtual object TrackedVisitTypeOfExpression(TypeOfExpression typeOfExpression, object data) {
			return base.VisitTypeOfExpression(typeOfExpression, data);
		}
		
		public virtual object TrackedVisitTypeOfIsExpression(TypeOfIsExpression typeOfIsExpression, object data) {
			return base.VisitTypeOfIsExpression(typeOfIsExpression, data);
		}
		
		public virtual object TrackedVisitTypeReference(TypeReference typeReference, object data) {
			return base.VisitTypeReference(typeReference, data);
		}
		
		public virtual object TrackedVisitTypeReferenceExpression(TypeReferenceExpression typeReferenceExpression, object data) {
			return base.VisitTypeReferenceExpression(typeReferenceExpression, data);
		}
		
		public virtual object TrackedVisitUnaryOperatorExpression(UnaryOperatorExpression unaryOperatorExpression, object data) {
			return base.VisitUnaryOperatorExpression(unaryOperatorExpression, data);
		}
		
		public virtual object TrackedVisitUncheckedExpression(UncheckedExpression uncheckedExpression, object data) {
			return base.VisitUncheckedExpression(uncheckedExpression, data);
		}
		
		public virtual object TrackedVisitUncheckedStatement(UncheckedStatement uncheckedStatement, object data) {
			return base.VisitUncheckedStatement(uncheckedStatement, data);
		}
		
		public virtual object TrackedVisitUnsafeStatement(UnsafeStatement unsafeStatement, object data) {
			return base.VisitUnsafeStatement(unsafeStatement, data);
		}
		
		public virtual object TrackedVisitUsing(Using @using, object data) {
			return base.VisitUsing(@using, data);
		}
		
		public virtual object TrackedVisitUsingDeclaration(UsingDeclaration usingDeclaration, object data) {
			return base.VisitUsingDeclaration(usingDeclaration, data);
		}
		
		public virtual object TrackedVisitUsingStatement(UsingStatement usingStatement, object data) {
			return base.VisitUsingStatement(usingStatement, data);
		}
		
		public virtual object TrackedVisitVariableDeclaration(VariableDeclaration variableDeclaration, object data) {
			return base.VisitVariableDeclaration(variableDeclaration, data);
		}
		
		public virtual object TrackedVisitWithStatement(WithStatement withStatement, object data) {
			return base.VisitWithStatement(withStatement, data);
		}
		
		public virtual object TrackedVisitYieldStatement(YieldStatement yieldStatement, object data) {
			return base.VisitYieldStatement(yieldStatement, data);
		}
	}
}
