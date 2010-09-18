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

namespace ICSharpCode.NRefactory.Ast
{
	using System;
	using System.Collections.Generic;

	public class LocalVariableDeclaration : Statement
	{
		TypeReference             typeReference;
		Modifiers                  modifier = Modifiers.None;
		List<VariableDeclaration> variables = new List<VariableDeclaration>();
		
		public TypeReference TypeReference {
			get {
				return typeReference;
			}
			set {
				typeReference = TypeReference.CheckNull(value);
				if (!typeReference.IsNull) typeReference.Parent = this;
			}
		}
		
		public Modifiers Modifier {
			get {
				return modifier;
			}
			set {
				modifier = value;
			}
		}
		
		public List<VariableDeclaration> Variables {
			get {
				return variables;
			}
		}
		
		public TypeReference GetTypeForVariable(int variableIndex)
		{
			if (!typeReference.IsNull) {
				return typeReference;
			}
			
			for (int i = variableIndex; i < Variables.Count;++i) {
				if (!((VariableDeclaration)Variables[i]).TypeReference.IsNull) {
					return ((VariableDeclaration)Variables[i]).TypeReference;
				}
			}
			return null;
		}
		
		public LocalVariableDeclaration(VariableDeclaration declaration) : this(TypeReference.Null)
		{
			Variables.Add(declaration);
		}
		
		public LocalVariableDeclaration(TypeReference typeReference)
		{
			TypeReference = typeReference;
		}
		
		public LocalVariableDeclaration(TypeReference typeReference, Modifiers modifier)
		{
			TypeReference = typeReference;
			this.modifier      = modifier;
		}
		
		public LocalVariableDeclaration(Modifiers modifier)
		{
			typeReference = TypeReference.Null;
			this.modifier      = modifier;
		}
		
		public VariableDeclaration GetVariableDeclaration(string variableName)
		{
			foreach (VariableDeclaration variableDeclaration in variables) {
				if (variableDeclaration.Name == variableName) {
					return variableDeclaration;
				}
			}
			return null;
		}
				
		public override object AcceptVisitor(IAstVisitor visitor, object data)
		{
			return visitor.VisitLocalVariableDeclaration(this, data);
		}
		
		public override string ToString()
		{
			return String.Format("[LocalVariableDeclaration: Type={0}, Modifier ={1} Variables={2}]", 
			                     typeReference, 
			                     modifier, 
			                     GetCollectionString(variables));
		}
	}
}
