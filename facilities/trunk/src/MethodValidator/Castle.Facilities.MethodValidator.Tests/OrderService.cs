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

	public interface IOrderService
	{

		int CreateOrder(Order order);

		Order GetOrder(int id, int customerId);

	}

	public class OrderService : IOrderService
	{
		public int CreateOrder(Order order)
		{
			return -1;
		}

		public Order GetOrder([ValidateRange(1, Int32.MaxValue)] int id, [ValidateRange(1, Int32.MaxValue)] int customerId)
		{
			Order order = new Order();
			order.Id = id;
			return order;
		}
	}

	public class Order
	{

		public int Id { get; set; }

	}
}