using Castle.Components.DictionaryAdapter;
using Castle.MonoRail.Framework;

namespace AspViewTestSite.Controllers
{
	public class Controller<IView> : Controller
	{
		IView typedPropertyBag;
		protected IView TypedPropertyBag
		{
			get { return typedPropertyBag; }
		}

		protected override void Initialize()
		{
			base.Initialize();
			typedPropertyBag = new DictionaryAdapterFactory().GetAdapter<IView>(PropertyBag);
		}
	}
}
