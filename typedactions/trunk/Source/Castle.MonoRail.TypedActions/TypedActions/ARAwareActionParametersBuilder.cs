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
	using ActiveRecordSupport;
	using Castle.ActiveRecord.Framework.Internal;

	/// <summary>
	/// Inspects the action parameters created by the <see cref="DecoratingActionParametersBuilder.InnerBuilder"/>
	/// for ActiveRecord attributes and models.
	/// <para />
	/// Currently only supports the <see cref="ARFetchAttribute"/>.
	/// </summary>
	public class ARAwareActionParametersBuilder : DecoratingActionParametersBuilder
	{
		private static readonly Type arFetchAttrType = typeof(ARFetchAttribute);

		public ARAwareActionParametersBuilder(IActionParametersBuilder innerBuilder)
			: base(innerBuilder)
		{
		}

		protected override IEnumerable<ActionParameterModel> Decorate(ActionParameterModel paramToDecorate)
		{
			if (paramToDecorate.ParamInfo.IsDefined(arFetchAttrType, false))
			{
				ARFetchAttribute attr = (ARFetchAttribute) paramToDecorate.ParamInfo.GetCustomAttributes(arFetchAttrType, false)[0];
				yield return GetFetchParameter(paramToDecorate, attr);
			}
			else
			{
				yield return paramToDecorate;
			}
		}

		protected virtual ActionParameterModel GetFetchParameter(ActionParameterModel originalParam,
		                                                         ARFetchAttribute fetchAttribute)
		{
			Type arType = originalParam.ParamInfo.ParameterType;
			var arModel = ActiveRecordModel.GetModel(arType);
			if (arModel == null)
			{
				return originalParam;
			}
			if (arModel.PrimaryKey == null)
			{
				return originalParam;
			}

			string paramName = fetchAttribute.RequestParameterName;

			object paramValue = null;
			if (originalParam.Value != null)
			{
				paramValue = arModel.PrimaryKey.Property.GetValue(originalParam.Value, null);
			}

			if (paramValue == null)
			{
				return originalParam;
			}

			return new ActionParameterModel(originalParam.ParamInfo, paramName, paramValue);
		}
	}
}