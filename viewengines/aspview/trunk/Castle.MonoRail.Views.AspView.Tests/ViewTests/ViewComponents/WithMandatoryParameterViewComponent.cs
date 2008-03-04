using System;
using System.Collections.Generic;
using System.Text;
using Castle.MonoRail.Framework;

namespace Castle.MonoRail.Views.AspView.Tests.ViewTests.ViewComponents {
	public class WithMandatoryParameterViewComponent : ViewComponent {

		string text;

		[ViewComponentParam(Required = true)]
		public string Text {
			get { return text; }
			set { text = value; }
		}

		public override void Render() {
			RenderText(text);
		}


	}
}
