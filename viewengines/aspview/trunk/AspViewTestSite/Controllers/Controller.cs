using Castle.Components.DictionaryAdapter;
using Castle.MonoRail.Framework;

namespace AspViewTestSite.Controllers
{
	public class Controller<IView> : SmartDispatcherController
		where IView : class
	{
		IDictionaryAdapterFactory dictionaryAdapterFactory;
		IView typedPropertyBag;
		IView typedFlash;

		protected IView TypedPropertyBag
		{
			get 
			{
				if (typedPropertyBag == null)
					typedPropertyBag = dictionaryAdapterFactory.GetAdapter<IView>(PropertyBag);
				return typedPropertyBag; 
			}
		}

		protected IView TypedFlash
		{
			get 
			{
				if (typedFlash == null)
					typedFlash = dictionaryAdapterFactory.GetAdapter<IView>(Flash);
				return typedFlash; 
			}
		}

		protected override void Initialize()
		{
			base.Initialize();
			dictionaryAdapterFactory = new DictionaryAdapterFactory();
		}
	}
}
