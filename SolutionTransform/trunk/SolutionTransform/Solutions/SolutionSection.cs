using System.Collections.Generic;
using System.Linq;

namespace SolutionTransform.Solutions
{
    public class SolutionSection
    {
        List<KeyValuePair<string, string>> values = new List<KeyValuePair<string, string>>();

        public string ChapterSectionType { get; set;}  // e.g. GlobalSection
        public string SectionType { get; set; } // e.g. SolutionConfigurationPlatforms
        public string Position { get; set; }  // e.g. preSolution

        public ICollection<KeyValuePair<string, string>> Values { get { return values; } }


        public IEnumerable<string> Keys()
        {
            return values.Select(v => v.Key);
        }

        public IEnumerable<string> Lines()
        {
            yield return string.Format("\t{0}({1}) = {2}", ChapterSectionType, SectionType, Position);
            foreach (var pair in Values) {
                yield return string.Format("\t\t{0} = {1}", pair.Key, pair.Value);
            }
            yield return string.Format("\tEnd{0}", ChapterSectionType);
        }
    }
}
