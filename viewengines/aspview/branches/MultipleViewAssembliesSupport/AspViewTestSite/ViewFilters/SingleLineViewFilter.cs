using System;
using Castle.MonoRail.Views.AspView;
using System.Text.RegularExpressions;

namespace AspViewTestSite
{
    public class SingleLineViewFilter : IViewFilter
    {
        #region IViewFilter Members
        static readonly Regex singleLineTransform = new Regex("<filter[:]SingleLine>(?<content>.*)</filter[:]SingleLine>", RegexOptions.Singleline);
        static readonly Regex allWhiteSpaces = new Regex("\\s+", RegexOptions.Compiled);

        public string ApplyOn(string input)
        {
            /*
            return singleLineTransform.Replace(input, delegate(Match m)
            {
                return allWhiteSpaces.Replace(m.Groups["content"].Value, " ").Trim();
            });
             * */
            return allWhiteSpaces.Replace(input, " ");
        }

        #endregion
    }
}
