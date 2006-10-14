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
    class ContextParameter : Parameter
    {
        protected string m_contextField;

        public string ContextField
        {
            get { return m_contextField; }
            set { m_contextField = value; }
        }

        public ContextParameter()
        {
        }

        public ContextParameter(ContextParameter parameter) : base(parameter)
        {
            ContextField = parameter.ContextField;
        }

        protected override object Evaluate(System.Web.HttpContext context, System.Web.UI.Control control)
        {
            if (context == null)
                return null;

            if (context.Items.Contains(ContextField))
                return context.Items[ContextField];

            return null;
        }

        protected override Parameter Clone()
        {
            return new ContextParameter(this);
        }
    }
}
