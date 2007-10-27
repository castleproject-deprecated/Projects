using Castle.MonoRail.Framework.Helpers;

namespace Castle.MonoRail.Views.AspView
{
	public interface IHelpersAccesor
	{
		/// <summary>
		/// Gets the AjaxHelper instance
		/// </summary>
		AjaxHelper Ajax { get; }

		/// <summary>
		/// Gets the BehaviourHelper instance
		/// </summary>
		BehaviourHelper Behaviour { get; }

		/// <summary>
		/// Gets the DateFormatHelper instance
		/// </summary>
		DateFormatHelper DateFormat { get; }

		/// <summary>
		/// Gets the DictHelper instance
		/// </summary>
		DictHelper Dict { get; }

		/// <summary>
		/// Gets the EffectsFatHelper instance
		/// </summary>
		EffectsFatHelper EffectsFat { get; }

		/// <summary>
		/// Gets the FormHelper instance
		/// </summary>
		FormHelper Form { get; }

		/// <summary>
		/// Gets the HtmlHelper instance
		/// </summary>
		HtmlHelper Html { get; }

		/// <summary>
		/// Gets the PaginationHelper instance
		/// </summary>
		PaginationHelper Pagination { get; }

		/// <summary>
		/// Gets the PrototypeHelper instance
		/// </summary>
		PrototypeHelper Prototype { get; }

		/// <summary>
		/// Gets the ScriptaculousHelper instance
		/// </summary>
		ScriptaculousHelper ScriptaculousHelper { get; }

		/// <summary>
		/// Gets the UrlHelper instance
		/// </summary>
		UrlHelper Url { get; }

		/// <summary>
		/// Gets the ValidationHelper instance
		/// </summary>
		ValidationHelper ValidationHelper { get; }

		/// <summary>
		/// Gets the WizardHelper instance
		/// </summary>
		WizardHelper WizardHelper { get; }

		/// <summary>
		/// Gets the ZebdaHelper instance
		/// </summary>
		ZebdaHelper Zebda { get; }
	}
}