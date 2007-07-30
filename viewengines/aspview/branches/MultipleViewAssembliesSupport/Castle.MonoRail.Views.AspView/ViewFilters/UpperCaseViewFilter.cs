using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.MonoRail.Views.AspView.ViewFilters
{
    public class UpperCaseViewFilter : IViewFilter
    {
        #region IViewFilter Members
        public string ApplyOn(string input)
        {
            return input.ToUpper();
        }
        #endregion
    }
}
