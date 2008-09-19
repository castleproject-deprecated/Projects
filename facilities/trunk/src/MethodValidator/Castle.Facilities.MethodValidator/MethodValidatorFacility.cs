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

using Castle.Components.Validator;

namespace Castle.Facilities.MethodValidator
{

	using Castle.MicroKernel.Facilities;

	public class MethodValidatorFacility : AbstractFacility
	{
		/// <summary>
		/// Initializes the MethodValidatorFacility
		/// </summary>
		protected override void Init()
		{
			IValidatorRegistry registry;
			if (Kernel.HasComponent(typeof(IValidatorRegistry)))
				registry = Kernel.Resolve<IValidatorRegistry>();
			else
				registry = new CachedValidationRegistry();

			IValidatorRegistry adapter = new ParameterValidatorRegistryAdapter(registry);
			Kernel.AddComponentInstance("methodValidator.metaStore", new MethodValidatorMetaStore(adapter));

			Kernel.AddComponent("methodValidator.interceptor", typeof(MethodValidatorInterceptor));
			Kernel.AddComponent("methodValidator.contributor", typeof(MethodValidationContributor));
			Kernel.ComponentModelBuilder.AddContributor(new MethodValidatorComponentInspector());
		}
	}
}
