namespace Castle.MonoRail.View.Xslt.NativeEngine.AdapterBuilder
{
	using System.Xml.XPath;
	using System.Collections;
	using System;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	/// <summary>
	/// Interface that should be implemented by all AdapterStages. Each is responsible
	/// for a single task in building/emitting an adapted method.
	/// </summary>
	public interface IDynamicAdapterBuilderStage
	{
		/// <summary>
		/// This method is the entry point to starting the stage.
		/// </summary>
		/// <param name="context">The context the stage should operate in</param>
		void DoWork(AdapterBuilderStageContext context);
	}
}
