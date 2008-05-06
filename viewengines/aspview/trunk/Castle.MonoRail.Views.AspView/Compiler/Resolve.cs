using System;
using System.Collections.Generic;
using Castle.MonoRail.Views.AspView.Compiler.MarkupTransformers;
using Castle.MonoRail.Views.AspView.Compiler.PreCompilationSteps;

namespace Castle.MonoRail.Views.AspView.Compiler
{
	public static class Resolve
	{
		static Type preCompilationStepsProviderType;
		static Type markupTransformersProviderType;

		static Resolve()
		{
			InitializeToDefaults();
		}

		public static IPreCompilationStepsProvider PreCompilationStepsProvider
		{
			get { return (IPreCompilationStepsProvider)Activator.CreateInstance(preCompilationStepsProviderType); }
		}

		public static IMarkupTransformersProvider MarkupTransformersProvider
		{
			get { return (IMarkupTransformersProvider)Activator.CreateInstance(markupTransformersProviderType); }
		}

		public static void Initialize(IDictionary<Type, Type> customProviders)
		{
			InitializeToDefaults();
			
			if (customProviders == null)
				return;

			if (customProviders.ContainsKey(typeof(IPreCompilationStepsProvider)))
				preCompilationStepsProviderType = customProviders[typeof(IPreCompilationStepsProvider)];

			if (customProviders.ContainsKey(typeof(IMarkupTransformersProvider)))
				markupTransformersProviderType = customProviders[typeof(IMarkupTransformersProvider)];
		}

		static void InitializeToDefaults()
		{
			preCompilationStepsProviderType = typeof(DefaultPreCompilationStepsProvider);
			markupTransformersProviderType = typeof(DefaultMarkupTransformersProvider);
		}
	}
}
