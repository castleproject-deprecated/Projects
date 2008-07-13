#region Licence

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

#endregion

namespace Castle.MonoRail.TypedActions
{
	using System;
	using System.Collections.Generic;
	using System.Linq.Expressions;
	using System.Reflection;
	using Castle.MonoRail.Framework.Descriptors;
	using Castle.MonoRail.Framework.Services.Utils;
	using Framework;

	public class DefaultActionMethodBuilder : IActionMethodBuilder
	{
		public DefaultActionMethodBuilder(IActionParametersBuilder parametersBuilder)
		{
			ParametersBuilder = parametersBuilder;
		}

		public IActionParametersBuilder ParametersBuilder { get; set; }


		public virtual ActionMethodModel BuildAction<TController>(Expression<Action<TController>> expression)
			where TController : IController
		{
			var methodCall = expression.Body as MethodCallExpression;
			if (methodCall == null)
			{
				throw new TypedActionException(
					"The expression body must be a MethodCallExpression, " +
					"that is, a lamda in the form of 'c => c.Action(optional, action, parameters)', " +
					"where the action parameters can be anything you want (variable, property, method call etc.) " +
					"as long as they return the right parameter and the function will compile.");
			}

			var actionMethod = methodCall.Method;
			if (false == actionMethod.DeclaringType.IsAssignableFrom(typeof(TController)))
			{
				throw new TypedActionException(
					"Expression body must be a lambda with a call to a action method on the controller type '{0}', " +
					"and can not be a call to '{1}.{2}'.",
					typeof(TController).FullName, actionMethod.DeclaringType.FullName, actionMethod.Name);
			}

			var descriptor = GetDescriptor(typeof(TController));
			var model = new ActionMethodModel(descriptor, actionMethod);
			SetActionParameters(model, methodCall);

			return model;
		}


		protected virtual ControllerDescriptor GetDescriptor(Type controllerType)
		{
			return ControllerInspectionUtil.Inspect(controllerType);
		}

		protected virtual void SetActionParameters(ActionMethodModel actionModel, MethodCallExpression methodCall)
		{
			var givenValues = GetGivenParameterValues(methodCall);

			var parameters = ParametersBuilder.BuildParameters(givenValues);
			actionModel.SetParameters(parameters);
		}

		/// <summary>
		/// Gets the given parameter values from the Linq <paramref name="methodCall"/>.
		/// <para>
		/// It calls <see cref="GetGivenParameterValue"/> for each parameter 
		/// that is passed to the action expression.
		/// </para>
		/// </summary>
		/// <param name="methodCall">The expression that represents 
		/// calling the action method with the needed parameters.</param>
		/// <returns></returns>
		protected virtual Dictionary<ParameterInfo, object> GetGivenParameterValues(MethodCallExpression methodCall)
		{
			var param2value = new Dictionary<ParameterInfo, object>();

			if (methodCall.Arguments == null || methodCall.Arguments.Count == 0)
			{
				return param2value;
			}

			var parameters = methodCall.Method.GetParameters();

			for(int i = 0; i < parameters.Length; i++)
			{
				Expression argExp = methodCall.Arguments[i];
				object argValue = GetGivenParameterValue(argExp);
				param2value.Add(parameters[i], argValue);
			}
			return param2value;
		}

		/// <summary>
		/// Gets the given parameter value from the Linq expression <paramref name="param"/>.
		/// <para>
		/// It the expression is a ConstantExpression, it returns the constant value.
		/// Otherwise it compiles the expression and invokes it in order to return the value.
		/// </para>
		/// </summary>
		/// <param name="param">The expression that represents the parameter.</param>
		/// <returns></returns>
		/// <seealso href="http://blog.bittercoder.com/PermaLink,guid,d1831805-dbf7-4b74-a6fd-2e9ed437c3d9.aspx"/>
		/// <seealso href="http://blog.bittercoder.com/PermaLink,guid,206e64d1-29ae-4362-874b-83f5b103727f.aspx"/>
		/// <seealso href="http://blechie.com/WPierce/archive/2007/12/29/A-Better-Dictionary-Initializer.aspx"/>
		/// <seealso href="http://blechie.com/WPierce/archive/2007/12/30/Updated-A-Better-Dictionary-Initializer.aspx"/>
		protected virtual object GetGivenParameterValue(Expression param)
		{
			ConstantExpression constant = param as ConstantExpression;
			if (constant != null)
			{
				return constant.Value;
			}

			return Expression.Lambda(param).Compile().DynamicInvoke();
		}
	}
}