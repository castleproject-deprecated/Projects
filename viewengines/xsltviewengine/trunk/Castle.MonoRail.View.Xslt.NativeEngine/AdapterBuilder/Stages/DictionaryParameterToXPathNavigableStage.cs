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
		using System.Collections;
		using System.Collections.Generic;
		using System.Reflection;
		using System.Xml.XPath;

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
