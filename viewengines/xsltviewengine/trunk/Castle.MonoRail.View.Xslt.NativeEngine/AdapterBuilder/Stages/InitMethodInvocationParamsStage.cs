
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
