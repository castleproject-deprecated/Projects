// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.MonoRail.Rest
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using Castle.MonoRail.Rest.Mime;
	using System.IO;
	
	public class ControllerBridge : IControllerBridge
	{
		private RestfulController _controller;
		private string _controllerAction;

		public ControllerBridge(RestfulController controller, string controllerAction)
		{
			_controller = controller;
			_controllerAction = controllerAction;
		}

		public void SetResponseType(MimeType mime)
		{
			_controller.Response.ContentType = mime.MimeString;
		}

		public void SendRenderView(string view)
		{
			_controller.RenderView(view);
		}

		public void SendCancelLayoutAndView()
		{
			_controller.CancelLayout();
			_controller.CancelView();
		}

		public void UseResponseWriter(Action<TextWriter> writerAction)
		{
			writerAction(_controller.Response.Output);
		}

		public void SetResponseCode(int code)
		{
			_controller.Response.StatusCode = code;
		}

		public void AppendResponseHeader(string headerName, string value)
		{
			_controller.Response.AppendHeader(headerName, value);
		}

		public void SendRenderText(string text)
		{
			_controller.RenderText(text);
		}

		#region IControllerBridge Members


		public string ControllerAction
		{
			get { return _controllerAction; }
		}

		public bool IsFormatDefined
		{
			get {
				if (String.IsNullOrEmpty(_controller.Params["format"]))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}

		public string GetFormat()
		{
			return _controller.Params["format"];
		}

		#endregion
	}
}
