

namespace Castle.MonoRail.View.Xslt.NativeEngine.AdapterBuilder
{
	namespace Stages
	{

		using System.Xml.XPath;
		using System.Collections;
		using System;
		using Castle.DynamicProxy.Generators.Emitters;
		using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

		/// <summary>
		/// This stages adds code to return from the method.
		/// </summary>
		public class ReturnStage : IDynamicAdapterBuilderStage
		{

			public void DoWork(AdapterBuilderStageContext context)
			{
				context.NewMethodEmitter.CodeBuilder.AddStatement(
					new ReturnStatement(
						new ReturnReferenceExpression(context.NewMethodEmitter.ReturnType)));
			}

		}
	}
}
