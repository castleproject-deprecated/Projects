

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
