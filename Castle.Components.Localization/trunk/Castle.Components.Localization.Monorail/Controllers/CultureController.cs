
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

namespace Castle.Components.Localization.MonoRail.Controllers
{
    #region Using Directives

    using System;
    using System.Collections;
    using System.Globalization;

    using Castle.MonoRail.Framework;

    #endregion Using Directives

	/// <summary>
	/// This <see cref="Controller"/> has only one action method <see cref="CultureController.SetCulture"/>
	/// which can be used to set the current user culture.
	/// </summary>
    public partial class CultureController : SmartDispatcherController
    {

		#region Public Methods 

		/// <summary>
		/// Sets the current's user culture by creating a cookie named <c>culture</c>.
		/// </summary>
		/// <param name="cultureCode">The culture code as a .NET code : <c>en</c>, <c>en-US</c>, etc.</param>
		/// <param name="backUrl">The URL to redirect to.</param>
        public void SetCulture( string cultureCode, string backUrl )
        {
            Response.CreateCookie( "culture", cultureCode );

            RedirectToUrl( backUrl );
        }

		#endregion Public Methods 

    }
}
