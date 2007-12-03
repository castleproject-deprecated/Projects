using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Castle.MonoRail.Framework;
using Castle.MonoRail.Views.AspView;
namespace CompiledViews
{
public class home_nullablevaluetypeswithdefaultvalue : AspViewBase
{
private int? someIntegerWithDefaultValue { get { return (int?)GetParameter("someIntegerWithDefaultValue", default(int)); } }
public override void Render()
{
Output(someIntegerWithDefaultValue);
Output(@"
");

}
protected override string ViewName { get { return "\\Home\\NullableValueTypesWithDefaultValue.aspx"; } }
protected override string ViewDirectory { get { return "\\Home"; } }
}
}
