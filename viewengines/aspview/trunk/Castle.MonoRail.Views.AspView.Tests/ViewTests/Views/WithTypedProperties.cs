namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.Views
{
	public class WithTypedProperties : AspViewBase<IAmSimple>
	{

		public override void Render()
		{
			Output(@"WithTypedProperties
");
			Output(view.Id);
			Output(view.Name);
			Output(@"
WithTypedProperties");
		}

		protected override string ViewDirectory
		{
			get { return ""; }
		}

		protected override string ViewName
		{
			get { throw new System.Exception("The method or operation is not implemented."); }
		}
	}
}
