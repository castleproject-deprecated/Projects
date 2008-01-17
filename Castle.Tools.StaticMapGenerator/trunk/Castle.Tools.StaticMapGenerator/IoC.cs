#region license
// Copyright 2008 Ken Egozi http://www.kenegozi.com/blog
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
#endregion

namespace Castle.Tools.StaticMapGenerator
{
	using System.Reflection;
	using System;
	using System.Collections.Generic;

	static class IoC
	{
		static readonly IDictionary<Type, Type> types = new Dictionary<Type, Type>();

		public static void Register<TContract, TImplementation>()
		{
			types[typeof(TContract)] = typeof(TImplementation);
		}

		public static T Resolve<T>()
		{
			return (T)Resolve(typeof(T));
		}
		
		public static object Resolve(Type contract)
		{
			Type implementation = types[contract];

			ConstructorInfo constructor = implementation.GetConstructors()[0];

			ParameterInfo[] constructorParameters = constructor.GetParameters();

			if (constructorParameters.Length == 0)
				return Activator.CreateInstance(implementation);

			List<object> parameters = new List<object>(constructorParameters.Length);

			foreach (ParameterInfo parameterInfo in constructorParameters)
			{
				parameters.Add(Resolve(parameterInfo.ParameterType));
			}

			return constructor.Invoke(parameters.ToArray());
		}
	}
}