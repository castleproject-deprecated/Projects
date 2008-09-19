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

using Castle.Facilities.MethodValidator.Attributes;

namespace Castle.Facilities.MethodValidator.Tests
{

	using System;
	using Castle.Components.Validator;

	public interface IComplexService
	{
		void MethodWithRefParameters(string set, string creditCard, ref string nonEmpty);

		void MethodWithOutParameters(string date, string dateTime, out string outValue);

		void MethodWithRefAndOutParameters(string @decimal, string @double, out string someValue, string email,
		                                   ref string integer);

		void MethodWithParams(params string[] range0To10);

		void GenericMethod<T>(string single, int range0To10, T range0to10);

		void MethodWithObject(string creditCard, ComplexObject @object);
	}

	public class ComplexService : IComplexService
	{

		public void MethodWithRefParameters(
			[ValidateSet("someValue", "secondValue")] string set,
			[ValidateCreditCard] string creditCard,
			[ValidateNonEmpty] ref string nonEmpty)
		{
		}

		public void MethodWithOutParameters(
			[ValidateDate] string date, 
			[ValidateDateTime] string dateTime,
			out string outValue)
		{
			outValue = "output";
		}

		public void MethodWithRefAndOutParameters(
			[ValidateDecimal] string @decimal,
			[ValidateDouble] string @double, 
			out string someValue, 
			[ValidateEmail] string email, 
			[ValidateInteger] ref string integer)
		{
			someValue = "someValue";
		}

		public void MethodWithParams(
			[ValidateRegExp("abc")] params string[] range0To10)
		{
		}

		public void GenericMethod<T>(
			[ValidateSingle] string single, 
			[ValidateRange(0, 10)] int range0To10,
			[ValidateRange(0, 10)] T range0to10)
		{
		}

		public void MethodWithObject(
			[ValidateCreditCard] string creditCard,
			[ValidateObject] ComplexObject @object)
		{
		}
	}

	public class ComplexObject
	{
		private int id = -1;

		[ValidateRange(0, 20)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		private string[] names;

		[ValidateCollectionNotEmpty]
		public string[] Names
		{
			get { return names; }
			set { names = value; }
		}
	}

}
