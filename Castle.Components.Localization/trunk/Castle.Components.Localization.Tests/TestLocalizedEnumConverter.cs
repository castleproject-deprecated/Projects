
#region License

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
// See the License for the specific culture governing permissions and
// limitations under the License.

#endregion License

namespace Castle.Components.Localization.Tests
{
	#region Using Directives

	using System;

	using NUnit.Framework;
	using System.ComponentModel;

	#endregion Using Directives

	[TestFixture]
	public class TestLocalizedEnumConverter : AbstractTest
	{

		#region Public Methods 

		[Test]
		public void TestLocalizationByConfiguration()
		{
			MyOtherEnumeration enumeration = MyOtherEnumeration.Value1;

			string localized = LocalizedEnumConverter.ConvertValueToString( enumeration );

			Assert.AreEqual( Resources.MyResources.Value1, localized );
		}

		[Test]
		public void TestLocalizationByConfigurationOtherAssembly()
		{
			MyLocalizedInOtherAssemblyEnumeration enumeration = MyLocalizedInOtherAssemblyEnumeration.Value1;

			string localized = LocalizedEnumConverter.ConvertValueToString( enumeration );

			Assert.AreEqual( Resources.MyFarResources.Value1, localized );
		}

		[Test]
		public void TestLocalizationByConfigurationOtherNamingPattern()
		{
			MyNamedEnumeration enumeration = MyNamedEnumeration.Value1;

			string localized = LocalizedEnumConverter.ConvertValueToString( enumeration );

			Assert.AreEqual( Resources.MyResources2.MyNamedEnumeration_Value1, localized );
		}

		[Test]
		public void TestLocalizationByConvention()
		{
			MyEnumeration enumeration = MyEnumeration.Value1;

			string localized = LocalizedEnumConverter.ConvertValueToString( enumeration );

			Assert.AreEqual( Resources.MyEnumeration.Value1, localized );
		}

		[Test]
		public void TestLocalizationFlagedEnumeration()
		{
			MyFlagedEnumeration enumeration = MyFlagedEnumeration.Value1;
			MyFlagedEnumeration multipleValues = MyFlagedEnumeration.Value1 | MyFlagedEnumeration.Value2;
			
			string localized = LocalizedEnumConverter.ConvertValueToString( enumeration );
			string multipleValueLocalized = LocalizedEnumConverter.ConvertValueToString( multipleValues );

			Assert.AreEqual( Resources.MyResources.Value1, localized );
			Assert.AreEqual( string.Format( "{0}, {1}", Resources.MyResources.Value1, Resources.MyResources.Value2 ), multipleValueLocalized );
		}

		#endregion Public Methods 

	}
}
