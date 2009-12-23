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

using System.Text.RegularExpressions;

namespace SolutionTransform.ProjectFile
{
    using System.Xml;

    public class RemoveTarget : MSBuild2003Transform
    {
        private readonly Regex target;

        public RemoveTarget(string target)
        {
            this.target = new Regex(Regex.Escape(target), RegexOptions.IgnoreCase);
        }

        public override void DoApplyTransform(XmlDocument document)
        {
            foreach (XmlElement node in document.SelectNodes("/*/x:Import", namespaces))
            {
                if (target.IsMatch(node.GetAttribute("Project")))
                {
                    Delete(node);
                }
            }
        }
    }
}
