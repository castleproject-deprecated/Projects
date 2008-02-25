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
		/// This stage adds code to invoke the original method of the adapted object
		/// with the parameters supplied in the MethodInvoArgs context-parameter.
		/// </summary>
		public class InvokeMethodStage : IDynamicAdapterBuilderStage
		{
			public void DoWork(AdapterBuilderStageContext context)
			{
				context.NewMethodEmitter.CodeBuilder.AddStatement(
					new ExpressionStatement(
						new MethodInvocationExpression(
							context.AdaptedObjectRef,
							context.OriginalMethod,
							(context["MethodInvoArgs"] as List<Expression>).ToArray())));
			}
		}
	}
}
