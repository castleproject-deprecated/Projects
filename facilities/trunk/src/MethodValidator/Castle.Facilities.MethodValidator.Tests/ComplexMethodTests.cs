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

	using System;
	using Castle.Components.Validator;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;
	using Castle.Windsor.Installer;
	using NUnit.Framework;


	[TestFixture]
	public class ComplexMethodTests
	{
		private IKernel kernel;
		private IComplexService service;

		[SetUp]
		public void Setup()
		{
			kernel = new DefaultKernel();
			kernel.AddFacility<MethodValidatorFacility>();
			kernel.Register(Component.For<IComplexService>().ImplementedBy<ComplexService>());
			//Add the WindsorContainer so we have an IProxyFactory instance
			WindsorContainer container = new WindsorContainer(kernel, new DefaultComponentInstaller());
			service = kernel.Resolve<IComplexService>();
		}

		[Test]
		public void MethodWithOutParameters()
		{
			string outValue;
			ExpectFail(delegate
			           	{
			           		service.MethodWithOutParameters("invalid", "invalid", out outValue);
			           	}, 2);

			service.MethodWithOutParameters("2008-01-01", "2008-01-01", out outValue);
		}

		[Test]
		public void MethodWithParams()
		{
			ExpectFail(delegate
			           	{
			           		service.MethodWithParams("11", "12", "13");
			           	}, 3);

			ExpectFail(delegate
			           	{
			           		service.MethodWithParams("abc", "2", "abc");
			           	}, 1);

			ExpectFail(delegate
			{
				service.MethodWithParams("a2bZc");
			}, 1);

			service.MethodWithParams("abc", "abc", "abc");
			service.MethodWithParams("abc");
			service.MethodWithParams();
		}

		[Test]
		public void MethodWithRef()
		{

			string nonEmpty = "";
			ExpectFail(delegate
			           	{
			           		service.MethodWithRefParameters("invalid", "invalid", ref nonEmpty);
			           	}, 3);

			nonEmpty = "value";
			service.MethodWithRefParameters("", "4111-1111-1111-1111", ref nonEmpty);
		}

		[Test]
		public void MethodWithOutAndRefParams()
		{
			string invalidInteger = "invalidInteger";
			string someValue;
			ExpectFail(delegate
			           	{

							service.MethodWithRefAndOutParameters("invalid", "invalid", out someValue, "notValid", ref invalidInteger);
			           	}, 4);

			string validInteger = "50";
			service.MethodWithRefAndOutParameters("2.0", "3.0", out someValue, "eric@erichauser.net", ref validInteger);
			Assert.AreEqual("someValue", someValue);
		}

		[Test]
		public void GenericMethod()
		{
			ExpectFail(delegate
			           	{
			           		service.GenericMethod("invalid", 30, "invalid");
			           	}, 3);

			service.GenericMethod("10", 5, "5");
		}

		[Test]
		public void MethodWithObjectValidator()
		{
			ExpectFail(delegate
			           	{
			           		service.MethodWithObject("invalid", new ComplexObject());
			           	}, 3);

			ComplexObject complex = new ComplexObject();
			complex.Id = 1;

			ExpectFail(delegate
			           	{
			           		service.MethodWithObject("4111-1111-1111-1111", complex);
			           	}, 1);

			complex.Names = new string[] { "valid" };

			service.MethodWithObject("4111-1111-1111-1111", complex);
		}

		private void ExpectFail(Action action, int count)
		{
			ValidationException error = null;
			try
			{
				action();
			}
			catch (ValidationException e)
			{
				error = e;
			}
			Assert.IsNotNull(error);
			Assert.AreEqual(count, error.ValidationErrorMessages.Length, 
				string.Format("Error messages: {0}", string.Join(",", error.ValidationErrorMessages)));
		}
	}
}
