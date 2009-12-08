// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace SolutionTransform
{
	using System;
	using SolutionTransform.CodingStandards;

	public class StandardTransforms
	{
		public static ITransform SilverlightTransform() {
			return new CompositeTransform(
				new MainSolutionTransform(),
				new AddDefineConstant("SILVERLIGHT"),
				new RemoveDefineConstant("DOTNET35"),
				new ReferenceMapTransform()
					{
						{ "System", "mscorlib", "system" },
						{ "System.Data" },
						{ "System.Data.DataSetExtensions" },
						{ "System.Web" },
						{ "System.Configuration" },
						{ "System.Runtime.Remoting" },
					},
				new AddTarget(@"$(MSBuildExtensionsPath32)\Microsoft\Silverlight\v3.0\Microsoft.Silverlight.CSharp.targets"),
				new RemoveTarget(@"$(MSBuildToolsPath)\Microsoft.CSharp.targets")
				// ,new NameTransform(rename)
				);
		}


		public static ITransform CastleStandardsTransform()
		{
			return new StandardizeTransform(new CastleStandardizer());
		}
	}
}
