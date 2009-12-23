using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SolutionTransform.Solutions;

namespace SolutionTransform
{
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

        public bool ShouldApply(SolutionProject project)
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
}
