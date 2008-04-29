using Castle.Components.DictionaryAdapter;
using Castle.MonoRail.Framework.Helpers;

namespace Castle.MonoRail.Views.AspView
{
	public interface IHelpersAccesor
	{
		/// <summary>
		/// Gets the AjaxHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Ajax", "AjaxHelper")]
		AjaxHelper Ajax { get; }

		/// <summary>
		/// Gets the BehaviourHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Behaviour", "BehaviourHelper")]
		BehaviourHelper Behaviour { get; }

		/// <summary>
		/// Gets the DateFormatHelper instance
		/// </summary>
		[DictionaryKeySubstitution("DateFormat", "DateFormatHelper")]
		DateFormatHelper DateFormat { get; }

		/// <summary>
		/// Gets the DictHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Dict", "DictHelper")]
		DictHelper Dict { get; }

		/// <summary>
		/// Gets the EffectsFatHelper instance
		/// </summary>
		[DictionaryKeySubstitution("EffectsFat", "EffectsFatHelper")]
		EffectsFatHelper EffectsFat { get; }

		/// <summary>
		/// Gets the FormHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Form", "FormHelper")]
		FormHelper Form { get; }

		/// <summary>
		/// Gets the HtmlHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Html", "HtmlHelper")]
		HtmlHelper Html { get; }

		/// <summary>
		/// Gets the PaginationHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Pagination", "PaginationHelper")]
		PaginationHelper Pagination { get; }

		/// <summary>
		/// Gets the ScriptaculousHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Scriptaculous", "ScriptaculousHelper")]
		ScriptaculousHelper Scriptaculous { get; }

		/// <summary>
		/// Gets the UrlHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Url", "UrlHelper")]
		UrlHelper Url { get; }

		/// <summary>
		/// Gets the ValidationHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Validation", "ValidationHelper")]
		ValidationHelper Validation { get; }

		/// <summary>
		/// Gets the WizardHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Wizard", "WizardHelper")]
		WizardHelper Wizard { get; }

		/// <summary>
		/// Gets the ZebdaHelper instance
		/// </summary>
		[DictionaryKeySubstitution("Zebda", "ZebdaHelper")]
		ZebdaHelper Zebda { get; }
	}
}