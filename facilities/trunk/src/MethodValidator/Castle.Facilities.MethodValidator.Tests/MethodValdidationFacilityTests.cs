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

namespace Castle.Facilities.MethodValidator.Tests
{

	using System.Reflection;
	using Castle.Components.Validator;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.ModelBuilder;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;
	using Castle.Windsor.Installer;
	using NUnit.Framework;

	[TestFixture]
	public class MethodValdidationFacilityTests
	{
		private IKernel kernel;

		[SetUp]
		public void Setup()
		{
			kernel = new DefaultKernel();
			kernel.AddFacility<MethodValidatorFacility>();
			kernel.Register(Component.For<IOrderService>().ImplementedBy<OrderService>());
			//Add the WindsorContainer so we have an IProxyFactory instance
			WindsorContainer container = new WindsorContainer(kernel, new DefaultComponentInstaller());
		}

		[Test]
		public void FacilityRegistersRequiredComponents()
		{
			Assert.IsTrue(kernel.HasComponent(typeof(MethodValidatorMetaStore)));
			Assert.IsTrue(kernel.HasComponent(typeof(MethodValidatorInterceptor)));
			IContributeComponentModelConstruction methodValidatorContributor = null;
			foreach (IContributeComponentModelConstruction contributor in kernel.ComponentModelBuilder.Contributors)
			{
				if (contributor is MethodValidatorComponentInspector)
					methodValidatorContributor = contributor;
			}
			Assert.IsNotNull(methodValidatorContributor);
		}

		[Test]
		public void CreatesMetaForValidatableComponent()
		{
			MethodValidatorMetaStore metaStore = kernel.Resolve<MethodValidatorMetaStore>();
			MethodValidatorMetaInfo meta = metaStore.GetMetaFor(typeof(IOrderService));

			MethodInfo getOrder = typeof(IOrderService).GetMethod("GetOrder");
			Assert.IsNotNull(getOrder);
			Assert.AreEqual(2, meta.Methods.Count);
			Assert.IsTrue(meta.HasBuilders(getOrder));

			MethodInfo createOrder = typeof(IOrderService).GetMethod("CreateOrder");
			Assert.IsNotNull(createOrder);
			Assert.IsFalse(meta.HasBuilders(createOrder));
		}

		[Test, ExpectedException(typeof(FacilityException))]
		public void ThrowsOnNonVirtualClassProxyWithValidator()
		{
			kernel.Register(Component.For<CustomerService>());
		}

		[Test, ExpectedException(typeof(ValidationException))]
		public void CanRunSimpleValidators()
		{
			IOrderService orderService = kernel.Resolve<IOrderService>();
			orderService.GetOrder(-1, -1);
		}


	}
}
