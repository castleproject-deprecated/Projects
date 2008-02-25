using System.Xml.XPath;
using System.Collections;
using System;
using Castle.DynamicProxy.Generators.Emitters;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using System.Collections.Generic;
using System.Reflection;

namespace Castle.MonoRail.View.Xslt.NativeEngine.AdapterBuilder
{
	namespace Stages
	{

		/// <summary>
		/// This stage replaces all IDictionary parameters of the original method with a
		/// IXPathNavigable type. 
		/// The context parameter IDArgRefs will contain the indexes of all parameters
		/// that have been replaced.
		/// </summary>
		public class DictionaryParameterToXPathNavigableStage : IDynamicAdapterBuilderStage
		{
			#region IDynamicAdapterBuilderStage Members

			public void DoWork(AdapterBuilderStageContext context)
			{
				List<Type> correctedList = new List<Type>();
				List<int> argumentIndices = new List<int>();
				// Walk through all parameters of the original method
				foreach (ParameterInfo reference in context.OriginalMethod.GetParameters())
				{
					//If it is an IDictionary we register the index of the parameter and replace
					//it by an IXPathNavigable
					if (typeof(IDictionary).IsAssignableFrom(reference.ParameterType))
					{
						argumentIndices.Add(correctedList.Count);
						correctedList.Add(typeof(IXPathNavigable));
					}
						//else just copy the parameter type
					else
					{
						correctedList.Add(reference.ParameterType);
					}
				}

				context["IDArgRefs"] = argumentIndices;
				//Set the parameters of the adapter method
				context.NewMethodEmitter.SetParameters(correctedList.ToArray());
			}

			#endregion
		}
	}
}
