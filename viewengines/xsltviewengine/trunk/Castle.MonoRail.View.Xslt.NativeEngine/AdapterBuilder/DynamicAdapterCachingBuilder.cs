
namespace Castle.MonoRail.Framework.View.Xslt.NativeEngine.AdapterBuilder
{

	using System.Xml.XPath;
	using System.Collections;
	using System;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.MonoRail.Framework.View.Xslt.NativeEngine.AdapterBuilder.Stages;
	using System.Collections.Generic;
	using System.Reflection;
	using Castle.DynamicProxy;


	/// <summary>
	/// Class that can create adapters for specific objects
	/// and stores the generated types for later reuse.
	/// </summary>
	public class DynamicAdapterCachingBuilder
	{

		private ModuleScope _Module;

		/// <summary>
		/// Initializes a new instance of the DynamicAdapterStore class.
		/// </summary>
		public DynamicAdapterCachingBuilder()
		{
			_Module = new ModuleScope(false);
		}

		private object _cacheLock = new object();
		private IDictionary<Type, Type> _TypeAdapterCache = new Dictionary<Type, Type>();
		private IDictionary<Type, Type> TypeAdapterCache
		{
			get { return _TypeAdapterCache; }
			set { _TypeAdapterCache = value; }
		}

		public Type GetAdapter(Type typeToBeAdapted)
		{
			try
			{
				lock (_cacheLock)
				{
					Type adapterType;
					if (!TypeAdapterCache.TryGetValue(typeToBeAdapted, out adapterType))
					{
						DynamicAdapterBuilder builder = new DynamicAdapterBuilder(typeToBeAdapted, _Module);
						adapterType = builder.Build();
						TypeAdapterCache.Add(typeToBeAdapted, adapterType);
					}
					return adapterType;
				}
			}
			catch 
			{
				//On exception, reset module scope
				_Module = new ModuleScope(false);
				throw;
			}
		}

		public object Adapt(object original)
		{
			return Activator.CreateInstance(GetAdapter(original.GetType()), original);
		}

	}
}
