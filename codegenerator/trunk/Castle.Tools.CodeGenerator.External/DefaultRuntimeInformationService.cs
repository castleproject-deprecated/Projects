// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.Tools.CodeGenerator.External
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Threading;
	using External;

	public class DefaultRuntimeInformationService : IRuntimeInformationService
	{
		private readonly ReaderWriterLock @lock = new ReaderWriterLock();
		private readonly Dictionary<Type, Dictionary<MethodSignature, MethodInformation>> methods = 
			new Dictionary<Type, Dictionary<MethodSignature, MethodInformation>>();

		public MethodInformation ResolveMethodInformation(MethodSignature signature)
		{
			MethodInformation returned;

			@lock.AcquireReaderLock(-1);

			if (methods.ContainsKey(signature.Type))
				if (methods[signature.Type].ContainsKey(signature))
				{
					returned = methods[signature.Type][signature];
					@lock.ReleaseLock();
					return returned;
				}
			
			@lock.UpgradeToWriterLock(-1);

			if (!methods.ContainsKey(signature.Type))
				methods[signature.Type] = new Dictionary<MethodSignature, MethodInformation>();
			
			var method = signature.Type.GetMethod(
				signature.Name, BindingFlags.Public | BindingFlags.Instance, null, signature.Types, new ParameterModifier[0]);
			
			if (method == null)
			{
				@lock.ReleaseLock();
				throw new MissingMethodException(
					String.Format("Missing method: '{0}' on '{1}'", signature.Name, signature.Type.FullName));
			}

			returned = new MethodInformation(method, GetParameters(method), method.GetCustomAttributes(true));
			methods[signature.Type][signature] = returned;

			@lock.ReleaseLock();

			return returned;
		}

		public MethodInformation ResolveMethodInformation(Type type, string name, Type[] types)
		{
			return ResolveMethodInformation(new MethodSignature(type, name, types));
		}

		protected virtual ParameterInformation[] GetParameters(MethodInfo method)
		{
			var parameters = new List<ParameterInformation>();
			
			foreach (var parameterInfo in method.GetParameters())
				parameters.Add(new ParameterInformation(parameterInfo, parameterInfo.GetCustomAttributes(true)));
			
			return parameters.ToArray();
		}
	}
}