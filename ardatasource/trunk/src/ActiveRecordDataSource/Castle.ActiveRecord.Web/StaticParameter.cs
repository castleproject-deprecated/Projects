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
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace Castle.ActiveRecord.Web
{
    class StaticParameter : Parameter
    {      
        protected string m_value;       

        public string Value
        {
            get { return m_value; }
            set { m_value = value; }
        }

        public StaticParameter()
        {

        }

        public StaticParameter(StaticParameter parameter) : base(parameter)
        {
            Value = parameter.Value;
        }

        protected override object Evaluate(System.Web.HttpContext context, System.Web.UI.Control control)
        {
            return Value;
        }

        protected override Parameter Clone()
        {
            return new StaticParameter(this);
        }
    }
}
