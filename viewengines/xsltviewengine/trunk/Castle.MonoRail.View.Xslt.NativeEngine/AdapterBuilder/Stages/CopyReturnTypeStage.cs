

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
		/// This stage copies the return type of the original method to
		/// the new method
		/// </summary>
		public class CopyReturnTypeStage : IDynamicAdapterBuilderStage
		{
			#region IDynamicAdapterBuilderStage Members

			public void DoWork(AdapterBuilderStageContext context)
			{
				context.NewMethodEmitter.SetReturnType(context.OriginalMethod.ReturnType);
			}

			#endregion
		}
	}
}
