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

		public virtual void SetResponseType(MimeType mime)
		{
			_controller.Response.ContentType = mime.MimeString;
		}

		public virtual void SendRenderView(string view)
		{
			_controller.RenderView(view);
		}

		public virtual void SendCancelLayoutAndView()
		{
			_controller.CancelLayout();
			_controller.CancelView();
		}

		public virtual void UseResponseWriter(Action<TextWriter> writerAction)
		{
			writerAction(_controller.Response.Output);
		}

		public virtual void UseResponseStream(Action<Stream> streamAction)
		{
			streamAction(_controller.Response.OutputStream);
		}

		public virtual void SetResponseCode(int code)
		{
			_controller.Response.StatusCode = code;
		}

		public virtual void AppendResponseHeader(string headerName, string value)
		{
			_controller.Response.AppendHeader(headerName, value);
		}

		public virtual void SendRenderText(string text)
		{
			_controller.RenderText(text);
		}

		public virtual string ControllerAction
		{
			get { return _controllerAction; }
		}

		public virtual bool IsFormatDefined
		{
			get 
			{
				if (String.IsNullOrEmpty(GetFormat()))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
		}

		public virtual string GetFormat()
		{
			return _controller.Params["format"];
		}
	}
}
