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

namespace Castle.Tools.CodeGenerator.Services
{
	using System;

	public interface INamingService
	{
		string ToActionWrapperName(string name);
		string ToAreaWrapperName(string name);
		string ToClassName(string name);
		string ToControllerName(string name);
		string ToControllerWrapperName(string name);
		string ToMemberVariableName(string name);
		string ToMethodSignatureName(string name, string[] types);
		string ToMethodSignatureName(string name, Type[] types);
		string ToPropertyName(string name);
		string ToRouteWrapperName(string name);
		string ToVariableName(string name);
		string ToViewWrapperName(string name);
		string ToWizardStepWrapperName(string name);
	}
}