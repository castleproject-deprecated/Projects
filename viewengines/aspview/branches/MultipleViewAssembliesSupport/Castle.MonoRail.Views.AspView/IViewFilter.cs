using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.MonoRail.Views.AspView
{
    public interface IViewFilter
    {
        string ApplyOn(string input);
    }
}
