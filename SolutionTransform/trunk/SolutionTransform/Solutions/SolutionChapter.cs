using System.Collections.Generic;
using System.Linq;

namespace SolutionTransform.Solutions
{
    public class SolutionChapter
    {
        List<SolutionSection> sections = new List<SolutionSection>();

        public SolutionChapter(string start, string end)
        {
            Start = start;
            End  = end;
        }

        public virtual string Start { get; protected set; }
        public string End { get; private set; }

        public ICollection<SolutionSection> Sections { get { return sections; } }

        public IEnumerable<string> Lines()
        {
            yield return Start;
            foreach (var line in Sections.SelectMany(s => s.Lines()))
            {
                yield return line;
            }
            yield return End;
        }
    }
}
