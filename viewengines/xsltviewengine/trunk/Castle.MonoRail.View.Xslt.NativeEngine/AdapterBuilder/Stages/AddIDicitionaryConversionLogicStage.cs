
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
		using System.Reflection;


	/// <summary>
	/// This stage adds code to the adapter to convert an incomming IXPathNavigable
	/// to a HashTable it does this by calling the base method "GetDictionary" on the
	/// AdapterBase class.
	/// This stage sets the IDLocalRefs-context parameter to a list local variables
	/// that are assigned the converted IXPathNavigables.
	/// </summary>
		public class AddIDicitionaryConversionLogicStage : IDynamicAdapterBuilderStage
		{
			#region IDynamicAdapterBuilderStage Members

			public void DoWork(AdapterBuilderStageContext context)
			{
				//First declare some locals (with type IDictionary) for each converted parameter
				List<int> convertedArgs = context["IDArgRefs"] as List<int>;
				List<LocalReference> locals = convertedArgs.ConvertAll<LocalReference>(delegate(int aref)
				{
					return context.NewMethodEmitter.CodeBuilder.DeclareLocal(typeof(IDictionary));
				});

				int j = 0;
				//For each converted parameter
				foreach (int index in convertedArgs)
				{
					//Invoke the GetDictionary method on the relevant parameter and assign the result to the local
					MethodInvocationExpression invocation = 
						new MethodInvocationExpression(SelfReference.Self,
							_GetDictionaryMethod,
							new ReferenceExpression(context.NewMethodEmitter.Arguments[index]));
					//Virtual call
					invocation.VirtualCall = true;
					context.NewMethodEmitter.CodeBuilder.AddStatement(
						new AssignStatement(locals[j++], invocation));

				}
				//Store the local reference for usage in other stages
				context["IDLocalRefs"] = locals;

			}

			private static readonly MethodInfo _GetDictionaryMethod = typeof(AdapterBase).GetMethod("GetDictionary");

			#endregion
		}
	}
}
