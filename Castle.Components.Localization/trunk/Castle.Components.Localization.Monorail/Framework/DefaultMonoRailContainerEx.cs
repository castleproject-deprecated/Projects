
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
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion License

namespace Castle.Components.Localization.MonoRail.Framework
{
	#region Using Directives

	using System;
	using System.Reflection;
	using System.Web;

	using Castle.Core;
	using Castle.Core.Logging;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Container;
	using Castle.MonoRail.Framework.Services;

	#endregion Using Directives

	public class DefaultMonoRailContainerEx : DefaultMonoRailContainer, IMonoRailContainerEx, IMonoRailServicesEx, IServiceProviderEx, IServiceProvider
	{

		#region Instance Variables 

		IStaticResourceRegistryEx _StaticResourceRegistryEx;

		#endregion Instance Variables 

		#region Properties 

		/// <summary>
		/// Gets or sets the <see cref="IStaticResourceRegistryEx"/> service.
		/// </summary>
		/// <value>The <see cref="IStaticResourceRegistryEx"/> service.</value>
		public IStaticResourceRegistryEx StaticResourceRegistryEx
		{
			get
			{
				if ( _StaticResourceRegistryEx == null )
				{
					if ( !base.HasService<IStaticResourceRegistryEx>() )
					{
						base.AddService<IStaticResourceRegistryEx>( base.CreateService<DefaultStaticResourceRegistryEx>() );
					}

					_StaticResourceRegistryEx = base.GetService<IStaticResourceRegistryEx>();
				}
				return _StaticResourceRegistryEx;
			}
			set
			{
				_StaticResourceRegistryEx = value;
			}
		}

		#endregion Properties 

		#region Constructors 

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultMonoRailContainerEx"/> class.
		/// </summary>
		public DefaultMonoRailContainerEx()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultMonoRailContainerEx"/> class.
		/// </summary>
		/// <param name="serviceProvider">The service provider.</param>
		public DefaultMonoRailContainerEx( IServiceProvider serviceProvider )
			: base( serviceProvider )
		{
		}

		#endregion Constructors 

	}
}
