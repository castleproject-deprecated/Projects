

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
		/// This stage sets up a new method emiter
		/// </summary>
		public class SetupEmitterStage : IDynamicAdapterBuilderStage
		{
			#region IDynamicAdapterBuilderStage Members

			public void DoWork(AdapterBuilderStageContext context)
			{
				context.NewMethodEmitter = context.ClassEmitter.CreateMethod(context.OriginalMethod.Name, context.OriginalMethod.Attributes);
			}

			#endregion
		}
	}
}
