// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.View.Xslt.NativeEngine.AdapterBuilder
{

	namespace Stages
	{

		using System.Collections;
		using System;
		using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
		using System.Collections.Generic;
		using System.Reflection;


	/// <summary>
	/// This stage adds code to the adapter to convert an incomming IXPathNavigable
	/// to a HashTable it does this by calling the base method "GetDictionary" on the
	/// AdapterBase class.
	/// This stage sets the IDLocalRefs-context parameter to a list local variables
	/// that are assigned the converted IXPathNavigables.
	/// </summary>
		public class AddIDicitionaryConversionLogicStage : IDynamicAdapterBuilderStage
		{
			#region IDynamicAdapterBuilderStage Members

			public void DoWork(AdapterBuilderStageContext context)
			{
				//First declare some locals (with type IDictionary) for each converted parameter
				List<int> convertedArgs = context["IDArgRefs"] as List<int>;
				List<LocalReference> locals = convertedArgs.ConvertAll<LocalReference>(delegate(int aref)
				{
					return context.NewMethodEmitter.CodeBuilder.DeclareLocal(typeof(IDictionary));
				});

				int j = 0;
				//For each converted parameter
				foreach (int index in convertedArgs)
				{
					//Invoke the GetDictionary method on the relevant parameter and assign the result to the local
					MethodInvocationExpression invocation = 
						new MethodInvocationExpression(SelfReference.Self,
							_GetDictionaryMethod,
							new ReferenceExpression(context.NewMethodEmitter.Arguments[index]));
					//Virtual call
					invocation.VirtualCall = true;
					context.NewMethodEmitter.CodeBuilder.AddStatement(
						new AssignStatement(locals[j++], invocation));

				}
				//Store the local reference for usage in other stages
				context["IDLocalRefs"] = locals;

			}

			private static readonly MethodInfo _GetDictionaryMethod = typeof(AdapterBase).GetMethod("GetDictionary");

			#endregion
		}
	}
}
