
namespace Castle.MonoRail.ViewComponents
{
	using System;
	using Castle.MonoRail.Framework;
	using anrControls;

	/// <summary>
	/// 
	/// </summary>
	[ViewComponentDetails("Markdown")]
	public class MarkdownViewComponent : ViewComponentEx
	{
		[Flags]
		public enum MarkDownStyle { Markdown = 0x01, SmartyPants=0x02, All = -1 }

		[ViewComponentParam(Default=MarkDownStyle.All)]
		public MarkDownStyle Style { get; set; }

		public override void Render()
		{
			base.Render();
			string text = GetBodyText();

			if ( (Style & MarkDownStyle.Markdown) == MarkDownStyle.Markdown)
			{
	            Markdown    md = new Markdown ();
				text = md.Transform(text);
			}
			if ( (Style & MarkDownStyle.SmartyPants) == MarkDownStyle.SmartyPants)
			{
	            SmartyPants sm = new SmartyPants();
				text = sm.Transform(text, ConversionMode.EducateDefault);
			}

			RenderText(text);
			CancelView();

		}


	}
}
