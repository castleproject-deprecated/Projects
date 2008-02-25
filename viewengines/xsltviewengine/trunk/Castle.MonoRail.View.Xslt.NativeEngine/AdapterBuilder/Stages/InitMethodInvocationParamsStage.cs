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
		using System.Collections.Generic;
		using Castle.DynamicProxy.Generators.Emitters.SimpleAST;


		/// <summary>
		/// This stage prepares a context-parameter MethodInvoArgs with
		/// a list of Expression-s that refer to the arguments of the 
		/// new method.
		/// </summary>
		public class InitMethodInvocationParamsStage : IDynamicAdapterBuilderStage
		{

			public void DoWork(AdapterBuilderStageContext context)
			{
				List<Expression> arguments = new List<Expression>();
				
				//For each parameter in the new method
				foreach (ArgumentReference reference in context.NewMethodEmitter.Arguments)
				{
					//create new referenceexpression
					arguments.Add(new ReferenceExpression(reference));
				}

				context["MethodInvoArgs"] = arguments;
			}

		}
	}
}
