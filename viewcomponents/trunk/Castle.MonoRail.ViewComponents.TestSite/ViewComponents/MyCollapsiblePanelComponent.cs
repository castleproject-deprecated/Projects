namespace Castle.MonoRail.ViewComponents.TestSite.ViewComponents
{
	using Castle.MonoRail.Framework;

    /// <summary>
    /// A CollapsiblePanelComponent that sets show and hide images.
    /// </summary>
    public class MyCollapsiblePanelComponent : CollapsiblePanelComponent
    {
        public override void Initialize()
        {
            Context.ComponentParameters.Add("expandImagePath", "/Images/expand.jpg");
            Context.ComponentParameters.Add("collapseImagePath", "/Images/collapse.jpg");

            base.Initialize();
        }
    }
}