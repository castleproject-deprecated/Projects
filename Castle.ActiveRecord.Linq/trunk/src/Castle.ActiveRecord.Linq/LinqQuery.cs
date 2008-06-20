﻿// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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


namespace Castle.ActiveRecord.Linq
{
    using System;
    using System.Linq;
    using NHibernate;
    using NHibernate.Linq;

    /// <summary>
    /// Class used internally to glue NHibernate.Linq, NHibernate, and ActiveRecord together
    /// via ExecuteQuery. This appeared to be the cleanest way of connecting to the appropriate
    /// type and scope specific ISession instance. 
    /// </summary>
    /// <typeparam name="T">The type of the active record class the Linq collection 
    /// is being bound against.</typeparam>
    class LinqQuery<T> : IActiveRecordQuery<IOrderedQueryable<T>>
    {
        #region IActiveRecordQuery<IOrderedQueryable<T>> Members

        public IOrderedQueryable<T> Execute(ISession session)
        {
            return new Query<T>(new NHibernateQueryProvider(session));
        }

        #endregion

        #region IActiveRecordQuery Members

        System.Collections.IEnumerable IActiveRecordQuery.Enumerate(ISession session)
        {
            return Execute(session);
        }

        object IActiveRecordQuery.Execute(ISession session)
        {
            return Execute(session);
        }

        Type IActiveRecordQuery.RootType
        {
            get { return typeof(T); }
        }

        #endregion
    }
}
