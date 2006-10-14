// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

using System;
using System.Collections;
using System.Collections.Generic;
using Castle.ActiveRecord;
using NHibernate;
using NHibernate.Engine;
using NHibernate.Expression;
using NHibernate.SqlCommand;

namespace Castle.ActiveRecord.Queries
{
    /// <summary>
    /// An ActiveRecord count query that allows for criteria. 
    /// </summary>    
    public class CriteraCountQuery : ActiveRecordBaseQuery
    {
        protected Type m_type;
        protected ICriterion[] m_criterions;

        public CriteraCountQuery(Type _type) : this(_type, new ICriterion[0])
        {

        }

        public CriteraCountQuery(Type _type, ICriterion[] _criterions) : base(_type)
        {
            m_type = _type;
            m_criterions = _criterions;
        }

        public Type Type
        {
            get { return m_type; }
            set { m_type = value; }
        }

        public ICriterion[] Criterions
        {
            get { return m_criterions; }
            set { m_criterions = value; }
        }

        protected string ConvertTypedValueToString(TypedValue tv)
        {
            if (tv.Value == null)
                return "NULL";

            Type type = tv.Value.GetType();

            if (type == typeof(string))
            {
                string temp = tv.Value.ToString();
                temp = temp.Replace("'", "''");
                return String.Concat("'", temp, "'");
            }

            return tv.Value.ToString();
        }

        protected override IQuery CreateQuery(ISession session)
        {            
            string alias = m_type.Name;

            Dictionary<string, string> aliasClasses = new Dictionary<string, string>();
            List<string> queryParts = new List<string>();
            List<string> conditionParts = new List<string>();

            foreach (ICriterion criterion in m_criterions)
            {
                SqlString sql = criterion.ToSqlString((ISessionFactoryImplementor)session.SessionFactory, m_type, alias, aliasClasses);
                TypedValue[] values = criterion.GetTypedValues((ISessionFactoryImplementor)session.SessionFactory, m_type, aliasClasses);

                int j = 0;
                foreach (int i in sql.ParameterIndexes)
                {
                    TypedValue value = values[j++];
                    sql.SqlParts[i] = ConvertTypedValueToString(value);
                }

                string temp = sql.ToString();
                conditionParts.Add(temp);
            }

            queryParts.Add(String.Format("select count(*) from {0} as {1}", alias, alias));

            if (conditionParts.Count > 0)
            {
                queryParts.Add(" where ");
                queryParts.Add(String.Join(" and ", conditionParts.ToArray()));
            }

            IQuery query = session.CreateQuery(String.Join("", queryParts.ToArray()));

            return query;
        }

        protected override object InternalExecute(ISession session)
        {
            IQuery query = CreateQuery(session);
            return query.List();
        }
    }

    /// <summary>
    /// </summary>
    public class SimpleCountQuery<T> : CriteraCountQuery
    {
        public SimpleCountQuery()
            : base(typeof(T))
        {
        }

        public SimpleCountQuery(ICriterion[] _criterions)
            : base(typeof(T), _criterions)
        {
        }
    }
}
