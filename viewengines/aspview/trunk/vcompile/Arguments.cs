namespace Castle.MonoRail.AspView.VCompile
{
	public class Arguments
	{
		private bool _wait;
		private string _siteRoot;
		private string _viewPathRoot;

		public bool Wait
		{
			get { return _wait; }
			set { _wait = value; }
		}

		public string SiteRoot
		{
			get { return _siteRoot; }
			set { _siteRoot = value; }
		}

		public string ViewPathRoot
		{
			get { return _viewPathRoot; }
			set { _viewPathRoot = value; }
		}
	}
}