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

		using System;
		using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
		using System.Collections.Generic;

		/// <summary>
		/// This method adjusts the MethodInvoArgs - context parameter to replace
		/// the Expressions with reference to IXPathNavigable parameters with
		/// Expressions with references to the convert locals as determined by the
		/// IDArgRefs and IDLocalRefs context parameters.
		/// </summary>
		public class CorrectIDictionaryMethodInvocationParamsStage : IDynamicAdapterBuilderStage
		{
			#region IDynamicAdapterBuilderStage Members

			public void DoWork(AdapterBuilderStageContext context)
			{
				//Get parameters
				List<int> convertedArgs = context["IDArgRefs"] as List<int>;
				List<LocalReference> locals = context["IDLocalRefs"] as List<LocalReference>;
				List<Expression> actualArgs = context["MethodInvoArgs"] as List<Expression>;
				
				int i = 0;
				//For each argument
				for (int j = 0; j < actualArgs.Count; j++)
				{
					//If it has been converted
					if (convertedArgs.Contains(j))
					{
						//Change expression with reference to relevant local 
						actualArgs[j] = new ReferenceExpression(locals[i++]);
					}
				}
			}

			#endregion
		}
	}
}
