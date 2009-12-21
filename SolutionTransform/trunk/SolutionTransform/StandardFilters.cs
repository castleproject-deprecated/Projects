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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text.RegularExpressions;

    public interface IProjectFilter
    {
        bool ShouldIncludeProject(SolutionProject project);
    }

    public class RegexFilter : IProjectFilter
    {
        private readonly IEnumerable<Regex> patterns;

        public RegexFilter(IEnumerable patterns) : this(patterns.Cast<string>())
        {
            
        }

        public RegexFilter(IEnumerable<Regex> patterns)
        {
            this.patterns = patterns;
        }

        public RegexFilter(IEnumerable<string> patterns) 
            : this(patterns.Select(p => new Regex(Regex.Escape(p), RegexOptions.IgnoreCase)).ToList())
        {
            
        }

        public bool ShouldIncludeProject(SolutionProject project)
        {
            if (project.IsFolder) {
                return true;
            }
            foreach (var validProject in patterns) {
                if (validProject.IsMatch(project.Name)) {
                    return true;
                }
            }
            return false;
        }
    }

    public class DontFilter : IProjectFilter
    {
        public bool ShouldIncludeProject(SolutionProject project)
        {
            return true;
        }
    }
}
