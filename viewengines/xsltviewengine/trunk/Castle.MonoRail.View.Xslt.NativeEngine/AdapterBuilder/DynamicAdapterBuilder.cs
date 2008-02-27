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
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using Castle.DynamicProxy;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.MonoRail.Framework.View.Xslt.NativeEngine.AdapterBuilder.Stages;


	/// <summary>
	/// Class that builds an Adapter for the type specified
	/// at construction.
	/// 
	/// The following stages are used for every method.
	/// 
	///SetupEmitterStage
	///CopyReturnTypeStage
	///DictionaryParameterToXPathNavigableStage
	///AddIDicitionaryConversionLogicStage
	///InitMethodInvocationParamsStage
	///CorrectIDictionaryMethodInvocationParamsStage
	///InvokeMethodStage
	///ReturnStage
	/// </summary>
	class DynamicAdapterBuilder
	{

		private List<IDynamicAdapterBuilderStage> stagePipeline = new List<IDynamicAdapterBuilderStage>();
		/// <summary>
		/// Initializes a new instance of the DynamicAdapterBuilder class.
		/// </summary>
		/// <param name="originalObject">The type of object to build an adapter for</param>
		public DynamicAdapterBuilder(Type originalObject, ModuleScope scope)
		{
			OriginalObject = originalObject;
			ModuleScope = scope;

			stagePipeline.Add(new SetupEmitterStage());
			stagePipeline.Add(new CopyReturnTypeStage());
			stagePipeline.Add(new DictionaryParameterToXPathNavigableStage());
			stagePipeline.Add(new AddIDicitionaryConversionLogicStage());
			stagePipeline.Add(new InitMethodInvocationParamsStage());
			stagePipeline.Add(new CorrectIDictionaryMethodInvocationParamsStage());
			stagePipeline.Add(new InvokeMethodStage());
			stagePipeline.Add(new ReturnStage());
		}

		private ModuleScope _ModuleScope;
		public ModuleScope ModuleScope
		{
			get { return _ModuleScope; }
			private set { _ModuleScope = value; }
		}

		private Type _OriginalObject;
		public Type OriginalObject
		{
			get { return _OriginalObject; }
			private set { _OriginalObject = value; }
		}

		private void WalkPipeline(AdapterBuilderStageContext context)
		{
			foreach (IDynamicAdapterBuilderStage stage in stagePipeline)
			{
				stage.DoWork(context);
			}
		}

		/// <summary>
		/// Builds the adapter.
		/// </summary>
		/// <returns></returns>
		public Type Build()
		{

			//Setup emitter 
			ClassEmitter classEmitter = new ClassEmitter(ModuleScope,
				OriginalObject.Name + "Adapter",
				typeof(AdapterBase),
				new Type[] { }, TypeAttributes.Class,
				true);

			//Add a field to hold a reference to the original object that is being adapter.
			FieldReference adaptedObjectReference = classEmitter.CreateField("_Original", OriginalObject);

			//Add a constructor that accepts a reference to the original object and
			//assigns that reference to the field.
			ArgumentReference parameter = new ArgumentReference(OriginalObject);
			ConstructorEmitter constructor = classEmitter.CreateConstructor(parameter);
			constructor.CodeBuilder.AddStatement(
				new AssignStatement(adaptedObjectReference, new ReferenceExpression(parameter)));
			constructor.CodeBuilder.AddStatement(new ReturnStatement());

			//For each method, walk the pipeline
			foreach (MethodInfo method in OriginalObject.GetMethods())
			{
				AdapterBuilderStageContext context =
					new AdapterBuilderStageContext(OriginalObject, classEmitter, adaptedObjectReference, method);
				WalkPipeline(context);
			}

			//build the type			
			return classEmitter.BuildType();

		}
	}
}