

namespace Castle.MonoRail.View.Xslt.NativeEngine.AdapterBuilder
{
	namespace Stages
	{
		using System.Xml.XPath;
		using System.Collections;
		using System;
		using Castle.DynamicProxy.Generators.Emitters;
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
