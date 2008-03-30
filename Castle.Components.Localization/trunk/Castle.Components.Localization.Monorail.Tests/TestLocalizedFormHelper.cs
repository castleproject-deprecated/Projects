
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

namespace Castle.Components.Localization.MonoRail.Tests
{
	#region Using Directives

	using System;

	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Tests.Helpers;
	using Castle.MonoRail.Framework.Test;

	using NUnit.Framework;
	using System.Globalization;
	using System.Threading;
	using Castle.MonoRail.Framework;
	using Castle.Components.Localization.MonoRail.Helpers;

	#endregion Using Directives

	[TestFixture]
	public class TestLocalizedFormHelper
	{

		#region Instance Variables 

		MyController _Controller;
		ControllerContext _ControllerContext;
		LocalizedFormHelper _Helper;

		#endregion Instance Variables 

		#region Public Methods 

		[SetUp]
		public void Init()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
			Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

			_Helper = new LocalizedFormHelper();
			_Helper.ServerUtility = new MockServerUtility();

			_Controller = new MyController();
			_ControllerContext = new ControllerContext();
		}

		#endregion Public Methods

		#region DateFormat Test Methods

		[Test]
		public void AlternativeFriendlyFormatFromNow()
		{
			Assert.AreEqual(
				Resources.Date.Today,
				_Helper.AlternativeFriendlyFormatFromNow( DateTime.Now ) );

			Assert.AreEqual(
				Resources.Date.Today,
				_Helper.AlternativeFriendlyFormatFromNow( DateTime.Now ) );

			Assert.AreEqual(
				Resources.Date.Today,
				_Helper.AlternativeFriendlyFormatFromNow( DateTime.Now ) );
		}

		[Test]
		public void ToShortDate()
		{
			DateTime now = DateTime.Now;

			string expected = now.ToShortDateString();
			Assert.AreEqual( expected, _Helper.ToShortDate( now ) );
			Assert.AreEqual( String.Empty, _Helper.ToShortDate( null ) );
		}

		[Test]
		public void ToShortDateTime()
		{
			DateTime now = DateTime.Now;

			string expected = now.ToShortDateString() + " " + now.ToShortTimeString();
			Assert.AreEqual( expected, _Helper.ToShortDateTime( now ) );
			Assert.AreEqual( String.Empty, _Helper.ToShortDateTime( null ) );
		}

		#region Past 

		[Test]
		public void AlternativeFriendlyFormatFromNowMinus120Days()
		{
			Assert.AreEqual(
				string.Format( Resources.Date.XMonthsAgo, 4, "s" ),
				_Helper.AlternativeFriendlyFormatFromNow( DateTime.Now.AddDays( -120 ) ) );
		}

		[Test]
		public void AlternativeFriendlyFormatFromNowMinus3Days()
		{
			Assert.AreEqual(
				string.Format( Resources.Date.XDaysAgo, 3, "s" ),
				_Helper.AlternativeFriendlyFormatFromNow( DateTime.Now.AddDays( -3 ) ) );
		}

		[Test]
		public void AlternativeFriendlyFormatFromNowMinus44Hours()
		{
			Assert.AreEqual(
				Resources.Date.Yesterday,
				_Helper.AlternativeFriendlyFormatFromNow( DateTime.Now.AddHours( -44 ) ) );
		}

		[Test]
		public void AlternativeFriendlyFormatFromNowMinus4Hours()
		{
			Assert.AreEqual(
				Resources.Date.Today,
				_Helper.AlternativeFriendlyFormatFromNow( DateTime.Now.AddHours( -4 ) ) );
		}

		[Test]
		public void FriendlyFormatWithNegativeDiffOf120Minutes()
		{
			Assert.AreEqual(
				string.Format( Resources.Date.XHoursAgo, 2, "s" ),
				_Helper.FriendlyFormatFromNow( DateTime.Now.AddMinutes( -120 ) ) );
		}

		[Test]
		public void FriendlyFormatWithNegativeDiffOf17Hours()
		{
			Assert.AreEqual(
				string.Format( Resources.Date.XHoursAgo, 17, "s" ),
				_Helper.FriendlyFormatFromNow( DateTime.Now.AddMinutes( -( 17 * 60 + 30 ) ) ) );
		}

		[Test]
		public void FriendlyFormatWithNegativeDiffOf2Days()
		{
			Assert.AreEqual(
				string.Format( Resources.Date.XDaysAgo, 2, "s" ),
				_Helper.FriendlyFormatFromNow( DateTime.Now.AddDays( -2 ) ) );
		}

		[Test]
		public void FriendlyFormatWithNegativeDiffOfOneSecond()
		{
			Assert.AreEqual(
				string.Format( Resources.Date.XSecondsAgo, 1, "" ),
				_Helper.FriendlyFormatFromNow( DateTime.Now.AddSeconds( -1 ) ) );
		}

		[Test]
		public void FriendlyFormatWithNegativeDiffOfTenMinutes()
		{
			Assert.AreEqual(
				string.Format( Resources.Date.XMinutesAgo, 10, "s" ),
				_Helper.FriendlyFormatFromNow( DateTime.Now.AddMinutes( -10 ) ) );
		}

		[Test]
		public void FriendlyFormatWithNegativeDiffOfTenSeconds()
		{
			Assert.AreEqual(
				string.Format( Resources.Date.XSecondsAgo, 10, "s" ),
				_Helper.FriendlyFormatFromNow( DateTime.Now.AddSeconds( -10 ) ) );
		}

		#endregion Past 
		#region Future

		[Test]
		public void AlternativeFriendlyFormatFromNowPlus120Days()
		{
			Assert.AreEqual(
				string.Format( Resources.Date.InXMonths, 4, "s" ),
				_Helper.AlternativeFriendlyFormatFromNow( DateTime.Now.AddDays( 120 ) ) );
		}

		[Test]
		public void AlternativeFriendlyFormatFromNowPlus3Days()
		{
			Assert.AreEqual(
				string.Format( Resources.Date.InXDays, 3, "s" ),
				_Helper.AlternativeFriendlyFormatFromNow( DateTime.Now.AddDays( 3 ) ) );
		}

		[Test]
		public void AlternativeFriendlyFormatFromNowPlus44Hours()
		{
			Assert.AreEqual(
				Resources.Date.Tomorrow,
				_Helper.AlternativeFriendlyFormatFromNow( DateTime.Now.AddHours( 44 ) ) );
		}

		[Test]
		public void AlternativeFriendlyFormatFromNowPlus4Hours()
		{
			Assert.AreEqual(
				Resources.Date.Today,
				_Helper.AlternativeFriendlyFormatFromNow( DateTime.Now.AddHours( 4 ) ) );
		}

		[Test]
		public void FriendlyFormatWithPositiveDiffOf120Minutes()
		{
			Assert.AreEqual(
				string.Format( Resources.Date.InXHours, 2, "s" ),
				_Helper.FriendlyFormatFromNow( DateTime.Now.AddMinutes( 120 ) ) );
		}

		[Test]
		public void FriendlyFormatWithPositiveDiffOf17Hours()
		{
			Assert.AreEqual(
				string.Format( Resources.Date.InXHours, 17, "s" ),
				_Helper.FriendlyFormatFromNow( DateTime.Now.AddMinutes( ( 17 * 60 + 30 ) ) ) );
		}

		[Test]
		public void FriendlyFormatWithPositiveDiffOf2Days()
		{
			Assert.AreEqual(
				string.Format( Resources.Date.InXDays, 2, "s" ),
				_Helper.FriendlyFormatFromNow( DateTime.Now.AddDays( 2 ) ) );
		}

		[Test]
		public void FriendlyFormatWithPositiveDiffOfOneSecond()
		{
			Assert.AreEqual(
				string.Format( Resources.Date.InXSeconds, 1, "" ),
				_Helper.FriendlyFormatFromNow( DateTime.Now.AddSeconds( 1 ) ) );
		}

		[Test]
		public void FriendlyFormatWithPositiveDiffOfTenMinutes()
		{
			Assert.AreEqual(
				string.Format( Resources.Date.InXMinutes, 10, "s" ),
				_Helper.FriendlyFormatFromNow( DateTime.Now.AddMinutes( 10 ) ) );
		}

		[Test]
		public void FriendlyFormatWithPositiveDiffOfTenSeconds()
		{
			Assert.AreEqual(
				string.Format( Resources.Date.InXSeconds, 10, "s" ),
				_Helper.FriendlyFormatFromNow( DateTime.Now.AddSeconds( 10 ) ) );
		}

		#endregion Future
		#endregion DateFormat Test Methods

		#region Select For Enum Tests Methods

		[Test]
		public void TestSimpleSelect()
		{
			_ControllerContext.PropertyBag.Add( "theTarget", MyEnumeration.Value1 );
			_Helper.SetController( _Controller, _ControllerContext );

			string html = _Helper.Select( "theTarget", typeof( MyEnumeration ) );

			Assert.IsNotEmpty( html );
			Assert.AreEqual( 
				"<select id=\"theTarget\" name=\"theTarget\" >\r\n<option selected=\"selected\" value=\"Value1\">The first value</option>\r\n<option value=\"Value2\">The second value</option>\r\n<option value=\"Value3\">The third value</option>\r\n</select>", 
				html );
		}

		[Test]
		public void TestSelectWithSelectedValue()
		{
			_ControllerContext.PropertyBag.Add( "theTarget", MyEnumeration.Value3 );
			_Helper.SetController( _Controller, _ControllerContext );

			string html = _Helper.Select( "theTarget", typeof( MyEnumeration ) );

			Assert.IsNotEmpty( html );
			Assert.AreEqual( 
				"<select id=\"theTarget\" name=\"theTarget\" >\r\n<option value=\"Value1\">The first value</option>\r\n<option value=\"Value2\">The second value</option>\r\n<option selected=\"selected\" value=\"Value3\">The third value</option>\r\n</select>", 
				html );
		}

		[Test]
		public void TestSelectIncorrectTextAndValueAttributes()
		{
			_ControllerContext.PropertyBag.Add( "theTarget", MyEnumeration.Value3 );
			_Helper.SetController( _Controller, _ControllerContext );

			string html = _Helper.Select( 
				"theTarget", 
				typeof( MyEnumeration ),
				DictHelper.CreateN( "text", "IncorrectText" ).N( "value", "IncorrectValue" ) );

			Assert.IsNotEmpty( html );
			Assert.AreEqual(
				"<select id=\"theTarget\" name=\"theTarget\" >\r\n<option value=\"Value1\">The first value</option>\r\n<option value=\"Value2\">The second value</option>\r\n<option selected=\"selected\" value=\"Value3\">The third value</option>\r\n</select>",
				html );
		}

		[Test]
		public void TestSelectWithFirstOption()
		{
			_ControllerContext.PropertyBag.Add( "theTarget", MyEnumeration.Value3 );
			_Helper.SetController( _Controller, _ControllerContext );

			string html = _Helper.Select( 
				"theTarget", 
				typeof( MyEnumeration ), 
				DictHelper.CreateN( "firstoption", "Please select" ) );

			Assert.IsNotEmpty( html );
			Assert.AreEqual(
				"<select id=\"theTarget\" name=\"theTarget\" >\r\n<option value=\"0\">Please select</option>\r\n<option value=\"Value1\">The first value</option>\r\n<option value=\"Value2\">The second value</option>\r\n<option selected=\"selected\" value=\"Value3\">The third value</option>\r\n</select>",
				html );
		}

		#endregion Select For Enum Tests Methods

	}
}
